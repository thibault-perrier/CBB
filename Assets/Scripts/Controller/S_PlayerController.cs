using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerController : MonoBehaviour
{
    private GameObject _cam;
    private S_CameraView _cameraView;

    private S_WheelsController _wheelsController;

    private int _viewIndex = 0;

    private void Start()
    {
        _cam = GameObject.Find("CameraManager");
        _wheelsController = GetComponent<S_WheelsController>();

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
            float movement = context.ReadValue<float>();

            if (movement > 0)
            {
                _wheelsController.Movement = S_WheelsController.Move.backward;
            }
            else if (movement < 0)
            {
                _wheelsController.Movement = S_WheelsController.Move.toward;
            }
        }
        else if (context.canceled)
        {
            _wheelsController.Movement = S_WheelsController.Move.neutral;
        }
    }

    //Rotate the robot on the Y axis
    public void OnRotate(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            Debug.Log(context.ReadValue<float>());

            _wheelsController.Direction = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            _wheelsController.Direction = 0;
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
}
