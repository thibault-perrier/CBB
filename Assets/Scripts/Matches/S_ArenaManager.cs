using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(S_RobotSpawner))]
public class S_ArenaManager : MonoBehaviour
{
    [Header("Bet display")]
    [SerializeField] private GameObject _participantsStats;
    [SerializeField] private GameObject _p1Stats;
    [SerializeField] private GameObject _p2Stats;
    [SerializeField] private GameObject _keypad;
    [SerializeField] private GameObject _key;

    [Header("Managers")]
    [SerializeField] private S_CameraView _cameraView;
    [SerializeField] private S_TournamentManager _tournamentManager;

    [Header("Match UI")]
    [SerializeField] private GameObject _matchUI;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _playerUI;
    [SerializeField] private GameObject _botsUI;
    [SerializeField] private S_SkillsController _skills;
    private Image _participant1Health;
    private Image _participant2Health;

    [Header("Bot Initialization")]
    [SerializeField] private GameObject _botPlayerPrefab, _botEnemyPrefab;
    [SerializeField] private Transform _botPosition1, _botPosition2;

    private GameObject _bot1, _bot2;
    private S_BetSystem _betSystem;
    private float _minutesTimer, _secondsTimer;
    private bool _timerRunning = false;

    private S_TournamentManager.Participant _p1;
    private S_TournamentManager.Participant _p2;

    private EventSystem _eventSystem;

    public float MatchDuration = 3f;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
        _betSystem = _keypad.GetComponent<S_BetSystem>();
    }
    private void Start()
    {
        _participantsStats.SetActive(false);
        _matchUI.SetActive(false);
        _playerUI.SetActive(false);
        _botsUI.SetActive(false);
    }
    private void Update()
    {
        if (_timerRunning)
        {
            _secondsTimer -= Time.deltaTime;
            if (_secondsTimer <= 0f)
            {
                if (_minutesTimer <= 0f && _secondsTimer <= 0f)
                    TimerFinish();

                _secondsTimer += 60f;
                _minutesTimer--;
            }

            SetTimerText();
            UpdateHPbars();
        }
    }

    /// <summary>
    /// Start the match
    /// TODO : need to implement what really start the match and not just the UI change
    /// </summary>
    public void StartMatch()
    {
        _participantsStats.SetActive(false);
        InitializeMatchUI();

        _betSystem.ResetBetText();
        EnableBot();
        _timerRunning = true;
    }
    /// <summary>
    /// Initialize the UI, it changes if the match is bot vs bot or player vs bot
    /// </summary>
    private void InitializeMatchUI()
    {
        _matchUI.SetActive(true);
        _playerUI.SetActive(false);
        _botsUI.SetActive(false);

        GameObject currentUI = null;

        if (_p1.isPlayer || _p2.isPlayer)
        {
            _playerUI.SetActive(true);
            currentUI = _playerUI;

            if (_p1.isPlayer)
            {
                var frame = _bot1.GetComponent<S_FrameManager>();
                _skills.InitializeSkills(frame);
            }
            else
            {
                var frame = _bot2.GetComponent<S_FrameManager>();
                _skills.InitializeSkills(frame);
            }
        }
        else
        {
            _botsUI.SetActive(true);
            currentUI = _botsUI;
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(currentUI.transform.Find("Skip match").gameObject);
        }

        if (currentUI != null)
        {
            _participant1Health = currentUI.transform.Find("ChallengerLeft").transform.Find("Health").transform.Find("Forward").GetComponent<Image>();
            _participant2Health = currentUI.transform.Find("ChallengerRight").transform.Find("Health").transform.Find("Forward").GetComponent<Image>();

            //TODO : change color for Logo
            currentUI.transform.Find("ChallengerLeft").transform.Find("Logo").GetComponent<Image>().color = _p1.logo;
            currentUI.transform.Find("ChallengerRight").transform.Find("Logo").GetComponent<Image>().color = _p2.logo;
        }
    }
    /// <summary>
    /// Change the fill amount of the hp bar and the color depending of the total HP of the participants
    /// </summary>
    private void UpdateHPbars()
    {
        if (_participant1Health != null)
        {
            float botLife = _bot1.GetComponent<S_FrameManager>().PercentLife;
            _participant1Health.fillAmount = botLife;
            _participant1Health.color = Color.HSVToRGB(Mathf.Lerp(0f, 120f, botLife)/360f, 1f, 1f);
        }
        if (_participant2Health != null)
        {
            float botLife = _bot2.GetComponent<S_FrameManager>().PercentLife;
            _participant2Health.fillAmount = botLife;

            _participant2Health.color = Color.HSVToRGB(Mathf.Lerp(0f, 120f, botLife)/360f, 1f, 1f);
        }
    }
    /// <summary>
    /// Create all bot in the arena
    /// </summary>
    private void CreateBot()
    {
        ResetArenaBot();
        S_RobotSpawner robotSpawner = GetComponent<S_RobotSpawner>();

        // set all pair with bot and bot info
        List<(Transform, S_TournamentManager.Participant)> botPair = new(2)
        {
            (_botPosition1, _p1),
            (_botPosition2, _p2)
        };

        // for each bot in arena
        foreach (var bot in botPair)
        {
            GameObject newBot;
            // if one participant is the player
            if (bot.Item2.isPlayer)
            {
                newBot = robotSpawner.GenerateRobotAt(bot.Item2.robot, bot.Item1);
                var frame = newBot.GetComponent<S_FrameManager>();
                frame.SelectWeapons();

                var ai = newBot.GetComponent<S_AIController>();
                ai.enabled = false;

                var playerInput = newBot.GetComponent<PlayerInput>();
                playerInput.enabled = true;
            }
            else
            {
                newBot = robotSpawner.GenerateRobotAt(bot.Item2.robot, bot.Item1);
                var stats = newBot.GetComponent<S_AIStatsController>();
                var aiController = newBot.GetComponent<S_AIController>();
                var playerInput = newBot.GetComponent<PlayerInput>();
                playerInput.enabled = false;
                var ai = newBot.GetComponent<S_AIController>();
                ai.enabled = true;

                aiController.State = S_AIController.AIState.Disable;
                stats.BotDifficulty = (S_AIStatsController.BotRank)bot.Item2.rank;
            }

            if (!_bot1)
                _bot1 = newBot;
            else
                _bot2 = newBot;
        }
    }
    /// <summary>
    /// set the bot tag and the controller target tag
    /// </summary>
    private void SetBotTag()
    {
        _bot1.transform.tag = "BotA";
        _bot2.transform.tag = "BotB";

        if (_bot1.TryGetComponent<S_AIController>(out var crtlA))
            crtlA.EnemyTag = "BotB";

        if (_bot2.TryGetComponent<S_AIController>(out var crtlB))
            crtlB.EnemyTag = "BotA";
    }
    /// <summary>
    /// for each bot with AIController enable the state
    /// </summary>
    private void EnableBot()
    {
        List<GameObject> bots = new()
        {
            _bot1,
            _bot2,
        };

        foreach (var bot in bots)
        {
            if (bot.TryGetComponent<S_AIController>(out var crtl))
                crtl.State = S_AIController.AIState.Enable;
        }
    }
    /// <summary>
    /// for each bot with ai controller desable the state
    /// </summary>
    private void DisableBot()
    {
        List<GameObject> bots = new()
        {
            _bot1,
            _bot2,
        };

        foreach (var bot in bots)
        {
            if (bot.TryGetComponent<S_AIController>(out var crtl))
                crtl.State = S_AIController.AIState.Disable;
        }
    }
    /// <summary>
    /// Reset the bot with Untagged and destroy current bot
    /// </summary>
    private void ResetArenaBot()
    {
        if (_bot1)
        {
            _bot1.transform.tag = "Untagged";
            Destroy(_bot1);
            _bot1 = null;
        }

        if (_bot2)
        {
            _bot2.transform.tag = "Untagged";
            Destroy(_bot2);
            _bot2 = null;
        }

        DestroyAllDroppedWeapon();
    }
    /// <summary>
    /// set the on die event in the bot one and two
    /// </summary>
    private void SetBotEventForEndGame()
    {
        var frameBot1 = _bot1.GetComponent<S_FrameManager>();
        var frameBot2 = _bot2.GetComponent<S_FrameManager>();

        var immobileBot1 = _bot1.GetComponent<S_ImmobileDefeat>();
        var immobileBot2 = _bot2.GetComponent<S_ImmobileDefeat>();

        frameBot1.OnDie += (_) => Bot2Win();
        frameBot2.OnDie += (_) => Bot1Win();

        immobileBot1.IsImmobile += () => Bot2Win();
        immobileBot2.IsImmobile += () => Bot1Win();
    }
    private void Bot1Win()
    {
        DisableDeathBot();
        CancelMatch();
        _tournamentManager.MakeWinForParticipantOne();
        _cameraView.StartReturnToTournament();
        DisableBot();
    }
    private void Bot2Win()
    {
        DisableDeathBot();
        CancelMatch();
        _tournamentManager.MakeWinForParticipantTwo();
        _cameraView.StartReturnToTournament();
        DisableBot();
    }
    private void DisableDeathBot()
    {
        var frameBot1 = _bot1.GetComponent<S_FrameManager>();
        var frameBot2 = _bot2.GetComponent<S_FrameManager>();
        var immobileBot1 = _bot1.GetComponent<S_ImmobileDefeat>();
        var immobileBot2 = _bot2.GetComponent<S_ImmobileDefeat>();

        frameBot1.enabled = false;
        frameBot2.enabled = false;
        immobileBot1.enabled = false;
        immobileBot2.enabled = false;
    }
    public void OnSimulateMatch()
    {
        CancelMatch();
        _tournamentManager.SimulateMatch();
        _cameraView.StartReturnToTournament();
        DisableBot();
    }
    private void DestroyAllDroppedWeapon()
    {
        var weaponsDropped = GameObject.FindGameObjectsWithTag("Weapon");

        foreach (var weapon in weaponsDropped)
        {
            Destroy(weapon);
        }
    }

    public void CancelMatch()
    {
        _matchUI?.SetActive(false);
    }
    public void SetTimerText()
    {
        _timerText.text = _minutesTimer.ToString("00") + ":" + _secondsTimer.ToString("00");
    }
    public void ResetTimer()
    {
        _minutesTimer = MatchDuration;
        _secondsTimer = 0f;
        _timerRunning = false;
        SetTimerText();
    }
    public void TimerFinish()
    {
        var frameBot1 = _bot1.GetComponent<S_FrameManager>();
        var frameBot2 = _bot2.GetComponent<S_FrameManager>();

        if (frameBot1.PercentLife > frameBot2.PercentLife)
        {
            Bot1Win();
            return;
        }

        Bot2Win();
    }

    /// <summary>
    /// Will show the name, logo, and rating of the current participants of the match
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public void ShowStats(S_TournamentManager.Participant p1, S_TournamentManager.Participant p2)
    {
        InitializeBetButtons();
        ResetTimer();

        _p1 = p1;
        _p2 = p2;

        CreateBot();
        SetBotTag();
        SetBotEventForEndGame();

        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_bot1.transform);
        _cameraView.AddObjectToView(_bot2.transform);

        if (p1.isPlayer || p2.isPlayer)
        {
            StartMatch();
            return;
        }

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_p1Stats.transform.GetChild(0).gameObject);

        _participantsStats.SetActive(true);

        SetStatsOnUi(p1.rating.ToString(), p1.name, p1.logo, _p1Stats);
        SetStatsOnUi(p2.rating.ToString(), p2.name, p2.logo, _p2Stats);
    }
    private void InitializeBetButtons()
    {
        Button[] buttons = _participantsStats.GetComponentsInChildren<Button>();

        if (buttons.Length > 0)
        {
            foreach (Button button in buttons)
            {
                button.enabled = true;
            }
        }
    }
    public void ShowMatch(S_TournamentManager.Participant p1, S_TournamentManager.Participant p2)
    {
        ResetTimer();

        _p1 = p1;
        _p2 = p2;

        CreateBot();
        SetBotTag();
        SetBotEventForEndGame();
        StartMatch();

        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_bot1.transform);
        _cameraView.AddObjectToView(_bot2.transform);
    }

    /// <summary>
    /// Set the values of the participant into the UI
    /// </summary>
    /// <param name="rating"></param>
    /// <param name="name"></param>
    /// <param name="logo"></param>
    /// <param name="pStatGameObject"></param>
    public void SetStatsOnUi(string rating,  string name, Color logo , GameObject pStatGameObject)
    {
        _betSystem.ActivateButtons();

        TextMeshProUGUI ratingTxt = pStatGameObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameTxt = pStatGameObject.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

        Image logoImage = pStatGameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>();

        ratingTxt.text = rating;
        nameTxt.text = name;
        logoImage.color = logo;
    }

    public void CloseStats()
    {
        _participantsStats.SetActive(false);
    }

    /// <summary>
    /// Open or close (if it's already opened) the key pad to write the bet amount
    /// </summary>
    public void OpenCloseKeypad()
    {
        if (_keypad.activeSelf)
        {
            _keypad.SetActive(false);
        }
        else
        {
            _keypad.SetActive(true);
            //_eventSystem.SetSelectedGameObject(_key.transform.GetChild(0).gameObject);
        }
    }

    // The use of two separate methods was needed because it's was complicated to give a reference to the participants in the editor

    /// <summary>
    /// Set the participant inside the bet method to keep track for which one the player bet
    /// </summary>
    public void BetForParticipantOne()
    {
        _betSystem.SetChosenParticipant(_p1);
    }

    public void BetForParticipantTwo()
    {
        _betSystem.SetChosenParticipant(_p2);
    }
}
