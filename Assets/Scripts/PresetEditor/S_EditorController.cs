using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class S_EditorController : MonoBehaviour
{
    [SerializeField] private GameObject _newPresetObjectIcon;

    [SerializeField] private GameObject _presetGroup;
    [SerializeField] private List<GameObject> _presets;

    [SerializeField] private GameObject _weaponGroup1;
    [SerializeField] private GameObject _weaponGroup2;

    [SerializeField] private GameObject _frameGroup1;
    [SerializeField] private GameObject _frameGroup2;

    [SerializeField] private List<GameObject> _weapons;     
    [SerializeField] private List<S_WeaponData> _weaponsData;       //list of scriptable object player's weapons
    
    [SerializeField] private List<GameObject> _frame;       
    [SerializeField] private List<S_FrameData> _frameData;          //list of scriptable object player's frames

    [SerializeField] private Material _selectedMaterial;
    [SerializeField] private int _selecteIndex = 0;

    [SerializeField] private int _nbWeapon = 6;
    [SerializeField] private int _nbFrame = 3;

    [SerializeField] private List<S_WeaponData> _selectedWeaponsData;
    [SerializeField] private S_FrameData _selectedFrameData;
    [SerializeField] private int _selectedPreset;

    [SerializeField] private List<GameObject> _presetWeaponsHookPoints;
    [SerializeField] private S_FrameData _presetFrameData;
    [SerializeField] private List<S_WeaponData> _presetWeaponData;

    
    
    [SerializeField] private EditState _editState;



    enum EditState
    {
        PresetChoice,
        WeaponChoice,
        FrameChoice
    }

    private void Awake()
    {
        UpdatePiece();
        _selectedMaterial.SetFloat("_Selected", 1);
        _selectedPreset = -1;
        _selectedMaterial.SetVector("_Selected_Object_Position", _weapons[_selecteIndex].gameObject.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _selecteIndex -= 1;
            if (_selecteIndex < 0)
                _selecteIndex = _weapons.Count - 1;
            Debug.Log(_weapons[_selecteIndex].gameObject.transform.position);
            _selectedMaterial.SetVector("_Selected_Object_Position", _weapons[_selecteIndex].gameObject.transform.position);
            
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _selecteIndex += 1;
            if (_selecteIndex > _weapons.Count -1)
                _selecteIndex = 0;
            _selectedMaterial.SetVector("_Selected_Object_Position", _weapons[_selecteIndex].gameObject.transform.position);
        }

        
    }

    /// <summary>
    /// get player's items not use in player's inventory
    /// </summary>
    public void GetPlayerItem()
    {
        
    }

    /// <summary>
    ///set player's items use in player's inventory
    /// </summary>
    public void SetPlayerItemUse()
    {
        
    }

    /// <summary>
    ///unset player's items not use in player's inventory
    /// </summary>
    public void UnsetPlayerItemUse()
    {
        
    }


    /// <summary>
    /// Set active state of this choice
    /// </summary>
    /// <param name="active">Active state</param>
    private void SetActiveWeaponChoice(bool active)
    {

    }

    /// <summary>
    /// Set active state of this choice 
    /// </summary>
    /// <param name="active">Active state</param>
    private void SetActiveChassisChoice(bool active)
    {

    }

    private void UpdatePiece()
    {
        //ClearPieces();

        //get piece Data


        //init piece prefab


        foreach (S_FrameData frame in _frameData)
        {
            GameObject newFrame = Instantiate(frame.Prefab);
            newFrame.transform.position = Vector3.zero;

            RectTransform rectTransform = newFrame.AddComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(1, 1);

            _frame.Add(newFrame);
            newFrame.transform.position = Vector3.zero;
            if (_frameGroup1.transform.childCount < _nbFrame)
            {
                newFrame.transform.parent = _frameGroup1.transform;
            }
            else
            {
                newFrame.transform.parent = _frameGroup2.transform;
            }
        }

        foreach (S_WeaponData weapon in _weaponsData)
        {
            GameObject newWeapon = Instantiate(weapon.Prefab);
            newWeapon.transform.position = Vector3.zero;

            RectTransform rectTransform = newWeapon.AddComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(1, 1);

            _weapons.Add(newWeapon);
            newWeapon.transform.position = Vector3.zero;
            if (_weaponGroup1.transform.childCount < _nbWeapon)
            {
                newWeapon.transform.parent = _weaponGroup1.transform;
            }
            else
            {
                newWeapon.transform.parent = _weaponGroup2.transform;
            }
        }

        //foreach (GameObject  in )
        //{
        //    GameObject newWeapon = Instantiate(weapon.Prefab);
        //    newWeapon.transform.position = Vector3.zero;

        //    RectTransform rectTransform = newWeapon.AddComponent<RectTransform>();

        //    rectTransform.sizeDelta = new Vector2(1, 1);

        //    _weapons.Add(newWeapon);
        //    newWeapon.transform.position = Vector3.zero;
        //    if (_weaponGroup1.transform.childCount < _nbWeapon)
        //    {
        //        newWeapon.transform.parent = _weaponGroup1.transform;
        //    }
        //    else
        //    {
        //        newWeapon.transform.parent = _weaponGroup2.transform;
        //    }
        //}

    }

    public void Back()
    {
        switch (_editState)
        {
            case EditState.PresetChoice:
                // go to Menu
                break;
            case EditState.FrameChoice:
                _editState = EditState.PresetChoice;
                break;
            case EditState.WeaponChoice:
                _editState = EditState.FrameChoice;
                break;
            default:
                break;
        }
    }

    private void SelectItem()
    {
        switch (_editState)
        {
            case EditState.PresetChoice:

                break;
            case EditState.FrameChoice:

                break;
            case EditState.WeaponChoice:

                break;
            default:
                break;
        }
    }
    
    private void UnSelectItem()
    {
        switch (_editState)
        {
            case EditState.PresetChoice:
                
                break;
            case EditState.FrameChoice:

                break;
            case EditState.WeaponChoice:

                break;
            default:
                break;
        }
    }

    private void GetPreset()
    {

    }

    private void GetWeaponsHookPoint(S_FrameManager frameManager)
    {
        _presetWeaponsHookPoints = frameManager.WeaponHookPoints;
    }

    /// <summary>
    /// Clear lists of choice piece
    /// </summary>
    private void ClearChoicePieces()
    {
        _frame.Clear();
        _frameData.Clear();
        _weapons.Clear();
        _weaponsData.Clear();
    }

    private void CreatePresetPrefab(int id)
    {
        //recuperer les info du preset
        S_FrameData frameData = null;
        List<S_WeaponData> weaponsData = null;

        GameObject frame = Instantiate(frameData.Prefab);
        List<GameObject> weapons = new List<GameObject>();

        foreach (S_WeaponData weaponData in weaponsData)
        {
            weapons.Add(Instantiate(weaponData.Prefab));
        }

        List<GameObject> hookPoits = frame.GetComponent<S_FrameManager>().WeaponHookPoints.ToList();
        
    }
}


/*

recûperer les preset et si le nombre de preset et inferieur a 5 ajouter un objet UI pour creer un nouveau preset
(Supression preset ?)
Selectionner preset => garder l'ID du preset en memoire
si le preset = _newPresetObjectIcon creer le preset et lui mettre le premier chassis disponible dans la save

passage en cameras etablie

initialiser les items disponible selon l'inventaire avec le nombre d'item par stack
potentiellement creer une struct par type d'item pour avoir la Data et le nombre disponible

Definir toute les parties dans des varibles _presetFrameData et _presetWeaponData
initialiser les prefabs du preset chassis et armes selon la save

faire un selector sur _presetFrameData et _presetWeaponData

si l'objet selectionner est une arme et que je clik sur B je suprime l'objet du preset et met a jour les armes

selon le type de l'objet selectionner par la touche A lancer le selector sur les armes et sur les chassis

Selector arme

si appuie sur A l'objet se met sur le point d'accroche, je met a jour l'arme du point d'accroche du preset du preset et je met a jour les armes
si j'appui sur B retour a la selection sur le preset

Selector Chassis

si appuie sur A l'objet se met sur le point d'accroche, je met a jour le chassis du preset et je met a jour les chassis
si j'appui sur B retour a la selection des preset


  
predefinir la selection sur le preset 


*/