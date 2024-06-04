using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_EditorController : MonoBehaviour
{
    [SerializeField] private GameObject _weaponGroup1;
    [SerializeField] private GameObject _weaponGroup2;
    [SerializeField] private GameObject _frameGroup1;
    [SerializeField] private GameObject _frameGroup2;

    [SerializeField] private List<GameObject> _weapons;     //list of scriptable object player's weapons
    [SerializeField] private List<S_WeaponData> _weaponsData;
    [SerializeField] private List<GameObject> _frame;     //list of scriptable object player's chassis
    [SerializeField] private List<S_FrameData> _frameData;

    /// <summary>
    /// number of Weapon on the line
    /// </summary>
    [SerializeField] private int _nbWeapon;
    /// <summary>
    /// number of Chassis on the line
    /// </summary>
    [SerializeField] private int _nbChassis;

    enum EditState
    {
        PresetChoice,
        WeaponChoice,
        FrameChoice
    }

    private void Awake()
    {
        //UpdatePiece();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdatePiece();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            GameObject uiInstance = new GameObject(newFrame.name, typeof(RectTransform));
            Destroy(newFrame);
            _frame.Add(uiInstance);

            

            if (_frameGroup1.transform.childCount < 3)
            {
                uiInstance.transform.parent = _frameGroup1.transform;
            }
            else
            {
                uiInstance.transform.parent = _frameGroup2.transform;
            }
            

        }

        foreach (S_WeaponData weapon in _weaponsData)
        {
            GameObject newWeapon = Instantiate(weapon.Prefab);
            GameObject uiInstance = new GameObject(newWeapon.name, typeof(RectTransform));
            _weapons.Add(uiInstance);
            //Destroy(newWeapon);
            if (_weaponGroup1.transform.childCount < 6)
            {
                uiInstance.transform.parent = _weaponGroup1.transform;
            }
            else
            {
                uiInstance.transform.parent = _weaponGroup2.transform;
            }
            //uiInstance.transform.position = Vector3.zero;
        }
        
    }

    private void ClearPieces()
    {
        _frame.Clear();
        _frameData.Clear();
        _weapons.Clear();
        _weaponsData.Clear();
    }
}
