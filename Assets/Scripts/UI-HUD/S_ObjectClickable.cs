using UnityEngine;
using UnityEngine.SceneManagement;

public class S_ObjectClickable : MonoBehaviour
{
    private void OnMouseUpAsButton()
    {
        if (gameObject.CompareTag("Garage"))
        {
            SceneManager.LoadScene("Shop");
        }

        else if (gameObject.CompareTag("Tournament"))
        {
            SceneManager.LoadScene("TournamentScene");
        }
    }

    private void OnMouseOver()
    {
        gameObject.LeanColor(Color.blue, 0.1f);

    }

    private void OnMouseExit()
    {
        gameObject.LeanColor(Color.white, 0.1f);
    }
}