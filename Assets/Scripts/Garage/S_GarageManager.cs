using UnityEngine;
using UnityEngine.InputSystem;

public class GarageManager : MonoBehaviour
{
    public void LeaveGarage(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy && S_ClickablesManager.Instance.activeBackGarage == true )
        {           
            S_ObjectClickable.Instance.LaunchAnimBackToMenuFromGarage();
            S_DataGame.Instance.SaveInventory();
            S_ClickablesManager.Instance.ResetAllClickables();
        }
    }

    public void LeaveBoard(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy)
        {
            S_ObjectClickable.Instance._animatorCameraGarage.SetBool("Board", false);
            S_ClickablesManager.Instance.ReactivateAllClickables();
            S_ClickablesManager.Instance.CircleFade.SetActive(false);
        }
    }

    public void LeaveShelves(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy)
        {
            S_ObjectClickable.Instance._animatorCameraGarage.SetBool("Shelves", false);
            S_ClickablesManager.Instance.ReactivateAllClickables();
            S_ClickablesManager.Instance.CircleFade.SetActive(false);
        }
    }

    public void LeaveWorkBench(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy)
        {
            S_ObjectClickable.Instance._animatorCameraGarage.SetBool("WorkBench", false);
            S_ClickablesManager.Instance.ReactivateAllClickables();
            S_ClickablesManager.Instance.CircleFade.SetActive(false);
        }
    }

    public void LeaveTournament(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy)
        {
            S_ObjectClickable.Instance._animatorCameraGarage.SetBool("MoveToTournament", false);
            S_ClickablesManager.Instance.ReactivateAllClickables();
        }
    }

    public void LeaveRue(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy)
        {
            S_ObjectClickable.Instance._animatorCameraGarage.SetBool("MoveToRue", false);
            S_ClickablesManager.Instance.ReactivateAllClickables();
        }
    }
}