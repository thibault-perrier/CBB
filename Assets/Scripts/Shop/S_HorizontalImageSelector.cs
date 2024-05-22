using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalImageSelector : MonoBehaviour
{
    public RectTransform _frame;
    public float moveSpeed = 800f;
    public GameObject _slot;

    private RectTransform[] _selectedImages;

    void Start()
    {
        int childCount = _frame.childCount;
        _selectedImages = new RectTransform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            _selectedImages[i] = _frame.GetChild(i).GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        float horizontalInput = -Input.GetAxis("Horizontal");
        foreach (RectTransform imageRect in _selectedImages)
        {
            Vector3 currentPosition = imageRect.position;
            currentPosition.x += horizontalInput * moveSpeed * Time.deltaTime;
            imageRect.position = currentPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SelectableImage"))
        {
            Image image = other.GetComponent<Image>();
            if (image != null)
            {
                image.transform.position = _slot.transform.position;
                StartCoroutine(Select());
                Color imageColor = image.color;
                imageColor.a = 1f;
                image.color = imageColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SelectableImage"))
        {
            Image image = other.GetComponent<Image>();
            if (image != null)
            {
                Color imageColor = image.color;
                imageColor.a = 0.5f;
                image.color = imageColor;
            }
        }
    }

    IEnumerator Select()
    {
        moveSpeed = 0;
        yield return new WaitForSeconds(1f);
        moveSpeed = 1200;
        yield return null;
    }
}
