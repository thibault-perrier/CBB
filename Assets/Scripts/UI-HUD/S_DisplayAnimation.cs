using UnityEngine;

public class S_DisplayAnimation : MonoBehaviour
{
    /// <summary>
    /// Disable the gameobject, is used at the end of an animation
    /// </summary>
    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}
