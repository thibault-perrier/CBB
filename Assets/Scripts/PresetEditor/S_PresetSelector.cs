using System.Collections.Generic;
using UnityEngine;

public class S_PresetSelector : MonoBehaviour
{
    [SerializeField] private GameObject _Group1;
    [SerializeField] private GameObject _Group2;

    [SerializeField] private List<GameObject> _weapons;     
    [SerializeField] private List<S_WeaponData> _weaponsData;       //list of scriptable object player's weapons
    [SerializeField] private List<GameObject> _frame;       
    [SerializeField] private List<S_FrameData> _frameData;          //list of scriptable object player's frames

    [SerializeField] private Material _selectedMaterial;

    /// <summary>
    /// Max number of Frame on the line
    /// </summary>
    [SerializeField] private int _nbFrame = 3;

    [SerializeField] private int _selecteIndex = 0;

    private void Awake()
    {
        UpdatePiece();
        _selectedMaterial.SetFloat("_Selected", 1);
        
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


        //foreach (S_FrameData frame in _frameData)
        //{
        //    GameObject newFrame = Instantiate(frame.Prefab);
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

    private void ClearPieces()
    {
        _frame.Clear();
        _frameData.Clear();
        _weapons.Clear();
        _weaponsData.Clear();
    }
}
