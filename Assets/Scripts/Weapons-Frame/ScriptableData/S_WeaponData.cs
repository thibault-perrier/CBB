using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Robot/Weapon")]
public class S_WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _prefab;

    [Header("SFX")]
    [SerializeField] private GameObject _vfxHitContact;
    [SerializeField] private Vector3 _scaleVfxHitContact = Vector3.one;
    [SerializeField] private GameObject _vfxLowUp;
    [SerializeField] private GameObject _vfxDestroy;

    [Header("Statistiques")]
    [SerializeField] private float _maxLife;
    [SerializeField] private int _cost;
    [SerializeField] private float _damage;
    [SerializeField] private float _mass;

    [Header("Attack time")]
    [SerializeField] private float _cooldownAttack;
    [SerializeField] private float _attackTime;

    [Header("Attack toggles")]
    [SerializeField] private bool _attackOneTime;
    [SerializeField] private bool _alwaysActive;

    public GameObject Prefab
    {
        get { return _prefab; }
    }
    
    public GameObject VfxHitContact
    {
        get => _vfxHitContact;
    }
    public Vector3 ScaleVfxHitContact
    {
        get => _scaleVfxHitContact;
    }
    public GameObject VfxLowUp
    {
        get => _vfxLowUp;
    }
    public GameObject VfxDestroy
    {
        get => _vfxDestroy;
    }

    public float MaxLife
    {
        get { return _maxLife; }
    }
    public int Cost
    {
        get { return _cost; }
    }
    public float Damage
    {
        get { return _damage; }
    }
    public float Mass
    {
        get { return _mass; }
    }

    public float AttackCooldown
    {
        get => _cooldownAttack;
    }
    public float AttackTime
    {
        get => _attackTime;
    }

    public bool AlwayActive
    {
        get { return _alwaysActive; }
    }
    public bool AttackOneTime
    {
        get => _attackOneTime;
    }
}
