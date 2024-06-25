using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S_CustomName : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _prefixDropDown;
    [SerializeField] private TMP_Dropdown _suffixDropDown;

    private List<string> _prefixes = new List<string>();
    private List<string> _suffixes = new List<string>();

    private void Awake()
    {
        InitializeNameLists();
    }

    private void Start()
    {
        if (_prefixDropDown != null && _suffixDropDown != null)
        {
            InitializeDropDown(_prefixDropDown, _prefixes);
            InitializeDropDown(_suffixDropDown, _suffixes);

            LoadName();
        }
    }

    /// <summary>
    /// Put in two list every names present in a text document
    /// </summary>
    private void InitializeNameLists()
    {
        string prefixeContent = "";
        string suffixeContent = "";
        string prefixeFilePath = "NamesData/Prefixes";
        string suffixeFilePath = "NamesData/Suffixes";

        try
        {
            prefixeContent = Resources.Load<TextAsset>(prefixeFilePath).text.ToUpper();
            suffixeContent = Resources.Load<TextAsset>(suffixeFilePath).text.ToUpper();
        }
        catch (System.Exception)
        {
            Debug.LogError("Text file not found)");
        }

        string[] prefixeArray = prefixeContent.Split('\n');
        string[] suffixeArray = suffixeContent.Split('\n');
        _prefixes.AddRange(prefixeArray);
        _suffixes.AddRange(suffixeArray);
    }

    /// <summary>
    /// Return a random name as a string
    /// </summary>
    public string GetRandomName()
    {
        string prefix = _prefixes[Random.Range(0, _prefixes.Count)].Trim();
        string suffix = _suffixes[Random.Range(0, _prefixes.Count)].Trim();

        Debug.Log("You name is : " + prefix + " " + suffix);

        return prefix + " " + suffix;
    }

    public void OnRandomizeDropdowns()
    {
        _prefixDropDown.value = Random.Range(0, _prefixDropDown.options.Count + 1);
        _suffixDropDown.value = Random.Range(0, _suffixDropDown.options.Count + 1);
    }

    /// <summary>
    /// Add the names in the dropdowns to display them
    /// </summary>
    /// <param name="dropdown"></param>
    public void InitializeDropDown(TMP_Dropdown dropdown, List<string> name)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(name);
    }

    /// <summary>
    /// Save the name when leaving
    /// </summary>
    public void SaveName()
    {
        S_DataGame.Instance.inventory.SavePrefixName(_prefixDropDown.value);
        S_DataGame.Instance.inventory.SaveSuffixname(_suffixDropDown.value);
        S_DataGame.Instance.inventory.SavePrefixString(_prefixDropDown.options[_prefixDropDown.value].text);
        S_DataGame.Instance.inventory.SaveSuffixString(_suffixDropDown.options[_suffixDropDown.value].text);
    }

    public void LoadName()
    {
        if (S_DataGame.Instance.inventory.LoadPrefixName())
            _prefixDropDown.value = S_DataGame.Instance.inventory.prefixIndex;
        if (S_DataGame.Instance.inventory.LoadSuffixName())
            _suffixDropDown.value = S_DataGame.Instance.inventory.suffixIndex;
    }
}
