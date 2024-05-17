using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log(context.ReadValue<float>());
        }
    }

    public void OnRotate(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            Debug.Log(context.ReadValue<float>());
        }
    }
}
