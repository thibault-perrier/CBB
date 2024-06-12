using System;
using System.Collections.Generic;
using UnityEngine;

public class S_FrameManager : MonoBehaviour, I_Damageable
{
    private List<S_WeaponManager> _weaponManagers = new();
    private Rigidbody _rb;

    public event Action<S_FrameManager> OnDie;
    
    [SerializeField, Tooltip("the current health point of the frama")] 
    private float _life;
    [SerializeField, Tooltip("all statistic in the frame")] 
    private S_FrameData _data;
    [SerializeField, Tooltip("All kook point for get weapons")] 
    private List<GameObject> _weaponHookPoints;

    /// <summary>
    /// return the number of hook point
    /// </summary>
    public int NBWeaponHookPoints
    {
        get { return _weaponHookPoints.Count; }
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

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();
        _rb.mass = _data.Mass;
        _life = _data.MaxLife;
    }
    
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
        // remove life and if the life is lower or equal at 0 is die
        _life -= amount;
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
        OnDie?.Invoke(this);
    }
    /// <summary>
    /// repear the frame, set the life with the data
    /// </summary>
    public void Repear()
    {
        _life = _data.MaxLife;
    }
}
