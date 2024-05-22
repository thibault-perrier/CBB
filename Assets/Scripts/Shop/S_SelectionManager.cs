using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public List<ItemStats> itemStatsList;
    public List<ListItemController> itemControllers;

    private int selectedIndex = -1;

    public void SelectItem(int index)
    {
        if (selectedIndex != -1)
        {
            itemControllers[selectedIndex].gameObject.SetActive(false);
        }
        selectedIndex = index;
        itemControllers[selectedIndex].gameObject.SetActive(true);
        itemControllers[selectedIndex].SetStats(itemStatsList[selectedIndex]);
    }
}
