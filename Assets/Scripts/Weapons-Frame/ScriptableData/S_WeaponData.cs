using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Robot/Weapon")]
public class S_WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _prefab;

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
    public bool AlwayActive
    {
        get { return _alwaysActive; }
    }
    public bool AttackOneTime
    {
        get => _attackOneTime;
    }
    public float AttackCooldown
    {
        get => _cooldownAttack;
    }
    public float AttackTime
    {
        get => _attackTime;
    }
}
