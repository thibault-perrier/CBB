using System;
using System.Collections;
using System.Collections.Generic;
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

    public bool activeBackShop = false;
    private bool _navigating = false;

    public static S_ShopManager Instance;

    private int[] _lastSelectedIndexes;

    public delegate void OnChooseLogoComplete(int index, Sprite sprite);
    public event OnChooseLogoComplete ChooseLogoComplete;

    [Serializable]
    public struct ElementInfo
    {
        public int Price;
        public int Hp;
        public int Weight;
        public Sprite Sprite;
    }

    [Serializable]
    public class FrameData
    {
        public ElementInfo[] frameElements;
    }

    public List<FrameData> frameData = new List<FrameData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        InitializeFrames();
        UpdateFrameLabel();
        UpdateShopText();
    }

    private void OnEnable()
    {
        _navigating = false;
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
            if (_statsText != null)
                _statsText.SetActive(false);
        }
        else
        {
            if (_statsText != null)
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
        if (_moneyText != null)
            _moneyText.text = "Money: " + S_DataGame.Instance.inventory.CurrentMoney + " $";//TODO: GET MONEY FROM PLAYER_DATA--------------------------------------
        if (_pageText != null)
            _pageText.text = "Page: " + (_currentFrameIndex + 1).ToString();
        if (_priceItem != null && _currentFrameIndex < frameData.Count && _currentItemIndex < frameData[_currentFrameIndex].frameElements.Length)
        {
            ElementInfo currentElement = frameData[_currentFrameIndex].frameElements[_currentItemIndex];
            _priceItem.text = "Price: " + currentElement.Price.ToString();
            _statItem.text = "HP: " + currentElement.Hp.ToString() + "\nWeight: " + currentElement.Weight.ToString();
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

        if (BuyElement.Hp != 0 && gameObject.activeInHierarchy) //it mean we're in the shop
        {
            if (S_DataGame.Instance.inventory.CurrentMoney >= BuyElement.Price)
            {
                if (BuyElement.Sprite != null)
                {

                }
                S_DataGame.Instance.inventory.CurrentMoney = S_DataGame.Instance.inventory.CurrentMoney - BuyElement.Price;
                UpdateShopText();

                //TODO : Update Inventory Quantity -------------------------------------------------------------------------------------
            }
            else
            {
                Debug.Log("Need more Money");
            }
        }
        else
        {
            ChooseLogoComplete?.Invoke(_currentItemIndex, BuyElement.Sprite);
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
        if (context.performed && gameObject.activeInHierarchy)
        {
            float horizontalInput = context.ReadValue<Vector2>().x;
            float verticalInput = context.ReadValue<Vector2>().y;

            if (_movementCoroutines[_currentFrameIndex] == null)
            {
                if (!_navigating && horizontalInput > 0.5f && _currentItemIndex < _shopItems[_currentFrameIndex].Length - 1)
                {
                    _currentItemIndex++;
                    _lastSelectedIndexes[_currentFrameIndex] = _currentItemIndex;
                    _movementCoroutines[_currentFrameIndex] = StartCoroutine(MoveHorizontalFrameImages(_currentFrameIndex, _currentItemIndex));
                    _navigating = true;
                }
                else if (!_navigating && horizontalInput < -0.5f && _currentItemIndex > 0)
                {
                    _currentItemIndex--;
                    _lastSelectedIndexes[_currentFrameIndex] = _currentItemIndex;
                    _movementCoroutines[_currentFrameIndex] = StartCoroutine(MoveHorizontalFrameImages(_currentFrameIndex, _currentItemIndex));
                    _navigating = true;
                }
            }

            if (_canMoveVertically)
            {
                if (!_navigating && verticalInput > 0.5f && _currentFrameIndex < _frames.Length - 1)
                {
                    _navigating = true;
                    StartCoroutine(MoveVerticalFrames(1));
                }
                else if (!_navigating && verticalInput < -0.5f && _currentFrameIndex > 0)
                {
                    _navigating = true;
                    StartCoroutine(MoveVerticalFrames(-1));
                }
            }
        }
        if (context.canceled)
            _navigating = false;
    }

    public void LeaveShop(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy && activeBackShop == true)
        {
            S_ClickablesManager.Instance.mainMenu.SetActive(true);
            S_ClickablesManager.Instance.shopMenu.SetActive(false);
            S_DataGame.Instance.SaveInventory();
            S_ObjectClickable.Instance.LaunchAnimBackToMenuFromShop();
            S_ClickablesManager.Instance.ResetAllClickables();
        }
    }

    public void LeaveShop()
    {
        S_ClickablesManager.Instance.mainMenu.SetActive(true);
        S_ClickablesManager.Instance.shopMenu.SetActive(false);
        S_DataGame.Instance.SaveInventory();
        S_ObjectClickable.Instance.LaunchAnimBackToMenuFromShop();
        S_ClickablesManager.Instance.ResetAllClickables();
    }
}
