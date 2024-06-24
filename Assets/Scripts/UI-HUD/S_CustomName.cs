using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S_CustomName : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _prefixDropDown;
    [SerializeField] private TMP_Dropdown _suffixDropDown;

    private List<string> _prefixes = new List<string>();
    private List<string> _suffixes = new List<string>();

    private void Start()
    {
        InitializeNameLists();
        InitializeDropDown(_prefixDropDown, _prefixes);
        InitializeDropDown(_suffixDropDown, _suffixes);

        LoadName();
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
        string prefix = _prefixes[Random.Range(0, _prefixes.Count + 1)].ToUpper();
        string suffix = _prefixes[Random.Range(0, _prefixes.Count + 1)].ToUpper();

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
    }

    public void LoadName()
    {
        if (s_datagame.instance.inventory.loadsuffixname() && s_datagame.instance.inventory.loadsuffixname())
        {
            if (s_datagame.instance.inventory.loadprefixname())
                _prefixdropdown.value = s_datagame.instance.inventory.prefixindex;
            if (s_datagame.instance.inventory.loadsuffixname())
                _suffixdropdown.value = s_datagame.instance.inventory.suffixindex;
        }
        if (s_datagame.instance.inventory.loadprefixname())
            _prefixdropdown.value = s_datagame.instance.inventory.prefixindex;
        if (s_datagame.instance.inventory.loadsuffixname())
            _suffixdropdown.value = S_DataGame.Instance.inventory.suffixIndex;
    }
}
