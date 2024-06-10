using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_FrameManager : MonoBehaviour, I_Damageable
{
    private int _life;
    [SerializeField] private S_FrameData _data;
    private Rigidbody _rb;
    [SerializeField] private List<GameObject> _weaponHookPoints;
    [SerializeField] private bool _player = false;
    public List<S_WeaponManager> _weaponManagers;

    public int NBWeaponHookPoints
    {
        get { return _weaponHookPoints.Count; }
    }

    public List<GameObject> WeaponHookPoints
    {
        get { return _weaponHookPoints; }
    }


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.mass = _data.Mass;
        _life = _data.MaxLife;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject gameObject in _weaponHookPoints)
        {
            S_WeaponManager weaponManager = gameObject.GetComponentInChildren<S_WeaponManager>();
            if(weaponManager != null)
            {
                _weaponManagers.Add(weaponManager);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public event Action<S_FrameManager> OnDie;

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

    public void Repair()
    {
        _life = _data.MaxLife;
    }
}
