using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_BetSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField _betInputTxt;
    [SerializeField] private S_TournamentManager _tournamentManager;

    [SerializeField] private Button[] _betScreenButtons;
    [SerializeField] private TextMeshProUGUI _currentBetTxt;
    [SerializeField] private TextMeshProUGUI _playerMoney;

    [SerializeField] private GameObject _launchMatch;
    private TextMeshProUGUI _launchMatchTxt;

    private int _betAmount = 0;
    private float _currentBetRating = 0;
    private S_TournamentManager.Participant _currentParticipantChosen;

    private EventSystem _eventSystem;

    private bool _isLeavingInput = false;

    public delegate void OnStartMatch();
    public OnStartMatch StartMatch;

    public InputActionReference Confirm;

    public int _testMoney = 1000; //TODO : get the player's money in the inventory instead

    private bool _hasBet = false;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
        _launchMatchTxt = _launchMatch.GetComponentInChildren<TextMeshProUGUI>();
        _launchMatchTxt.text = "Skip bet";

        _betInputTxt.onValidateInput += ValidateChar;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        _currentBetTxt.transform.parent.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _betInputTxt.text = "";
        _launchMatchTxt.text = "Skip bet";
        _playerMoney.text = "$ " + _testMoney;

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_betInputTxt.gameObject);
    }

    private void OnDestroy()
    {
        _betInputTxt.onValidateInput -= ValidateChar;
    }

    private void Update()
    {
        if (_betInputTxt.isFocused && _isLeavingInput)
        {
            _isLeavingInput = false;
        }
    }

    /// <summary>
    /// Prevent the use of "-" in the inputfield (so we can prevent a negative number being typed)
    /// </summary>
    /// <param name="text"></param>
    /// <param name="charIndex"></param>
    /// <param name="addedChar"></param>
    /// <returns></returns>
    char ValidateChar(string text, int charIndex, char addedChar)
    {
        if (char.IsDigit(addedChar))
        {
            if (addedChar == '-')
            {
                return '\0'; // Returning '\0' ignores the character
            }

            return addedChar;
        }
        return '\0';
    }

    /// <summary>
    /// Enter the number in the inputfield text (used for buttons)
    /// maximum 9 numbers
    /// </summary>
    /// <param name="amount"></param>
    public void EnterAmount(int amount)
    {
        if (_betInputTxt.text.Length < 9)
        {
            _betInputTxt.text += amount.ToString();

            CorrectBet();
        }
    }

    /// <summary>
    /// Will write the maximum amount the player can bet
    /// if he tries to write a number higher than what he curently has
    /// </summary>
    public void CorrectBet()
    {
        int betInput;
        int.TryParse(_betInputTxt.text, out betInput);

        if (betInput > _testMoney)
        {
            _betInputTxt.text = _testMoney.ToString();
        }
    }

    /// <summary>
    /// Save the number typed in the input field and change the UI accordingly
    /// </summary>
    public void ConfirmBet()
    {
        if (!_isLeavingInput)
        {
            int betAmount;
            int.TryParse(_betInputTxt.text, out betAmount);

            //check if player has enough money

            _betAmount = betAmount;

            //remove money from the player

            Debug.Log("You bet : " + _betAmount);

            gameObject.SetActive(false);
            ActivateButtons();

            if (_betInputTxt.text.Length > 0)
            {
                _launchMatchTxt.text = "Launch Match";

                for (int i = 0; i < _betScreenButtons.Length - 1; i++)
                {
                    _betScreenButtons[i].interactable = false;
                }

                _currentBetTxt.transform.parent.gameObject.SetActive(true);
                _currentBetTxt.text = "$ " + _betAmount;

                int newMoney = _testMoney - _betAmount;

                _testMoney = newMoney < 0 ? 0 : newMoney;

                _playerMoney.text = "$ " + _testMoney;
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_launchMatch);
            }
            else
            {
                _launchMatchTxt.text = "Skip bet";
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_launchMatch);
                _currentBetTxt.transform.parent.gameObject.SetActive(false);
            }
        }

        _isLeavingInput = false;
    }

    public void ConfirmBetInputField()
    {
        if (Confirm.action.triggered)
        {
            ConfirmBet();
        }
    }

    /// <summary>
    /// Remove a number from the input field
    /// </summary>
    public void EraseNumber()
    {
        string currentAmountTxt = _betInputTxt.text;

        if (currentAmountTxt.Length > 0)
        {
            string nextAmountTxt = (currentAmountTxt.Substring(0, currentAmountTxt.Length - 1));
            _betInputTxt.text = nextAmountTxt;
        }
    }

    public void WinBet()
    {
        if (_hasBet)
        {
            if (!_currentParticipantChosen.hasLost)
            {
                int newMoney = _testMoney + _betAmount;
                _testMoney = newMoney > 999999999 ? 999999999 : newMoney; //Mathf.RoundToInt(_currentBetRating)
            }
        }
    }

    /// <summary>
    /// Deactivate the buttons in the list of button that have to be deactivated
    /// </summary>
    public void DeactivateButtons()
    {
        foreach(Button button in _betScreenButtons)
        {
            button.interactable = false;
        }
    }

    /// <summary>
    /// Activate the buttons in the list of button that have to be deactivated
    /// </summary>
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

    public S_TournamentManager.Participant GetChosenParticipant()
    {
        return _currentParticipantChosen;
    }

    /// <summary>
    /// Let the player leave the input field using the UI navigation
    /// without sending the OnEdit event
    /// </summary>
    /// <param name="context"></param>
    public void OnInputFieldUnfocus(InputAction.CallbackContext context)
    {
        if (context.started && gameObject.activeSelf)
        {
            if (context.ReadValue<Vector2>().y < 0f)
            {
                _isLeavingInput = true;

                _betInputTxt.DeactivateInputField();
            }
        }
    }

    public void SetIsLeavingInput(bool isLeavingInput)
    {
        _isLeavingInput = isLeavingInput;
    }

    /// <summary>
    /// Reset the text that show what amount of money the player has bet
    /// </summary>
    public void ResetBetText()
    {
        _currentBetTxt.transform.parent.gameObject.SetActive(false);
        _currentBetTxt.text = "$ " + _betAmount;
        _betAmount = 0;
    }
}
