using UnityEngine;
using UnityEngine.InputSystem;

public class S_TestControls : MonoBehaviour
{
    public void OnAtk1(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("atk1");
    }
    public void OnAtk2(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("atk2");
    }
    public void OnAtk3(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("atk3");
    }
    public void OnAtk4(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("atk4");
    }
}
