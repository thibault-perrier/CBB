using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerController : MonoBehaviour
{
    private GameObject _cam;
    private S_CameraView _cameraView;

    private int _viewIndex = 0;

    private void Start()
    {
        _cam = GameObject.Find("CameraManager");

        if (_cam != null )
        {
            _cameraView = _cam.GetComponent<S_CameraView>();
        }
    }

    //Move the robot on the X axis
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log(context.ReadValue<float>());
        }
    }

    //Rotate the robot on the Y axis
    public void OnRotate(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            Debug.Log(context.ReadValue<float>());
        }
    }

    //Change the camera view
    public void OnCangeView(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int newValue = _viewIndex + (int)context.ReadValue<float>();
            _viewIndex = newValue > 3 ? 0 : newValue < 0 ? 3 : newValue;

            _cameraView.SetViewType((S_CameraView.ViewType)_viewIndex);
        }
    }

    public void OnMoveTournament(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
    }
}
