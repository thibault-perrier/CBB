
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_AIController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField, Tooltip("Tag for find player")] 
    private string _playerTag;

    [Header("Togles Actions")]
    [SerializeField, Tooltip("if he focus the plaeyr weapon")]
    private bool _attackPlayerWeapon;
    [SerializeField, Tooltip("if he use her best weapon for attack")]
    private bool _attackWithBestWeapon;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100)] private float _attackProbability = 100f;

    private GameObject _player;
    private GameObject _target;
    private GameObject _currentWeapon;

    private WheelsController _wheelsController;
    
    // test varaible 
    public List<GameObject> Weapons;
    public List<GameObject> PlayerWeapons;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag(_playerTag);
        _wheelsController = GetComponent<WheelsController>();
    }
    private void FixedUpdate()
    {
        if (CurrentWeaponCanAttack())
        {
            _wheelsController.Movement = WheelsController.Move.neutral;
            // TO DO: make a brake and attack with current weapon
        }
        else
        {
            UpdateAIMovement();
        }

    }

    /// <summary>
    /// update the AI movement from target
    /// </summary>
    private void UpdateAIMovement()
    {
        if (!_attackPlayerWeapon)
        {
            _target = _player;
        }
        else
        {
            GameObject playerBestWeapon = GetBestPlayerWeaponFromTarget(transform);
            _target = playerBestWeapon;
        }

        _currentWeapon = GetBestWeaponFromTarget(_target.transform);
        MoveToPoint(_target.transform.position, _currentWeapon.transform);
    }
    /// <summary>
    /// set wheel velocity from the target
    /// </summary>
    /// <param name="target"></param>
    private void MoveToPoint(Vector3 target, Transform weapon)
    {
        float TurnAmount = 0f;

        // for set the movement
        Vector3 dir = (target - weapon.position).normalized;
        float dotDirection = Vector3.Dot(weapon.forward, dir);
        float angleToDir = Vector3.SignedAngle(weapon.forward, dir, Vector3.up);
        float dotWeaponBody = Vector3.Dot(weapon.forward, transform.forward);

        if (dotDirection < 0f && dotWeaponBody < 0f)
        {
            Debug.Log("OK");
            _wheelsController.Direction = 1f;
            _wheelsController.Movement = WheelsController.Move.toward;
            return;
        }

        if (dotDirection > 0f)
        {
            // go forward
            _wheelsController.Movement = dotWeaponBody > 0f ? WheelsController.Move.toward : WheelsController.Move.backward;
        }
        else
        {
            // go backward
            _wheelsController.Movement = dotWeaponBody > 0f ? WheelsController.Move.backward : WheelsController.Move.toward;
        }

        if (angleToDir > 0f)
        {
            // turn right
            TurnAmount = dotWeaponBody > 0f ? 1f : -1f;
        }
        else
        {
            // turn left
            TurnAmount = dotWeaponBody > 0f ? -1f : 1f;
        }

        // set controller direction
        _wheelsController.Direction = TurnAmount;
    }
    /// <summary>
    /// select the best player weapons
    /// </summary>
    /// <returns>return the best player weapon object</returns>
    private GameObject GetBestPlayerWeaponFromTarget(Transform target)
    {
        if (PlayerWeapons.Count < 1)
            return _player;

        // sort the best weapon
        return PlayerWeapons
            .Where(x => Vector3.Dot(x.transform.forward, (target.position - x.transform.position).normalized) > 0f)
            .Select(x => x.transform)
            .OrderBy(x => int.Parse(x.name) + Vector3.Distance(x.position, target.position))
            .Reverse()
            .ToList()[0].gameObject;
    }
    /// <summary>
    /// sort weapons from power, distance and if he is not behind to the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns>return the best weapon from the distance to the target</returns>
    private GameObject GetBestWeaponFromTarget(Transform target)
    {
        if (!_attackWithBestWeapon || Weapons.Count < 1)
            return this.gameObject;

        // sort the best weapon by power, distance and look
        return Weapons
            .Select(x => x.transform)
            .OrderBy(x => int.Parse(x.name) + Vector3.Distance(x.position, target.position))
            .Reverse()
            .ToList()[0].gameObject;
    }
    /// <summary>
    /// if the target is closed the current weapon
    /// </summary>
    /// <returns>return if he attack with current we hit something</returns>
    private bool CurrentWeaponCanAttack()
    {
        if (!_currentWeapon || !_target)
            return false;

        float targetDistance = Vector3.Distance(_target.transform.position, _currentWeapon.transform.position);
        if (targetDistance < 2f)
            return true;
        
        return false;
    }
}
