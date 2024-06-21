using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class S_EditorController : MonoBehaviour
{
    [SerializeField] private GameObject _newPresetObjectIcon;
    [SerializeField] private GameObject _numberPanel;

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

    [SerializeField] private int _selectedIndex = 0;

    [SerializeField] private int _nbWeapon = 6;
    [SerializeField] private int _nbFrame = 3;

    [SerializeField] private List<S_WeaponData> _presetWeaponsData;
    [SerializeField] private S_FrameData _presetFrameData;

    [SerializeField] private List<GameObject> _presetWeaponsHookPoints;
    [SerializeField] private int _indexHookPointToModify;

    [SerializeField] private List<GameObject> _presetObjectPart;
    [SerializeField] private int _nbMaxPreset;


    [SerializeField] private EditState _editState = EditState.PresetChoice;

    private static readonly int EmissionColorKey = Shader.PropertyToID("_EmissionColor");
    private const string EmissionKey = "_EMISSION";

    [SerializeField] private Material _selectMaterial;
    private Material _defaultMaterial;

    private MeshRenderer renderer = new MeshRenderer();


    enum EditState
    {
        nullChoice,
        PartChoice,
        PresetChoice,
        WeaponChoice,
        FrameChoice
    }

    public void SetNullChoice()
    {
        _editState = EditState.nullChoice;
        DisableActiveRenderer();
    }

    public void SetPartChoice()
    {
        DisableActiveRenderer();
        if (S_DataGame.Instance.inventory.Robots.Count() == 0)
            return;
        _editState = EditState.PartChoice;
        Selector();
    }

    public void SetPresetChoice()
    {
        _editState = EditState.PresetChoice;
        DisableActiveRenderer();
        Selector();
    }

    private void Awake()
    {

        
    }

    // Start is called before the first frame update
    void Start()
    {
        //foreach (S_FrameData data in _frameData)
        //{
        //    Frame frame2 = new Frame(data);
        //    Debug.Log("frame name : " + frame2._name);
        //    S_DataGame.Instance.inventory.Frames.Add(frame2);
        //}
        //foreach (S_WeaponData data in _weaponsData)
        //{
        //    bool haveWeapon = false;
        //    foreach (Weapon weapon in S_DataGame.Instance.inventory.Weapons)
        //    {
        //        if (weapon.GetWeaponData() == data)
        //        {
        //            weapon._number++;
        //            haveWeapon = true;
        //        }
        //    }
        //    if (!haveWeapon)
        //    {
        //        S_DataGame.Instance.inventory.Weapons.Add(new Weapon(data));
        //    }

        //}

        //Robot robot = new Robot(S_DataGame.Instance.inventory.Frames[0]);

        //S_DataGame.Instance.inventory.Robots.Add(robot);

        S_DataGame.Instance.LoadInventory();
        UpdatePiece();
        UpdatePresetRobotGroup();

        if(S_DataGame.Instance.inventory.Robots.Count() > 0)
        {
            UpdatePrefabRobot();
        }

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

        if (Input.GetKey(KeyCode.RightArrow))
        {
            PresetRotation(1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            PresetRotation(-1);
        }


    }

    private void PresetRotation(float move)
    {
        _presetHold.transform.Rotate(Vector3.up, move * 200 * Time.deltaTime);
        Selector();
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

    private void UpdatePiece()
    {
        S_DataGame.Instance.inventory.UpdateUseItem();
        foreach (GameObject gameObject in _weapons)
        {
            Destroy(gameObject);
        }
        foreach (GameObject gameObject in _frame)
        {
            Destroy(gameObject);
        }
        _weapons.Clear();
        _frame.Clear();

        int count = 0;

        for (int i = 0; i < S_DataGame.Instance.inventory.Weapons.Count(); i++)
        {
            Weapon saveWeapon = S_DataGame.Instance.inventory.Weapons[i];

            S_WeaponData weapon = saveWeapon.GetWeaponData();

            if (saveWeapon._number - saveWeapon._useNumber > 0)
            {
                GameObject newWeapon = Instantiate(weapon.Prefab);

                RectTransform rectTransform = newWeapon.AddComponent<RectTransform>();

                _weapons.Add(newWeapon);

                count++;
                if (count < _nbWeapon)
                {
                    newWeapon.transform.parent = _weaponGroup1.transform;
                }
                else
                {
                    newWeapon.transform.parent = _weaponGroup2.transform;
                }

                rectTransform.localPosition = Vector3.zero;
                rectTransform.sizeDelta = new Vector2(0, 0);
                rectTransform.localRotation = Quaternion.Euler(0, 45, 0);

                GameObject numberPanel = Instantiate(_numberPanel);
                numberPanel.transform.parent = newWeapon.transform;
                numberPanel.transform.localPosition = Vector3.zero;
                numberPanel.transform.localRotation = _numberPanel.transform.rotation;
                numberPanel.GetComponentInChildren<Text>().text = (saveWeapon._number - saveWeapon._useNumber).ToString();
            }
        }

        count = 0;

        for (int i = 0; i < S_DataGame.Instance.inventory.Frames.Count(); i++)
        {
            Frame saveFrame = S_DataGame.Instance.inventory.Frames[i];
            S_FrameData frame = saveFrame.GetFrameData();

            if (saveFrame._number - saveFrame._useNumber > 0)
            {
                GameObject newFrame = Instantiate(frame.Prefab);

                newFrame.GetComponent<Rigidbody>().isKinematic = true;
                newFrame.GetComponent<PlayerInput>().enabled = false;

                RectTransform rectTransform = newFrame.AddComponent<RectTransform>();

                _frame.Add(newFrame);

                count++;
                if (count < _nbFrame)
                {
                    Debug.Log(_nbFrame);
                    newFrame.transform.parent = _frameGroup1.transform;
                }
                else
                {
                    newFrame.transform.parent = _frameGroup2.transform;
                }

                rectTransform.localPosition = Vector3.zero;
                rectTransform.sizeDelta = new Vector2(0, 0);
                rectTransform.localRotation = Quaternion.Euler(0, 45, 0);

                GameObject numberPanel = Instantiate(_numberPanel);
                numberPanel.transform.parent = newFrame.transform;
                numberPanel.transform.localPosition = Vector3.zero;
                numberPanel.transform.localRotation = _numberPanel.transform.rotation;
                numberPanel.GetComponentInChildren<Text>().text = (saveFrame._number - saveFrame._useNumber).ToString();

            }
        }
    }

    public void Back()
    {
        switch (_editState)
        {
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
                Debug.Log(_presets[_selectedIndex].name);
                SetHovered(_presets[_selectedIndex].gameObject);
                break;
             case EditState.PartChoice:
                if (_selectedIndex < 0)
                    _selectedIndex = _presetObjectPart.Count-1;
                _selectedIndex = _selectedIndex % _presetObjectPart.Count;
                Debug.Log(_presetObjectPart[_selectedIndex].name);
                SetHovered(_presetObjectPart[_selectedIndex].gameObject);
                break;
            case EditState.FrameChoice:
                if (_selectedIndex < 0)
                    _selectedIndex = _frame.Count-1;
                _selectedIndex = _selectedIndex % _frame.Count;
                Debug.Log(_frame[_selectedIndex].name);
                SetHovered(_frame[_selectedIndex].gameObject);
                break;
            case EditState.WeaponChoice:
                if (_selectedIndex < 0)
                    _selectedIndex = _weapons.Count-1;
                _selectedIndex = _selectedIndex % _weapons.Count;
                Debug.Log(_weapons[_selectedIndex].name);
                SetHovered(_weapons[_selectedIndex].gameObject);
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
                
                if (_presets[_selectedIndex].GetComponent<S_FrameManager>() == null)
                {
                    Robot robot = new Robot(GetUnuseFrame());
                    S_DataGame.Instance.inventory.Robots.Add(robot);
                }
                S_DataGame.Instance.inventory.SelectedRobot = _selectedIndex;
                UpdatePresetRobotGroup();
                break;
            case EditState.PartChoice:

                if (_presetObjectPart[_selectedIndex].GetComponent<S_FrameManager>() != null)
                {
                    if(_frame.Count() > 0)
                        _editState = EditState.FrameChoice;
                }
                else
                {
                    if (_weapons.Count() > 0)
                    {
                        _indexHookPointToModify = _selectedIndex;
                        _editState = EditState.WeaponChoice;
                    }
                }
                DisableActiveRenderer();
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
        UpdatePiece();
        UpdatePrefabRobot();
        Selector();
    }

    /// <summary>
    /// Set save of robot for new weapons selected
    /// </summary>
    /// <param name="weaponData"></param>
    private void UpdatePresetWeapon(S_WeaponData weaponData)
    {
        S_DataGame.Instance.inventory.Robots[S_DataGame.Instance.inventory.SelectedRobot].AddWeapon(S_DataGame.Instance.inventory.GetWeapon(weaponData), _indexHookPointToModify);
        UpdatePrefabRobot();
    }

    /// <summary>
    /// Set save of robot for new frame selected
    /// </summary>
    /// <param name="weaponData"></param>
    private void UpdatePresetFrame(S_FrameData frameData)
    {
        S_DataGame.Instance.inventory.Robots[S_DataGame.Instance.inventory.SelectedRobot]._frame = S_DataGame.Instance.inventory.GetFrame(frameData);
        S_DataGame.Instance.inventory.Robots[S_DataGame.Instance.inventory.SelectedRobot].UpdateWeaponMaxList();
        UpdatePrefabRobot();
    }

    private Frame GetUnuseFrame()
    {
        S_DataGame.Instance.inventory.UpdateUseItem();


        foreach (Frame frame in S_DataGame.Instance.inventory.Frames)
        {
            Debug.Log("frame name : "+frame._name +" || "+ frame._number + " - " + frame._useNumber);
            Debug.Log("calcul: "+ (frame._number - frame._useNumber));
            if (frame._number - frame._useNumber >= 1)
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
        frame.GetComponent<PlayerInput>().enabled = false;

        if (robot._weapons == null || robot._weapons.Count() == 0)
            return frame;

        List<GameObject> hookPoits = frame.GetComponent<S_FrameManager>().WeaponHookPoints.ToList();
        
        for(int i=0; i < hookPoits.Count(); i++)
        {
            Weapon weapon = S_DataGame.Instance.inventory.Robots[S_DataGame.Instance.inventory.SelectedRobot].GetHookPointWeapon(i);
            if (weapon != null)
            {
                GameObject objWeapon = Instantiate(weapon.GetWeaponData().Prefab);
                objWeapon.transform.parent = hookPoits[i].transform;
                objWeapon.transform.localPosition = Vector3.zero;
                objWeapon.transform.localRotation = Quaternion.identity;
            }
        }
        return frame;
    }

    public void UpdatePrefabRobot()
    {
        S_FrameData frameData = S_DataGame.Instance.inventory.Robots[S_DataGame.Instance.inventory.SelectedRobot]._frame.GetFrameData();
        List<S_WeaponData> weaponsData = new List<S_WeaponData>();

        GameObject frame = Instantiate(frameData.Prefab);

        frame.GetComponent<Rigidbody>().isKinematic = true;
        frame.GetComponent<PlayerInput>().enabled = false;

        frame.transform.parent = _presetHold.transform;
        frame.transform.localPosition = Vector3.zero;
        frame.transform.localRotation = Quaternion.identity;

        foreach (GameObject gameObject in _presetObjectPart)
        {
            Destroy(gameObject);
        }

        GetWeaponsHookPoint(frame.GetComponent<S_FrameManager>());

        _presetObjectPart.Clear();

        
        for (int i = 0; i < _presetWeaponsHookPoints.Count(); i++)
        {
            Weapon weapon = S_DataGame.Instance.inventory.Robots[S_DataGame.Instance.inventory.SelectedRobot].GetHookPointWeapon(i);
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

        _presetObjectPart.Add(frame);
        S_DataGame.Instance.SaveInventory();
        UpdatePresetRobotGroup();
    }

    private void UpdatePresetRobotGroup()
    {
        foreach(GameObject robot in _presets)
        {
            Destroy(robot);
        }

        _presets.Clear();

        foreach (Robot robot in S_DataGame.Instance.inventory.Robots)
        {
            GameObject frame = CreatePresetPrefab(robot);
            RectTransform rectTransform = frame.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1, 2);

            frame.transform.parent = _presetGroup.transform;
            frame.transform.localPosition = Vector3.zero;
            frame.transform.localRotation = Quaternion.identity;
            _presets.Add(frame);
        }
        if (S_DataGame.Instance.inventory.Robots.Count() < _nbMaxPreset && GetUnuseFrame() != null)
        {
            GameObject add = Instantiate(_newPresetObjectIcon);

            RectTransform rectTransform = add.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1, 2);

            add.transform.parent = _presetGroup.transform;
            add.transform.localPosition = Vector3.zero;
            add.transform.localRotation = Quaternion.identity;
            _presets.Add(add);
        }

        S_DataGame.Instance.SaveInventory();
    }

    private void RemoveWeapon()
    {
        if (_presetObjectPart[_selectedIndex].GetComponent<S_FrameManager>() == null)
        {
            S_DataGame.Instance.inventory.Robots[S_DataGame.Instance.inventory.SelectedRobot].RemoveWeapon(_selectedIndex);
            UpdatePrefabRobot();
        }
        UpdatePiece();
    }

    /// <summary>
    /// Makes the object glow when hovered.
    /// </summary>
    /// <param name="gameObject">object is hovered.</param>
    public void SetHovered(GameObject gameObject)
    {
        DisableActiveRenderer();
        renderer = gameObject.GetComponentInChildren<MeshRenderer>();

        _defaultMaterial = renderer.material;
        renderer.material = _selectMaterial;
    }
    
    private void DisableActiveRenderer()
    {
        if(renderer != null)
            renderer.material = _defaultMaterial;
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