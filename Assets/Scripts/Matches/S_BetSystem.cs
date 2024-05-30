using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_BetSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _betAmountTxt;
    [SerializeField] private S_TournamentManager _tournamentManager;

    [SerializeField] private Button[] _betScreenButtons;

    [SerializeField] private GameObject _skipBet, _launchMatch;

    private int _betAmount = 0;
    private float _currentBetRating = 0;
    private S_TournamentManager.Participant _currentParticipantChosen;

    public delegate void OnStartMatch();
    public OnStartMatch StartMatch;

    private void OnEnable()
    {
        _betAmountTxt.text = "";
        _betAmount = 0;
    }

    private void OnDisable()
    {
        _launchMatch.SetActive(false);
        _skipBet.SetActive(true);
    }

    private void Start()
    {
        _launchMatch.SetActive(false);
    }

    public void EnterAmount(int amount)
    {
        if (_betAmountTxt.text.Length < 11)
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

        if (_betAmountTxt.text.Length > 0)
        {
            _skipBet.SetActive(false);
            _launchMatch.SetActive(true);
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
