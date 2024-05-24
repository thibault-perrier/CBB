using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Robot/Weapon")]
public class S_WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _maxLife;
    [SerializeField] private int _cost;
    [SerializeField] private int _damage;
    [SerializeField] private float _mass;

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
}
