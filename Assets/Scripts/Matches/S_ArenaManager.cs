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

    [SerializeField] private GameObject _testP1, _testP2, _matchUI;

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
        _eventSystem.SetSelectedGameObject(_matchUI.transform.GetChild(0).gameObject);
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
        _eventSystem.SetSelectedGameObject(_p1Stats.transform.GetChild(0).gameObject);

        _participantsStats.SetActive(true);

        SetStatsOnUi(p1.rating.ToString(), p1.name, p1.logo, _p1Stats);
        SetStatsOnUi(p2.rating.ToString(), p2.name, p2.logo, _p2Stats);

        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_testP1.transform);
        _cameraView.AddObjectToView(_testP2.transform);

        _p1 = p1;
        _p2 = p2;
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
            _eventSystem.SetSelectedGameObject(_key.transform.GetChild(0).gameObject);
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
