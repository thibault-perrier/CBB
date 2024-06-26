using UnityEngine;
using UnityEngine.SceneManagement;

public class S_ObjectClickable : MonoBehaviour
{
    private static S_ObjectClickable _instance;
    public bool _interactionLocked = false;
    [SerializeField] private Animator _animatorDoor;
    [SerializeField] public Animator _animatorCameraGarage;
    [SerializeField] private Light _light;
    private float _originalIntensity;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    private void Start()
    {
        _originalIntensity = _light.intensity;
    }

    public void OnMouseOver()
    {
        if (Time.timeScale.Equals(0f))
            return;

        OnFocus();
    }

    public void OnMouseExit()
    {
        OnFocusLost();
    }

    public void OnMouseDown()
    {
        if (Time.timeScale.Equals(0f))
            return;

        OnActivated();
    }

    public void OnFocus()
    {
        if (!_interactionLocked)
        {
            _light.intensity = _originalIntensity * 2;
            _light.renderMode = LightRenderMode.ForcePixel;
        }
    }

    public void OnFocusLost()
    {
        if (!_interactionLocked)
        {
            _light.intensity = _originalIntensity;
            _light.renderMode = LightRenderMode.ForceVertex;
        }
    }

    public void OnActivated()
    {
        if (!_interactionLocked)
        {
            Activate();
        }
    }

    private void Activate()
    {
        LockInteraction();

        switch (gameObject.tag)
        {
            case "Garage":
                _animatorCameraGarage.SetBool("MoveToGarage", true);
                break;
            case "Rue":
                _animatorCameraGarage.SetBool("MoveToRue", true);
                break;
            case "Tournament":
                _animatorCameraGarage.SetBool("MoveToTournament", true);
                break;
            case "Shop":
                _animatorCameraGarage.SetBool("MoveToShop", true);
                break;
            case "Board":
                _animatorCameraGarage.SetBool("Board", true);
                break;
            case "WorkBench":
                _animatorCameraGarage.SetBool("WorkBench", true);
                S_ClickablesManager.Instance._editorCanvasHelper.SetActive(true);
                S_ClickablesManager.Instance._menuCanvasHelper.SetActive(false);
                break;
            case "Shelves":
                _animatorCameraGarage.SetBool("Shelves", true);
                break;
            case "Bronze":
                if (S_DataGame.Instance.inventory.GetSelectRobot() != null)
                {
                    _animatorCameraGarage.SetBool("FadeTournamentBronze", true);
                    Debug.Log("bronze");
                    UnlockInteraction();
                }
                break;
            case "Silver":
                if (S_DataGame.Instance.inventory.GetSelectRobot() != null)
                {

                    Debug.Log("Silver");
                    UnlockInteraction();
                    _animatorCameraGarage.SetBool("FadeTournamentSilver", true);
                }
                break;
            case "Gold":
                if (S_DataGame.Instance.inventory.GetSelectRobot() != null)
                {

                    Debug.Log("Gold");
                    UnlockInteraction();
                    _animatorCameraGarage.SetBool("FadeTournamentGold", true);
                }
                break;
            case "Diamond":
                if (S_DataGame.Instance.inventory.GetSelectRobot() != null)
                {
                    Debug.Log("Diamond");
                    UnlockInteraction();
                    _animatorCameraGarage.SetBool("FadeTournamentDiamond", true);
                }
                break;

            default:

                break;
        }
    }

    private void SetColor(Color color)
    {
        gameObject.LeanColor(color, 0.1f);
    }

    private void LockInteraction()
    {
        _interactionLocked = true;
    }

    public void UnlockInteraction()
    {
        _interactionLocked = false;
    }

    public Animator AnimatorCameraGarage
    {
        get { return _animatorCameraGarage; }
    }

    public static S_ObjectClickable Instance
    { get { return _instance; } }

    public void ResetState()
    {
        _interactionLocked = false;
    }

    public void LaunchAnimBackToMenuFromShop()
    {

        _animatorCameraGarage.SetBool("BackToMainFromShop", true);
    }

    public void StopAnimBackToMenuFromShop()
    {
        _animatorCameraGarage.SetBool("BackToMainFromShop", false);
    }
    public void CurrentAnimFalse()
    {
        _animatorCameraGarage.SetBool("MoveToShop", false);
        UnlockInteraction();
    }

    public void GoOnIdleDisableShop()
    {
        _animatorCameraGarage.SetBool("Idle", false);
    }

    public void LaunchAnimBackToMenuFromGarage()
    {
        _animatorCameraGarage.SetBool("MoveToGarage", false);
        _animatorCameraGarage.SetBool("BackToMainFromGarage", true);
    }

    public void StopAnimBackToMenuFromGarage()
    {
        _animatorCameraGarage.SetBool("BackToMainFromGarage", false);
    }

    public void GoOnIdleGarage()
    {
        _animatorCameraGarage.SetBool("Idle", true);
    }

    public void GoOnIdleDisableGarage()
    {
        _animatorCameraGarage.SetBool("Idle", false);
    }

    public void CloseGarageDoor()
    {
        _animatorDoor.SetBool("Open", false);
    }

    public void IdleEnableCloseGarageDoor()
    {
        _animatorDoor.SetBool("IdleDoor", true);
    }

    public void IdleDisableCloseGarageDoor()
    {
        _animatorDoor.SetBool("IdleDoor", false);
    }

    public void BackGarageDoorEnable()
    {
        _animatorDoor.SetBool("BackDoor", true);
    }

    public void BackBoardEnable()
    {
        _animatorDoor.SetBool("Board", true);
    }
    public void BackBoardDisable()
    {
        _animatorDoor.SetBool("Board", false);
    }

    public void BackGarageDoorDisable()
    {
        _animatorDoor.SetBool("BackDoor", false);
    }
    public void StopGarageAnim()
    {
        _animatorCameraGarage.SetBool("MoveToGarage", false);
    }
    public void StopAnimBoard()
    {
        _animatorCameraGarage.SetBool("Board", false);
    }
    public void EnableIdleInGarage()
    {
        _animatorCameraGarage.SetBool("IdleInGarage", true);
    }
    public void DisableInGarage()
    {
        _animatorCameraGarage.SetBool("IdleInGarage", false);
    }

    public void ResetLightIntensity()
    {
        _light.intensity = _originalIntensity;
    }


}