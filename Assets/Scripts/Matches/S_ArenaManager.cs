using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_ArenaManager : MonoBehaviour
{
    [SerializeField] private GameObject _participantsStats;
    [SerializeField] private GameObject _p1Stats;
    [SerializeField] private GameObject _p2Stats;
    [SerializeField] private GameObject _keypad;
    [SerializeField] private GameObject _key;

    [SerializeField] private S_CameraView _cameraView;
    [SerializeField] private S_TournamentManager _tournamentManager;

    [SerializeField] private GameObject _matchUI;
    [SerializeField] private GameObject _botPlayerPrefab, _botEnemyPrefab;
    [SerializeField] private Transform _botPosition1, _botPosition2;

    private GameObject _bot1, _bot2;
    private S_BetSystem _betSystem;

    private S_TournamentManager.Participant _p1;
    private S_TournamentManager.Participant _p2;

    private EventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
        _betSystem = _keypad.GetComponent<S_BetSystem>();
    }
    private void Start()
    {
        _participantsStats.SetActive(false);
        _matchUI.SetActive(false);
    }

    /// <summary>
    /// Start the match
    /// TODO : need to implement what really start the match and not just the UI change
    /// </summary>
    public void StartMatch()
    {
        _participantsStats.SetActive(false);
        _matchUI.SetActive(true);
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_matchUI.transform.GetChild(0).gameObject);

        _betSystem.ResetBetText();
        EnableBot();
    }
    /// <summary>
    /// Create all bot in the arena
    /// </summary>
    private void CreateBot()
    {
        ResetAreneBot();

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
                newBot = Instantiate(_botPlayerPrefab, bot.Item1.position, Quaternion.Euler(bot.Item1.eulerAngles));
                var frame = newBot.GetComponent<S_FrameManager>();
                frame.SelectWeapons();
            }
            else
            {
                newBot = Instantiate(_botEnemyPrefab, bot.Item1.position, Quaternion.Euler(bot.Item1.eulerAngles));
                var stats = newBot.GetComponent<S_AIStatsController>();
                var aiController = newBot.GetComponent<S_AIController>();

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
    private void DesableBot()
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
    private void ResetAreneBot()
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
    }
    /// <summary>
    /// set the on die event in the bot one and two
    /// </summary>
    private void SetBotEventForEndGame()
    {
        var frameBot1 = _bot1.GetComponent<S_FrameManager>();
        var frameBot2 = _bot2.GetComponent<S_FrameManager>();

        frameBot1.OnDie += (_) =>
        {
            CancelMatch();
            _tournamentManager.MakeWinForParticipantTwo();
            _cameraView.StartReturnToTournament();
            DesableBot();
        };
        frameBot2.OnDie += (_) =>
        {
            CancelMatch();
            _tournamentManager.MakeWinForParticipantOne();
            _cameraView.StartReturnToTournament();
            DesableBot();
        };
    }

    public void CancelMatch()
    {
        _matchUI?.SetActive(false);
    }

    /// <summary>
    /// Will show the name, logo, and rating of the current participants of the match
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public void ShowStats(S_TournamentManager.Participant p1, S_TournamentManager.Participant p2)
    {
        _p1 = p1;
        _p2 = p2;

        CreateBot();
        SetBotTag();
        SetBotEventForEndGame();

        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_bot1.transform);
        _cameraView.AddObjectToView(_bot2.transform);

        if (p1.isPlayer ||p2.isPlayer)
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
