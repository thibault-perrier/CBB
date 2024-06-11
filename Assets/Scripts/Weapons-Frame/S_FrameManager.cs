using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_FrameManager : MonoBehaviour, I_Damageable
{
    private List<S_WeaponManager> _weaponManagers;
    private Rigidbody _rb;
    
    [SerializeField] private float _life;
    [SerializeField] private S_FrameData _data;
    [SerializeField] private List<GameObject> _weaponHookPoints;

    public int NBWeaponHookPoints
    {
        get { return _weaponHookPoints.Count; }
    }
    public List<S_WeaponManager> Weapons
    {
        get => _weaponManagers;
    }

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();
        _rb.mass = _data.Mass;
        _life = _data.MaxLife;
    }
    
    public event Action<S_FrameManager> OnDie;

    public bool AllWeaponIsBroken()
    {
        foreach (var weapon in _weaponManagers)
        {
            if (weapon.CurrentState.Equals(S_WeaponManager.State.ok))
                return false;
        }

        return true;
    }
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
    public void TakeDamage(float amount)
    {
        _life -= amount;
        if (_life <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {
        OnDie?.Invoke(this);
    }
    public void Repear()
    {
        _life = _data.MaxLife;
    }
}
