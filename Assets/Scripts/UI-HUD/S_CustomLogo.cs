using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class S_CustomLogo : MonoBehaviour
{
    public TMP_Dropdown overlayImageDropdown;
    public Image backgroundImage;
    public Image overlayImage;
    public List<Sprite> overlayImages;
    public List<GameObject> buttons;
    public GameObject logosScrollView;
    public GameObject logoCustomize;

    private Color _defaultBackgroundColor = Color.white; // Couleur par défaut de fond
    private Color _defaultOverlayColor = Color.white;    // Couleur par défaut d'incrustation

    private Color _currentBackgroundColor;
    private Color _currentOverlayColor;
    private int _currentOverlayImageIndex;

    private S_ShopManager _shopManager;
    private bool _isChoosingLogo = false;

    private void Awake()
    {
        _shopManager = GetComponentInChildren<S_ShopManager>();
        _shopManager.ChooseLogoComplete += OnSetLogo;
    }

    void Start()
    {
        S_DataGame.Instance.LoadInventory();
        overlayImageDropdown.onValueChanged.AddListener(delegate { ChangeOverlayImage(); });

        // Charger les données sauvegardées
        LoadLogo();
        logosScrollView.SetActive(false);

        logoCustomize.SetActive(false);
    }

    private void OnDestroy()
    {
        _shopManager.ChooseLogoComplete -= OnSetLogo;
    }

    /// <summary>
    /// Event that make it possible to set a sprite to the logo creation
    /// </summary>
    /// <param name="sprite"></param>
    private void OnSetLogo(int index)
    {
        if (_isChoosingLogo)
        {
            overlayImage.sprite = overlayImages[index];
            _currentOverlayImageIndex = index;

            EventSystem.current.SetSelectedGameObject(null);

            foreach (GameObject button in buttons)
            {
                button.SetActive(true);
            }

            StartCoroutine(SetButtonAsSelected(buttons[0]));

            logosScrollView.SetActive(false);
            _isChoosingLogo = false;
        }
    }

    public void OpenColorPickerForBackground()
    {
        ColorPicker.Create(_currentBackgroundColor, "SELECT BACKGROUND COLOR", OnBackgroundColorChanged, SaveBackgroundColor, true);
    }

    public void OpenColorPickerForOverlay()
    {
        ColorPicker.Create(_currentOverlayColor, "SELECT OVERLAY COLOR", OnOverlayColorChanged, SaveOverlayColor, true);
    }

    void OnBackgroundColorChanged(Color color)
    {
        _currentBackgroundColor = color;
        backgroundImage.color = color;
        SaveBackgroundColor(color);
    }

    void OnOverlayColorChanged(Color color)
    {
        _currentOverlayColor = color;
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
        string hexColor = ColorUtility.ToHtmlStringRGBA(_currentBackgroundColor);
        S_DataGame.Instance.inventory.SaveBackgroundColor(hexColor);

        // Sauvegarder la couleur d'incrustation
        string overlayHexColor = ColorUtility.ToHtmlStringRGBA(_currentOverlayColor);
        S_DataGame.Instance.inventory.SaveOverlayColor(overlayHexColor);

        // Sauvegarder l'index de l'image d'incrustation
        S_DataGame.Instance.inventory.SaveOverlayImage(_currentOverlayImageIndex);

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
                    backgroundImage.color = backgroundColor;
                    _currentBackgroundColor = backgroundImage.color;
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
                _currentOverlayColor = overlayColor;
                Debug.Log("Overlay Color Applied: " + overlayColorHex);
            }
            else
            {
                Debug.LogError("Failed to parse overlay color from hex string: " + overlayColorHex);
            }

            // Charger l'image d'incrustation sélectionnée dans le dropdown
            S_DataGame.Instance.inventory.LoadOverlayImageIndex();
            int overlayImageIndex = S_DataGame.Instance.inventory.overlayImageIndex;
            if (overlayImageIndex >= 0 && overlayImageIndex < overlayImages.Count)
            {
                overlayImage.sprite = overlayImages[overlayImageIndex];
                _currentOverlayImageIndex = overlayImageIndex;
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
        _currentBackgroundColor = _defaultBackgroundColor;
        _currentOverlayColor = _defaultOverlayColor;

        // Appliquer les couleurs par défaut
        backgroundImage.color = new Color(_defaultBackgroundColor.r, _defaultBackgroundColor.g, _defaultBackgroundColor.b, 1f);
        overlayImage.color = new Color(_defaultOverlayColor.r, _defaultOverlayColor.g, _defaultOverlayColor.b, 1f);

        // Réinitialiser l'image d'incrustation à la première image ou à une valeur par défaut
        overlayImageDropdown.value = 0;
        ChangeOverlayImage();
    }


    private void SaveBackgroundColor(Color color)
    {
        _currentBackgroundColor = color;
        SaveLogo();
    }

    private void SaveOverlayColor(Color color)
    {
        _currentOverlayColor = color;
        SaveLogo();
    }

    private IEnumerator SetButtonAsSelected(GameObject button)
    {
        yield return new WaitForSeconds(0.1f);
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void SetIsChoosingLogo(bool isChoosingLogo)
    {
        _isChoosingLogo = isChoosingLogo;
    }
}
