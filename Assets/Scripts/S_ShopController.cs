using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Systems;

public class S_ShopController : MonoBehaviour
{
    public List<Image> _row1Images;
    public List<Image> _row2Images;
    public List<Image> _row3Images;

    public GameObject _row1;
    public GameObject _row2;
    public GameObject _row3;

    public GameObject _imagePrefab;

    void Start()
    {
        GenerateImages();
    }

    void GenerateImages()
    {
        foreach (S_WeaponData weaponData in S_DataRobotComponent.Instance._weaponDatas)
        {
            RectTransform rectParent = _row1.GetComponent<RectTransform>();
            GameObject imageObject = Instantiate(_imagePrefab, _row1.transform);
            RectTransform rectObj = _imagePrefab.GetComponent<RectTransform>();

            Image image = imageObject.GetComponent<Image>();
            image.sprite = weaponData.WeaponSrite;
            rectObj.sizeDelta = rectParent.sizeDelta;
            Debug.Log(rectObj.sizeDelta);
            _row1Images.Add(image);
        }

        foreach (S_WeaponData weaponData in S_DataRobotComponent.Instance._weaponDatas)
        {
            GameObject imageObject = Instantiate(_imagePrefab, _row2.transform);

            Image image = imageObject.GetComponent<Image>();

            image.sprite = weaponData.WeaponSrite;
            _row2Images.Add(image);
        }

        foreach (S_WeaponData weaponData in S_DataRobotComponent.Instance._weaponDatas)
        {
            GameObject imageObject = Instantiate(_imagePrefab, _row3.transform);

            Image image = imageObject.GetComponent<Image>();

            image.sprite = weaponData.WeaponSrite;
            _row3Images.Add(image);
        }

        //foreach (S_FrameData frameData in S_DataRobotComponent.Instance._frameDatas)
        //{
        //    GameObject imageObject = Instantiate(_imagePrefab, _row2.transform);

        //    Image image = imageObject.GetComponent<Image>();

        //    image.sprite = frameData.FrameSrite;
        //    _row2Images.Add(image);
        //}

        //foreach (S_FrameData frameData in S_DataRobotComponent.Instance._frameDatas)
        //{
        //    GameObject imageObject = Instantiate(_imagePrefab, _row3.transform);

        //    Image image = imageObject.GetComponent<Image>();

        //    image.sprite = frameData.FrameSrite;
        //    _row3Images.Add(image);
        //}
    }
}
