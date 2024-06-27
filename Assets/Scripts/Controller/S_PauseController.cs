using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class S_PauseController : MonoBehaviour
{
    public enum WorldLocation
    {
        Menu,
        Match
    }

    public static S_PauseController Instance;

    [SerializeField]
    private string _sceneToLoadForGiveUp = "MainScene";

    [Header("UI")]
    [SerializeField]
    private GameObject _uiPausePanel;

    [Header("Button Location")]
    [SerializeField]
    private GameObject _uiGiveUpButton;
    [SerializeField]
    private GameObject _uiQuitButton;
    [SerializeField]
    private WorldLocation _location;

    [Header("Button focus")]
    [SerializeField]
    private GameObject _resumeButton;
    [SerializeField]
    private GameObject _backButtonRebindKeyboard;
    [SerializeField]
    private GameObject _backButtonRebindController;

    [Header("Input")]
    [SerializeField]
    private InputActionReference _pauseActionReference;

    private InputAction _pauseAction;
    private GameObject _currentSelectedGameObject;
    private bool _pauseIsActive = false;

    public WorldLocation Location
    {
        get => _location;
        set
        {
            _location = value;
            SetWorldLocation(value);
        }
    }

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;

        _pauseAction = _pauseActionReference.action;
        _pauseAction.Enable();

        _pauseAction.performed += ActivePauseMenu;
    }
    private void Start()
    {
        Time.timeScale = 1f;
        SetWorldLocation(_location);
    }

    public void ActivePauseMenu(InputAction.CallbackContext ctx)
    {
        if (_uiPausePanel.activeSelf)
        {
            ResumeButtonPress();
            return;
        }

        if (_pauseIsActive)
            return;

        _currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(_resumeButton);

        _pauseIsActive = true;
        _uiPausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeButtonPress()
    {
        if (_currentSelectedGameObject)
            EventSystem.current.SetSelectedGameObject(_currentSelectedGameObject);

        _pauseIsActive = false;
        _uiPausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void GiveUpButtonPress()
    {
        SceneManager.LoadScene(_sceneToLoadForGiveUp, LoadSceneMode.Single);
    }
    public void QuitButtonPress()
    {
        Application.Quit();
    }
    public void BindBackButtons()
    {
        if (_backButtonRebindController.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(_backButtonRebindController);
        else
            EventSystem.current.SetSelectedGameObject(_backButtonRebindKeyboard);
    }
    public void SelectedButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(button);
    }

    private void SetWorldLocation(WorldLocation location)
    {
        if (location.Equals(WorldLocation.Match))
        {
            _uiGiveUpButton.SetActive(true);
            _uiQuitButton.SetActive(false);
        }
        else
        {
            _uiGiveUpButton.SetActive(false);
            _uiQuitButton.SetActive(true);
        }
    }
}
