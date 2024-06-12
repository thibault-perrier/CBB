using UnityEngine;
using UnityEngine.UI;

public class S_PolygonButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Prevent the mouse to click on an invisible part of a button
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}
