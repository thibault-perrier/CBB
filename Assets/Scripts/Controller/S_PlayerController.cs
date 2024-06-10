using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerController : MonoBehaviour
{
    private GameObject _cam;
    private S_CameraView _cameraView;
    private S_CameraView _mainCam;

    private S_WheelsController _wheelsController;
    private S_FrameManager _frameManager;

    private int _viewIndex = 0;

    private void Start()
    {
        _cam = GameObject.Find("CameraManager");
        _mainCam = Camera.main.GetComponent<S_CameraView>();
        _wheelsController = GetComponent<S_WheelsController>();
        _frameManager = GetComponent<S_FrameManager>();
        _frameManager.SelectWeapons();

        if (_cam != null )
        {
            _cameraView = _cam.GetComponent<S_CameraView>();
        }
    }

    //Move the robot on the X axis
    public void OnMove(InputAction.CallbackContext context)
    {
        float movement = context.ReadValue<float>();

        _wheelsController.Movement = movement;

        //if (context.performed)
        //{
        //    Debug.Log(context.ReadValue<float>());
        //    float movement = context.ReadValue<float>();

        //    if (movement > 0)
        //    {
        //        _wheelsController.Movement = S_WheelsController.Move.backward;
        //    }
        //    else if (movement < 0)
        //    {
        //        _wheelsController.Movement = S_WheelsController.Move.toward;
        //    }
        //}
        //else if (context.canceled)
        //{
        //    _wheelsController.Movement = S_WheelsController.Move.neutral;
        //}
    }

    //Rotate the robot on the Y axis
    public void OnRotate(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            _wheelsController.Direction = -context.ReadValue<float>();
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

    //public void OnMoveTournamentCamera(InputAction.CallbackContext context)
    //{
    //    _mainCam.SetMovement(context.ReadValue<Vector2>());
    //}

    public void OnAttack1(InputAction.CallbackContext context)
    {
        if (_frameManager._weaponManagers.Count < 1)
            return;

        if (context.performed && _frameManager._weaponManagers[0] != null)
        {
            _frameManager._weaponManagers[0].LaunchAttack();
        }
    }
    public void OnAttack2(InputAction.CallbackContext context)
    {
        if (_frameManager._weaponManagers.Count < 2)
            return;

        if (context.performed && _frameManager._weaponManagers[1] != null)
        {
            _frameManager._weaponManagers[1].LaunchAttack();
        }
    }
    public void OnAttack3(InputAction.CallbackContext context)
    {
        if (_frameManager._weaponManagers.Count < 3)
            return;

        if (context.performed && _frameManager._weaponManagers[2] != null)
        {
            _frameManager._weaponManagers[2].LaunchAttack();
        }
    }
}
