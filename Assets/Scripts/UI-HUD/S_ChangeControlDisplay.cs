using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class S_ChangeControlDisplay : MonoBehaviour
{
    private InputDevice _lastActiveDevice;

    private void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (eventPtr.IsA<StateEvent>() || eventPtr.IsA<DeltaStateEvent>())
        {
            _lastActiveDevice = device;
            if (_lastActiveDevice != null)
            {
                //Debug.Log($"Last active device: {_lastActiveDevice.displayName}");

                if (_lastActiveDevice is Gamepad)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                else if (_lastActiveDevice is Mouse)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            };
        }
    }
}
