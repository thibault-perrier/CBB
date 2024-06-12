using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GarageManager : MonoBehaviour
{
    public void LeaveGarage(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy && S_ClickablesManager.Instance.activeBackGarage == true)
        {
            S_ObjectClickable.Instance.LaunchAnimBackToMenuFromGarage();
            S_DataGame.Instance.SaveInventory();
            S_ClickablesManager.Instance.ResetAllClickables();
        }
    }
}
