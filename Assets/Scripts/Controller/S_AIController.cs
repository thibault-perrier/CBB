
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_AIController : MonoBehaviour
{
    public enum FleeType
    {
        None,
        Failure,
        Trap,
        Distance
    }

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
    [SerializeField, Tooltip("if he flee the enemy when he cant attack")]
    private bool _canFleeEnemy;
    [SerializeField, Tooltip("if he can failed any attack")]
    private bool _canFailedAnyAttack;
    [SerializeField, Tooltip("if he can ignore the trap when we are enough near the target")]
    private bool _canIgnoreTrap;
    [SerializeField, Tooltip("if he can reverse her movement")]
    private bool _canReverseMovement;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100), Tooltip("probability to make an attack when he can do it")] 
    private float _attackSuccesProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to make a movement every frame")]
    private float _movementProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to turn for make a dodge")]
    private float _dodgeProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to start the flee")]
    private float _fleeProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to fail an attack when he cant do any attack")]
    private float _attackFailedProbability = 0f;
    [SerializeField, Range(0, 100), Tooltip("probability to reverse her movement")]
    private float _reverseMovementProbability = 0f;
    [SerializeField, Range(0, 100), Tooltip("probability to get enemy weapons for the current target")]
    private float _attackEnemyWeaponProbabiltiy = 0f;

    [Header("Toggles actions variable")]
    [SerializeField, Tooltip("layer for hit trap with raycast")]
    private LayerMask _trapLayer;
    [SerializeField, Tooltip("flee object define the flee direction")]
    private Transform _fleeDirection;
    [SerializeField, Min(0f), Tooltip("coolDown for the flee failure come back to None")]
    private float _fleeCooldown;

    [Header("Attack")]
    [SerializeField, Tooltip("if he can attack")]
    private bool _canAttack = true;
    [SerializeField, Min(0f), Tooltip("coolDown for the next attack")]
    private float _attackCooldown = 1f;
    [SerializeField, Min(0f), Tooltip("min distance for failed an attack")]
    private float _attackFailedDistance = 5f;

    [Header("Targets (Debug)")]
    [SerializeField, Tooltip("enemy bot")]
    private GameObject _enemy;
    [SerializeField, Tooltip("this is the focus target he can be the enemy or the enemy weapons")]
    private GameObject _target;
    [SerializeField, Tooltip("this is the current weapon used for attack the target")]
    private GameObject _currentWeapon;

    private S_WheelsController _wheelsController;
    private GameObject[] _traps;
    private GameObject[] _fleeTraps;
    private Vector3 _fleeDestination;
    private WaitForSeconds _attackCooldownCoroutine = new(1f);
    private WaitForSeconds _fleeFailureCooldownCoroutine = new(.5f);

    [SerializeField]
    private FleeType _fleeMethode = FleeType.None;

    // test varaible 
    public List<GameObject> Weapons;
    public List<GameObject> EnemyWeapons;


    /// <summary>
    /// if he focus the enemy weapon
    /// </summary>
    public bool AttackEnemyWeapon
    { 
        get => _attackEnemyWeapon;    
        set => _attackEnemyWeapon = value; 
    }
    /// <summary>
    /// if he use her best weapon for attack
    /// </summary>
    public bool AttackWithBestWeapon 
    { 
        get => _attackWithBestWeapon; 
        set => _attackWithBestWeapon = value; 
    }
    /// <summary>
    /// if he dodge the trap
    /// </summary>
    public bool DodgeTrap 
    { 
        get => _dodgeTrap;            
        set => _dodgeTrap = value; 
    }
    /// <summary>
    /// if he flee the enemy when he cant attack
    /// </summary>
    public bool CanFleeEnemy 
    { 
        get => _canFleeEnemy;         
        set => _canFleeEnemy = value; 
    }
    /// <summary>
    /// if he can failed any attack
    /// </summary>
    public bool CanFailedAnyAttack 
    { 
        get => _canFailedAnyAttack;   
        set => _canFailedAnyAttack = value; 
    }
    /// <summary>
    /// if he can ignore the trap when we are enough near the target
    /// </summary>
    public bool CanIgnoreTrap 
    { 
        get => _canIgnoreTrap;        
        set => _canIgnoreTrap = value; 
    }
    /// <summary>
    /// if he can reverse her movement
    /// </summary>
    public bool CanReverseMovement 
    { 
        get => _canReverseMovement; 
        set => _canReverseMovement = value; 
    }


    /// <summary>
    /// probability to make an attack when he can do it
    /// </summary>
    public float AttackSuccesProbability 
    { 
        get => _attackSuccesProbability; 
        set => _attackSuccesProbability = Mathf.Clamp(value, 0f, 100f); 
    }
    /// <summary>
    /// probability to make a movement every frame
    /// </summary>
    public float MovementProbability 
    { 
        get => _movementProbability; 
        set => _movementProbability = Mathf.Clamp(value, 0f, 100f); 
    }
    /// <summary>
    /// probability to turn for make a dodge
    /// </summary>
    public float DodgeProbability 
    { 
        get => _dodgeProbability; 
        set => _dodgeProbability = Mathf.Clamp(value, 0f, 100f); 
    }
    /// <summary>
    /// probability to start the flee
    /// </summary>
    public float FleeProbability 
    { 
        get => _fleeProbability; 
        set => _fleeProbability = Mathf.Clamp(value, 0f, 100f); 
    }
    /// <summary>
    /// probability to fail an attack when he cant do any attack
    /// </summary>
    public float AttackFailedProbability 
    {
        get => _attackFailedProbability; 
        set => _attackFailedProbability = Mathf.Clamp(value, 0f, 100f); 
    }
    /// <summary>
    /// probability to reverse her movement
    /// </summary>
    public float ReverseMovementProbability
    {
        get => _reverseMovementProbability;
        set => _reverseMovementProbability = Mathf.Clamp(value, 0f, 100f);
    }
    /// <summary>
    /// probability to get enemy weapons for the current target
    /// </summary>
    public float AttackEnemyWeaponProbability
    {
        get => _attackEnemyWeaponProbabiltiy;
        set => _attackEnemyWeaponProbabiltiy = Mathf.Clamp(value, 0f, 100f);
    }
    /// <summary>
    /// coolDown for the next attack
    /// </summary>
    public float AttackCooldown
    {
        get => _attackCooldown;
        set
        {
            _attackCooldown = Mathf.Max(0f, value);
            _attackCooldownCoroutine = new(_attackCooldown);
        }
    }
    /// <summary>
    /// the cooldown for the failure flee
    /// </summary>
    public float FleeCooldown
    {
        get => _fleeCooldown;
        set
        {
            _fleeCooldown = Mathf.Max(0f, value);
            _fleeFailureCooldownCoroutine = new(_fleeCooldown);
        }
    }

    private void Start()
    {
        _attackCooldownCoroutine = new(_attackCooldown);
        _fleeFailureCooldownCoroutine = new(_fleeCooldown);

        _enemy = GameObject.FindGameObjectWithTag(_enemyTag);
        _target = _enemy;

        _wheelsController = GetComponent<S_WheelsController>();
        GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);
        _traps = FindGameObjectsInLayer(6);
    }
    private void FixedUpdate()
    {
        // reset the movement state
        _wheelsController.Movement = S_WheelsController.Move.neutral;

        // if he can attack
        if (CurrentWeaponCanAttack())
        {
            AttackWithCurrrentWeapon();
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
        TryFailedAttack();

        // get movement probability
        float movementRnd = Random.Range(0, 101);
        if (movementRnd > _movementProbability)
            return;

        // if he has not weapon for attack or he cant attack
        if (!_currentWeapon.activeSelf || !_canAttack)
        {
            FleeEnemy();
        }
        else
        {
            // reset the flee
            _fleeDestination = Vector3.positiveInfinity;
            _fleeMethode = FleeType.None;

            MoveBotToTarget();
        }
    }


    /// <summary>
    /// flee the enemy, he go behind nearest trap if he can or he turn back
    /// </summary>
    private void FleeEnemy()
    {
        GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);

        // if he failed the flee with probability
        if (_fleeMethode.Equals(FleeType.Failure))
            return;

        if (!_canFleeEnemy)
            return;

        SelectTheFleeMethode();
        switch (_fleeMethode)
        {
            case FleeType.Trap: FleeEnemyWithTrap(); break;
            case FleeType.Distance: FleeEnemyWithDistance(); break;
            default: break;
        }
    }
    /// <summary>
    /// Select the methode for flee the enemy
    /// </summary>
    private void SelectTheFleeMethode()
    {
        // if he is not try to flee them try to failure
        if (_fleeMethode.Equals(FleeType.None))
        {
            // get random flee probability
            float fleeRnd = Random.Range(0, 101);
            if (_fleeProbability < fleeRnd)
            {
                StartCoroutine(CoolDownFailureFlee());
                return;
            }
        }

        if (_fleeMethode != FleeType.None)
            return;

        // if he cant move in the current direction
        if (IsBlocked())
            _fleeMethode = FleeType.Distance;
        else
            _fleeMethode = FleeType.Trap;
    }
    /// <summary>
    /// Detect if he can go to any trap
    /// </summary>
    /// <returns>return True if he can move</returns>
    private bool IsBlocked()
    {
        if (_traps == null)
            return true;

        // if there are no trap in the world
        if (_traps.Length < 1)
            return true;

        // select the trap behind us
        _fleeTraps = _traps
            .Where(x => Vector3.Dot((_enemy.transform.position - transform.position).normalized, (x.transform.position - transform.position).normalized) < -.2f)
            .ToArray();
        
        // return True if he is not empty
        return _fleeTraps.Length < 1;
    }
    /// <summary>
    /// Flee the enemy in puuting us between us and the enemy 
    /// </summary>
    private void FleeEnemyWithTrap()
    {
        Transform nearestTrap = FindNearestObject(_fleeTraps, transform.position).transform;
        Vector3 dirSelfToTrap = nearestTrap.position - transform.position;

        // if he has not already fleeDestination
        if (_fleeDestination.Equals(Vector3.positiveInfinity))
            _fleeDestination = transform.position + dirSelfToTrap + dirSelfToTrap.normalized * 3f;

        if (IsBlocked())
        {
            _fleeMethode = FleeType.None;
            return;
        }

        float distanceToFleeDesination = Vector3.Distance(transform.position, _fleeDestination);

        // while we are not nearest to fleeDestination
        if (distanceToFleeDesination > 3f)
        {
            // choice the direction for flee the enemy
            Vector3 dirToEnemy = (_enemy.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, dirToEnemy);
            _fleeDirection.localPosition = Vector3.zero;

            if (dot > 0)
                _fleeDirection.localPosition = -Vector3.forward;
            else
                _fleeDirection.localPosition = Vector3.forward;

            MoveToPoint(_fleeDestination, _fleeDirection);
        }
    }
    /// <summary>
    /// Flee the enemy in make some distance between us
    /// </summary>
    private void FleeEnemyWithDistance()
    {
        Vector3 dirToEnemy = (_enemy.transform.position - transform.position);
        Vector3 destination = Vector3.zero;
        float dot = Vector3.Dot(transform.forward, dirToEnemy.normalized);

        if (dot > 0)
            destination = transform.position - transform.forward;
        else
            destination = transform.position + transform.forward;

        // move to the point
        MoveToPoint(destination, transform);
    }
    /// <summary>
    /// start the cooldown for set the <b>Failure</b> flee and after 1 seconde the <b>None</b>
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoolDownFailureFlee()
    {
        _fleeMethode = FleeType.Failure;
        yield return _fleeFailureCooldownCoroutine;
        _fleeMethode = FleeType.None;
    }


    /// <summary>
    /// if we are enough near the target just ignore trap
    /// </summary>
    /// <returns>return <b>True</b> if we are enough near to the target</returns>
    private bool IgnoreTrap()
    {
        if (!_canIgnoreTrap)
            return false;

        Vector3 dirToTarget = (_target.transform.position - transform.position);
        float dot = Vector3.Dot(transform.forward, dirToTarget.normalized);

        if (dot > Mathf.Cos(40f) && dirToTarget.magnitude < 2.5f)
            return true;

        return false;
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
            // get random for not attack enemy weapons
            float attackEnemyWeaponsRnd = Random.Range(0, 101);
            if (attackEnemyWeaponsRnd < _attackEnemyWeaponProbabiltiy)
            {
                _target = _enemy;
            } 
            else
            {
                // set the target from the nearest enemy weapon
                GameObject playerBestWeapon = GetBestEnemyWeaponFromTarget(transform);
                _target = playerBestWeapon;
            }
        }

        // get him self best weapon
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
        float dotBehindEnemy = Vector3.Dot(transform.forward, dir);

        // get turn amount if he hit any trap
        float dodgeRnd = Random.Range(0, 101);
        if (dodgeRnd < _dodgeProbability)
        {
            if (!IgnoreTrap())
            {
                turnAmount = GetTurnAmountForDodgeTrap(GetDodgeTurnAmountScale(dotDirection, dotWeaponBody));

                if (turnAmount != 0f)
                    dodge = true;
            }
        }

        // if the weapon is behind the target
        if (dotDirection < 0f)
        {
            float direction = angleToDir > 0f ? -1f : 1f;
            _wheelsController.Direction = 2f;
            // if we are in front of the enemy go backward else fo toward
            _wheelsController.Movement = dotBehindEnemy > 0f ? Reverse(S_WheelsController.Move.backward) : Reverse(S_WheelsController.Move.toward);
            return;
        }

        if (dotDirection > 0f)
        {
            // go forward
            _wheelsController.Movement = dotWeaponBody > 0f ? Reverse(S_WheelsController.Move.toward) : Reverse(S_WheelsController.Move.backward);
        }
        else
        {
            // go backward
            _wheelsController.Movement = dotWeaponBody > 0f ? Reverse(S_WheelsController.Move.backward) : Reverse(S_WheelsController.Move.toward);
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
    /// reverse the current movement with probability if
    /// he move toward return backward else return forward
    /// </summary>
    /// <param name="currentMovement">the current movement for reverse</param>
    /// <returns>return the currentMovement reserve or just currentMovement</returns>
    private S_WheelsController.Move Reverse(S_WheelsController.Move currentMovement)
    {
        if (!_canReverseMovement)
            return currentMovement;

        float reverseMovementRnd = Random.Range(0, 101);
        if (reverseMovementRnd < _reverseMovementProbability)
        {
            if (currentMovement.Equals(S_WheelsController.Move.toward))
                return S_WheelsController.Move.backward;

            return S_WheelsController.Move.toward;
        }

        return currentMovement;
    }


    /// <summary>
    /// get the scale for the GetTurnAmountForDodgeTrap methode
    /// </summary>
    /// <param name="dotDirection"></param>
    /// <param name="dotWeapon"></param>
    /// <returns>return the scale</returns>
    private float GetDodgeTurnAmountScale(float dotDirection, float dotWeapon)
    {
        // if weapon is toward target
        if (dotDirection > 0f)
        {
            return dotWeapon > 0f ? 1f : -1f;
        }
        else
        {
            return dotWeapon > 0f ? -1f : 1f;
        }
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

            // make a raycast and add the turn amount
            bool hit = Physics.Raycast(transform.position, transform.TransformDirection(direction), 5f, _trapLayer);
            if (hit)
            {
                // add the turn amount by the raycast angle
                turnAmount += (angle - 50f) > 0f ? -1f : 1f;
                tuchOneTime = true;
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
        // get all gameObject in scene
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            // if the current gameObject has the same layer
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
        // sort all element by the distance and get the first
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
        // get the dot product of the weapon and the current bot
        Vector3 dirSelfWeapon = (weapon.transform.position - bot.position).normalized;
        float dot = Vector3.Dot(bot.forward, dirSelfWeapon);

        return dot >= 0f ? bot.forward : -bot.forward;
    }
    /// <summary>
    /// detect if the weapon is in view
    /// </summary>
    /// <param name="weapon">current weapon</param>
    /// <returns>return True if he is in view</returns>
    private bool RaycastWeapon(GameObject weapon, GameObject bot)
    {
        Vector3 dir = weapon.transform.position - bot.transform.position;
        return Physics.Raycast(bot.transform.position, dir.normalized, dir.magnitude);
    }
    /// <summary>
    /// detect if the weapon is valid, if he enough far is valid else if is behind the enemy is not valid
    /// </summary>
    /// <param name="weapon">current weapon to validate</param>
    /// <returns>return <b>True</b> if it is valid</returns>
    private bool IsValidWeaponForTarget(Transform weapon)
    {
        // if the enemy is enough far the weapon if valid
        Vector3 dirToEnemy = (_target.transform.position - weapon.position);
        if (dirToEnemy.magnitude > 2f)
            return true;

        // if the weapon is before the enemy its a valid weapon
        return Vector3.Dot(GetForwardWeapon(weapon, transform), dirToEnemy.normalized) > 0f;
    }
    /// <summary>
    /// select the best player weapons
    /// </summary>
    /// <returns>return the best player weapon object</returns>
    private GameObject GetBestEnemyWeaponFromTarget(Transform target)
    {
        // return the current enemy if he has not weapons
        if (EnemyWeapons.Count < 1)
            return _enemy;

        // sort the weapon if he is behind him self
        var cloneList = EnemyWeapons
            .Where(x => Vector3.Dot(GetForwardWeapon(x.transform, _enemy.transform), (target.position - x.transform.position).normalized) > 0f)
            .Where(x => !RaycastWeapon(x, gameObject))
            .ToList();

        // if all enemy weapons are behind him
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

        // sort the weapon who can attack
        var cloneList = Weapons
            .Where(x => x.activeSelf)
            .ToList();

        // if he not attack with best weapon return the random pick up of random weapon
        if (!_attackWithBestWeapon)
        {
            weapon = Weapons[0];
            return true;
        }

        // if all weapons cant attack
        if (cloneList.Count < 1)
            return false;

        // sort by the distance
        weapon = cloneList
            .Select(x => x.transform)
            .OrderBy(x => Vector3.Distance(x.position, target.position))
            .ToList()[0].gameObject;

        return true;
    }


    /// <summary>
    /// try to failed any attack with probability
    /// </summary>
    /// <returns>return <b>True</b> if he make an attack</returns>
    private void TryFailedAttack()
    {
        if (!_canAttack)
            return;

        // if he is not enough close to the enemy
        float distanceToEnemy = Vector3.Distance(transform.position, _enemy.transform.position);
        if (distanceToEnemy > _attackFailedDistance)
            return;

        if (!_canFailedAnyAttack)
            return;
        
        // get random for the attack failed
        float failedAttackRnd = Random.Range(0, 101);
        if (_attackFailedProbability >= failedAttackRnd)
        {
            if (!CurrentWeaponCanAttack())
                AttackWithCurrrentWeapon();
        }
    }
    /// <summary>
    /// use the current weapon for make an attack
    /// </summary>
    private void AttackWithCurrrentWeapon()
    {
        if (!_canAttack)
            return;

        StartCoroutine(AttackCooldownCoroutine());

        // make a probabiblity for attack
        float attackRnd = Random.Range(0, 101);
        if (attackRnd > _attackSuccesProbability)
            return;

        StartCoroutine(SetActiveWeapon(_currentWeapon));
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
    /// Cooldown for attack, set the _canAttack <b>False</b> and true with 1 seconde
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackCooldownCoroutine()
    {
        _canAttack = false;
        yield return _attackCooldownCoroutine;
        _canAttack = true;
    }
    private IEnumerator SetActiveWeapon(GameObject weapon)
    {
        if (!weapon.activeSelf)
            yield break;

        weapon.SetActive(false);
        yield return new WaitForSeconds(10f);
        weapon.SetActive(true);
    }
}