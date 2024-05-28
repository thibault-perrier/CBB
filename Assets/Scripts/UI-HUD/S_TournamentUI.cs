using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class S_TournamentBracket : MonoBehaviour
{
    [SerializeField] private S_TournamentManager _tournamentManager;
    [SerializeField] S_CameraView _cameraView;

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

    private void Start()
    {
        if (!_tournamentManager.IsRunning)
        {
            InitializeLogo(_eightParticipantsBracket.transform);
            _currentUsedBracket = _eightParticipantsBracket;
        }

        _botMatchButtons.SetActive(false);
        _playerMatchButton.SetActive(false);

        _movingLogoCoroutine = StartCoroutine(MoveTowardNewRound(_currentUsedBracket.transform, _logos[_currentMatch].gameObject, _logos[_currentMatch + 1].gameObject, _currentLevel, _currentMatch));
    }

    //Put the logo of the participant in the bottom of the bracket
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

    /* After a match it will put the winner's logo at the right place */
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
                StartCoroutine(LoserMoveBack(loserLogo.gameObject));
            }
        }
    }


    public void AddWinnerLogo(Transform winner, Transform loser)
    {
        _winnersLogo.Add(winner);
        _losersLogo.Add(loser);
    }

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
        }
    }

    /* Check which match and round it is to update the correct logos */
    public void UpdateMatchAndRound()
    {
        _currentMatch = _tournamentManager.GetMatchNb();
        _currentLevel = _tournamentManager.GetRoundNb();
    }

    public void PopUpButton()
    {
        List<S_TournamentManager.Participant> participants = _tournamentManager.GetParticipants();

        if (participants != null && _tournamentManager.IsEven())
        {
            S_TournamentManager.Participant p1 = participants[_currentMatch * 2]; //need to multiply so the previous participant does not fight again
            S_TournamentManager.Participant p2 = participants[(_currentMatch * 2) + 1];

            if (_tournamentManager.IsPlayerPlaying(p1, p2))
            {
                ActivateUI(_playerMatchButton, true);
                ActivateUI(_botMatchButtons, false);
            }
            else
            {
                ActivateUI(_botMatchButtons, true);
                ActivateUI(_playerMatchButton, false);
            }
        }
    }

    public void ClosePopUpButton()
    {
        ActivateUI(_playerMatchButton, false);
        ActivateUI(_botMatchButtons, false);
    }

    //Activate or deactivate a button
    public void ActivateUI(GameObject button, bool active)
    {
        button.SetActive(active);
    }

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

        while (Vector3.Distance(p1.transform.position, waypoints.GetChild(0).position) > 0.1f)
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

        while (Vector3.Distance(winner.transform.position, waypoint.position) > 0.1f)
        {
            winner.transform.position = Vector3.MoveTowards(winner.transform.position, waypoint.transform.position, Time.deltaTime * _logoSpeed);
            yield return null;
        }

        winner.transform.position = waypoint.transform.position;

        _tournamentManager.NextMatch();
        UpdateMatchAndRound();

        if (_tournamentManager.IsEven())
        {
            _movingLogoCoroutine = StartCoroutine(MoveTowardNewRound(bracket,
                _logos[_currentMatch * 2].gameObject, 
                _logos[_currentMatch * 2 + 1].gameObject,
                _currentLevel, _currentMatch));
        }
        else
        {
            _movingLogoCoroutine = StartCoroutine(MoveTournamentWinner(bracket, currentLevel, currentMatch, _logos[currentMatch * 2].gameObject));
        }
    }

    /// <summary>
    /// The loser move back at the bottom after a match
    /// </summary>
    /// <param name="loser"></param>
    /// <returns></returns>
    private IEnumerator LoserMoveBack(GameObject loser)
    {
        Vector3 endPos = loser.transform.position - new Vector3(0, 42f, 0f);

        _cameraView.RemoveObjectToView(loser.transform);

        while (Vector3.Distance(loser.transform.position, endPos) > 0.1f)
        {
            loser.transform.position = Vector3.MoveTowards(loser.transform.position, endPos, Time.deltaTime * _logoSpeed);
            yield return null;
        }

        loser.transform.position = endPos;
    }

    private IEnumerator MoveTournamentWinner(Transform bracket, int currentMatch, int currentLevel, GameObject winner)
    {
        ClosePopUpButton();

        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(winner.transform);

        Transform waypoint = bracket.GetChild(2).GetChild(3).GetChild(0);

        while (Vector3.Distance(winner.transform.position, waypoint.position) > 0.1f)
        {
            winner.transform.position = Vector3.MoveTowards(winner.transform.position, waypoint.transform.position, Time.deltaTime * _logoSpeed);
            yield return null;
        }

        winner.transform.position = waypoint.transform.position;
    }
}