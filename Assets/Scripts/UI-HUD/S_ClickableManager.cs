using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class S_ClickablesManager : MonoBehaviour
{
    public static S_ClickablesManager Instance;
    public GameObject[] _clickables;
    private int _currentIndex = 0;
    private float _navigationCooldown = 0.2f;
    private float _nextNavigationTime = 0f;
    public GameObject mainMenu;
    public GameObject shopMenu;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
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
                Navigate(-1);
                _nextNavigationTime = Time.time + _navigationCooldown;
            }
            else if (joystickInput.x < -0.5f)
            {
                Navigate(1);
                _nextNavigationTime = Time.time + _navigationCooldown;
            }
        }

        if (gamepad.buttonSouth.wasPressedThisFrame) 
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
    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
        shopMenu.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        shopMenu.SetActive(false);
    }
    public void ShowHideMainMenu()
    {
        shopMenu.SetActive(!shopMenu.activeSelf);
        mainMenu.SetActive(!mainMenu.activeSelf);
    }

    public void LoadTournament()
    {
        SceneManager.LoadScene("TournamentScene");
    }
    public void ResetAllClickables()
    {
        foreach (GameObject clickable in _clickables)
        {
            var clickableScript = clickable.GetComponent<S_ObjectClickable>();
            if (clickableScript != null)
            {
                clickableScript.ResetState();
            }
        }
    }

    public void StopAnimShop()
    {
       S_ObjectClickable.Instance.CurrentAnimFalse();
       S_ObjectClickable.Instance.GoOnIdleShop();
    }

    public void StopAnimBackToMainMenuFromShop()
    {
        S_ObjectClickable.Instance.StopAnimBackToMenuFromShop();
    }

    public void StopIdleShop()
    {
        S_ObjectClickable.Instance.GoOnIdleDisableShop();
    }

    public void StopAnimBackToMainMenuFromGarage()
    {
        S_ObjectClickable.Instance.StopAnimBackToMenuFromGarage();
    }

    public void StartIdleGarage()
    {
        S_ObjectClickable.Instance.GoOnIdleGarage();
    }

    public void StopIdleGarage()
    {
        S_ObjectClickable.Instance.GoOnIdleDisableGarage();
    }
    
    public void BackGarageDoorEnable()
    {
        S_ObjectClickable.Instance.BackGarageDoorEnable();
    }

    public void BackGarageDoorDisable()
    {
        S_ObjectClickable.Instance.BackGarageDoorDisable();
    }

  

   
}
