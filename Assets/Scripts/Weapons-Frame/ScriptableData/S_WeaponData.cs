using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Robot/Weapon")]
public class S_WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _maxLife;
    [SerializeField] private int _cost;
    [SerializeField] private int _damage;
    [SerializeField] private float _mass;
    /// <summary>
    /// whether it's an active or passive weapon
    /// </summary>
    [SerializeField] private bool _activeWeapon;

    public GameObject Prefab
    {
        get { return _prefab; }
    }
    public int MaxLife
    {
        get { return _maxLife; }
    }
    public int Cost
    {
        get { return _cost; }
    }
    public int Damage
    {
        get { return _damage; }
    }
    public float Mass
    {
        get { return _mass; }
    }
    public bool Active
    {
        get { return _activeWeapon; }
    }
}
