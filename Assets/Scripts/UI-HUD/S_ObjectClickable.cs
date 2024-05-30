using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_ObjectClickable : MonoBehaviour
{
    private bool _isFocused = false;
    [SerializeField] private Animator _animatorDoor;
    [SerializeField] private Animator _animatorCameraGarage;
    [SerializeField] private Animator _animatorFade;

    private void OnMouseUpAsButton()
    {
        Activate();
    }

    private void OnMouseOver()
    {
        if (!_isFocused)
        {
            SetColor(Color.blue);
        }
    }

    private void OnMouseExit()
    {
        if (!_isFocused)
        {
            SetColor(Color.white);
        }
    }

    public void OnFocus()
    {
        _isFocused = true;
        SetColor(Color.blue);
    }

    public void OnFocusLost()
    {
        _isFocused = false;
        SetColor(Color.white);
    }

    public void OnActivated()
    {
        Activate();
    }

    private void Activate()
    {
        if (gameObject.CompareTag("Garage"))
        {
            _animatorDoor.SetBool("Open", true);
            _animatorCameraGarage.SetBool("MoveToGarage", true);
            Debug.Log(_animatorCameraGarage);
        }
        else if (gameObject.CompareTag("Tournament"))
        {
            SceneManager.LoadScene("TournamentScene");
        }
        else if (gameObject.CompareTag("Shop"))
        {
            StartCoroutine(SwitchShop());
        }
    }

    private void SetColor(Color color)
    {
        gameObject.LeanColor(color, 0.1f);
    }

    private IEnumerator SwitchShop()
    {
        _animatorCameraGarage.SetBool("MoveToShop", true);
        yield return new WaitForSeconds(3.29f);
        _animatorFade.SetBool("Fade", true);
        SceneManager.LoadScene("Shop");
        yield return null;
    }
}
