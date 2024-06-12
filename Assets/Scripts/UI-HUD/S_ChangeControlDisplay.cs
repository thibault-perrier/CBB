using UnityEngine;
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
                    _keyboardControls.SetActive(false);
                    _gamepadControls.SetActive(true);
                }
                else if (_lastActiveDevice is Keyboard)
                {
                    _keyboardControls.SetActive(true);
                    _gamepadControls.SetActive(false);
                }
            };
        }
    }
}
