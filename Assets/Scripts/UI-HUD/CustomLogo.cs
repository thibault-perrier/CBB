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

    private Color defaultBackgroundColor = Color.white; // Couleur par défaut de fond
    private Color defaultOverlayColor = Color.white;    // Couleur par défaut d'incrustation

    private Color currentBackgroundColor;
    private Color currentOverlayColor;

    void Start()
    {
        S_DataGame.Instance.LoadInventory();
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
        SaveBackgroundColor(color);
    }

    void OnOverlayColorChanged(Color color)
    {
        currentOverlayColor = color;
        overlayImage.color = color;
        SaveOverlayColor(color);
    }

    void ChangeOverlayImage()
    {
        int index = overlayImageDropdown.value;
        if (index >= 0 && index < overlayImages.Count)
        {
            overlayImage.sprite = overlayImages[index];
            SaveLogo(); // Sauvegarder le changement de l'image d'incrustation
        }
    }

    public void SaveLogo()
    {
        // Sauvegarder la couleur de fond
        string hexColor = ColorUtility.ToHtmlStringRGBA(currentBackgroundColor);
        S_DataGame.Instance.inventory.SaveBackgroundColor(hexColor);

        // Sauvegarder la couleur d'incrustation
        string overlayHexColor = ColorUtility.ToHtmlStringRGBA(currentOverlayColor);
        S_DataGame.Instance.inventory.SaveOverlayColor(overlayHexColor);

        // Sauvegarder l'index de l'image d'incrustation
        PlayerPrefs.SetInt("OverlayImageIndex", overlayImageDropdown.value);
        PlayerPrefs.Save();

        // Sauvegarder les données de jeu
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
                if (ColorUtility.TryParseHtmlString("#" + backgroundColorHex, out backgroundColor))
                {
                    // Appliquer la couleur de fond chargée avec alpha
                    backgroundImage.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 255f);
                    Debug.Log("Background Color Applied: " + backgroundColorHex);
                }
                else
                {
                    Debug.LogError("Failed to parse background color from hex string: " + backgroundColorHex);
                }
            }

            // Charger la couleur d'incrustation
            S_DataGame.Instance.inventory.LoadOverlayColor();
            string overlayColorHex = S_DataGame.Instance.inventory.overlayColorHex;
            Color overlayColor;
            if (ColorUtility.TryParseHtmlString("#" + overlayColorHex, out overlayColor))
            {
                overlayImage.color = overlayColor;
                Debug.Log("Overlay Color Applied: " + overlayColorHex);
            }
            else
            {
                Debug.LogError("Failed to parse overlay color from hex string: " + overlayColorHex);
            }

            // Charger l'image d'incrustation sélectionnée dans le dropdown
            int overlayImageIndex = PlayerPrefs.GetInt("OverlayImageIndex", 0);
            if (overlayImageIndex >= 0 && overlayImageIndex < overlayImages.Count)
            {
                overlayImage.sprite = overlayImages[overlayImageIndex];
                overlayImageDropdown.value = overlayImageIndex;
                overlayImageDropdown.RefreshShownValue(); // Assurez-vous que la valeur affichée est mise à jour
                Debug.Log("Overlay Image Applied: Index " + overlayImageIndex);
            }
            else
            {
                Debug.LogError("Failed to load overlay image index: " + overlayImageIndex);
            }
        }
        else
        {
            Debug.LogError("S_DataGame Instance is null.");
        }
    }

    public void ResetLogo()
    {
        // Réinitialiser les couleurs aux valeurs par défaut
        currentBackgroundColor = defaultBackgroundColor;
        currentOverlayColor = defaultOverlayColor;

        // Appliquer les couleurs par défaut
        backgroundImage.color = new Color(defaultBackgroundColor.r, defaultBackgroundColor.g, defaultBackgroundColor.b, 1f);
        overlayImage.color = new Color(defaultOverlayColor.r, defaultOverlayColor.g, defaultOverlayColor.b, 1f);

        // Réinitialiser l'image d'incrustation à la première image ou à une valeur par défaut
        overlayImageDropdown.value = 0;
        ChangeOverlayImage();
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
