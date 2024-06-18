using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_FrameManager : MonoBehaviour, I_Damageable
{
    private List<S_WeaponManager> _weaponManagers = new();
    private Rigidbody _rb;
    
    [SerializeField, Tooltip("the current health point of the frama")] 
    public float _life;
    [SerializeField, Tooltip("all statistic in the frame")] 
    private S_FrameData _data;
    [SerializeField, Tooltip("All kook point for get weapons")] 
    private List<GameObject> _weaponHookPoints;
    [SerializeField, Tooltip("vfx spawned when the frame is destroy")]
    private GameObject _vfxDestroyFrame;

    public event Action OnReceiveDamage;

    /// <summary>
    /// return the number of hook point
    /// </summary>
    public int NBWeaponHookPoints
    {
        get { return _weaponHookPoints.Count; }
    }

    public List<GameObject> WeaponHookPoints
    {
        get { return _weaponHookPoints; }
    }

    /// <summary>
    /// return all the weapon in the frame
    /// </summary>
    public List<S_WeaponManager> Weapons
    {
        get => _weaponManagers;
    }
    public float PercentLife
    {
        get => _life / _data.MaxLife;
    }

    public S_FrameData Data
    {
        get { return _data; }
    }


    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();
        _rb.mass = _data.Mass;
        _life = _data.MaxLife;
    }
    
    public event Action<S_FrameManager> OnDie;

    /// <summary>
    /// detect if there is any weapon is always OK
    /// </summary>
    /// <returns>return True if all weapon is not OK</returns>
    public bool AllWeaponIsBroken()
    {
        foreach (var weapon in _weaponManagers)
        {
            if (weapon.CurrentState.Equals(S_WeaponManager.State.ok))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Get all weapon with hook points
    /// </summary>
    public void SelectWeapons()
    {
        foreach (GameObject gameObject in _weaponHookPoints)
        {
            S_WeaponManager weaponManager = gameObject.GetComponentInChildren<S_WeaponManager>(true);
            if (weaponManager != null)
            {
                _weaponManagers.Add(weaponManager);
            }
        }
    }

    /// <summary>
    /// take damage in the current frame
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        _life -= amount;
        OnReceiveDamage?.Invoke();

        if (_life <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Start when the life of the current frame is lower or equal at 0
    /// </summary>
    public void Die()
    {
        Instantiate(_vfxDestroyFrame, transform.position, Quaternion.identity);
        DetachAllWeapons();
        OnDie?.Invoke(this);

        Debug.Log("Player died!");
        // Logic to remove destroy items in inventory
    }

    /// <summary>
    /// repair the frame, set the life with the data
    /// </summary>
    public void Repair()
    {
        _life = _data.MaxLife;
    }

    public bool CanRecieveDamage()
    {
        return _life > 0f;
    }

    private void DetachAllWeapons()
    {
        foreach (var weapon in _weaponManagers)
        {
            if (weapon.CurrentState == S_WeaponManager.State.broken)
                continue;

            if (!weapon.transform.parent.gameObject.transform.parent)
                continue;

            weapon.DetachWeapon();
        }
    }
    
    public void RepairAll()
    {
        Repair();
        foreach (S_WeaponManager weaponManager in _weaponManagers)
        {
            if(weaponManager.CurrentState != S_WeaponManager.State.destroy)
                weaponManager.Repair();
        }
    }
}
