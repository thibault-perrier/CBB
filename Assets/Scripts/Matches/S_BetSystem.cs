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
    [SerializeField] private GameObject _betWinDisplay;
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
    private int _maxMoney = 999999999;

    private bool _hasBet = false;

    public InputActionReference NavigateReference;
    private InputAction _navigate;
    private InputAction _confirmAction;
    private InputAction _cancelAction;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
        _launchMatchTxt = _launchMatch.GetComponentInChildren<TextMeshProUGUI>();

        _betInputTxt.onValidateInput += ValidateChar;
        _navigate = NavigateReference.action;

        _confirmAction = new InputAction("ConfirmBet");
        _confirmAction.AddBinding("<Gamepad>/start");
        _confirmAction.AddBinding("<Keyboard>/enter");
        _cancelAction = new InputAction("CancelBet");
        _cancelAction.AddBinding("<Gamepad>/buttonEast");
        _cancelAction.AddBinding("<Keyboard>/escape");

        _confirmAction.Enable();
        _cancelAction.Enable();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        _betWinDisplay.SetActive(false);
        _currentBetTxt.transform.parent.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _betInputTxt.text = "";
        _playerMoney.text = "$ " + _testMoney;

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_betInputTxt.gameObject);

        _navigate.Enable();
    }

    private void OnDestroy()
    {
        _betInputTxt.onValidateInput -= ValidateChar;

        _navigate?.Disable();
        _cancelAction?.Disable();
        _confirmAction?.Disable();
    }

    private void Update()
    {
        BetInputs();
        if (_betInputTxt.isFocused && _isLeavingInput)
        {
            _isLeavingInput = false;
        }
    }

    private void BetInputs()
    {
        if (gameObject.activeSelf && _confirmAction.triggered)
            ConfirmBet();
        if (gameObject.activeSelf && _cancelAction.triggered)
            EraseNumber();

        InputFieldUnfocus();
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

            if (_betInputTxt.text.Length > 0 && _betAmount > 0)
            {
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


                _hasBet = _betAmount == 0 ? false : true;
            }
            else
            {
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
        else
        {
            gameObject.SetActive(false);
            ActivateButtons();
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_launchMatch.gameObject);
        }
    }

    /// <summary>
    /// Give the player his money back with some gain if he won the bet
    /// </summary>
    public void WinBet()
    {
        if (_hasBet)
        {
            if (!HasLostBet())
            {
                Debug.Log("YOU WON THE BET !");
                int amountWon = _betAmount * 2; //Change this when we have the definitive calcul of the rating
                int newMoney = _testMoney + amountWon;

                _testMoney = newMoney > _maxMoney ? _maxMoney : newMoney; //Mathf.RoundToInt(_currentBetRating)
                _playerMoney.text = "$ " + _testMoney;
                _betWinDisplay.SetActive(true);
                _betWinDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "BET : YOU WON $ " + amountWon + " !";

                _betAmount = 0;
            }
        }
    }

    private bool HasLostBet()
    {
        Debug.Log("Loser : " + _tournamentManager.GetCurrentLoser().name + " , the one chosen : " + _currentParticipantChosen.name);
        if (_tournamentManager.GetCurrentLoser().name == _currentParticipantChosen.name)
        {
            return true;
        }
        return false;
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
    private void InputFieldUnfocus()
    {
        Vector2 direction = _navigate.ReadValue<Vector2>();
        if (_navigate.triggered && gameObject.activeSelf)
        {
            if (direction.y < 0f && _eventSystem.currentSelectedGameObject == _betInputTxt.gameObject)
            {
                _isLeavingInput = true;

                _betInputTxt.DeactivateInputField();
            }
            else
            {
                _isLeavingInput = false;
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
    }

    public void SetHasBet(bool hasBet)
    {
        _hasBet = hasBet;
    }
}
