using System.Collections.Generic;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_ShopController : MonoBehaviour
{
    public static Canvas Shop;

    public List<Image> _row1Images;
    public List<Image> _row2Images;

    public GameObject _rowContainer;  
    public GameObject _row1;          
    public GameObject _row2;          

    public GameObject _imagePrefab;

    public GameObject _selectPoint;

    [SerializeField] private Text _Category;
    [SerializeField] private Text _Solde;
    [SerializeField] private Text _Name;
    [SerializeField] private Text _Cost;
    [SerializeField] private Text _Life;
    [SerializeField] private Text _Damage;

    private int _currentRow = 0;
    private int _currentIndex = 0;

    void Start()
    {
        GenerateImages();
        DisplaySolde();
    }

    void GenerateImages()
    {
        foreach (S_WeaponData weaponData in S_DataRobotComponent.Instance._weaponDatas)
        {
            GameObject imageObject = Instantiate(_imagePrefab, _row1.transform);
            Image image = imageObject.GetComponent<Image>();
            image.sprite = weaponData.WeaponSrite;
            _row1Images.Add(image);
        }

        foreach (S_FrameData _frameDatas in S_DataRobotComponent.Instance._frameDatas)
        {
            GameObject imageObject = Instantiate(_imagePrefab, _row2.transform);
            Image image = imageObject.GetComponent<Image>();
            image.sprite = _frameDatas.FrameSrite;
            _row2Images.Add(image);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeRow(-1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeRow(1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeImage(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeImage(-1);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BuyObject();
        }   
        Debug.Log("ligne : " + _currentRow + "  colonne : " + _currentIndex);

        MoveRowContainer();
        MoveRowToImage();
        DisplayInfoObject();
    }

    void ChangeRow(int direction)
    {
        _currentRow += direction;
        if (_currentRow < 0) _currentRow = 1;
        if (_currentRow > 1) _currentRow = 0;
        
        EnsureIndexInBounds();
    }

    public void ChangeRow()
    {
        ChangeRow(1);
    }

    void ChangeImage(int direction)
    {
        _currentIndex += direction;
        
        EnsureIndexInBounds();
    }

    void EnsureIndexInBounds()
    {
        List<Image> currentRowImages = GetRowImages(_currentRow);
        if (_currentIndex < 0) _currentIndex = currentRowImages.Count - 1;
        if (_currentIndex >= currentRowImages.Count) _currentIndex = 0;
    }

    List<Image> GetRowImages(int row)
    {
        switch (row)
        {
            case 0: return _row1Images;
            case 1: return _row2Images;
            default: return null;
        }
    }

    void MoveRowContainer()
    {
        Vector3 selectPointWorldPos = _selectPoint.transform.position;

        Vector3 currentRowWorldPos = GetRowRectTransform(_currentRow).position;

        float distance = selectPointWorldPos.y - currentRowWorldPos.y;

        RectTransform rowContainerRectTransform = _rowContainer.GetComponent<RectTransform>();
        Vector3 rowContainerLocalPos = rowContainerRectTransform.localPosition;
        rowContainerRectTransform.localPosition = new Vector3(rowContainerLocalPos.x, rowContainerLocalPos.y + distance, rowContainerLocalPos.z);
    }

    void MoveRowToImage()
    {
        List<Image> currentRowImages = GetRowImages(_currentRow);
        if (currentRowImages == null || currentRowImages.Count == 0) return;

        Vector3 selectPointWorldPos = _selectPoint.transform.position;

        Vector3 selectedImageWorldPos = currentRowImages[_currentIndex].transform.position;
        
        float distance = selectPointWorldPos.x - selectedImageWorldPos.x;

        RectTransform currentRowRectTransform = GetRowRectTransform(_currentRow);
        Vector3 currentRowLocalPos = currentRowRectTransform.localPosition;
        currentRowRectTransform.localPosition = new Vector3(currentRowLocalPos.x + distance, currentRowLocalPos.y, currentRowLocalPos.z);
    }

    RectTransform GetRowRectTransform(int row)
    {
        switch (row)
        {
            case 0: return _row1.GetComponent<RectTransform>();
            case 1: return _row2.GetComponent<RectTransform>();
            default: return null;
        }
    }

    public void DisplayInfoObject()
    {
        string name = "";
        switch (_currentRow)
        {
            case 0:
                S_WeaponData weaponData = S_DataRobotComponent.Instance._weaponDatas[_currentIndex];
                _Category.text = "Weapons";
                name = weaponData.name.Split("_")[1].Split("Data")[0];
                _Name.text = name;
                _Cost.text =    "Cost : " + weaponData.Cost.ToString();
                _Life.text =    "Life : " + weaponData.MaxLife.ToString();
                _Damage.text =  "Attk : " + weaponData.Damage.ToString();
                break;
            case 1:
                S_FrameData frameData = S_DataRobotComponent.Instance._frameDatas[_currentIndex];
                _Category.text = "Frames";
                name = frameData.name.Split("_")[1].Split("Data")[0];
                _Name.text = name;
                _Cost.text = "Cost : " + frameData.Cost.ToString();
                _Life.text = "Life : " + frameData.MaxLife.ToString();
                _Damage.text = "";
                break;
            default: break;
        }
    }

    public void DisplaySolde()
    {
        _Solde.text = "Money : " + S_DataGame.Instance.inventory.CurrentMoney.ToString();
    }

    public void BuyObject()
    {
        switch (_currentRow)
        {
            case 0:
                S_WeaponData weaponData = S_DataRobotComponent.Instance._weaponDatas[_currentIndex];
                S_DataGame.Instance.inventory.BuyWeapon(weaponData);
                break;
            case 1:
                S_FrameData frameData = S_DataRobotComponent.Instance._frameDatas[_currentIndex];
                S_DataGame.Instance.inventory.BuyFrame(frameData);
                break;
            default: break;
        }
        S_DataGame.Instance.SaveInventory();
        DisplaySolde();
    }

    public void LeaveShop(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeInHierarchy)
        {
            LeaveShop();
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
