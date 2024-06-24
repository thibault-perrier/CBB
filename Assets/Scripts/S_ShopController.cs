using System.Collections.Generic;
using Systems;
using UnityEngine;
using UnityEngine.UI;

public class S_ShopController : MonoBehaviour
{
    public List<Image> _row1Images;
    public List<Image> _row2Images;

    public GameObject _rowContainer;  
    public GameObject _row1;          
    public GameObject _row2;          

    public GameObject _imagePrefab;

    public GameObject _selectPoint;

    private int _currentRow = 0;
    private int _currentIndex = 0;

    void Start()
    {
        GenerateImages();
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
        Debug.Log("ligne : " + _currentRow + "  colonne : " + _currentIndex);

        MoveRowContainer();
        MoveRowToImage();
    }

    void ChangeRow(int direction)
    {
        _currentRow += direction;
        if (_currentRow < 0) _currentRow = 1;
        if (_currentRow > 1) _currentRow = 0;
        
        EnsureIndexInBounds();
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
}
