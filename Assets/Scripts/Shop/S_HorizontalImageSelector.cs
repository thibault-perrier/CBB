using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_HorizontalImageSelector : MonoBehaviour
{
    public RectTransform[] _frames;
    private RectTransform[][] _selectedImages;
    public GameObject _slot;
    public TextMeshProUGUI _frameLabel;
    public TextMeshProUGUI _moneyText;
    public TextMeshProUGUI _pageText;

    public float _stepDelay = 0.1f;
    public float _verticalCooldown = 1f;

    private Coroutine[] _movementCoroutines;
    private bool _canMoveVertically = true;

    private int _currentFrameIndex = 0;
    private int _currentIndex = 0;
    public int _currentMoney = 0;
    public int _pageNumber = 1;

    public static S_HorizontalImageSelector Instance;

    // List to save the last selected index for each frame
    private int[] _lastSelectedIndexes;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeFrames();
        UpdateFrameLabel();
        UpdateShopText();
    }

    private void InitializeFrames()
    {
        int frameCount = _frames.Length;
        _selectedImages = new RectTransform[frameCount][];
        _movementCoroutines = new Coroutine[frameCount];
        _lastSelectedIndexes = new int[frameCount]; // Initialize the list with the length of frame count

        for (int j = 0; j < frameCount; j++)
        {
            int childCount = _frames[j].childCount;
            _selectedImages[j] = new RectTransform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                _selectedImages[j][i] = _frames[j].GetChild(i).GetComponent<RectTransform>();
            }

            if (_selectedImages[j].Length > 0)
            {
                _lastSelectedIndexes[j] = _selectedImages[j].Length / 2; // Set initial selected index to middle
                _currentIndex = _lastSelectedIndexes[j]; // Set the current index to the middle
                UpdateSelectedImage(j, _lastSelectedIndexes[j]);
                _frames[j].localPosition = new Vector3(-_selectedImages[j][_lastSelectedIndexes[j]].localPosition.x, _frames[j].localPosition.y, _frames[j].localPosition.z);
                _slot.transform.position = new Vector3(_selectedImages[j][_lastSelectedIndexes[j]].position.x, _slot.transform.position.y, _slot.transform.position.z);
            }
        }
    }

    public void UpdateShopText()
    {
        _moneyText.text = "Money: " + _currentMoney.ToString();
        _pageText.text = "Page: " + _pageNumber.ToString();
    }

    private void Update()
    {
        float horizontalInput = -Input.GetAxis("Horizontal");
        float verticalInput = -Input.GetAxis("Vertical");

        if (_movementCoroutines[_currentFrameIndex] == null)
        {
            if (horizontalInput > 0.5f && _currentIndex < _selectedImages[_currentFrameIndex].Length - 1)
            {
                _currentIndex++;
                _lastSelectedIndexes[_currentFrameIndex] = _currentIndex; // Update the last selected index
                _movementCoroutines[_currentFrameIndex] = StartCoroutine(MoveFrameToImage(_currentFrameIndex, _currentIndex));
            }
            else if (horizontalInput < -0.5f && _currentIndex > 0)
            {
                _currentIndex--;
                _lastSelectedIndexes[_currentFrameIndex] = _currentIndex; // Update the last selected index
                _movementCoroutines[_currentFrameIndex] = StartCoroutine(MoveFrameToImage(_currentFrameIndex, _currentIndex));
            }
        }

        if (_canMoveVertically)
        {
            if (verticalInput > 0.5f && _currentFrameIndex < _frames.Length - 1)
            {
                StartCoroutine(HandleVerticalInput(1));
            }
            else if (verticalInput < -0.5f && _currentFrameIndex > 0)
            {
                StartCoroutine(HandleVerticalInput(-1));
            }
        }
    }

    private IEnumerator HandleVerticalInput(int direction)
    {
        _canMoveVertically = false;

        if (direction == 1)
        {
            S_SwipeController.instance.Next();
        }
        else if (direction == -1)
        {
            S_SwipeController.instance.Previous();
        }

        yield return new WaitForSeconds(_verticalCooldown);

        _canMoveVertically = true;
    }

    private IEnumerator MoveFrameToImage(int frameIndex, int imageIndex)
    {
        Vector3 startPosition = _frames[frameIndex].localPosition;
        Vector3 endPosition = new Vector3(-_selectedImages[frameIndex][imageIndex].localPosition.x, _frames[frameIndex].localPosition.y, _frames[frameIndex].localPosition.z);

        float elapsedTime = 0f;

        while (elapsedTime < _stepDelay)
        {
            _frames[frameIndex].localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / _stepDelay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _frames[frameIndex].localPosition = endPosition;

        UpdateSelectedImage(frameIndex, imageIndex);

        _slot.transform.position = new Vector3(_selectedImages[frameIndex][imageIndex].position.x, _slot.transform.position.y, _slot.transform.position.z);

        _movementCoroutines[frameIndex] = null;
    }

    private void UpdateSelectedImage(int frameIndex, int imageIndex)
    {
        for (int i = 0; i < _selectedImages[frameIndex].Length; i++)
        {
            Image image = _selectedImages[frameIndex][i].GetComponent<Image>();
            Color imageColor = image.color;
            imageColor.a = (i == imageIndex) ? 1f : 0.5f;
            image.color = imageColor;
        }
    }

    public void ChangeFrame(int direction)
    {
        _currentFrameIndex = (_currentFrameIndex + direction + _frames.Length) % _frames.Length;
        _currentIndex = _lastSelectedIndexes[_currentFrameIndex]; // Use the last selected index for the new frame
        UpdateSelectedImage(_currentFrameIndex, _currentIndex);
        _slot.transform.position = new Vector3(_selectedImages[_currentFrameIndex][_currentIndex].position.x, _slot.transform.position.y, _slot.transform.position.z);

        UpdateFrameLabel();
    }

    private void UpdateFrameLabel()
    {
        string[] frameLabels = { "Frames", "Weapons", "Starter Pack", "Background Logo", "Front Logo" };
        if (_frameLabel != null && _currentFrameIndex < frameLabels.Length)
        {
            _frameLabel.text = frameLabels[_currentFrameIndex];
        }
    }
}
