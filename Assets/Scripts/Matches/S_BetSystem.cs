using TMPro;
using UnityEngine;

public class S_BetSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _betAmountTxt;

    private int _betAmount = 0;

    private void OnEnable()
    {
        _betAmount = 0;
    }

    public void EnterAmount(int amount)
    {
        _betAmountTxt.text += amount.ToString();
    }

    public void ConfirmBet()
    {

    }
}
