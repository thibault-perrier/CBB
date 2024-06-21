
using UnityEngine;
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

    [Header("Button")]
    [SerializeField]
    private GameObject _uiGiveUpButton;
    [SerializeField]
    private GameObject _uiQuitButton;
    [SerializeField]
    private WorldLocation _location;

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
    }
    private void Start()
    {
        SetWorldLocation(_location);
    }

    public void ActivePauseMenu(InputAction.CallbackContext context)
    {
        _uiPausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeButtonPress()
    {
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
