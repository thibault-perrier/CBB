using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_EditorController : MonoBehaviour
{
    [SerializeField] private GameObject _weaponGroup1;
    [SerializeField] private GameObject _weaponGroup2;
    [SerializeField] private GameObject _chassisGroup1;
    [SerializeField] private GameObject _chassisGroup2;

    //[SerializeField] private List<Weapon> _weappons;   //list of scriptable object player's weapons
    //[SerializeField] private List<Chassis> _chassis;    //list of scriptable object player's chassis

    enum EditState
    {
        PresetChoice,
        WeaponChoice,
        ChassisChoice
        

    }

    // Start is called before the first frame update
    void Start()
    {
        
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
}
