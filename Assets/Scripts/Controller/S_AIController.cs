
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class S_AIController : MonoBehaviour
{
    [SerializeField, Tooltip("layer for hit trap with raycast")]
    private LayerMask _trapLayer;

    [Header("Enemy")]
    [SerializeField, Tooltip("Tag for find enemy")] 
    private string _enemyTag;

    [Header("Togles Actions")]
    [SerializeField, Tooltip("if he focus the enemy weapon")]
    private bool _attackEnemyWeapon;
    [SerializeField, Tooltip("if he use her best weapon for attack")]
    private bool _attackWithBestWeapon;
    [SerializeField, Tooltip("if he dodge the trap")]
    private bool _dodgeTrap;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100), Tooltip("probability to make an attack when he can do it")] 
    private float _attackProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to make a movement every frame")]
    private float _movementProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to turn for make a dodge")]
    private float _dodgeProbability = 100f;

    [Header("Attack")]
    [SerializeField ,ReadOnly, Tooltip("if he can attack")] 
    private bool _canAttack = true;
    [SerializeField, Min(0f), Tooltip("coolDown for the next attack")]
    private float _attackCooldown = 1f;

    [Header("Targets (Debug)")]
    [SerializeField, ReadOnly, Tooltip("enemy bot")]
    private GameObject _enemy;
    [SerializeField, ReadOnly, Tooltip("this is the focus target he can be the enemy or the enemy weapons")]
    private GameObject _target;
    [SerializeField, ReadOnly, Tooltip("this is the current weapon used for attack the target")]
    private GameObject _currentWeapon;

    private WheelsController _wheelsController;
    private GameObject[] _traps;
    private Vector3 _fleeDestination;

    // test varaible 
    public List<GameObject> Weapons;
    public List<GameObject> EnemyWeapons;

    private void Start()
    {
        _enemy = GameObject.FindGameObjectWithTag(_enemyTag);
        _wheelsController = GetComponent<WheelsController>();
        MoveBotToTarget();
    }
    private void FixedUpdate()
    {
        if (CurrentWeaponCanAttack())
        {
            _wheelsController.Movement = WheelsController.Move.neutral;
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

        if (!_currentWeapon.activeSelf || !_canAttack)
        {
            FleeEnemyWithTrap();
        }
        else
        {
            _fleeDestination = Vector3.positiveInfinity;
            MoveBotToTarget();
        }
    }
    /// <summary>
    /// Flee the enemy in puuting us between us and the enemy 
    /// </summary>
    private void FleeEnemyWithTrap()
    {
        _traps = FindGameObjectsInLayer(6);
        Transform nearestTrap = FindNearestObject(_traps, transform.position).transform;
        Vector3 dirSelfToTrap = nearestTrap.position - transform.position;

        if (_fleeDestination.Equals(Vector3.positiveInfinity))
            _fleeDestination = transform.position + dirSelfToTrap + dirSelfToTrap.normalized * 3f;

        float distanceToFleeDesination = Vector3.Distance(transform.position, _fleeDestination);

        if (distanceToFleeDesination > 3f)
            MoveToPoint(_fleeDestination, transform);
    }
    /// <summary>
    /// update AI movement and set the current weapon for i and enemy
    /// </summary>
    private void MoveBotToTarget()
    {
        if (!_attackEnemyWeapon)
        {
            _target = _enemy;
        }
        else
        {
            GameObject playerBestWeapon = GetBestEnemyWeaponFromTarget(transform);
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
        float turnAmount = 0f;
        bool dodge = false;

        // for set the movement
        Vector3 dir = (target - weapon.position).normalized;
        Vector3 weaponForward = GetForwardWeapon(weapon, transform);

        float dotDirection = Vector3.Dot(weaponForward, dir);
        float angleToDir = Vector3.SignedAngle(weaponForward, dir, Vector3.up);
        float dotWeaponBody = Vector3.Dot(weaponForward, transform.forward);

        // get turn amount if he hit any trap
        float dodgeRnd = Random.Range(0, 101);
        if (dodgeRnd < _dodgeProbability)
        {
            turnAmount = GetTurnAmountForDodgeTrap(dotDirection > 0f ? 1f : -1f);
            
            if (turnAmount != 0f)
                dodge = true;
        }

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

        if (!dodge)
        {
            if (angleToDir > 0f)
            {
                // turn right
                turnAmount = dotWeaponBody > 0f ? 1f : -1f;
            }
            else
            {
                // turn left
                turnAmount = dotWeaponBody > 0f ? -1f : 1f;
            }
        }

        // set controller direction
        _wheelsController.Direction = turnAmount;
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
        bool tuchOneTime = false;

        for (float angle = 0; angle <= 100f; angle += 20f)
        {
            // calcul direction of ray
            float xDirection = Mathf.InverseLerp(-50f, 50f, angle - 50f) * 2f - 1f;
            var direction = new Vector3(xDirection, 0f, scaleDirection);

            // make a raycast
            bool hit = Physics.Raycast(transform.position, transform.TransformDirection(direction), 5f, _trapLayer);
            if (hit)
            {
                turnAmount += (angle - 50f) > 0f ? -1f : 1f;
                tuchOneTime = true;
                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * 5f, Color.green, 0f);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * 5f, Color.red, 0f);
            }
        }

        if (turnAmount == 0f && tuchOneTime)
            return 1f;

        return Mathf.Clamp(turnAmount, -1f, 1f);
    }

    
    /// <summary>
    /// find all gameobject with layer
    /// </summary>
    /// <param name="layer">layer to find</param>
    /// <returns>return gameObject array of all object with this layer</returns>
    private GameObject[] FindGameObjectsInLayer(int layer)
    {
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }
    /// <summary>
    /// Find the nearest gameobject
    /// </summary>
    /// <param name="gameObjects">all object to calcul distance</param>
    /// <param name="position">point zero for find distance</param>
    /// <returns>return the nearest object</returns>
    private GameObject FindNearestObject(GameObject[] gameObjects, Vector3 position)
    {
        return gameObjects
            .OrderBy(x => Vector3.Distance(x.transform.position, position))
            .ToList()[0];
    }
    /// <summary>
    /// get the forward vector
    /// </summary>
    /// <param name="weapon">current weapon</param>
    /// <param name="bot">him self</param>
    /// <returns>return the forward vector of current weapon</returns>
    private Vector3 GetForwardWeapon(Transform weapon, Transform bot)
    {
        Vector3 dirSelfWeapon = (weapon.transform.position - bot.position).normalized;
        float dot = Vector3.Dot(bot.forward, dirSelfWeapon);

        return dot >= 0f ? bot.forward : -bot.forward;
    }


    /// <summary>
    /// select the best player weapons
    /// </summary>
    /// <returns>return the best player weapon object</returns>
    private GameObject GetBestEnemyWeaponFromTarget(Transform target)
    {
        if (EnemyWeapons.Count < 1)
            return _enemy;

        var cloneList = EnemyWeapons
            .Where(x => Vector3.Dot(GetForwardWeapon(x.transform, _enemy.transform), (target.position - x.transform.position).normalized) > 0f)
            .ToList();

        if (cloneList.Count < 1)
            return _enemy;

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
        if (Weapons.Count < 1)
            return false;

        if (!_attackWithBestWeapon)
        {
            weapon = Weapons[Random.Range(0, Weapons.Count)];
            return true;
        }

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
    /// use the current weapon for make an attack
    /// </summary>
    private void AttackWhithCurrrentWeapon()
    {
        if (!_canAttack)
            return;

        StartCoroutine(AttackCooldown());

        // make a probabiblity for attack
        float attackRnd = Random.Range(0, 101);
        if (attackRnd > _attackProbability)
            return;

        _currentWeapon.SetActive(false);
        GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);
        // TO DO: make attack with current weapon
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
    /// if i can take any damage with enemy
    /// </summary>
    /// <returns>return true if i can take any damage</returns>
    private bool CanTakeDamageWithEnemy()
    {
        var activePlayerWeapons = GetActiveEnemyWeapons();

        foreach (var weapon in activePlayerWeapons)
        {
            float distanceWeaponToSelf = Vector3.Distance(transform.position, weapon.transform.position);

            if (distanceWeaponToSelf < 4f)
                return true;
        }

        return false;
    }
    /// <summary>
    /// Get all weapon who can attack him self
    /// </summary>
    /// <returns>return an array of all weapons object who can attack him self</returns>
    private GameObject[] GetActiveEnemyWeapons()
    {
        return EnemyWeapons.Where(x => x.activeSelf).ToArray();
    }
    /// <summary>
    /// Cooldown for attack, set the _canAttack false and true with 1 seconde
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(1f);
        _canAttack = true;
    }
}