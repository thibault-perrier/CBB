using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CustomLogo : MonoBehaviour
{
    public TMP_Dropdown overlayImageDropdown;
    public Image backgroundImage;
    public Image overlayImage;
    public List<Sprite> overlayImages;

    private Color currentBackgroundColor;
    private Color currentOverlayColor;

    void Start()
    {
        overlayImageDropdown.onValueChanged.AddListener(delegate { ChangeOverlayImage(); });

        // Charger les données sauvegardées
        LoadLogo();

    }
    public void OpenColorPickerForBackground()
    {
        ColorPicker.Create(currentBackgroundColor, "Select Background Color", OnBackgroundColorChanged, SaveBackgroundColor, true);
    }

    public void OpenColorPickerForOverlay()
    {
        ColorPicker.Create(currentOverlayColor, "Select Overlay Color", OnOverlayColorChanged, SaveOverlayColor, true);
    }

    void OnBackgroundColorChanged(Color color)
    {
        currentBackgroundColor = color;
        backgroundImage.color = color;
    }

    void OnOverlayColorChanged(Color color)
    {
        currentOverlayColor = color;
        overlayImage.color = color;
    }

    void ChangeOverlayImage()
    {
        int index = overlayImageDropdown.value;
        if (index >= 0 && index < overlayImages.Count)
        {
            overlayImage.sprite = overlayImages[index];
        }
    }

    public void SaveLogo()
    {
        PlayerPrefs.SetString("BackgroundColor", ColorUtility.ToHtmlStringRGBA(currentBackgroundColor));
        PlayerPrefs.SetString("OverlayColor", ColorUtility.ToHtmlStringRGBA(currentOverlayColor));
        PlayerPrefs.SetInt("OverlayImageIndex", overlayImageDropdown.value);
        PlayerPrefs.Save();
        S_DataGame.Instance.SaveInventory();
    }

    void LoadLogo()
    {
        // Charger les couleurs depuis S_DataGame
        if (S_DataGame.Instance != null)
        {
            S_DataGame.Instance.inventory.LoadColors();
            string backgroundColorHex = S_DataGame.Instance.inventory.backgroundColorHex;
            float backgroundAlpha = S_DataGame.Instance.inventory.backgroundAlpha;

            if (!string.IsNullOrEmpty(backgroundColorHex))
            {
                Color backgroundColor;
                if (ColorUtility.TryParseHtmlString(backgroundColorHex, out backgroundColor))
                {
                    // Appliquer la couleur de fond chargée avec alpha
                    backgroundImage.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundAlpha);
                }
                else
                {
                    Debug.LogError("Failed to parse background color from hex string: " + backgroundColorHex);
                }
            }
        }
        else
        {
            Debug.LogError("S_DataGame Instance is null.");
        }
    }


    private void SaveBackgroundColor(Color color)
    {
        currentBackgroundColor = color;
        SaveLogo();
    }

    private void SaveOverlayColor(Color color)
    {
        currentOverlayColor = color;
        SaveLogo(); 
    }
}

