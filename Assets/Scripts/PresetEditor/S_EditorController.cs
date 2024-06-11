using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class S_EditorController : MonoBehaviour
{
    [SerializeField] private GameObject _newPresetObjectIcon;

    [SerializeField] private GameObject _presetGroup;
    [SerializeField] private List<GameObject> _presets;

    [SerializeField] private GameObject _weaponGroup1;
    [SerializeField] private GameObject _weaponGroup2;

    [SerializeField] private GameObject _frameGroup1;
    [SerializeField] private GameObject _frameGroup2;

    [SerializeField] private GameObject _presetHold;

    [SerializeField] private List<GameObject> _weapons;     
    [SerializeField] private List<S_WeaponData> _weaponsData;       //list of scriptable object player's weapons
    [SerializeField] private List<Weapon> _inventoryWeapons;

    [SerializeField] private List<GameObject> _frame;       
    [SerializeField] private List<S_FrameData> _frameData;          //list of scriptable object player's frames
    [SerializeField] private List<Frame> _invetoryFrames;

    [SerializeField] private Material _selectedMaterial;
    [SerializeField] private int _selectedIndex = 0;

    [SerializeField] private int _nbWeapon = 6;
    [SerializeField] private int _nbFrame = 3;

    [SerializeField] private List<S_WeaponData> _presetWeaponsData;
    [SerializeField] private S_FrameData _presetFrameData;
    [SerializeField] private int _selectedPreset;

    [SerializeField] private List<GameObject> _presetWeaponsHookPoints;
    [SerializeField] private int _indexHookPointToModify;

    [SerializeField] private List<GameObject> _presetObjectPart;


    [SerializeField] private EditState _editState = EditState.PresetChoice;



    enum EditState
    {
        PartChoice,
        PresetChoice,
        WeaponChoice,
        FrameChoice
    }

    private void Awake()
    {
        
        Frame frame = new Frame(_frameData[0]);

        Robot robot = new Robot(frame);


        //robot._weapons[0] = new Weapon(_weaponsData[0]);

        S_DataGame.Instance.inventory.Robots.Add(robot);

        foreach(S_FrameData data in _frameData)
        {
            S_DataGame.Instance.inventory.Frames.Add(new Frame(data));
        }
        foreach (S_WeaponData data in _weaponsData)
        {
            S_DataGame.Instance.inventory.Weapons.Add(new Weapon(data));
        }
        UpdatePiece();
        UpdatePresetRobotGroup();
    }

    // Start is called before the first frame update
    void Start()
    {
        _selectedMaterial.SetFloat("_Selected", 1);
        _selectedPreset = -1;
        Selector();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _selectedIndex -= 1;
            Selector();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _selectedIndex += 1;
            Selector();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
            Selector();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectItem();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            RemoveWeapon();
        }


    }

    /// <summary>
    /// get player's items not use in player's inventory
    /// </summary>
    public void GetPlayerItem()
    {
        _weaponsData.Clear();
        S_DataGame.Instance.inventory.UpdateUseItem();
        _inventoryWeapons = S_DataGame.Instance.inventory.Weapons.ToList();

        foreach (Weapon weapon in S_DataGame.Instance.inventory.Weapons)
        {
            _weaponsData.Add(weapon.GetWeaponData());
        }

        _invetoryFrames = S_DataGame.Instance.inventory.Frames.ToList();
        foreach (Frame frame in S_DataGame.Instance.inventory.Frames)
        {
            _frameData.Add(frame.GetFrameData());
        }
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

    private void UpdatePiece()
    {
        Debug.Log(S_DataGame.Instance.inventory.Weapons.Count());

        foreach (Weapon saveWeapon in S_DataGame.Instance.inventory.Weapons)
        {
            //_weaponsData.Add(SaveWeapon.GetWeaponData());

            S_WeaponData weapon = saveWeapon.GetWeaponData();

            if (saveWeapon._number - saveWeapon._useNumber > 0)
            {
                GameObject newWeapon = Instantiate(weapon.Prefab);
                newWeapon.transform.position = Vector3.zero;

                RectTransform rectTransform = newWeapon.AddComponent<RectTransform>();

                rectTransform.sizeDelta = new Vector2(1, 1);
                newWeapon.transform.localRotation = Quaternion.Euler(0, 45, 0);
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
        }
            
        foreach (Frame saveFrame in S_DataGame.Instance.inventory.Frames)
        {
            S_FrameData frame = saveFrame.GetFrameData();

            if (saveFrame._number - saveFrame._useNumber > 0)
            {
                GameObject newFrame = Instantiate(frame.Prefab);

                newFrame.GetComponent<Rigidbody>().isKinematic = true;


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
        }

        //foreach (S_FrameData frame in _frameData)
        //{
        //    GameObject newFrame = Instantiate(frame.Prefab);

        //    newFrame.GetComponent<Rigidbody>().isKinematic = true;


        //    newFrame.transform.position = Vector3.zero;

        //    RectTransform rectTransform = newFrame.AddComponent<RectTransform>();

        //    rectTransform.sizeDelta = new Vector2(1, 1);

        //    _frame.Add(newFrame);
        //    newFrame.transform.position = Vector3.zero;
        //    if (_frameGroup1.transform.childCount < _nbFrame)
        //    {
        //        newFrame.transform.parent = _frameGroup1.transform;
        //    }
        //    else
        //    {
        //        newFrame.transform.parent = _frameGroup2.transform;
        //    }
        //}

        //foreach (S_WeaponData weapon in _weaponsData)
        //{
        //    GameObject newWeapon = Instantiate(weapon.Prefab);
        //    newWeapon.transform.position = Vector3.zero;

        //    RectTransform rectTransform = newWeapon.AddComponent<RectTransform>();

        //    rectTransform.sizeDelta = new Vector2(1, 1);
        //    newWeapon.transform.localRotation = Quaternion.Euler(0, 45, 0);
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
                
                break;
            case EditState.PartChoice:
                _editState = EditState.PresetChoice;
                break;
            case EditState.FrameChoice:
                _editState = EditState.PartChoice;
                break;
            case EditState.WeaponChoice:
                _editState = EditState.PartChoice;
                break;
            default:
                break;
        }
    }

    private void Selector()
    {
        switch (_editState)
        {
            case EditState.PresetChoice:
                if (_selectedIndex < 0)
                    _selectedIndex = _presets.Count - 1;
                _selectedIndex = _selectedIndex % _presets.Count;
                _selectedMaterial.SetVector("_Selected_Object_Position", _presets[_selectedIndex].gameObject.transform.position);
                break;
             case EditState.PartChoice:
                if (_selectedIndex < 0)
                    _selectedIndex = _presetObjectPart.Count-1;
                _selectedIndex = _selectedIndex % _presetObjectPart.Count;
                _selectedMaterial.SetVector("_Selected_Object_Position", _presetObjectPart[_selectedIndex].gameObject.transform.position);
                break;
            case EditState.FrameChoice:
                if (_selectedIndex < 0)
                    _selectedIndex = _frame.Count-1;
                _selectedIndex = _selectedIndex % _frame.Count;
                _selectedMaterial.SetVector("_Selected_Object_Position", _frame[_selectedIndex].gameObject.transform.position);
                break;
            case EditState.WeaponChoice:
                if (_selectedIndex < 0)
                    _selectedIndex = _weapons.Count-1;
                _selectedIndex = _selectedIndex % _weapons.Count;
                _selectedMaterial.SetVector("_Selected_Object_Position", _weapons[_selectedIndex].gameObject.transform.position);
                Debug.Log(_weapons[_selectedIndex].GetComponentInChildren<S_WeaponManager>().Data);
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
                
                if (_presetObjectPart[_selectedIndex].GetComponent<S_FrameManager>() == null)
                {
                    Robot robot = new Robot(GetUnuseFrame());
                    S_DataGame.Instance.inventory.Robots.Add(robot);
                }
                _selectedPreset = _selectedIndex;
                _editState = EditState.PartChoice;
                break;
            case EditState.PartChoice:

                if (_presetObjectPart[_selectedIndex].GetComponent<S_FrameManager>() != null)
                {
                    _editState = EditState.FrameChoice;
                }
                else
                {
                    _indexHookPointToModify = _selectedIndex;
                    _editState = EditState.WeaponChoice;
                }

                break;
            case EditState.FrameChoice:

                if (_frame[_selectedIndex].TryGetComponent<S_FrameManager>(out S_FrameManager frameManager))
                {
                    UpdatePresetFrame(frameManager.Data);
                }
                else
                {
                    Debug.LogWarning("S_FrameManager component not found on the selected frame. Checking children...");
                    frameManager = _frame[_selectedIndex].GetComponentInChildren<S_FrameManager>();
                    if (frameManager != null)
                    {
                        UpdatePresetFrame(frameManager.Data);
                    }
                    else
                    {
                        Debug.LogError("S_FrameManager component not found in the selected frame or its children.");
                    }
                }
                _editState = EditState.PartChoice;
                break;
            case EditState.WeaponChoice:
                if (_weapons[_selectedIndex].TryGetComponent<S_WeaponManager>(out S_WeaponManager weaponManager))
                {
                    UpdatePresetWeapon(weaponManager.Data);
                }
                else
                {
                    Debug.LogWarning("S_WeaponManager component not found on the selected weapon. Checking children...");
                    weaponManager = _weapons[_selectedIndex].GetComponentInChildren<S_WeaponManager>();
                    if (weaponManager != null)
                    {
                        UpdatePresetWeapon(weaponManager.Data);
                    }
                    else
                    {
                        Debug.LogError("S_WeaponManager component not found in the selected weapon or its children.");
                    }
                }
                _editState = EditState.PartChoice;
                break;
            default:
                break;
        }
        _selectedIndex = 0;
        UpdatePrefabRobot();
        Selector();
    }

    /// <summary>
    /// Set save of robot for new weapons selected
    /// </summary>
    /// <param name="weaponData"></param>
    private void UpdatePresetWeapon(S_WeaponData weaponData)
    {
        S_DataGame.Instance.inventory.Robots[_selectedPreset]._weapons[_indexHookPointToModify] = S_DataGame.Instance.inventory.GetWeapon(weaponData);
        UpdatePrefabRobot();
    }

    /// <summary>
    /// Set save of robot for new frame selected
    /// </summary>
    /// <param name="weaponData"></param>
    private void UpdatePresetFrame(S_FrameData frameData)
    {
        S_DataGame.Instance.inventory.Robots[_selectedPreset]._frame = S_DataGame.Instance.inventory.GetFrame(frameData);
        S_DataGame.Instance.inventory.Robots[_selectedPreset].UpdateWeaponMaxList();
        UpdatePrefabRobot();
    }

    private void UnSelectItem()
    {
        if (_editState == EditState.PartChoice && _presetObjectPart[_selectedIndex].GetComponent<S_WeaponManager>() != null)
        {
            GameObject gameObject = _presetObjectPart[_selectedIndex];
            _presetObjectPart[_selectedIndex] = gameObject.transform.parent.gameObject;
            Destroy(gameObject);
            UpdatePrefabRobot();
        }
    }

    private Frame GetUnuseFrame()
    {
        S_DataGame.Instance.inventory.UpdateUseItem();
        foreach (Frame frame in S_DataGame.Instance.inventory.Frames)
        {
            Debug.Log(frame._number - frame._useNumber > 1);
            if(frame._number - frame._useNumber > 1)
            {
                return frame;
            }
        }
        return null;
    }

    private void GetWeaponsHookPoint(S_FrameManager frameManager)
    {
        _presetWeaponsHookPoints = frameManager.WeaponHookPoints.ToList();
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

    private GameObject CreatePresetPrefab(Robot robot)
    {
        S_FrameData frameData = robot._frame.GetFrameData();
        List<S_WeaponData> weaponsData = new List<S_WeaponData>();

        GameObject frame = Instantiate(frameData.Prefab);
        List<GameObject> weapons = new List<GameObject>();

        frame.GetComponent<Rigidbody>().isKinematic = true;

        if (robot._weapons.Count() == 0)
            return frame;

        List<GameObject> hookPoits = frame.GetComponent<S_FrameManager>().WeaponHookPoints.ToList();
        
        for(int i=0; i < robot._weapons.Count(); i++)
        {
            if(robot._weapons[i] != null)
            {
                _weaponsData.Add(robot._weapons[i].GetWeaponData());
                weapons.Add(Instantiate(_weaponsData[i].Prefab));
                weapons[i].transform.parent = hookPoits[i].transform;
                weapons[i].transform.localPosition = Vector3.zero;
                weapons[i].transform.localRotation = Quaternion.identity;
            }
        }

        return frame;
    }

    public void UpdatePrefabRobot()
    {
        S_FrameData frameData = S_DataGame.Instance.inventory.Robots[_selectedPreset]._frame.GetFrameData();
        List<S_WeaponData> weaponsData = new List<S_WeaponData>();

        GameObject frame = Instantiate(frameData.Prefab);

        frame.GetComponent<Rigidbody>().isKinematic = true;

        frame.transform.parent = _presetHold.transform;
        frame.transform.localPosition = Vector3.zero;
        frame.transform.localRotation = Quaternion.identity;

        foreach (GameObject gameObject in _presetObjectPart)
        {
            Destroy(gameObject);
        }

        GetWeaponsHookPoint(frame.GetComponent<S_FrameManager>());

        _presetObjectPart.Clear();
        
        for (int i = 0; i < S_DataGame.Instance.inventory.Robots[_selectedPreset]._weapons.Count(); i++)
        {
            if (_presetWeaponsHookPoints.Count() - 1 >= i)
            {     
                Weapon weapon = S_DataGame.Instance.inventory.Robots[_selectedPreset]._weapons[i];
                if (weapon != null)
                {
                    GameObject objWeapon = Instantiate(weapon.GetWeaponData().Prefab);
                    _presetObjectPart.Add(objWeapon);
                    objWeapon.transform.parent = _presetWeaponsHookPoints[i].transform;
                    objWeapon.transform.localPosition = Vector3.zero;
                    objWeapon.transform.localRotation = Quaternion.identity;
                    _presetWeaponsHookPoints[i].GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    _presetObjectPart.Add(_presetWeaponsHookPoints[i]);
                    _presetWeaponsHookPoints[i].GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
        _presetObjectPart.Add(frame);
    }

    private void UpdatePresetRobotGroup()
    {
        foreach (Robot robot in S_DataGame.Instance.inventory.Robots)
        {
            GameObject frame = CreatePresetPrefab(robot);
            RectTransform rectTransform = frame.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1, 1);

            frame.transform.parent = _presetGroup.transform;
            frame.transform.localPosition = Vector3.zero;
            frame.transform.localRotation = Quaternion.identity;
            _presets.Add(frame);
        }
        if (S_DataGame.Instance.inventory.Robots.Count() < 5 && GetUnuseFrame() != null)
        {
            GameObject add = Instantiate(_newPresetObjectIcon);

            RectTransform rectTransform = add.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1, 1);

            add.transform.parent = _presetGroup.transform;
            add.transform.localPosition = Vector3.zero;
            add.transform.localRotation = Quaternion.identity;
            _presets.Add(add);
        }
    }

    private void RemoveWeapon()
    {
        if (_presetObjectPart[_selectedIndex].GetComponent<S_FrameManager>() == null)
        {
            S_DataGame.Instance.inventory.Robots[_selectedPreset]._weapons[_selectedIndex] = null;
            UpdatePrefabRobot();
        }
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