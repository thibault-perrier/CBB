using System.Collections.Generic;
using Systems;
using UnityEngine;
using UnityEngine.UI;

public class S_ShopController : MonoBehaviour
{
    public List<Image> _row1Images;
    public List<Image> _row2Images;
    public List<Image> _row3Images;

    public GameObject _rowContainer;  // Le conteneur des lignes
    public GameObject _row1;          // Ligne 1
    public GameObject _row2;          // Ligne 2
    public GameObject _row3;          // Ligne 3

    public GameObject _imagePrefab;   // Le prefab d'image

    public GameObject _selectPoint;   // Point de sélection pour l'affichage

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

        foreach (S_WeaponData weaponData in S_DataRobotComponent.Instance._weaponDatas)
        {
            GameObject imageObject = Instantiate(_imagePrefab, _row3.transform);
            Image image = imageObject.GetComponent<Image>();
            image.sprite = weaponData.WeaponSrite;
            _row3Images.Add(image);
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

        // Déplacer le conteneur pour afficher la bonne ligne
        MoveRowContainer();
        // Déplacer la ligne pour afficher la bonne image
        MoveRowToImage();
    }

    void ChangeRow(int direction)
    {
        _currentRow += direction;
        if (_currentRow < 0) _currentRow = 2;
        if (_currentRow > 2) _currentRow = 0;

        // S'assurer que l'index actuel est dans les limites de la nouvelle ligne
        EnsureIndexInBounds();
    }

    void ChangeImage(int direction)
    {
        _currentIndex += direction;

        // Boucler l'index actuel dans les limites des images de la ligne actuelle
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
            case 2: return _row3Images;
            default: return null;
        }
    }

    void MoveRowContainer()
    {
        // Calculer la position en coordonnée mondiale de _selectPoint
        Vector3 selectPointWorldPos = _selectPoint.transform.position;

        // Calculer la position en coordonnée mondiale de la ligne actuelle
        Vector3 currentRowWorldPos = GetRowRectTransform(_currentRow).position;

        // Calculer la distance en coordonnée mondiale
        float distance = selectPointWorldPos.y - currentRowWorldPos.y;

        // Appliquer le déplacement à _rowContainer pour aligner la ligne actuelle avec _selectPoint
        RectTransform rowContainerRectTransform = _rowContainer.GetComponent<RectTransform>();
        Vector3 rowContainerLocalPos = rowContainerRectTransform.localPosition;
        rowContainerRectTransform.localPosition = new Vector3(rowContainerLocalPos.x, rowContainerLocalPos.y + distance, rowContainerLocalPos.z);
    }

    void MoveRowToImage()
    {
        // Récupérer les images de la ligne actuelle
        List<Image> currentRowImages = GetRowImages(_currentRow);
        if (currentRowImages == null || currentRowImages.Count == 0) return;

        // Calculer la position en coordonnée mondiale de _selectPoint
        Vector3 selectPointWorldPos = _selectPoint.transform.position;

        // Calculer la position en coordonnée mondiale de l'image sélectionnée
        Vector3 selectedImageWorldPos = currentRowImages[_currentIndex].transform.position;

        // Calculer la distance en coordonnée mondiale
        float distance = selectPointWorldPos.x - selectedImageWorldPos.x;

        // Appliquer le déplacement à la ligne actuelle pour centrer l'image sélectionnée
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
            case 2: return _row3.GetComponent<RectTransform>();
            default: return null;
        }
    }
}
