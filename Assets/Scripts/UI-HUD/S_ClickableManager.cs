using UnityEngine;
using UnityEngine.InputSystem;

public class ClickablesManager : MonoBehaviour
{
    public GameObject[] _clickables;
    private int _currentIndex = 0;
    private float _navigationCooldown = 0.2f;
    private float _nextNavigationTime = 0f;

    void Start()
    {
        if (_clickables.Length > 0)
        {
            SetFocus(_clickables[_currentIndex]);
        }
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        if (Time.time >= _nextNavigationTime)
        {
            Vector2 joystickInput = gamepad.leftStick.ReadValue();

            if (joystickInput.x > 0.5f)
            {
                Navigate(1);
                _nextNavigationTime = Time.time + _navigationCooldown;
            }
            else if (joystickInput.x < -0.5f)
            {
                Navigate(-1);
                _nextNavigationTime = Time.time + _navigationCooldown;
            }
        }

        if (gamepad.buttonSouth.wasPressedThisFrame) // A button on Xbox controller
        {
            ActivateCurrent();
        }
    }

    void Navigate(int direction)
    {
        RemoveFocus(_clickables[_currentIndex]);

        _currentIndex += direction;
        if (_currentIndex < 0) _currentIndex = _clickables.Length - 1;
        else if (_currentIndex >= _clickables.Length) _currentIndex = 0;

        SetFocus(_clickables[_currentIndex]);
    }

    void SetFocus(GameObject obj)
    {
        var clickable = obj.GetComponent<S_ObjectClickable>();
        if (clickable != null)
        {
            clickable.OnFocus();
        }
    }

    void RemoveFocus(GameObject obj)
    {
        var clickable = obj.GetComponent<S_ObjectClickable>();
        if (clickable != null)
        {
            clickable.OnFocusLost();
        }
    }

    void ActivateCurrent()
    {
        var clickable = _clickables[_currentIndex].GetComponent<S_ObjectClickable>();
        if (clickable != null)
        {
            clickable.OnActivated();
        }
    }
}
