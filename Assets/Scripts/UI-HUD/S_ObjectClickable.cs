using UnityEngine;

public class S_ObjectClickable : MonoBehaviour
{
    private static S_ObjectClickable _instance;
    public bool _interactionLocked = false; 
    [SerializeField] private Animator _animatorDoor;
    [SerializeField] private Animator _animatorCameraGarage;



    private void Awake()
    {
        if(_instance == null)
            _instance = this;
    }

    public void OnFocus()
    {
        if (!_interactionLocked) 
        {
            SetColor(Color.blue);
        }
    }

    public void OnFocusLost()
    {
        if (!_interactionLocked) 
        {
            SetColor(Color.white);
        }
    }

    public void OnActivated()
    {
        if (!_interactionLocked) 
        {
            Activate();
            SetColor(Color.white);
        }
    }

    private void Activate()
    {
        LockInteraction(); 

        if (gameObject.CompareTag("Garage"))
        {
            SetColor(Color.white);
            _animatorDoor.SetBool("Open", true);
            _animatorCameraGarage.SetBool("MoveToGarage", true);
        }
        else if (gameObject.CompareTag("Tournament"))
        {
            SetColor(Color.white);
            _animatorCameraGarage.SetBool("MoveToTournament", true);
        }
        else if (gameObject.CompareTag("Shop"))
        {
            _animatorCameraGarage.SetBool("MoveToShop", true);
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
        SetColor(Color.white);
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

    public void GoOnIdleShop()
    {
        _animatorCameraGarage.SetBool("Idle", true);
    }

    public void GoOnIdleDisableShop()
    {
        _animatorCameraGarage.SetBool("Idle", false);
    }

    public void LaunchAnimBackToMenuFromGarage()
    {
        _animatorCameraGarage.SetBool("MoveToGarage", false);
        _animatorCameraGarage.SetBool("BackToMainFromGarage", true);
        _animatorDoor.SetBool("BackDoor", true);
    }

    public void StopAnimBackToMenuFromGarage()
    {
        _animatorCameraGarage.SetBool("BackToMainFromGarage", false);
        _animatorDoor.SetBool("BackDoor", false);
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

    public void BackGarageDoorDisable()
    {
        _animatorDoor.SetBool("BackDoor", false);
    }


}
