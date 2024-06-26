using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class S_ChangeControlDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _gamepadHelper;
    [SerializeField] private GameObject _keyboardHelper;

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
                    if (_gamepadHelper && _keyboardHelper)
                    {
                        _gamepadHelper.SetActive(true);
                        _keyboardHelper.SetActive(false);
                    }
                    Cursor.visible = false;
                    // Cursor.lockState = CursorLockMode.Locked;
                    Mouse.current.WarpCursorPosition(Vector2.one);
                }

                else if (_lastActiveDevice is Mouse)
                {
                    if (_gamepadHelper && _keyboardHelper)
                    {
                        _keyboardHelper.SetActive(true);
                        _gamepadHelper.SetActive(false);
                    }
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            };
        }
    }
}
