using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_StreetFightManager : MonoBehaviour
{
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
    [SerializeField, Tooltip("the parent of street fight ui")]
    private GameObject _UIStreetFight;

    [Space(10)]
    [SerializeField, Tooltip("it is the 3, 2, 1 on begin of match")]
    private Image _timerBeforeStart;
    [SerializeField, Tooltip("the number for the timer in the begin match")]
    private Sprite[] _numberForTimer;

    [Header("Camera")]
    [SerializeField, Tooltip("camera for focus the view on two bots")]
    private S_CameraView _cameraView;
    [SerializeField, Tooltip("the main camera of menu")]
    private GameObject _mainCamera;

    [Header("Animation")]
    [SerializeField, Min(0f), Tooltip("the millstone for the camera field of view in end fight zoom")]
    private float _fliedOfViewEndAnimation = 5f;
    [SerializeField, Min(0f), Tooltip("the speed of field of view animation")]
    private float _fieldOfViewAnimationSpeed = 30f;

    [Header("Event")]
    [SerializeField, Tooltip("invoke when player or AI die")]
    private UnityEvent _onStreetFightEnd;

    private S_AIController _AIController;
    private PlayerInput _playerInput;
    private S_ImmobileDefeat _immobilePlayer, _immobileAI;

    private GameObject _playerBot;
    private GameObject _AIBot;

    private float _startFieldOfView;

    private void Start()
    {
        CreateStreetFightBot();
        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_playerBot.transform);
        _cameraView.AddObjectToView(_AIBot.transform);

        _cameraView.gameObject.SetActive(false);
        _UIStreetFight.SetActive(false);

        _AIController = _AIBot.GetComponent<S_AIController>();
        _playerInput = _playerBot.GetComponent<PlayerInput>();
        BindDeadEventForBots();
        SetEnableBots(false);

        var camera = _cameraView.gameObject.GetComponentInChildren<Camera>();
        _startFieldOfView = camera.fieldOfView;
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
        Debug.Log("AI victory");
        _cameraView.RemoveObjectToView(_playerBot.transform);
        EndFight();
    }
    private void BotPlayerVictorie()
    {
        Debug.Log("Player Victory");
        _cameraView.RemoveObjectToView(_AIBot.transform);
        EndFight();
    }
    private void EndFight()
    {
        ClearDroppedWeapon();
        SetEnableBots(false);
        _mainCamera?.SetActive(false);
        StartCoroutine(AnimationFieldOfView());
        _onStreetFightEnd?.Invoke();
    }
    [ContextMenu("Start fight")]
    public void StartStreetFight()
    {
        _UIStreetFight.SetActive(true);
        _cameraView.gameObject.SetActive(true);
        _mainCamera?.SetActive(false);

        StartCoroutine(TimerBeforeStart(() =>
        {
            _UIStreetFight.SetActive(false);
            SetEnableBots(true);
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
    private IEnumerator AnimationFieldOfView()
    {
        var camera = _cameraView.gameObject.GetComponentInChildren<Camera>();

        while (camera.fieldOfView > _fliedOfViewEndAnimation)
        {
            float speedScale = Mathf.InverseLerp(_fliedOfViewEndAnimation, _startFieldOfView, camera.fieldOfView);
            speedScale = Mathf.Max(speedScale, .5f);

            camera.fieldOfView -= Time.deltaTime * _fieldOfViewAnimationSpeed * speedScale;
            yield return null;
        }
    }
}
