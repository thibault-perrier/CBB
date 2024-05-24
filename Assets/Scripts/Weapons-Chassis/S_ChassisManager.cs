using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_ChassisManager : MonoBehaviour, I_Damageable
{
    private int _life;
    [SerializeField] private S_WeaponData _data;
    private Rigidbody _rb;
    [SerializeField] private List<GameObject> _weaponHookPoints;
    [SerializeField] private bool _player = false;

    public int NBWeaponHookPoints
    {
        get { return _weaponHookPoints.Count; }
    }


    private void Awake()
    {
        _rb = this.AddComponent<Rigidbody>();
        _rb.mass = _data.Mass;
        _life = _data.MaxLife;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public event Action<S_ChassisManager> OnDie;

    public void TakeDamage(int amount)
    {
        _life -= amount;
        if (_life <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnDie?.Invoke(this);

        Debug.Log("Player died!");
        // Logic to remove destroy items in inventory
    }

    public void Repear()
    {
        _life = _data.MaxLife;
    }
}
