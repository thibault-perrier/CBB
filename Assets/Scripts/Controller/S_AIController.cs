
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_AIController : MonoBehaviour
{
    [SerializeField, Tooltip("layer for hit trap with raycast")]
    private LayerMask _trapLayer;

    [Header("Player")]
    [SerializeField, Tooltip("Tag for find player")] 
    private string _playerTag;

    [Header("Togles Actions")]
    [SerializeField, Tooltip("if he focus the plaeyr weapon")]
    private bool _attackPlayerWeapon;
    [SerializeField, Tooltip("if he use her best weapon for attack")]
    private bool _attackWithBestWeapon;
    [SerializeField, Tooltip("if he dodge the trap")]
    private bool _dodgeTrap;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100), Tooltip("probability to make an attack when he can do it")] 
    private float _attackProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to make a movement every frame")]
    private float _movementProbability = 100f;

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
        MoveBotToTarget();
    }
    private void FixedUpdate()
    {
        if (CurrentWeaponCanAttack())
        {
            _wheelsController.Movement = WheelsController.Move.neutral;
            _currentWeapon.SetActive(false);
            AttackWhithCurrrentWeapon();
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
        float movementRnd = Random.Range(0, 101);

        if (movementRnd > _movementProbability)
            return;

        bool canTakeDamage = CanTakeDamageWithPlayer();
        if (canTakeDamage && !_currentWeapon.activeSelf)
        {
            MoveToPoint(transform.position - transform.forward * 4f, transform);
        }
        else
        {
            MoveBotToTarget();
        }
    }
    /// <summary>
    /// update AI movement and set the current weapon for i and enemy
    /// </summary>
    private void MoveBotToTarget()
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

        bool succes = GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);

        if (succes)
            MoveToPoint(_target.transform.position, _currentWeapon.transform);
    }
    /// <summary>
    /// set wheel velocity from the target
    /// </summary>
    /// <param name="target"></param>
    private void MoveToPoint(Vector3 target, Transform weapon)
    {
        // for set the movement
        Vector3 dir = (target - weapon.position).normalized;
        float dotDirection = Vector3.Dot(weapon.forward, dir);
        float angleToDir = Vector3.SignedAngle(weapon.forward, dir, Vector3.up);
        float dotWeaponBody = Vector3.Dot(weapon.forward, transform.forward);

        // get turn amount if he hit any trap
        float TurnAmount = GetTurnAmountForDodgeTrap(dotDirection > 0f ? 1f : -1f);

        if (dotDirection < 0f && dotWeaponBody < 0f)
        {
            _wheelsController.Direction = angleToDir > 0f ? 1f : -1f;
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

        if (TurnAmount == 0f)
        {
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

        var cloneList = PlayerWeapons
            .Where(x => Vector3.Dot(x.transform.forward, (target.position - x.transform.position).normalized) > 0f)
            .ToList();

        if (cloneList.Count < 1)
            return _player;

        // sort the best weapon
        return cloneList
            .Select(x => x.transform)
            .OrderBy(x => int.Parse(x.name) - Vector3.Distance(x.position, target.position))
            .Reverse()
            .ToList()[0].gameObject;
    }
    /// <summary>
    /// sort weapons from power, distance and if he is not behind to the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns>return the best weapon from the distance to the target</returns>
    private bool GetBestWeaponFromTarget(Transform target, ref GameObject weapon)
    {
        if (!_attackWithBestWeapon || Weapons.Count < 1)
            return false;

        var cloneList = Weapons
            .Where(x => x.activeSelf)
            .ToList();

        if (cloneList.Count < 1)
            return false;

        // sort the best weapon by power, distance and look
        weapon = cloneList
            .Select(x => x.transform)
            .OrderBy(x => int.Parse(x.name) - Vector3.Distance(x.position, target.position))
            .Reverse()
            .ToList()[0].gameObject;

        return true;
    }
    /// <summary>
    /// if the target is closed the current weapon
    /// </summary>
    /// <returns>return if he attack with current we hit something</returns>
    private bool CurrentWeaponCanAttack()
    {
        if (!_currentWeapon.activeSelf || !_target)
            return false;

        float targetDistance = Vector3.Distance(_target.transform.position, _currentWeapon.transform.position);
        if (targetDistance < 1f)
            return true;
        
        return false;
    }
    /// <summary>
    /// make raycasts for define the turnAmount
    /// </summary>
    /// <returns>return the new turn amount</returns>
    private float GetTurnAmountForDodgeTrap(float scaleDirection)
    {
        if (!_dodgeTrap)
            return 0f;

        float turnAmount = 0f;

        for (float angle = 0; angle < 100f; angle += 20f)
        {
            // calcul direction of ray
            float xDirection = Mathf.InverseLerp(-50f, 50f, angle - 50f) * 2f - 1f;
            var direction = new Vector3(xDirection, 0f, scaleDirection);

            // make a raycast
            bool hit = Physics.Raycast(transform.position, transform.TransformDirection(direction), 5f, _trapLayer);
            if (hit)
            {
                turnAmount += (angle - 50f) > 0f ? -1f : 1f;
                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * 5f, Color.green, 0f);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * 5f, Color.red, 0f);
            }
        }

        return Mathf.Clamp(turnAmount, -1f, 1f);
    }
    /// <summary>
    /// use the current weapon for make an attack
    /// </summary>
    private void AttackWhithCurrrentWeapon()
    {
        // make a probabiblity for attack
        float attackRnd = Random.Range(0, 101);

        if (attackRnd > _attackProbability)
            return;

        // TO DO: make attack with current weapon
    }
    /// <summary>
    /// if i can take any damage with chalenger
    /// </summary>
    /// <returns>return true if i can take any damage</returns>
    private bool CanTakeDamageWithPlayer()
    {
        var activePlayerWeapons = GetActivePlayerWeapons();

        foreach (var weapon in activePlayerWeapons)
        {
            float distanceWeaponToSelf = Vector3.Distance(transform.position, weapon.transform.position);

            if (distanceWeaponToSelf < 2f)
                return true;
        }

        return false;
    }
    /// <summary>
    /// Get all player who can attack player
    /// </summary>
    /// <returns>return an array of all weapons object who can attack the player</returns>
    private GameObject[] GetActivePlayerWeapons()
    {
        return PlayerWeapons.Where(x => x.activeSelf).ToArray();
    }
}
