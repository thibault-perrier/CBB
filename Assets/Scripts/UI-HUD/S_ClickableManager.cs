using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class S_ClickablesManager : MonoBehaviour
{
    public GameObject CircleFade;
    public GameObject destroyCup;
    public static S_ClickablesManager Instance;
    public GameObject[] clikableObjetGarage;
    public GameObject[] clikableObjetTournament;
    public GameObject[] clickables;
    private int _currentIndex = 0;
    private float _navigationCooldown = 0.2f;
    private float _nextNavigationTime = 0f;
    public GameObject mainMenu;
    public GameObject shopMenu;
    public bool activeBackGarage = false;
    private bool[] _clickableStates;
    private bool _useMouse = false;
    private bool _garageNavigable = false;
    private InputAction mouseMoveAction;
    private InputAction navigateAction;
    [SerializeField] private S_EditorController _editorController;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        _clickableStates = new bool[clickables.Length + clikableObjetGarage.Length + clickables.Length + clikableObjetTournament.Length];
        for (int i = 0; i < clickables.Length; i++)
        {
            _clickableStates[i] = true;
        }
        for (int i = clickables.Length; i < _clickableStates.Length; i++)
        {
            _clickableStates[i] = false;
        }

        var inputActions = new InputActionMap("UI");

        mouseMoveAction = inputActions.AddAction("MouseMove", binding: "<Pointer>/delta");
        mouseMoveAction.performed += OnMouseMove;

        navigateAction = inputActions.AddAction("Navigate", binding: "<Gamepad>/leftStick");
        navigateAction.performed += OnNavigate;

        inputActions.Enable();
    }

    public void ClickableObjectTournament()
    {
        destroyCup.SetActive(false);
        //Debug.Log("ClickableObjectTournament() called.");
        foreach (var clickableGroup in clikableObjetTournament)
        {
            if (clickableGroup != null)
            {
            //    Debug.Log("clickableGroup found: " + clickableGroup.name);
                var clickableScript = clickableGroup.GetComponent<S_ObjectClickable>();
                if (clickableScript != null)
                {
                    //Debug.Log("Clickable script found on: " + clickableGroup.name);
                    clickableScript.enabled = true;
                }
            }
        }
    }

    public void DisableObjectTournament()
    {
        destroyCup.SetActive(true);
        foreach (var clickableGroup in clikableObjetTournament)
        {
            if (clickableGroup != null)
            {
                var clickableScript = clickableGroup.GetComponent<S_ObjectClickable>();
                if (clickableScript != null)
                {
                    clickableScript.enabled = false;
                }
            }
        }
    }

    void Start()
    {
        if (clickables.Length > 0)
        {
            SetFocus(clickables[_currentIndex]);
            DisableGarageNavigation();
            DisableObjectTournament();
        }
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null && !_useMouse)
        {
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

            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                ActivateCurrent();
            }
        }
    }

    public void DisableNavigation()
    {
        _garageNavigable = false;
        for (int i = 0; i < clickables.Length; i++)
        {
            _clickableStates[i] = false;
        }
    }

    public void EnableNavigation()
    {
        _garageNavigable = true;
        for (int i = 0; i < clickables.Length; i++)
        {
            _clickableStates[i] = true;
        }
    }



    void OnMouseMove(InputAction.CallbackContext context)
    {
        _useMouse = true;
        ResetAllClickables();
    }

    void OnNavigate(InputAction.CallbackContext context)
    {
        _useMouse = false;
        if (Time.time >= _nextNavigationTime)
        {
            Vector2 joystickInput = context.ReadValue<Vector2>();

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
    }

    void Navigate(int direction)
    {
        RemoveFocus(GetCurrentClickable());

        do
        {
            _currentIndex += direction;
            if (_currentIndex < 0) _currentIndex = _clickableStates.Length - 1;
            else if (_currentIndex >= _clickableStates.Length) _currentIndex = 0;
        }
        while (!_clickableStates[_currentIndex]);

        SetFocus(GetCurrentClickable());
    }

    GameObject GetCurrentClickable()
    {
        if (_currentIndex < clickables.Length)
        {
            return clickables[_currentIndex];
        }
        else
        {
            return clikableObjetGarage[_currentIndex - clickables.Length];
        }
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
        var clickable = GetCurrentClickable().GetComponent<S_ObjectClickable>();
        if (clickable != null)
        {
            clickable.OnActivated();
            DisableAllClickablesExcept(_currentIndex);
        }
    }

    void DisableAllClickablesExcept(int index)
    {
        for (int i = 0; i < _clickableStates.Length; i++)
        {
            if (i != index)
            {
                _clickableStates[i] = false;
            }
        }
    }

    public void ReactivateAllClickables()
    {
        for (int i = 0; i < _clickableStates.Length; i++)
        {
            if (i < clickables.Length || _garageNavigable)
            {
                _clickableStates[i] = true;
            }
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

    public void ResetAllClickables()
    {
        foreach (GameObject clickable in clickables)
        {
            var clickableScript = clickable.GetComponent<S_ObjectClickable>();
            if (clickableScript != null)
            {
                clickableScript.ResetState();
            }
        }

        foreach (GameObject clickable in clikableObjetGarage)
        {
            var clickableScript = clickable.GetComponent<S_ObjectClickable>();
            if (clickableScript != null)
            {
                clickableScript.ResetState();
            }
        }
    }

    public void ClikableObjectGarage()
    {
        //Debug.Log("ClikableObjectGarage() called.");
        foreach (var clickableGroup in clikableObjetGarage)
        {
            if (clickableGroup != null)
            {
                //Debug.Log("clickableGroup found: " + clickableGroup.name);
                var clickableScript = clickableGroup.GetComponent<S_ObjectClickable>();
                if (clickableScript != null)
                {
                    //Debug.Log("Clickable script found on: " + clickableGroup.name);
                    clickableScript.enabled = true;
                }
            }
        }
    }

    public void DisableObjectGarage()
    {
        foreach (var clickableGroup in clikableObjetGarage)
        {
            if (clickableGroup != null)
            {
                var clickableScript = clickableGroup.GetComponent<S_ObjectClickable>();
                if (clickableScript != null)
                {
                    clickableScript.enabled = false;
                }
            }
        }
    }

    public void EnableGarageNavigation()
    {
        _garageNavigable = true;
        for (int i = clickables.Length; i < _clickableStates.Length; i++)
        {
            _clickableStates[i] = true;
        }
    }

    public void DisableGarageNavigation()
    {
        _garageNavigable = false;
        for (int i = clickables.Length; i < _clickableStates.Length; i++)
        {
            _clickableStates[i] = false;
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
        ReactivateAllClickables();
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

    public void ActiveBoolBackAnimGarage()
    {
        activeBackGarage = true;
    }

    public void FalseBoolBackAnimGarage()
    {
        activeBackGarage = false;
    }

    public void ActiveBoolBackShop()
    {
        S_ShopManager.Instance.activeBackShop = true;
    }

    public void FalseBoolBackShop()
    {
        S_ShopManager.Instance.activeBackShop = false;
    }
    public void StopBoolGoToGarage()
    {
        S_ObjectClickable.Instance.StopGarageAnim();
    }

    public void StopAnimBoard()
    {
        S_ObjectClickable.Instance.StopAnimBoard();
    }

    public void EnableIdleInGarage()
    {
        S_ObjectClickable.Instance.EnableIdleInGarage();
    }

    public void DisableIdleInGarage()
    {
        S_ObjectClickable.Instance.DisableInGarage();
    }

    public void ResetNavigationInGarageObjects()
    {
        for (int i = clickables.Length; i < _clickableStates.Length; i++)
        {
            _clickableStates[i] = true;
        }
    }

    public void SetNullChoice()
    {
        _editorController.SetNullChoice();
    }

    public void SetPartChoice()
    {
        _editorController.SetPartChoice();
    }

    public void SetPresetChoice()
    {
        _editorController.SetPresetChoice();
    }

    public void ActiveCircleFade()
    {
        CircleFade.SetActive(true);
    }
}