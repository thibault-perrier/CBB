using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        _eventSystem = EventSystem.current;

        _cameraView.ShowOffComplete += OnShowOffComplete; // Subscribe to the event
        _cameraView.FadeInComplete += OnFadeInComplete;
        _cameraView.ReturnToToTournamentComplete += OnReturnToTournament;

        _tournamentUI = _botMatchButtons.transform.parent.gameObject;

        if (!_tournamentManager.IsRunning)
        {
            InitializeLogo(_eightParticipantsBracket.transform);
            _currentUsedBracket = _eightParticipantsBracket;
        }

        _botMatchButtons.SetActive(false);
        _playerMatchButton.SetActive(false);

        _cameraView.StartShowOffObjects(_logos[_logos.Count - 1].gameObject, _logos[0].gameObject);
    }

    private void OnDestroy()
    {
        _cameraView.ShowOffComplete -= OnShowOffComplete;
        _cameraView.FadeInComplete -= OnFadeInComplete;
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
    private void OnFadeInComplete()
    {
        S_TournamentManager.Participant p1 = _tournamentManager.GetParticipants()[_currentMatch * 2];
        S_TournamentManager.Participant p2 = _tournamentManager.GetParticipants()[_currentMatch * 2 + 1];

        _arenaManager.ShowStats(p1, p2);
    }

    /// <summary>
    /// When the camera is done returning in front of the
    /// Tournament UI it will resume showing winner and loser
    /// </summary>
    private void OnReturnToTournament()
    {
        _cameraView.AddObjectToView(_logos[_currentMatch * 2]);
        _cameraView.AddObjectToView(_logos[_currentMatch * 2 + 1]);
        SkipBotMatch();
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
                StartCoroutine(LoserMoveBack(bracket, currentMatch, currentRound, loserLogo.gameObject));
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
    public void SkipBotMatch()
    {
        if (_tournamentManager.IsEven())
        {
            ClosePopUpButton();
            _tournamentManager.SimulateMatch();

            UpdateWinnerLogo(_currentUsedBracket.transform, _currentLevel, _currentMatch);
            _betSystem.WinBet(); //check if the player has won the bet
            _betSystem.SetHasBet(false);
        }
    }

    public void BetMatch()
    {
        if (_cameraView != null)
        {
            ClosePopUpButton();
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
                _eventSystem.SetSelectedGameObject(_botMatchButtons.transform.GetChild(0).gameObject);
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
    private IEnumerator LoserMoveBack(Transform bracket, int currentMatch, int currentLevel, GameObject loser)
    {
        Vector3 endPos = loser.transform.position - new Vector3(0, 42f, 0f);

        //_cameraView.RemoveObjectToView(loser.transform);

        while (Vector3.SqrMagnitude(loser.transform.position - endPos) > 0.1f)
        {
            loser.transform.position = Vector3.MoveTowards(loser.transform.position, endPos, Time.deltaTime * _logoSpeed);
            yield return null;
        }

        loser.transform.position = endPos;
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
        //Add more system that allow the player to win his prize and open another UI for the occasion
    }

    public void OnChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnActivateUI(GameObject gameObj)
    {
        gameObj.SetActive(true);
    }

    public void OnDeactivateUI(GameObject gameObj)
    {
        gameObj.SetActive(false);
    }
}