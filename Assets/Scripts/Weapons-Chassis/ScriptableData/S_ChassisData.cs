using UnityEngine;

[CreateAssetMenu(fileName = "ChassisData", menuName = "Robot/Chassis")]
public class S_ChassisData : ScriptableObject
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _maxLife;
    [SerializeField] private int _cost;
    [SerializeField] private float _mass;
    
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
    public float Mass
    {
        get { return _mass; }
    }

    public int GetNbWeaponMax()
    {
        return _prefab.GetComponent<S_ChassisManager>().NBWeaponHookPoints;
    }

}
