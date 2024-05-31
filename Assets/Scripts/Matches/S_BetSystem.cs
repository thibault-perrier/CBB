using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_BetSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _betAmountTxt;
    [SerializeField] private S_TournamentManager _tournamentManager;

    [SerializeField] private Button[] _betScreenButtons;

    [SerializeField] private GameObject _launchMatch;
    private TextMeshProUGUI _launchMatchTxt;

    private int _betAmount = 0;
    private float _currentBetRating = 0;
    private S_TournamentManager.Participant _currentParticipantChosen;

    private EventSystem _eventSystem;

    public delegate void OnStartMatch();
    public OnStartMatch StartMatch;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
        _launchMatchTxt = _launchMatch.GetComponentInChildren<TextMeshProUGUI>();
        _launchMatchTxt.text = "Skip bet";
    }

    private void OnEnable()
    {
        _betAmountTxt.text = "";
        _betAmount = 0;
        _launchMatchTxt.text = "Skip bet";
    }

    public void EnterAmount(int amount)
    {
        if (_betAmountTxt.text.Length < 9)
            _betAmountTxt.text += amount.ToString();
    }

    public void ConfirmBet()
    {
        int betAmount;
        int.TryParse(_betAmountTxt.text, out betAmount);

        //check if player has enough money

        _betAmount = betAmount;

        //remove money from the player

        Debug.Log("You bet : " + _betAmount);

        gameObject.SetActive(false);
        ActivateButtons();

        if (_betAmountTxt.text.Length > 0)
        {
            _launchMatchTxt.text = "Launch Match";
            _eventSystem.SetSelectedGameObject(_launchMatch);

            for (int i = 0; i < _betScreenButtons.Length - 1; i++)
            {
                _betScreenButtons[i].interactable = false;
            }
        }
        else
        {
            _launchMatchTxt.text = "Skip bet";
            _eventSystem.SetSelectedGameObject(_launchMatch);
        }
    }

    public void EraseNumber()
    {
        string currentAmountTxt = _betAmountTxt.text;

        if (currentAmountTxt.Length > 0)
        {
            string nextAmountTxt = (currentAmountTxt.Substring(0, currentAmountTxt.Length - 1));
            _betAmountTxt.text = nextAmountTxt;
        }
    }

    public int WinBet()
    {
        return _betAmount * Mathf.RoundToInt(_currentBetRating);
    }

    public void DeactivateButtons()
    {
        foreach(Button button in _betScreenButtons)
        {
            button.interactable = false;
        }
    }

    public void ActivateButtons()
    {
        foreach (Button button in _betScreenButtons)
        {
            button.interactable = true;
        }
    }

    public void SetChosenParticipant(S_TournamentManager.Participant participant)
    {
        _currentParticipantChosen = participant;
        _currentBetRating = participant.rating;
    }
}
