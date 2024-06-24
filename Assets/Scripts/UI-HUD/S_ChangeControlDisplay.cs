using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class S_ChangeControlDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _keyboardControls;
    [SerializeField] private GameObject _gamepadControls;

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
                    if (_keyboardControls != null)
                        _keyboardControls?.SetActive(false);
                    if (_gamepadControls != null)
                        _gamepadControls?.SetActive(true);
                    Cursor.visible = false;
                }
                else if (_lastActiveDevice is Keyboard)
                {
                    if (_keyboardControls != null)
                        _keyboardControls?.SetActive(true);
                    if (_gamepadControls != null)
                        _gamepadControls?.SetActive(false);
                }
                else if (_lastActiveDevice is Mouse)
                {
                    if (_keyboardControls != null)
                        _keyboardControls?.SetActive(true);
                    if (_gamepadControls != null)
                        _gamepadControls?.SetActive(false);

                    Cursor.visible = true;
                }
            };
        }
    }
}
