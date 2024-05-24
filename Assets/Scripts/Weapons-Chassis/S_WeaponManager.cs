using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class S_WeaponManager : MonoBehaviour, I_Damageable
{
    private int _life;
    private int _damage;
    [SerializeField] private S_WeaponData _data;
    [SerializeField] private State _state = State.ok;
    [SerializeField] private int _lifeBrakePoint = 15;    //In pourcent life level
    [SerializeField] private bool _shotWeapon = false;
    private float _brakePoint;
    private Rigidbody _rb;
    private bool _attacking = false;


    public enum State
    {
        ok,
        broken,
        destroy
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        _life -= amount;
        if (_life <= _brakePoint)
        {
            _state = State.broken;
        }
        if (_life <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        transform.parent = null;
        _state = State.destroy;
    }

    private void Awake()
    {
        _rb = this.AddComponent<Rigidbody>();
        _rb.mass = _data.Mass;
        _life = _data.MaxLife;
        _damage = _data.Damage;
        _brakePoint = (_lifeBrakePoint * _data.MaxLife) / 100;
    }

    public void Repear()
    {
        _life = _data.MaxLife;
        _state = State.ok;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(_attacking && collision.gameObject.TryGetComponent(out I_Damageable damageable))
        {
            damageable.TakeDamage(_damage);
        }
    }
}
