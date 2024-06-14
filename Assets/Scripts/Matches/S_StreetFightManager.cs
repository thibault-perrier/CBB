using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_StreetFightManager : MonoBehaviour
{
    public enum FightState
    {
        Fight,
        BotAIVictory,
        BotPlayerVictory
    }

    [SerializeField, Min(0f), Tooltip("if one bot is not in this radius")]
    private float _radiusDefeat = 10f;

    [Header("paricipants")]
    [SerializeField, Tooltip("the current player bot prefab")]
    private GameObject _playerBotPrefab;
    [SerializeField, Tooltip("the current AI bot prefab")]
    private GameObject _AIBotPrefab;

    [Space(10)]
    [SerializeField, Tooltip("where the player spawn in the start of fight")]
    private Transform _botPlayerTransformSpawn;
    [SerializeField, Tooltip("where the AI spawn in the start of fight")]
    private Transform _botAITransformSpawn;

    [Header("UI")]
    [SerializeField, Tooltip("the parent of the timer before the fight ui")]
    private GameObject _uiTimerBeforeFight;
    [SerializeField, Tooltip("the parent of the display winner text ui")]
    private GameObject _uiEndFightWinner;

    [Space(10)]
    [SerializeField, Tooltip("it is the 3, 2, 1 on begin of match")]
    private Image _timerBeforeStart;
    [SerializeField, Tooltip("the number for the timer in the begin match")]
    private Sprite[] _numberForTimer;

    [Space(10)]
    [SerializeField, Tooltip("for display the winner of the fight")]
    private TextMeshProUGUI _endFightText;

    [Header("Camera")]
    [SerializeField, Tooltip("camera for focus the view on two bots")]
    private S_CameraView _cameraView;
    [SerializeField, Tooltip("the main camera of menu")]
    private GameObject _mainCamera;

    [Header("Animation")]
    [SerializeField, Min(0f), Tooltip("the millstone for the camera field of view in end fight zoom")]
    private float _zoomEndAnimation = 5f;
    [SerializeField, Min(0f), Tooltip("the speed of field of view animation")]
    private float _zoomAnimationSpeed = 30f;

    [Header("Event")]
    [SerializeField, Tooltip("invoke when charcater or AI die and when the player press any key")]
    private UnityEvent _onStreetFightEnd;

    private S_AIController _AIController;
    private PlayerInput _playerInput;
    private S_ImmobileDefeat _immobilePlayer, _immobileAI;

    private GameObject _playerBot;
    private GameObject _AIBot;

    private float _startFieldOfView;
    private bool _streetFightEnd = false;

    private FightState _fightState;

    private void Start()
    {
        _cameraView.gameObject.SetActive(false);
        _uiTimerBeforeFight.SetActive(false);
        _uiEndFightWinner.SetActive(false);

        var camera = _cameraView.gameObject.GetComponentInChildren<Camera>();
        _startFieldOfView = camera.fieldOfView;
    }
    private void Update()
    {
        if (_streetFightEnd)
        {
            if (Input.anyKeyDown)
            {
                EndStreetFight();
            }
        }
        else
        {
            DetectDefeatRadius();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 1f, .2f);
        Gizmos.DrawSphere(transform.position, _radiusDefeat);
        Gizmos.color = new Color(1f, 1f, 1f, 1f);
    }

    [ContextMenu("Start fight")]
    public void StartStreetFight()
    {
        ClearDroppedWeapon();
        CreateStreetFightBot();
        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_playerBot.transform);
        _cameraView.AddObjectToView(_AIBot.transform);

        _uiTimerBeforeFight.SetActive(true);
        _uiEndFightWinner.SetActive(false);
        _cameraView.gameObject.SetActive(true);
        _mainCamera?.SetActive(false);

        StartCoroutine(TimerBeforeStart(() =>
        {
            _uiTimerBeforeFight.SetActive(false);
            SetEnableBots(true);
        }));
    }

    /// <summary>
    /// set the enabled of controller component for player and ai
    /// </summary>
    /// <param name="enabled">enabled value</param>
    private void SetEnableBots(bool enabled)
    {
        _AIController.enabled = enabled;
        _playerInput.enabled = enabled;

        _immobileAI.enabled = enabled;
        _immobilePlayer.enabled = enabled;
    }
    /// <summary>
    /// set the action for dead bot when thier life is lower 0 or if it immobile and not straight
    /// </summary>
    private void BindDeadEventForBots()
    {
        var frameAI     = _AIBot.GetComponent<S_FrameManager>();
        var framePlayer = _playerBot.GetComponent<S_FrameManager>();

        frameAI.OnDie       += (_) => BotPlayerVictorie();
        framePlayer.OnDie   += (_) => BotAiVictorie();

        _immobileAI     = _AIBot.GetComponent<S_ImmobileDefeat>();
        _immobilePlayer = _playerBot.GetComponent<S_ImmobileDefeat>();

        _immobileAI.IsImmobile       += () => BotPlayerVictorie();
        _immobilePlayer.IsImmobile   += () => BotAiVictorie();
    }
    private void BotAiVictorie()
    {
        _fightState = FightState.BotAIVictory;
        _cameraView.RemoveObjectToView(_playerBot.transform);
        EndCurrentFight();
    }
    private void BotPlayerVictorie()
    {
        _fightState = FightState.BotPlayerVictory;
        _cameraView.RemoveObjectToView(_AIBot.transform);
        EndCurrentFight();
    }
    private void EndCurrentFight()
    {
        SetEnableBots(false);
        _mainCamera?.SetActive(false);
        StartCoroutine(AnimationZoom(() =>
        {
            _uiEndFightWinner.SetActive(true);
            DisplayEndFightText();
            _streetFightEnd = true;
        }));
    }
    private void ClearDroppedWeapon()
    {
        var droppedWeapons = GameObject.FindGameObjectsWithTag("Weapon");

        foreach (var weapon in droppedWeapons)
            Destroy(weapon);
    }
    private void CreateStreetFightBot()
    {
        Destroy(_AIBot);
        Destroy(_playerBot);

        _playerBot = Instantiate(_playerBotPrefab,   _botPlayerTransformSpawn.position,  Quaternion.Euler(_botPlayerTransformSpawn.eulerAngles));
        _AIBot     = Instantiate(_AIBotPrefab,       _botAITransformSpawn.position,      Quaternion.Euler(_botAITransformSpawn.eulerAngles));

        _playerBot.tag = "BotB";
        _AIBot.GetComponent<S_AIController>().EnemyTag = "BotB";

        _AIController = _AIBot.GetComponent<S_AIController>();
        _playerInput = _playerBot.GetComponent<PlayerInput>();
        BindDeadEventForBots();
        SetEnableBots(false);
    }
    private void DisplayEndFightText()
    {
        switch (_fightState)
        {
            case FightState.BotAIVictory:
                _endFightText.text = "YOU LOSE";
                break;
            case FightState.BotPlayerVictory:
                _endFightText.text = "VICTORY";
                break;
            default:
                break;
        }
    }
    private void EndStreetFight()
    {
        _streetFightEnd = false;
        
        _mainCamera.SetActive(true);
        _cameraView.gameObject.SetActive(false);
        _uiEndFightWinner.SetActive(false);
        _onStreetFightEnd?.Invoke();
    }
    private void DetectDefeatRadius()
    {
        if (_playerBot == null || _AIBot == null)
            return;

        float distanceDefeatPlayer = Vector3.Distance(transform.position, _playerBot.transform.position);
        float distanceDefeatEnemy = Vector3.Distance(transform.position, _AIBot.transform.position);

        if (distanceDefeatEnemy > _radiusDefeat)
            BotPlayerVictorie();

        if (distanceDefeatPlayer > _radiusDefeat)
            BotAiVictorie();
    }

    private IEnumerator TimerBeforeStart(System.Action endTimer)
    {
        foreach (var number in _numberForTimer)
        {
            _timerBeforeStart.sprite = number;
            yield return new WaitForSeconds(1f);
        }

        endTimer?.Invoke();
    }
    private IEnumerator AnimationZoom(System.Action endZoomAnimation = null)
    {
        var camera = _cameraView.gameObject.GetComponentInChildren<Camera>();

        while (camera.fieldOfView > _zoomEndAnimation)
        {
            float speedScale = Mathf.InverseLerp(_zoomEndAnimation, _startFieldOfView, camera.fieldOfView);
            speedScale = Mathf.Max(speedScale, .5f);

            camera.fieldOfView -= Time.deltaTime * _zoomAnimationSpeed * speedScale;
            yield return null;
        }

        endZoomAnimation?.Invoke();
    }
}
