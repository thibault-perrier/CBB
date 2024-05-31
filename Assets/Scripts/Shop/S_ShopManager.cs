using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_ShopManager : MonoBehaviour
{
    public RectTransform[] _frames;
    private RectTransform[][] _shopItems;
    public GameObject _slot;
    public GameObject _statsText;

    public TextMeshProUGUI _frameLabel;
    public TextMeshProUGUI _moneyText;
    public TextMeshProUGUI _pageText;
    public TextMeshProUGUI _priceItem;
    public TextMeshProUGUI _statItem;

    public float _stepDelay = 0.1f;
    public float _verticalCooldown = 1f;

    private Coroutine[] _movementCoroutines;
    private bool _canMoveVertically = true;

    public int _currentFrameIndex = 0;
    public int _currentItemIndex = 0;


    public static S_ShopManager Instance;

    private int[] _lastSelectedIndexes;

    [Serializable]
    public struct ElementInfo
    {
        public int _price;
        public int _hp;
        public int _weight;
    }

    [Serializable]
    public class FrameData
    {
        public ElementInfo[] frameElements;
    }

    public FrameData[] frameData;

    void Awake()
    {
        if(Instance == null) 
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
        _shopItems = new RectTransform[frameCount][];
        _movementCoroutines = new Coroutine[frameCount];
        _lastSelectedIndexes = new int[frameCount];

        for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
        {
            int childCount = _frames[frameIndex].childCount;
            _shopItems[frameIndex] = new RectTransform[childCount];

            for (int pictureIndex = 0; pictureIndex < childCount; pictureIndex++)
            {
                _shopItems[frameIndex][pictureIndex] = _frames[frameIndex].GetChild(pictureIndex).GetComponent<RectTransform>();
            }

            if (_shopItems[frameIndex].Length > 0)
            {
                _lastSelectedIndexes[frameIndex] = _shopItems[frameIndex].Length / 2;
                UpdateImagesOnNewSelection(frameIndex, _lastSelectedIndexes[frameIndex]);
                _frames[frameIndex].localPosition = new Vector3(
                    _shopItems[frameIndex][_lastSelectedIndexes[frameIndex]].localPosition.x, 
                    _frames[frameIndex].localPosition.y, 
                    _frames[frameIndex].localPosition.z);

                _slot.transform.position = new Vector3(
                    _shopItems[frameIndex][_lastSelectedIndexes[frameIndex]].position.x, 
                    _slot.transform.position.y, 
                    _slot.transform.position.z);
            }
        }
        _currentItemIndex = _lastSelectedIndexes[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Logo"))
        {
            _statsText.SetActive(false);
        }
        else
        {
            _statsText.SetActive(true);
        }
    }

    private void UpdateImagesOnNewSelection(int frameIndex, int imageIndex)
    {
        for (int i = 0; i < _shopItems[frameIndex].Length; i++)
        {
            Image image = _shopItems[frameIndex][i].GetComponent<Image>();
            Color imageColor = image.color;
            imageColor.a = (i == imageIndex) ? 1f : 0.5f;
            image.color = imageColor;
        }
    }

    private void UpdateFrameLabel() //Change text
    {
        string[] frameLabels = { "Frames", "Weapons", "Starter Pack", "Background Logo", "Front Logo" };
        if (_frameLabel != null && _currentFrameIndex < frameLabels.Length)
        {
            _frameLabel.text = frameLabels[_currentFrameIndex];
        }
    }

    public void UpdateShopText()
    {
        _moneyText.text = "Money: " + S_DataGame.Instance.inventory.CurrentMoney;//TODO: GET MONEY FROM PLAYER_DATA--------------------------------------
        _pageText.text = "Page: " + (_currentFrameIndex + 1).ToString();
        if (_priceItem != null && _currentFrameIndex < frameData.Length && _currentItemIndex < frameData[_currentFrameIndex].frameElements.Length)
        {
            ElementInfo currentElement = frameData[_currentFrameIndex].frameElements[_currentItemIndex];
            _priceItem.text = "Price: " + currentElement._price.ToString();
            _statItem.text = "HP: " + currentElement._hp.ToString() + "\nWeight: " + currentElement._weight.ToString();
            //TODO : ADD DAMAGE IF WEAPON -----------------------------------------------------------------------------------------------
        }
    }
    
    public void ChangeFrame(int direction)
    {
        _currentFrameIndex = _currentFrameIndex + direction;
        _currentItemIndex = _lastSelectedIndexes[_currentFrameIndex];
        UpdateImagesOnNewSelection(_currentFrameIndex, _currentItemIndex);
        _slot.transform.position = new Vector3(_shopItems[_currentFrameIndex][_currentItemIndex].position.x, _slot.transform.position.y, _slot.transform.position.z);

        UpdateFrameLabel();
        UpdateShopText();
    }

    private IEnumerator MoveVerticalFrames(int direction) //move vertical
    {
        _canMoveVertically = false;

        if (direction == 1)
        {
            S_SwipeController.Instance.Next();
        }
        else if (direction == -1)
        {
            S_SwipeController.Instance.Previous();
        }

        yield return new WaitForSeconds(_verticalCooldown);

        _canMoveVertically = true;
    }

    private IEnumerator MoveHorizontalFrameImages(int frameIndex, int imageIndex) //move horizontal
    {
        Vector3 startPosition = _frames[frameIndex].localPosition;
        Vector3 endPosition = new Vector3(-_shopItems[frameIndex][imageIndex].localPosition.x, _frames[frameIndex].localPosition.y, _frames[frameIndex].localPosition.z);

        float elapsedTime = 0f;

        while (elapsedTime < _stepDelay)
        {
            _frames[frameIndex].localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / _stepDelay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _frames[frameIndex].localPosition = endPosition;

        UpdateImagesOnNewSelection(frameIndex, imageIndex);

        _slot.transform.position = new Vector3(_shopItems[frameIndex][imageIndex].position.x, _slot.transform.position.y, _slot.transform.position.z);

        _movementCoroutines[frameIndex] = null;

        UpdateShopText();
    }

    public void PurchaseItem()
    {
        ElementInfo BuyElement = frameData[_currentFrameIndex].frameElements[_currentItemIndex];
      
        if(S_DataGame.Instance.inventory.CurrentMoney >= BuyElement._price )
        {
            S_DataGame.Instance.inventory.CurrentMoney = S_DataGame.Instance.inventory.CurrentMoney - BuyElement._price;
            UpdateShopText();
            //TODO : Update Inventory Quantity -------------------------------------------------------------------------------------
        }
        else
        {
            Debug.Log("Need more Money");
        }
    }

    public void OnBuyItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PurchaseItem();
        }
    }

    public void OnShopControllers(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float horizontalInput = context.ReadValue<Vector2>().x;
            float verticalInput = context.ReadValue<Vector2>().y;   

            if (_movementCoroutines[_currentFrameIndex] == null)
            {
                if (horizontalInput > 0.5f && _currentItemIndex < _shopItems[_currentFrameIndex].Length - 1)
                {
                    _currentItemIndex++;
                    _lastSelectedIndexes[_currentFrameIndex] = _currentItemIndex;
                    _movementCoroutines[_currentFrameIndex] = StartCoroutine(MoveHorizontalFrameImages(_currentFrameIndex, _currentItemIndex));
                }
                else if (horizontalInput < -0.5f && _currentItemIndex > 0)
                {
                    _currentItemIndex--;
                    _lastSelectedIndexes[_currentFrameIndex] = _currentItemIndex;
                    _movementCoroutines[_currentFrameIndex] = StartCoroutine(MoveHorizontalFrameImages(_currentFrameIndex, _currentItemIndex));
                }
            }

            if (_canMoveVertically)
            {
                if (verticalInput > 0.5f && _currentFrameIndex < _frames.Length - 1)
                {
                    StartCoroutine(MoveVerticalFrames(1));
                }
                else if (verticalInput < -0.5f && _currentFrameIndex > 0)
                {
                    StartCoroutine(MoveVerticalFrames(-1));
                }
            }
        }
    }

    public void LeaveShop(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SceneManager.LoadScene("MainMenu");
            S_DataGame.Instance.SaveInventory();
        }
    }
}
