using TMPro;
using UnityEngine;

public class S_BetSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _betAmountTxt;
    [SerializeField] private S_TournamentManager _tournamentManager;

    private int _betAmount = 0;
    private float _currentBetRating = 0;

    private void OnEnable()
    {
        _betAmountTxt.text = "";
        _betAmount = 0;
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

        Debug.Log("You bet : " +  _betAmount);
    }

    public void EraseNumber()
    {
        string currentAmountTxt = _betAmountTxt.text;

        if (currentAmountTxt.Length > 0 )
        {
            string nextAmountTxt = (currentAmountTxt.Substring(0, currentAmountTxt.Length - 1));
            _betAmountTxt.text = nextAmountTxt;
        }
    }
    public void BetMatch(int betValue)
    {
        _betAmount = betValue;
    }

    public int WinBet()
    {
        return _betAmount * Mathf.RoundToInt(_currentBetRating);
    }
}
