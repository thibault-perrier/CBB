using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    [SerializeField, Tooltip("Scene to load when the fight is end and when we press on any key")]
    private string _sceneToLoadInEndFight = "MainMenu";
    [SerializeField, Tooltip("the toogle who difine if the fight begin in the start")]
    private bool _startStreetFightInStart = true;

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
    [SerializeField, Tooltip("it is the parent of the fight text")]
    private GameObject _fightParentText;
    [SerializeField, Tooltip("the number for the timer in the begin match")]
    private Sprite[] _numberForTimer;

    [Space(10)]
    [SerializeField, Tooltip("the parent of display the winner of the fight")]
    private GameObject _endFightTextDefeat;
    [SerializeField, Tooltip("the parent of display the loser of the fight")]
    private GameObject _endFightTextVictory;
    [SerializeField, Tooltip("End fight who describe how the fight is end")]
    private TMP_Text _endFightConditionText;

    [Space(10)]
    [SerializeField, Tooltip("the parent of the ui when we fight")]
    private GameObject _uiParentInFight;
    [SerializeField, Tooltip("the skill controller for bind the flower")]
    private S_SkillsController _skillsController;
    [SerializeField, Tooltip("Helth bar of the player")]
    private Image _healthBarPlayer;
    [SerializeField, Tooltip("Helth bar of the enemy")]
    private Image _healthBarEnemy;

    [Space(10)]
    [SerializeField, Tooltip("text mesh used for display the timer")]
    private TMP_Text _timerStreetFight;

    [Header("Camera")]
    [SerializeField, Tooltip("camera for focus the view on two bots")]
    private S_CameraView _cameraView;

    [Header("Animation")]
    [SerializeField, Min(1f), Tooltip("the millstone for the camera field of view in end fight zoom")]
    private float _zoomEndAnimation = 5f;
    [SerializeField, Min(1f), Tooltip("the speed of field of view animation")]
    private float _zoomAnimationSpeed = 30f;

    [Header("Event")]
    [SerializeField, Tooltip("invoke when charcater or AI die and when the player press any key")]
    private UnityEvent _onStreetFightEnd;

    public bool InFight
    {
        get
        {
            if (!_playerFrame || !_enemyFrame)
                return false;

            return true;
        }
    }

    private S_AIController _AIController;
    private PlayerInput _playerInput;
    private S_ImmobileDefeat _immobilePlayer, _immobileAI;

    private GameObject _playerBot;
    private GameObject _AIBot;

    private S_FrameManager _playerFrame;
    private S_FrameManager _enemyFrame;

    private float _startFieldOfView;
    private bool _timerUpdate = false;

    private float _minutesTimer = 3f;
    private float _secondsTimer;

    private FightState _fightState;

    private void Start()
    {
        _cameraView.gameObject.SetActive(false);
        _uiTimerBeforeFight.SetActive(false);
        _uiEndFightWinner.SetActive(false);
        _uiParentInFight.SetActive(false);

        var camera = _cameraView.gameObject.GetComponentInChildren<Camera>();
        _startFieldOfView = camera.fieldOfView;

        if (_startStreetFightInStart)
            StartStreetFight();
    }
    private void Update()
    {
        DetectDefeatRadius();
        UpdateHealthBarUI();
        UpdateTimeUI();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 1f, .1f);
        Gizmos.DrawSphere(transform.position, _radiusDefeat);
        Gizmos.color = new Color(1f, 1f, 1f, 1f);
    }

    /// <summary>
    /// clear all, create the bot and launch the timer before start
    /// </summary>
    [ContextMenu("Start fight")]
    public void StartStreetFight()
    {
        _minutesTimer = 3f;
        _secondsTimer = 0f;
        _skillsController.ResetSkills();

        ClearDroppedWeapon();
        _timerUpdate = false;

        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_botPlayerTransformSpawn);
        _cameraView.AddObjectToView(_botAITransformSpawn);

        StartCoroutine(CreateStreetFightBot(() =>
        {
            _cameraView.ClearObjectToView();
            _cameraView.AddObjectToView(_playerBot.transform);
            _cameraView.AddObjectToView(_AIBot.transform);

            _uiTimerBeforeFight.SetActive(true);
            _uiEndFightWinner.SetActive(false);
            _cameraView.gameObject.SetActive(true);

            var camera = _cameraView.gameObject.GetComponentInChildren<Camera>();
            camera.fieldOfView = _startFieldOfView;

            _timerBeforeStart.gameObject.SetActive(true);
            StartCoroutine(TimerBeforeStart(() =>
            {
                _uiTimerBeforeFight.SetActive(false);
                _uiParentInFight.SetActive(true);
                _timerUpdate = true;

                SetEnableBots(true);
            }));
        }));
    }
    /// <summary>
    /// disable all canvas and switch camera
    /// </summary>
    public void EndStreetFight()
    {
        SceneManager.LoadScene(_sceneToLoadInEndFight, LoadSceneMode.Single);
        _cameraView.gameObject.SetActive(false);
        _uiEndFightWinner.SetActive(false);
    }


    /// <summary>
    /// set the enabled of controller component for player and ai
    /// </summary>
    /// <param name="enabled">enabled value</param>
    private void SetEnableBots(bool enabled)
    {
        _AIController.State = enabled ? S_AIController.AIState.Enable : S_AIController.AIState.Disable;
        _playerInput.enabled = enabled;

        _immobileAI.enabled = enabled;
        _immobilePlayer.enabled = enabled;
    }
    /// <summary>
    /// set the action for dead bot when thier life is lower 0 or if it immobile and not straight
    /// </summary>
    private void BindDeadEventForBots()
    {
        _enemyFrame  = _AIBot.GetComponent<S_FrameManager>();
        _playerFrame = _playerBot.GetComponent<S_FrameManager>();
        _playerFrame.SelectWeapons();

        _enemyFrame.OnDie += (_) =>
        {
            BotPlayerVictory();
            _endFightConditionText.text = "the enemy is destroyed".ToUpper();
        };
        _playerFrame.OnDie += (_) =>
        {
            BotAiVictory();
            _endFightConditionText.text = "the player is destroyed".ToUpper();
        };

        _immobileAI     = _AIBot.GetComponent<S_ImmobileDefeat>();
        _immobilePlayer = _playerBot.GetComponent<S_ImmobileDefeat>();

        _immobileAI.IsImmobile += () =>
        {
            BotPlayerVictory();
            _endFightConditionText.text = "the enemy was immobile for 5 seconds".ToUpper();
        };
        _immobilePlayer.IsImmobile += () =>
        {
            BotAiVictory();
            _endFightConditionText.text = "the player was immobile for 5 seconds".ToUpper();
        };
    }
    /// <summary>
    /// make a victory for AI
    /// </summary>
    private void BotAiVictory()
    {
        _fightState = FightState.BotAIVictory;
        _cameraView.RemoveObjectToView(_playerBot.transform);
        EndCurrentFight();
    }
    /// <summary>
    /// make a victory for player
    /// </summary>
    private void BotPlayerVictory()
    {
        _fightState = FightState.BotPlayerVictory;
        _cameraView.RemoveObjectToView(_AIBot.transform);

        if (S_DataGame.Instance)
            S_DataGame.Instance.inventory.CurrentMoney += 70;

        EndCurrentFight();
    }
    /// <summary>
    /// disable all bot in the street and zoom on sur winner
    /// </summary>
    private void EndCurrentFight()
    {
        SetEnableBots(false);
        _uiParentInFight.SetActive(false);
        _timerUpdate = false;

        StartCoroutine(AnimationZoom(() =>
        {
            _uiEndFightWinner.SetActive(true);
            _onStreetFightEnd?.Invoke();
            DisplayEndFightText();
        }));
    }
    /// <summary>
    /// get all weapon dropped and delete it
    /// </summary>
    private void ClearDroppedWeapon()
    {
        var droppedWeapons = GameObject.FindGameObjectsWithTag("Weapon");

        foreach (var weapon in droppedWeapons)
            Destroy(weapon);
    }
    /// <summary>
    /// destroy the last bot and create new bot
    /// </summary>
    private IEnumerator CreateStreetFightBot(System.Action endCreationOfBots)
    {
        Destroy(_AIBot);
        Destroy(_playerBot);

        yield return new WaitForSeconds(.1f);

        _playerBot = Instantiate(_playerBotPrefab,   _botPlayerTransformSpawn.position,  Quaternion.Euler(_botPlayerTransformSpawn.eulerAngles));
        _AIBot     = Instantiate(_AIBotPrefab,       _botAITransformSpawn.position,      Quaternion.Euler(_botAITransformSpawn.eulerAngles));

        _AIController = _AIBot.GetComponent<S_AIController>();
        _playerInput = _playerBot.GetComponent<PlayerInput>();
        _AIBot.GetComponent<S_AIStatsController>().BotDifficulty = S_AIStatsController.BotRank.Diamond;

        _AIBot.tag = "BotA";
        _playerBot.tag = "BotB";
        _AIBot.GetComponent<S_AIController>().EnemyTag = "BotB";

        BindDeadEventForBots();
        SetEnableBots(false);

        _skillsController.InitializeSkills(_playerFrame);

        endCreationOfBots?.Invoke();
    }
    /// <summary>
    /// Select the text for display in the end text
    /// </summary>
    private void DisplayEndFightText()
    {
        _endFightTextDefeat.SetActive(false);
        _endFightTextVictory.SetActive(false);

        switch (_fightState)
        {
            case FightState.BotAIVictory:
                _endFightTextDefeat.SetActive(true);
                break;
            case FightState.BotPlayerVictory:
                _endFightTextVictory.SetActive(true);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// look the distance and if one is not in the radius he lose
    /// </summary>
    private void DetectDefeatRadius()
    {
        if (_playerBot == null || _AIBot == null)
            return;

        float distanceDefeatPlayer = Vector3.Distance(transform.position, _playerBot.transform.position);
        float distanceDefeatEnemy = Vector3.Distance(transform.position, _AIBot.transform.position);

        if (distanceDefeatEnemy > _radiusDefeat)
        {
            _endFightConditionText.text = "the enemy is out of the circle".ToUpper();
            BotPlayerVictory();
        }

        if (distanceDefeatPlayer > _radiusDefeat)
        {
            _endFightConditionText.text = "the player is out of the circle".ToUpper();
            BotAiVictory();
        }
    }
    /// <summary>
    /// update the health bar of bots
    /// </summary>
    private void UpdateHealthBarUI()
    {
        if (!InFight)
            return;

        List<(Image, S_FrameManager)> healthBarTuple = new()
        {
            (_healthBarEnemy, _enemyFrame),
            (_healthBarPlayer, _playerFrame)
        };

        foreach (var hp in healthBarTuple)
        {
            Image healthBar = hp.Item1;
            S_FrameManager frame = hp.Item2;

            healthBar.fillAmount = frame.PercentLife;
            healthBar.color = Color.HSVToRGB(Mathf.Lerp(0f, 120f, frame.PercentLife) / 360f, 1f, 1f);
        }
    }
    /// <summary>
    /// update the timer ui
    /// </summary>
    private void UpdateTimeUI()
    {
        if (!_timerUpdate)
            return;

        _secondsTimer -= Time.deltaTime;
        if (_secondsTimer <= 0f)
        {
            _minutesTimer--;
            _secondsTimer += 59f;

            if (_minutesTimer < 0f)
                TimerEnd();
        }

        _timerStreetFight.text = _minutesTimer.ToString("00") + " : " + _secondsTimer.ToString("00");
    }
    /// <summary>
    /// lauch whne the minutes is lower or equal at 0
    /// </summary>
    private void TimerEnd()
    {
        float percentLifePlayer = _playerFrame.PercentLife;
        float percentLifeEnemy = _enemyFrame.PercentLife;

        if (percentLifePlayer > percentLifeEnemy)
        {
            BotPlayerVictory();
            _endFightConditionText.text = "the player to win with more life".ToUpper();
        }
        else
        {
            BotAiVictory();
            _endFightConditionText.text = "the enemy won with more life".ToUpper();
        }
    }

    /// <summary>
    /// create a timer with each sprite in the array for the timer
    /// </summary>
    /// <param name="endTimer">launch at the end of the array</param>
    private IEnumerator TimerBeforeStart(System.Action endTimer)
    {
        foreach (var number in _numberForTimer)
        {
            _timerBeforeStart.sprite = number;
            yield return new WaitForSeconds(1f);
        }

        _timerBeforeStart.gameObject.SetActive(false);
        _fightParentText.SetActive(true);
        yield return new WaitForSeconds(1f);
        _fightParentText.SetActive(false);

        endTimer?.Invoke();
    }
    /// <summary>
    /// update field of view
    /// </summary>
    /// <param name="endZoomAnimation">launch when the field of view is equal at 5</param>
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
