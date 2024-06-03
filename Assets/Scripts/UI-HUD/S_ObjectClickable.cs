using UnityEngine;

public class S_ObjectClickable : MonoBehaviour
{
    public static S_ObjectClickable Instance;
    private bool _isFocused = false;
    public bool _interactionLocked = false; 
    [SerializeField] private Animator _animatorDoor;
    [SerializeField] private Animator _animatorCameraGarage;

    public void OnFocus()
    {
        if (!_interactionLocked) 
        {
            _isFocused = true;
            SetColor(Color.blue);
        }
    }

    public void OnFocusLost()
    {
        if (!_interactionLocked) 
        {
            _isFocused = false;
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
            //SetColor(Color.white);
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
}
