using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_TournamentBracket : MonoBehaviour
{
    [SerializeField] private S_TournamentManager _tournamentManager;
    [SerializeField] S_CameraView _cameraView;
    [SerializeField] private S_ArenaManager _arenaManager;
    [SerializeField] private S_BetSystem _betSystem;

    [Header("Tournament brackets")]
    [SerializeField] private GameObject _eightParticipantsBracket;
    [SerializeField] private GameObject _sixteenParticipantsBracket;

    private GameObject _currentUsedBracket;

    [Header("Match buttons")]
    [SerializeField] private GameObject _botMatchButtons;
    [SerializeField] private GameObject _playerMatchButton;

    [Header("Win / Lost text")]
    [SerializeField] private GameObject _playerLose;
    [SerializeField] private GameObject _playerWin;
    [SerializeField] private GameObject _tournamentPrizeDisplay;
    [SerializeField] private GameObject _sparksEffect;

    private int _currentLevel = 0;
    private int _currentMatch = 0;

    public float _logoSpeed = 20f;

    private List<Transform> _winnersLogo = new List<Transform>();
    private List<Transform> _losersLogo = new List<Transform>();

    private List<Transform> _points = new List<Transform>();
    private List<Transform> _logos = new List<Transform>();
    private List<Transform> _waypoints = new List<Transform>();

    private Coroutine _movingLogoCoroutine;
    private EventSystem _eventSystem;

    private GameObject _tournamentUI;

    private void Awake()
    {
        _sparksEffect.SetActive(false);
    }

    private void Start()
    {
        _eventSystem = EventSystem.current;

        _cameraView.ShowOffComplete += OnShowOffComplete; // Subscribe to the event
        _cameraView.ReturnToToTournamentComplete += OnReturnToTournament;

        _tournamentUI = _botMatchButtons.transform.parent.gameObject;

        if (!_tournamentManager.IsRunning)
        {
            InitializeLogo(_currentUsedBracket.transform);
            //_currentUsedBracket = _eightParticipantsBracket;
        }

        _botMatchButtons.SetActive(false);
        _playerMatchButton.SetActive(false);
        _playerLose.SetActive(false);
        _playerWin.SetActive(false);

        _cameraView.ClearObjectToView();
        foreach (Transform t in _currentUsedBracket.transform.GetChild(3))
        {
            Debug.Log(t.name);
            _cameraView.AddObjectToView(t);
        }

        _cameraView.IsTournamentView = true;

        _cameraView.StartShowOffObjects(_logos[_logos.Count - 1].gameObject, _logos[0].gameObject);
    }

    private void OnDestroy()
    {
        _cameraView.ShowOffComplete -= OnShowOffComplete;
        _cameraView.FadeInComplete -= OnFadeInCompleteShowState;
        _cameraView.ReturnToToTournamentComplete -= OnReturnToTournament;
    }

    /// <summary>
    /// When the camera is done showing the whole participants it will start to move the participants
    /// </summary>
    private void OnShowOffComplete()
    {
        _movingLogoCoroutine = StartCoroutine(MoveTowardNewRound(_currentUsedBracket.transform,
            _logos[_currentMatch].gameObject,
            _logos[_currentMatch + 1].gameObject, _currentLevel, _currentMatch));
    }

    /// <summary>
    /// When the camera is done with the fade in it will set the ccurrent participants
    /// and show their stats in the UI
    /// </summary>
    private void OnFadeInCompleteShowState()
    {
        S_TournamentManager.Participant p1 = _tournamentManager.GetParticipants()[_currentMatch * 2];
        S_TournamentManager.Participant p2 = _tournamentManager.GetParticipants()[_currentMatch * 2 + 1];

        _cameraView.FadeInComplete -= OnFadeInCompleteShowState;
        _arenaManager.ShowStats(p1, p2);
    }
    private void OnFadeCompleteShowMatch()
    {
        S_TournamentManager.Participant p1 = _tournamentManager.GetParticipants()[_currentMatch * 2];
        S_TournamentManager.Participant p2 = _tournamentManager.GetParticipants()[_currentMatch * 2 + 1];

        _cameraView.FadeInComplete -= OnFadeCompleteShowMatch;
        _arenaManager.ShowMatch(p1, p2);
    }

    /// <summary>
    /// When the camera is done returning in front of the
    /// Tournament UI it will resume showing winner and loser
    /// </summary>
    private void OnReturnToTournament()
    {
        DisplayBetScreen();
        _cameraView.AddObjectToView(_logos[_currentMatch * 2]);
        _cameraView.AddObjectToView(_logos[_currentMatch * 2 + 1]);
        EndBotMatch();
    }

    /// <summary>
    /// Put the logo of the participant in the bottom of the bracket
    /// </summary>
    /// <param name="bracket"></param>
    public void InitializeLogo(Transform bracket)
    {
        List<S_TournamentManager.Participant> participants = _tournamentManager.GetParticipants();

        if (_tournamentManager != null && participants != null)
        {
            Transform points = bracket.GetChild(0).GetChild(0);
            Transform logos = bracket.GetChild(1);
            Transform waypoints = bracket.GetChild(2);

            if (participants.Count > 0)
            {
                int i = 0;
                foreach (Transform point in points)
                {
                    _points.Add(point);

                    Transform logo = logos.GetChild(i);
                    _logos.Add(logo);

                    logo.position = point.position;
                    logo.GetComponent<Image>().color = participants[i].logo;
                    logo.GetComponent<Image>().sprite = participants[i].logoSprite;

                    if (_cameraView != null)
                    {
                        _cameraView.IsTournamentView = true;
                    }
                    i++;
                }
                foreach (Transform waypoint in waypoints)
                {
                    _waypoints.Add(waypoint);
                }
            }
            else
            {
                Debug.LogWarning("There is no participants !");
            }
        }
        else
        {
            Debug.LogWarning("There is maybe no reference to S_TournamentManager, or the list of participants is null.");
        }
        _tournamentManager.IsRunning = true;
    }

    /// <summary>
    /// After a match it will put the winner's logo at the right place
    /// </summary>
    /// <param name="bracket"></param>
    /// <param name="currentRound"></param>
    /// <param name="currentMatch"></param>
    public void UpdateWinnerLogo(Transform bracket, int currentRound, int currentMatch)
    {
        List<S_TournamentManager.Participant> winners = _tournamentManager.GetRoundWinners();

        if (_tournamentManager != null && winners != null)
        {
            if (currentRound < _points.Count - 1)
            {
                Transform winnerLogo = _winnersLogo[currentMatch];
                Transform loserLogo = _losersLogo[currentMatch];
                _movingLogoCoroutine = StartCoroutine(MoveWinner(bracket, currentMatch, currentRound, winnerLogo.gameObject));
                StartCoroutine(LoserMoveBack(loserLogo.gameObject, false));
            }
        }
    }

    /// <summary>
    /// Add the winner's logo into the list of participant's winner logo
    /// </summary>
    /// <param name="winner"></param>
    /// <param name="loser"></param>
    public void AddWinnerLogo(Transform winner, Transform loser)
    {
        _winnersLogo.Add(winner);
        _losersLogo.Add(loser);
    }

    /// <summary>
    /// Clear the list of logos after each level (or round) to keep track of winners and participants
    /// </summary>
    public void RefreshWinnerLogo()
    {
        _logos = new List<Transform>(_winnersLogo);

        _losersLogo.Clear();
        _winnersLogo.Clear();
    }

    /// <summary>
    /// If the player don't want to watch the match it will simulate a match and get a random winner based on rating Also update automatically the logo in the UI
    /// </summary>
    public void EndBotMatch()
    {
        if (_tournamentManager.IsEven())
        {
            ClosePopUpButton();

            if (_tournamentManager.GetCurrentLoser().isPlayer)
            {
                StartCoroutine(LoserMoveBack(_losersLogo[_currentMatch].gameObject, true));
                _cameraView.ClearObjectToView();
                _cameraView.AddObjectToView(_losersLogo[_currentMatch]);
            }
            else
            {
                UpdateWinnerLogo(_currentUsedBracket.transform, _currentLevel, _currentMatch);
            }
        }
    }

    public void BetMatch()
    {
        if (_cameraView != null)
        {
            ClosePopUpButton();

            _cameraView.FadeInComplete += OnFadeInCompleteShowState;
            _cameraView.StartZoomFadeIn();
        }
    }

    public void ShowMatch()
    {
        if (_cameraView != null)
        {
            ClosePopUpButton();

            _cameraView.FadeInComplete += OnFadeCompleteShowMatch;
            _cameraView.StartZoomFadeIn();
        }
    }

    /// <summary>
    /// Check which match and round it is to update the correct logos
    /// </summary>
    public void UpdateMatchAndRound()
    {
        _currentMatch = _tournamentManager.GetMatchNb();
        _currentLevel = _tournamentManager.GetRoundNb();
    }

    /// <summary>
    /// Open the buttons that start the matchs or bet depending of who is playing
    /// </summary>
    public void PopUpButton()
    {
        List<S_TournamentManager.Participant> participants = _tournamentManager.GetParticipants();

        if (participants != null && _tournamentManager.IsEven())
        {
            ActivateUI(_tournamentUI, true);

            S_TournamentManager.Participant p1 = participants[_currentMatch * 2]; //need to multiply so the previous participant does not fight again
            S_TournamentManager.Participant p2 = participants[(_currentMatch * 2) + 1];

            if (_tournamentManager.IsPlayerPlaying(p1, p2))
            {
                ActivateUI(_playerMatchButton, true);
                ActivateUI(_botMatchButtons, false);
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_playerMatchButton.transform.GetChild(0).gameObject);
            }
            else
            {
                ActivateUI(_botMatchButtons, true);
                ActivateUI(_playerMatchButton, false);
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_botMatchButtons.transform.GetChild(0).GetChild(0).gameObject);
            }
        }
    }

    /// <summary>
    /// Remove the start matchs/bet from the screen
    /// </summary>
    public void ClosePopUpButton()
    {
        ActivateUI(_tournamentUI, false);
        ActivateUI(_playerMatchButton, false);
        ActivateUI(_botMatchButtons, false);
    }

    /// <summary>
    /// Activate or deactivate a gameObject
    /// </summary>
    /// <param name="button"></param>
    /// <param name="active"></param>
    public void ActivateUI(GameObject button, bool active)
    {
        button.SetActive(active);
    }

    /// <summary>
    /// Display the UI showing the player has lose
    /// TODO : Make a better UI with button to return to the menu
    /// </summary>
    public void PlayerLostScreen()
    {
        ActivateUI(_playerLose, true);
    }

    public List<Transform> GetLogos()
    {
        return _logos;
    }

    /// <summary>
    /// Move the participants of the current match to show they're going to battle
    /// </summary>
    /// <param name="bracket"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="currentLevel"></param>
    /// <param name="currentMatch"></param>
    /// <returns></returns>
    private IEnumerator MoveTowardNewRound(Transform bracket, GameObject p1, GameObject p2, int currentLevel, int currentMatch)
    {
        Transform waypoints = bracket.GetChild(2).GetChild(currentLevel).GetChild(currentMatch);

        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(p1.transform);
        _cameraView.AddObjectToView(p2.transform);

        while (Vector3.SqrMagnitude(p1.transform.position - waypoints.GetChild(0).position) > 0.1f)
        {
            p1.transform.position = Vector3.MoveTowards(p1.transform.position, waypoints.GetChild(0).position, Time.deltaTime * _logoSpeed);
            p2.transform.position = Vector3.MoveTowards(p2.transform.position, waypoints.GetChild(1).position, Time.deltaTime * _logoSpeed);

            yield return null;
        }

        p1.transform.position = waypoints.GetChild(0).position;
        p2.transform.position = waypoints.GetChild(1).position;

        _movingLogoCoroutine = null;
        PopUpButton();
    }

    /// <summary>
    /// Move the winner at the center of the bracket after a match is over
    /// </summary>
    /// <param name="bracket"></param>
    /// <param name="currentMatch"></param>
    /// <param name="currentLevel"></param>
    /// <param name="winner"></param>
    /// <returns></returns>
    private IEnumerator MoveWinner(Transform bracket, int currentMatch, int currentLevel, GameObject winner)
    {
        ClosePopUpButton();

        Transform waypoint = bracket.GetChild(2).GetChild(currentLevel).GetChild(currentMatch);

        while (Vector3.SqrMagnitude(winner.transform.position - waypoint.position) > 0.1f)
        {
            winner.transform.position = Vector3.MoveTowards(winner.transform.position, waypoint.transform.position, Time.deltaTime * _logoSpeed);
            yield return null;
        }

        winner.transform.position = waypoint.transform.position;

        _tournamentManager.NextMatch();
        UpdateMatchAndRound();

        if (_tournamentManager.IsEven())
        {
            _movingLogoCoroutine = StartCoroutine(MoveTowardNewRound(_currentUsedBracket.transform,
                _logos[_currentMatch * 2].gameObject,
                _logos[_currentMatch * 2 + 1].gameObject,
                _currentLevel, _currentMatch));
        }
        else
        {
            _movingLogoCoroutine = StartCoroutine(MoveTournamentWinner(_currentUsedBracket.transform, _logos[_currentMatch * 2].gameObject));
        }

        _movingLogoCoroutine = null;
    }

    /// <summary>
    /// The loser move back at the bottom after a match
    /// </summary>
    /// <param name="loser"></param>
    /// <returns></returns>
    private IEnumerator LoserMoveBack(GameObject loser, bool playerLost)
    {
        Vector3 endPos = loser.transform.position - new Vector3(0, 26f, 0f);

        while (Vector3.SqrMagnitude(loser.transform.position - endPos) > 0.1f)
        {
            loser.transform.position = Vector3.MoveTowards(loser.transform.position, endPos, Time.deltaTime * _logoSpeed);
            yield return null;
        }
        loser.transform.position = endPos;
        if (playerLost)
        {
            yield return new WaitForSeconds(1f);
            PopUpButton();

            foreach (Transform t in _tournamentUI.transform)
            {
                t.gameObject.SetActive(false);
            }
            _playerLose.SetActive(true);

            yield return new WaitForSeconds(3f);
            _cameraView.StartFadeIn();
        }
    }

    /// <summary>
    /// Move the tournament winner at the top of the bracket
    /// </summary>
    /// <param name="bracket"></param>
    /// <param name="currentMatch"></param>
    /// <param name="currentLevel"></param>
    /// <param name="winner"></param>
    /// <returns></returns>
    private IEnumerator MoveTournamentWinner(Transform bracket, GameObject winner)
    {
        ClosePopUpButton();

        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(winner.transform);

        Transform waypoint = bracket.GetChild(2).GetChild(3).GetChild(0);

        while (Vector3.SqrMagnitude(winner.transform.position - waypoint.position) > 0.1f)
        {
            winner.transform.position = Vector3.MoveTowards(winner.transform.position, waypoint.transform.position, Time.deltaTime * _logoSpeed);
            yield return null;
        }

        _movingLogoCoroutine = null;

        winner.transform.position = waypoint.transform.position;

        yield return new WaitForSeconds(1);

        DisplayWinLogo();

        yield return new WaitForSeconds(5f);

        _cameraView.StartFadeIn();
    }

    public void OnChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnActivateUI(GameObject gameObj)
    {
        gameObj.SetActive(true);
        if (gameObj.TryGetComponent(out Button button))
        {
            _eventSystem.SetSelectedGameObject(button.gameObject);
        }
    }

    public void OnDeactivateUI(GameObject gameObj)
    {
        gameObj.SetActive(false);
        if (_tournamentUI.TryGetComponent(out Button button))
        {
            _eventSystem.SetSelectedGameObject(button.gameObject);
        }
    }

    /// <summary>
    /// Display the victory screen with some cool particle effect
    /// </summary>
    public void DisplayWinLogo()
    {
        _tournamentUI.SetActive(true);

        foreach (Transform child in _tournamentUI.transform) //deactivate the ui we don't want to see
        {
            child.gameObject.SetActive(false);
        }

        _playerWin.SetActive(true);
        _tournamentPrizeDisplay.SetActive(true);
        _tournamentPrizeDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "TOURNAMENT : YOU WON $ " + _tournamentManager.GetTournamentPrize() + " !";
        _sparksEffect.SetActive(true);

        S_DataGame.Instance.inventory.CurrentMoney += _tournamentManager.GetTournamentPrize();
        S_DataGame.Instance.SaveInventory();
    }

    /// <summary>
    ///Set the current bracket depending of tournament difficulty
    /// </summary>
    /// <param name="bracketNb">Put 8 for eight participant, 16 for sixteen participant brackets</param>
    public void SetBracket(int bracketNb)
    {
        _eightParticipantsBracket.SetActive(false);
        _sixteenParticipantsBracket.SetActive(false);
        switch (bracketNb)
        {
            case 8:
                _currentUsedBracket = _eightParticipantsBracket;
                _eightParticipantsBracket.SetActive(true);
                Debug.Log("SUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUii8");
                break;
            case 16:
                _currentUsedBracket = _sixteenParticipantsBracket;
                _sixteenParticipantsBracket.SetActive(true);
                Debug.Log("SUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUii16");
                break;
            default:
                Debug.LogError("Please write '8' or '16' to use valid brackets !");
                break;
        }
    }

    public void DisplayBetScreen()
    {
        _betSystem.WinBet();
        _betSystem.SetHasBet(false);
    }

    public void OnSkipMatch()
    {
        _tournamentManager.SimulateMatch();
        OnReturnToTournament();
    }

    /// <summary>
    /// Set as selected a gameobject from the UI for the event system
    /// </summary>
    /// <param name="obj"></param>
    public void OnSetAsSelected(GameObject obj)
    {
        if (obj.activeInHierarchy)
            _eventSystem.SetSelectedGameObject(obj);
    }
}