using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_HorizontalImageSelector : MonoBehaviour
{
    public RectTransform[] _frames;
    public GameObject _slot;
    public float _stepDelay = 0.1f;
    public TextMeshProUGUI _frameLabel;
    public TextMeshProUGUI _moneyText;

    private RectTransform[][] _selectedImages;
    private Coroutine[] _movementCoroutines;
    private int _currentFrameIndex = 0;
    private int _currentIndex = 0;
    public int _currentMoney = 200;

    public static S_HorizontalImageSelector Instance;

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
                UpdateSelectedImage(j, _currentIndex);
            }
        }
    }

    public void UpdateShopText()
    {
        _moneyText.text = _currentMoney + _moneyText.text.ToString();
    }

    private void Update()
    {
        float horizontalInput = -Input.GetAxis("Horizontal");

        if (_movementCoroutines[_currentFrameIndex] == null)
        {
            if (horizontalInput > 0.5f && _currentIndex < _selectedImages[_currentFrameIndex].Length - 1)
            {
                _currentIndex++;
                _movementCoroutines[_currentFrameIndex] = StartCoroutine(MoveFrameToImage(_currentFrameIndex, _currentIndex));
            }
            else if (horizontalInput < -0.5f && _currentIndex > 0)
            {
                _currentIndex--;
                _movementCoroutines[_currentFrameIndex] = StartCoroutine(MoveFrameToImage(_currentFrameIndex, _currentIndex));
            }
        }
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
        UpdateSelectedImage(_currentFrameIndex, _currentIndex);
        _slot.transform.position = new Vector3(_selectedImages[_currentFrameIndex][_currentIndex].position.x, _slot.transform.position.y, _slot.transform.position.z);

        UpdateFrameLabel();
    }

    private void UpdateFrameLabel()
    {
        string[] frameLabels = { "Chassis", "Weapons", "Pack" };
        if (_frameLabel != null && _currentFrameIndex < frameLabels.Length)
        {
            _frameLabel.text = frameLabels[_currentFrameIndex];
        }
    }
}
