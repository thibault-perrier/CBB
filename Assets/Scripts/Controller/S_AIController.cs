
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(S_FrameManager))]
public class S_AIController : MonoBehaviour
{
    public enum FleeType
    {
        None,
        Failure,
        Trap,
        Distance
    }
    public enum AIState
    {
        Enable,
        Disable,
    }

    [SerializeField, Tooltip("the current state of AI")]
    private AIState _aiState;

    [Header("Enemy")]
    [SerializeField, Tooltip("Tag for find enemy")] 
    private string _enemyTag;

    [Header("Togles Actions")]
    [SerializeField, Tooltip("if he focus the enemy weapon")]
    private bool _attackEnemyWeapon;
    [SerializeField, Tooltip("if he use her best weapon for attack")]
    private bool _attackWithBestWeapon;
    [SerializeField, Tooltip("if he can fail any attack")]
    private bool _canFailedAnyAttack;

    [Space(15)]
    [SerializeField, Tooltip("if he dodge the trap")]
    private bool _dodgeTrap;
    [SerializeField, Tooltip("if he flee the enemy when he cant attack")]
    private bool _canFleeEnemy;
    [SerializeField, Tooltip("if he can ignore the trap when we are enough near the target")]
    private bool _canIgnoreTrap;
    [SerializeField, Tooltip("if he can flee the enmy whith trap")]
    private bool _canFleeWithTrap;

    [Space(15)]
    [SerializeField, Tooltip("if he can make an accidental movement")]
    private bool _canMakeAccidentalMovement;
    [SerializeField, Tooltip("if he can make an accidental direction")]
    private bool _canMakeAccidentalDirection;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100), Tooltip("probability to make an attack when he can do it")] 
    private float _attackSuccesProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to get enemy weapons for the current target")]
    private float _attackEnemyWeaponProbabiltiy = 0f;
    [SerializeField, Range(0, 100), Tooltip("probability to fail an attack when he cant do any attack")]
    private float _attackFailProbability = 0f;

    [Space(15)]
    [SerializeField, Range(0, 100), Tooltip("probability to make a movement every frame")]
    private float _movementProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to turn for make a dodge")]
    private float _dodgeProbability = 100f;
    [SerializeField, Range(0, 100), Tooltip("probability to start the flee")]
    private float _fleeProbability = 100f;

    [Space(15)]
    [SerializeField, Range(0, 100), Tooltip("probability to make an accidental movement")]
    private float _accidentalMovementProbability = 0f;
    [SerializeField, Range(0, 100), Tooltip("probabiltiy to make ab accidental direction")]
    private float _accidentalDirectionProbability = 0f;

    [Header("Toggles actions variable")]
    [SerializeField, Tooltip("layer for hit trap with raycast")]
    private LayerMask _trapLayer;
    [SerializeField, Tooltip("flee object define the flee direction")]
    private Transform _fleeDirection;
    [SerializeField, Min(0f), Tooltip("coolDown for the flee failure come back to None")]
    private float _fleeCooldown;
    [SerializeField, Tooltip("if he get the current enemy and target in start")]
    private bool _getEnemyInStart;

    [Header("Attack")]
    [SerializeField, Tooltip("if he failed an attack")]
    private bool _failAttack;
    [SerializeField, Min(0f), Tooltip("cooldown for try to fail any attack")]
    private float _attackFailCooldown = 1f;
    [SerializeField, Min(0f), Tooltip("min distance for fail an attack")]
    private float _attackFailDistance = 5f;

    [Header("Targets (Debug)")]
    [SerializeField, Tooltip("enemy bot")]
    private GameObject _enemy;
    [SerializeField, Tooltip("this is the focus target he can be the enemy or the enemy weapons")]
    private GameObject _target;
    [SerializeField, Tooltip("this is the current weapon used for attack the target")]
    private S_WeaponManager _currentWeapon;
    [SerializeField] 
    private FleeType _fleeMethode = FleeType.None;

    private S_WheelsController _wheelsController;
    private S_FrameManager _frameManager;
    private GameObject[] _traps;
    private GameObject[] _fleeTraps;
    private Vector3 _fleeDestination;
    private WaitForSeconds _attackFailedCoroutine = new(1f);
    private WaitForSeconds _fleeFailureCooldownCoroutine = new(.5f);
    private float _scaleMovement;
    private float _scaleDirection;

    #region Property
    /// <summary>
    /// the current state of AI
    /// </summary>
    public AIState State
    {
        get => _aiState;
        set => _aiState = value;
    }
    /// <summary>
    /// the current frame of the bot
    /// </summary>
    public S_FrameManager Frame
    {
        get => _frameManager;
    }

    /// <summary>
    /// Set the enemy tag for find enemy
    /// </summary>
    public string EnemyTag
    {
        get => _enemyTag;
        set
        {
            _enemyTag = value;
            _enemy = GameObject.FindGameObjectWithTag(value);
            _target = _enemy;
            GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);
        }
    }

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
    /// if he can fail any attack
    /// </summary>
    public bool CanFailAnyAttack 
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
    public bool CanMakeAccidentalMovement 
    { 
        get => _canMakeAccidentalMovement; 
        set => _canMakeAccidentalMovement = value; 
    }
    /// <summary>
    /// if he can reverse her direction
    /// </summary>
    public bool CanMakeAccidentalDirection
    {
        get => _canMakeAccidentalDirection;
        set => _canMakeAccidentalDirection = value;
    }
    /// <summary>
    /// if he can flee the enmy whith trap
    /// </summary>
    public bool CanFleeEnemyWithTrap
    {
        get => _canFleeWithTrap;
        set => _canFleeWithTrap = value;
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
    public float AttackFailProbability 
    {
        get => _attackFailProbability; 
        set => _attackFailProbability = Mathf.Clamp(value, 0f, 100f); 
    }
    /// <summary>
    /// probability to make an accidental movement
    /// </summary>
    public float AccidentalMovementProbability
    {
        get => _accidentalMovementProbability;
        set => _accidentalMovementProbability = Mathf.Clamp(value, 0f, 100f);
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
    /// probabiltiy to make an accidental direction
    /// </summary>
    public float AccidentalDirectionProbability
    {
        get => _accidentalDirectionProbability;
        set => _accidentalDirectionProbability = Mathf.Clamp(value, 0f, 100f);
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
    /// <summary>
    /// cooldown for try to failed any attack
    /// </summary>
    public float AttackFailCooldown
    {
        get => _attackFailCooldown;
        set
        {
            _attackFailCooldown = Mathf.Max(0f, value);
            _attackFailedCoroutine = new(_attackFailCooldown);
        }
    }

    /// <summary>
    /// min distance for fail an attack
    /// </summary>
    public float AttackFailDistance
    {
        get => _attackFailDistance;
        set => _attackFailDistance = Mathf.Max(0f, value);
    }
    #endregion

    private void Awake()
    {
        _wheelsController = GetComponent<S_WheelsController>();
        _frameManager = GetComponent<S_FrameManager>();
        _frameManager.SelectWeapons();

        _fleeFailureCooldownCoroutine = new(_fleeCooldown);

        if (_getEnemyInStart)
        {
            _enemy = GameObject.FindGameObjectWithTag(_enemyTag);
            _target = _enemy;
            GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);
        }

        _traps = FindGameObjectsInLayer(6);
    }
    private void FixedUpdate()
    {
        // reset the movement state
        //_wheelsController.Movement = 0f;

        // verif if he is enable for work
        if (_aiState.Equals(AIState.Disable))
            return;

        if (!_enemy)
            return;

        // if he can attack
        if (CurrentWeaponCanAttack())
        {
            AttackWithCurrrentWeapon();
        }

        if (_frameManager.AllWeaponIsBroken())
        {
            FleeEnemy();
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
        TryToFindBestWeaponFromTarget(_target.transform);
        TryToAttackWithAnyWeapon();

        // get movement probability
        float movementRnd = Random.Range(0, 101);
        if (movementRnd > _movementProbability)
            return;

        // if he has not weapon for attack or he cant attack
        if (!_currentWeapon.CanAttack)
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

    #region Flee
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

        // select the flee methode and move with selected flee methode
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

        // if he cant flee with trap force the distance mode
        if (!_canFleeWithTrap)
            _fleeMethode = FleeType.Distance;

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
    /// Flee the enemy behind the nearest trap
    /// </summary>
    private void FleeEnemyWithTrap()
    {
        GameObject nearestTrap = FindNearestObject(_fleeTraps, _enemy.transform.position, 3f);
        // if he not find an valide trap
        if (!nearestTrap)
        {
            _fleeMethode = FleeType.Distance;
            return;
        }

        Vector3 dirSelfToTrap = nearestTrap.transform.position - transform.position;

        // if he has not already fleeDestination
        if (_fleeDestination.Equals(Vector3.positiveInfinity))
        {
            // set the flee destination by the behind of nearest trap
            _fleeDestination = transform.position + dirSelfToTrap + (dirSelfToTrap.normalized * 3f);
        }

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
            SelectTheFleeDirection(_fleeDestination);
            MoveToPoint(_fleeDestination, _fleeDirection);
        }
    }
    /// <summary>
    /// Flee the enemy in make some distance between us
    /// </summary>
    private void FleeEnemyWithDistance()
    {
        Vector3 dirToEnemy = (transform.position - _enemy.transform.position);
        Vector3 destination = transform.position + (dirToEnemy.normalized * 5f);

        // move to the point
        SelectTheFleeDirection(destination);
        MoveToPoint(destination, _fleeDirection);
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
    /// Select the flee direction
    /// the flee direction is behind us when the target is in front and he is in front when the target is behind
    /// </summary>
    /// <param name="destination">final destination for calcul the flee direction</param>
    private void SelectTheFleeDirection(Vector3 destination)
    {
        Vector3 dirToTarget = (destination - transform.position).normalized;
        float dotTarget = Vector3.Dot(transform.forward, dirToTarget);
        _fleeDirection.localPosition = Vector3.zero;

        if (dotTarget > 0f)
            _fleeDirection.localPosition = Vector3.forward;
        else
            _fleeDirection.localPosition = -Vector3.forward;
    }
    #endregion

    #region Movement
    /// <summary>
    /// if we are enough near the target just ignore trap
    /// </summary>
    /// <returns>return <b>True</b> if we are enough near to the target</returns>
    private bool IgnoreTrap()
    {
        // if he cant ignore the trap
        if (!_canIgnoreTrap)
            return false;

        Vector3 dirToTarget = (_target.transform.position - transform.position);
        float dotAngle = Vector3.Dot(transform.forward, dirToTarget.normalized);

        if (_wheelsController.Movement.Equals(1f))
        {
            // if he is in view of 40 degre and he is enough near he can ignore trap
            if (dotAngle > Mathf.Cos(40f) && dirToTarget.magnitude < 2.5f)
                return true;
        }
        else if (_wheelsController.Movement.Equals(-1f))
        {
            // if he is in view of 40 degre and he is enough near he can ignore trap
            if (Mathf.Abs(dotAngle) > Mathf.Cos(40f) && dirToTarget.magnitude < 2.5f)
                return true;
        }

        return false;
    }
    /// <summary>
    /// update AI movement and set the current weapon for i and enemy
    /// </summary>
    private void MoveBotToTarget()
    {
        // if he cant attack ennemy weapon _target is juste the enemy
        if (!_attackEnemyWeapon)
        {
            _target = _enemy;
        }
        else
        {
            // get random for not attack enemy weapons
            float attackEnemyWeaponsRnd = Random.Range(0, 101);
            if (attackEnemyWeaponsRnd > _attackEnemyWeaponProbabiltiy)
            {
                _target = _enemy;
            } 
            else
            {
                // set the target from the nearest enemy weapon
                S_WeaponManager enemyBestWeapon = GetBestEnemyWeaponFromTarget(transform);

                if (enemyBestWeapon)
                    _target = enemyBestWeapon.gameObject;
                else
                    _target = _enemy;
            }
        }

        // get him self best weapon
        bool succes = GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);

        // if he get any weapon move to target with your current weapon
        if (succes)
            MoveToPoint(_target.transform.position, _currentWeapon);
    }
    /// <summary>
    /// set wheel velocity from the target
    /// </summary>
    /// <param name="target"></param>
    private void MoveToPoint(Vector3 target, S_WeaponManager weapon)
    {
        // for set the movement
        Vector3 dir = (target - GetHitZone(weapon));
        Vector3 weaponForward = GetForwardWeapon(weapon, transform);
        Debug.DrawRay(target, dir.normalized * (-1f * dir.magnitude), Color.red);

        float angleToDir = Vector3.SignedAngle(weaponForward, dir.normalized, Vector3.up);
        float dotDirection = Vector3.Dot(weaponForward, dir.normalized);
        float dotWeaponBody = Vector3.Dot(weaponForward, transform.forward);
        float dotBehindEnemy = Vector3.Dot(transform.forward, dir.normalized);

        ApplyWheelColliderPhysics(angleToDir, dotDirection, dotWeaponBody, dotBehindEnemy);
    }
    /// <summary>
    /// set wheel velocity from the target
    /// </summary>
    /// <param name="target"></param>
    private void MoveToPoint(Vector3 target, Transform weapon)
    {
        // for set the movement
        Vector3 dir = (target - weapon.position).normalized;
        Vector3 weaponForward = GetForwardWeapon(weapon, transform);

        float angleToDir = Vector3.SignedAngle(weaponForward, dir, Vector3.up);
        float dotDirection = Vector3.Dot(weaponForward, dir);
        float dotWeaponBody = Vector3.Dot(weaponForward, transform.forward);
        float dotBehindEnemy = Vector3.Dot(transform.forward, dir);

        ApplyWheelColliderPhysics(angleToDir, dotDirection, dotWeaponBody, dotBehindEnemy);
    }
    /// <summary>
    /// Set the direction and movement of wheel collider
    /// </summary>
    /// <param name="angleToDir">angle of self bot from target</param>
    /// <param name="dotDirection">the direction from the weapon forward and the target</param>
    /// <param name="dotWeaponBody">the direction from the weapon forward and the self bot</param>
    /// <param name="dotBehindEnemy">the direction from the self forward and the target</param>
    private void ApplyWheelColliderPhysics(float angleToDir, float dotDirection, float dotWeaponBody, float dotBehindEnemy)
    {
        float turnAmount = 0f;
        float movementAmount = 0f;

        bool dodge = false;

        // get turn amount if he hit any trap
        float dodgeRnd = Random.Range(0, 101);
        if (dodgeRnd < _dodgeProbability)
        {
            // if he cant ignore trap
            if (!IgnoreTrap())
            {
                // try get turn amount with raycast
                turnAmount = GetTurnAmountForDodgeTrap(GetDodgeTurnAmountScale(dotDirection, dotWeaponBody));

                // if he hit any trap
                if (turnAmount != 0f)
                {
                    dodge = true;
                }
            }
        }

        // if the weapon is behind the target
        if (dotDirection < 0f)
        {
            _wheelsController.Direction = ReverseDir(2f);
            // if we are in front of the enemy go backward else fo toward
            _wheelsController.Movement = dotBehindEnemy > 0f ? ReverseMov(-.5f) : ReverseMov(.5f);
            return;
        }

        if (dotDirection > 0f)
        {
            // go forward
            movementAmount = dotWeaponBody > 0f ? ReverseMov(LerpMovement(1f)) : ReverseMov(LerpMovement(-1f));
        }
        else
        {
            // go backward
            movementAmount = dotWeaponBody > 0f ? ReverseMov(LerpMovement(-1f)) : ReverseMov(LerpMovement(1f));
        }

        // if he hit any trap dont calcul turn amount
        if (!dodge)
        {
            if (angleToDir > 0f)
            {
                // turn right
                turnAmount = dotWeaponBody > 0f ? CalCulDirection(angleToDir, 1f) : CalCulDirection(angleToDir, -1f);
            }
            else
            {
                // turn left
                turnAmount = dotWeaponBody > 0f ? CalCulDirection(angleToDir, -1f) : CalCulDirection(angleToDir, 1f);
            }
        }

        // set controller direction and movement
        _wheelsController.Direction = ReverseDir(turnAmount);
        _wheelsController.Movement = movementAmount * CalculScaleMovement(angleToDir);
    }
    /// <summary>
    /// lerp the movement speed from the delta time
    /// </summary>
    /// <param name="movement">the millstone movement for lerp</param>
    /// <returns>return the movement with a lerp</returns>
    private float LerpMovement(float movement)
    {
        if (movement <= 0f)
            _scaleMovement = Mathf.Clamp(_scaleMovement - Time.deltaTime, -1f, 1f);
        else
            _scaleMovement = Mathf.Clamp(_scaleMovement + Time.deltaTime, -1f, 1f);

        Debug.Log(movement);
        return _scaleMovement;
    }
    /// <summary>
    /// lerp the direction from the angle
    /// </summary>
    /// <param name="angle">the angle to make</param>
    /// <param name="scale">the scale of angle for turn amount</param>
    /// <returns>return the lerp direction</returns>
    private float CalCulDirection(float angle, float scale)
    {
        return ((Mathf.Abs(angle) / 180f) * scale);
    }
    /// <summary>
    /// reduce the movemen if he need to turn
    /// </summary>
    /// <param name="angle">the angle to turn</param>
    /// <returns>return 0.5 if he need turn else return 1</returns>
    private float CalculScaleMovement(float angle)
    {
        return (Mathf.Abs(angle) > 20f ? .5f : 1f);
    }
    /// <summary>
    /// reverse the current movement with probability if
    /// he move toward return backward else return forward
    /// </summary>
    /// <param name="currentMovement">the current movement for reverse</param>
    /// <returns>return the currentMovement reserve or just currentMovement</returns>
    private float ReverseMov(float currentMovement)
    {
        // if he cant reverse movement return the current
        if (!_canMakeAccidentalMovement)
            return currentMovement;

        float reverseMovementRnd = Random.Range(0, 101);
        // if movement equal 1 return -1 else return 1
        if (reverseMovementRnd < _accidentalMovementProbability)
            return currentMovement * -1f;

        return currentMovement;
    }
    /// <summary>
    /// reverse the current direction with probability if
    /// he turn right return turn left else return turn right
    /// </summary>
    /// <param name="direction">the current direction</param>
    /// <returns>return the direction reverse or not</returns>
    private float ReverseDir(float direction)
    {
        // if he cant reverse direction return the current
        if (!_canMakeAccidentalDirection)
            return direction;

        float reverseDirectionRnd = Random.Range(0, 101);
        // if direction equal 1 return -1 else return 1
        if (reverseDirectionRnd < _accidentalDirectionProbability)
            return direction * -1f;

        return direction;
    }
    #endregion

    #region Dodge
    /// <summary>
    /// get the scale for the GetTurnAmountForDodgeTrap methode
    /// </summary>
    /// <param name="dotDirection">the dot product from weapon forward and direction from me and enemy</param>
    /// <param name="dotWeapon">the dot product from weapon forward and self forward</param>
    /// <returns>return the scale</returns>
    private float GetDodgeTurnAmountScale(float dotDirection, float dotWeapon)
    {
        // if weapon is toward target
        if (dotDirection > 0f)
        {
            // if weapon isnt behind self forward return -1f
            return dotWeapon > 0f ? 1f : -1f;
        }
        else
        {
            // if weapon isnt behind self forward return 1f
            return dotWeapon > 0f ? -1f : 1f;
        }
    }
    /// <summary>
    /// make raycasts for define the turnAmount
    /// </summary>
    /// <returns>return the new turn amount</returns>
    private float GetTurnAmountForDodgeTrap(float scaleDirection)
    {
        // if he cant dodge trap return 0 for the new turnAmount
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
            bool hit = Physics.Raycast(transform.position, transform.TransformDirection(direction), 10f, _trapLayer);
            if (hit)
            {
                // add the turn amount by the raycast angle
                turnAmount += ((angle - 50f) > 0f ? -.1f : .1f) * scaleDirection;
                tuchOneTime = true;
                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * 10f, Color.green, 0f);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * 10f, Color.red, 0f);
            }
        }

        // if all raycast tuch any traps them return 1
        if (turnAmount == 0f && tuchOneTime)
            return 1f;

        return Mathf.Clamp(turnAmount, -1f, 1f);
    }
    #endregion

    #region Finder
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
    private GameObject FindNearestObject(GameObject[] gameObjects, Vector3 position, float minDistance = 0f)
    {
        // sort all element by the distance and get the first
        return gameObjects
            .Where(x => Vector3.Distance(x.transform.position, position) > minDistance)
            .OrderBy(x => Vector3.Distance(x.transform.position, position))
            .ToList()[0];
    }
    #endregion

    #region WeaponsFinder
    /// <summary>
    /// get the forward vector of weapon in relation of bot
    /// </summary>
    /// <param name="weapon">current weapon</param>
    /// <param name="bot">him self</param>
    /// <returns>return the forward vector of current weapon</returns>
    private Vector3 GetForwardWeapon(Transform weapon, Transform bot)
    {
        // get the dot product of the weapon and the current bot
        Vector3 dirSelfWeapon = (weapon.position - bot.position).normalized;
        float dot = Vector3.Dot(bot.forward, dirSelfWeapon);

        return dot >= 0f ? bot.forward : -bot.forward;
    }
    /// <summary>
    /// get the forward vector of weapon in relation of bot
    /// </summary>
    /// <param name="weapon">current weapon</param>
    /// <param name="bot">him self</param>
    /// <returns>return the forward vector of current weapon</returns>
    private Vector3 GetForwardWeapon(S_WeaponManager weapon, Transform bot)
    {
        // get the dot product of the weapon and the current bot
        Vector3 dirSelfWeapon = (GetHitZone(weapon) - bot.position).normalized;
        float dot = Vector3.Dot(bot.forward, dirSelfWeapon);

        return dot >= 0f ? bot.forward : -bot.forward;
    }
    /// <summary>
    /// detect if the weapon is in view
    /// </summary>
    /// <param name="weapon">current weapon of the <b>bot</b></param>
    /// <returns>return True if he is in view</returns>
    private bool IsValidEnemyWeapon(S_WeaponManager weapon)
    {
        Vector3 enemyWeaponForward = GetForwardWeapon(weapon.transform, _enemy.transform);
        Vector3 enemyFromCurrentBot = (transform.position - _enemy.transform.position);
        Vector3 enemyFromEnemyWeapon = (weapon.transform.position - _enemy.transform.position);

        float dot = Vector3.Dot(enemyWeaponForward, enemyFromCurrentBot);
        float angleFromCurrentBot = Vector3.SignedAngle(enemyWeaponForward, enemyFromCurrentBot.normalized, Vector3.up);
        float angleFromCurrentWeapon = Vector3.SignedAngle(_enemy.transform.forward, enemyFromEnemyWeapon, Vector3.up);
        float inSameSide = (angleFromCurrentBot / 180f) * (angleFromCurrentWeapon / 180f);

        if (weapon.CurrentState != S_WeaponManager.State.ok)
            return false;

        if (!weapon.CanRecieveDamage())
            return false;

        // if the weapon is behind the enemy
        if (dot < 0f)
            return false;

        // if the weapon isnt in the same side of the player
        if (inSameSide < 0f)
            return false;

        return true;
    }
    /// <summary>
    /// select the best player weapons
    /// </summary>
    /// <returns>return the best player weapon object</returns>
    private S_WeaponManager GetBestEnemyWeaponFromTarget(Transform target)
    {
        S_FrameManager enemyWeapon = _enemy.GetComponent<S_FrameManager>();

        // return the current enemy if he has not weapons
        if (enemyWeapon.Weapons.Count < 1)
            return null;

        // sort the weapon if he is behind him self
        var cloneList = enemyWeapon.Weapons
            .Where(x => IsValidEnemyWeapon(x))
            .ToList();

        // if all enemy weapons are behind him
        if (cloneList.Count < 1)
            return null;

        // sort the best weapon
        var bestWeapon = cloneList
            .OrderBy(x => Vector3.Distance(x.gameObject.transform.position, target.position))
            .Reverse()
            .ToList()[0];

        float distanceToBestWeapon = Vector3.Distance(bestWeapon.transform.position, transform.position);
        float distanceToEnemy = Vector3.Distance(_enemy.transform.position, transform.position);

        if (distanceToBestWeapon > distanceToEnemy)
            return null;

        return bestWeapon;
    }
    /// <summary>
    /// sort weapons from power, distance and if he is not behind to the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns>return the best weapon from the distance to the target</returns>
    private bool GetBestWeaponFromTarget(Transform target, ref S_WeaponManager weapon)
    {
        if (!target)
            return false;

        if (_frameManager.Weapons.Count < 1)
            return false;

        // if he not attack with best weapon return the random pick up of random weapon
        if (!_attackWithBestWeapon)
        {
            weapon = _frameManager.Weapons[0];
            return true;
        }

        // if the current weapon is currently attacking dont change current weapon
        if (_currentWeapon)
        {
            if (_currentWeapon.Attacking && _currentWeapon.CanAttack)
                return true;
        }

        var alwaysActiveWeapons = _frameManager.Weapons.Where(x => x.AlwaysActive && x.CanAttack).ToList();
        var notAlwaysActiveWeapons = _frameManager.Weapons.Where(x => !x.AlwaysActive && x.CanAttack).ToList();

        if (alwaysActiveWeapons.Count < 1 && notAlwaysActiveWeapons.Count < 1)
            return false;

        if (notAlwaysActiveWeapons.Count > 0)
        {
            // sort by the distance
            weapon = notAlwaysActiveWeapons
                .OrderBy(x => Vector3.Distance(x.gameObject.transform.position, target.position))
                .ToList()[0];
        }
        else
        {
            // sort by the distance
            weapon = alwaysActiveWeapons
                .OrderBy(x => Vector3.Distance(x.gameObject.transform.position, target.position))
                .ToList()[0];
        }

        return true;
    }
    /// <summary>
    /// get hit zone world position
    /// </summary>
    /// <param name="weapon">the current weapon</param>
    /// <returns>Return the hitZone World position of the weapon</returns>
    private Vector3 GetHitZone(S_WeaponManager weapon)
    {
        return weapon.HitZone.transform.TransformPoint(weapon.HitZone.center);
    }
    /// <summary>
    /// when he move if the current weapon is behind the target reselect the best weapon form the target
    /// </summary>
    /// <param name="target"></param>
    private void TryToFindBestWeaponFromTarget(Transform target)
    {
        if (!_currentWeapon)
            return;

        Vector3 dirToEnemy = target.position - _currentWeapon.transform.position;

        if (dirToEnemy.magnitude > 5f)
        {
            float dotWeaponForward = Vector3.Dot(GetForwardWeapon(_currentWeapon, transform), dirToEnemy.normalized);
            
            if (dotWeaponForward < 0f)
                GetBestWeaponFromTarget(target, ref _currentWeapon);
        }
    }
    #endregion

    #region Attack
    /// <summary>
    /// try to failed any attack with probability
    /// </summary>
    /// <returns>return <b>True</b> if he make an attack</returns>
    private void TryFailedAttack()
    {
        if (_failAttack)
            return;

        StartCoroutine(AttackFailedCooldownCoroutine());

        // if he is not enough close to the enemy
        Vector3 hitZonePosition = _currentWeapon.HitZone.transform.TransformPoint(_currentWeapon.HitZone.center);
        float distanceToEnemy = Vector3.Distance(hitZonePosition, _target.transform.position);
        if (distanceToEnemy > _attackFailDistance)
            return;

        if (!_canFailedAnyAttack)
            return;
        
        // get random for the attack failed
        float failedAttackRnd = Random.Range(0, 101);
        if (_attackFailProbability > failedAttackRnd)
        {
            if (!CurrentWeaponCanAttack())
            {
                AttackWithCurrrentWeapon();
            }
        }
    }
    /// <summary>
    /// use the current weapon for make an attack
    /// </summary>
    private void AttackWithCurrrentWeapon()
    {
        // make a probabiblity for attack
        float attackRnd = Random.Range(0, 101);
        if (attackRnd > _attackSuccesProbability)
            return;

        _currentWeapon.LaunchAttack();
        GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);
    }
    /// <summary>
    /// if the target is closed the current weapon
    /// </summary>
    /// <returns>return if he attack with current we hit something</returns>
    private bool CurrentWeaponCanAttack()
    {
        if (!_currentWeapon)
            return false;

        // if he is attacking and he cant take any damage with current weapon
        if (_currentWeapon.Attacking && !_currentWeapon.CanTakeAnyDamage)
            return false;

        if (!_currentWeapon.CanRecieveDamage())
            return false;

        // if he is attacking
        if (_currentWeapon.Attacking)
            return true;

        // if he can attack and he has not target
        if (!_currentWeapon.CanAttack || !_target)
            return false;

        // return True if if can make any damage with current weapon
        return _currentWeapon.CanTakeAnyDamage;
    }
    /// <summary>
    /// try to attack with any weapon when it in movement
    /// </summary>
    private void TryToAttackWithAnyWeapon()
    {
        foreach (var weapon in _frameManager.Weapons)
        {
            // if the weapon can make an attack and if he can take any damage with her attack
            if (weapon.CanAttack && weapon.CanTakeAnyDamage)
            {
                // make a probabiblity for attack
                float attackRnd = Random.Range(0, 101);
                if (attackRnd > _attackSuccesProbability)
                    return;

                weapon.LaunchAttack();
                GetBestWeaponFromTarget(_target.transform, ref _currentWeapon);
            }
        }
    }
    /// <summary>
    /// Cooldown for try to fail any attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackFailedCooldownCoroutine()
    {
        _failAttack = true;
        yield return _attackFailedCoroutine;
        _failAttack = false;
    }
    #endregion
}