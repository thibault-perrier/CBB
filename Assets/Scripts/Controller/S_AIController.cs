using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class S_AIController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField, Tooltip("Tag for find player")] 
    private string _playerTag;

    [Header("Agents")]
    [SerializeField, Tooltip("component for create path from player")]
    private NavMeshAgent _agents;

    [Header("Togles Actions")]
    [SerializeField, Tooltip("if he focus the plaeyr weapon")]
    private bool _attackPlayerWeapon;
    [SerializeField, Tooltip("if he dont touch the traps")]
    private bool _dodgeTrap;
    [SerializeField, Tooltip("if he use her best weapon for attack")]
    private bool _attackWithBestWeapon;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100)] private float _attackProbability = 100f;

    private GameObject _player;
    private GameObject _target;
    private GameObject _currentWeapon;

    public List<GameObject> Weapons;

    private WheelsController _wheelsController;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag(_playerTag);
        _wheelsController = GetComponent<WheelsController>();
    }
    private void FixedUpdate()
    {
        if (CurrentWeaponCanAttack())
        {
            // TO DO: make a brake and attack with current weapon
        }

        UpdateAIMovement();
    }

    /// <summary>
    /// update the AI movement from target
    /// </summary>
    private void UpdateAIMovement()
    {
        // get the path until the player
        Vector3 position = new();
        bool succesToFindPath = false;

        if (!_attackPlayerWeapon)
        {
            succesToFindPath = GetFirstPathPosition(_player.transform.position, ref position);
            _target = _player;
        }
        else
        {
            GameObject playerBestWeapon = GetBestPlayerWeapon();
            succesToFindPath = GetFirstPathPosition(playerBestWeapon.transform.position, ref position);
            _target = playerBestWeapon;
        }

        _currentWeapon = GetBestWeaponFromTarget(_target.transform);

        // if he find a path for focus player
        if (succesToFindPath)
        {
            MoveToPoint(position, _currentWeapon.transform);
        }
        else
        {
            Vector3 targetDirection = (_target.transform.position - transform.position).normalized;
            MoveToPoint(transform.position + targetDirection * 2f, _currentWeapon.transform);
        }
    }
    /// <summary>
    /// set wheel velocity from the target
    /// </summary>
    /// <param name="target"></param>
    private void MoveToPoint(Vector3 target, Transform weapon)
    {
        float forwardAmout = 0f;
        float TurnAmount = 0f;

        // for set the movement
        Vector3 dir = (target - weapon.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);
        float angleToDir = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        if (dot > 0f)
        {
            // go forward
            forwardAmout = 1f;
        }
        else
        {
            // go backward
            forwardAmout = -1f;
        }

        if (angleToDir > 0f)
        {
            // turn right
            TurnAmount = .5f;
        }
        else
        {
            // turn left
            TurnAmount = -.5f;
        }
        
        // set controller parameters
        _wheelsController.Direction = TurnAmount;
        if (forwardAmout > 0f)
        {
            _wheelsController.Movement = WheelsController.Move.toward;
        }
        else
        {
            _wheelsController.Movement = WheelsController.Move.backward;
        }
    }
    /// <summary>
    /// get the position to the seconds point for a path until the player
    /// </summary>
    /// <returns>return the secondes point of path</returns>
    private bool GetFirstPathPosition(Vector3 destination, ref Vector3 result)
    {
        if (_agents == null)
        {
            result = Vector3.zero;
            return false;
        }

        // create and calcute path
        var path = new NavMeshPath();
        _agents.CalculatePath(destination, path);

        // return the seconds point of path
        if (path.corners.Length > 1)
        {
            result = path.corners[1];
            return true;
        }

        result = Vector3.zero;
        return false;
    }
    /// <summary>
    /// select the best player weapons
    /// </summary>
    /// <returns>return the best player weapon object</returns>
    private GameObject GetBestPlayerWeapon()
    {
        // sort the best weapon
        return _player;
    }
    /// <summary>
    /// sort weapons from power, distance and if he is not behind to the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns>return the best weapon from the distance to the target</returns>
    private GameObject GetBestWeaponFromTarget(Transform target)
    {
        if (!_attackWithBestWeapon)
            return this.gameObject;

        // sort if weapon is behind the target
        Weapons = Weapons
            .Where(x => Vector3.Dot(x.transform.forward, (target.position - x.transform.position).normalized) > 0f)
            .ToList();

        // if all weapons are behind the target return self
        if (Weapons.Count < 1)
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
        
        return true;
    }
}
