using UnityEngine;
using TMPro;

public class ListItemController : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI itemStateText;

    public void SetStats(ItemStats stats)
    {
        itemNameText.text = stats.itemName;
        itemPriceText.text = "$" + stats.itemPrice.ToString("F2");
        itemStateText.text = stats.itemState;
    }
}
