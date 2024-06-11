using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class S_WeaponManager : MonoBehaviour, I_Damageable
{
    private float _damage;
    private float _brakePoint;
    private Rigidbody _rb;
    private bool _attackOneTime = true;
    private Animator _animator;
    public event Action OnDie;

    [Header("Hit zones data")]
    [SerializeField, Tooltip("the zone who the weapon make her attack")]
    private BoxCollider _hitZone;
    [SerializeField, Tooltip("the zone who take all damage when he is attacking")] 
    private BoxCollider[] _damageZones;

    [Header("Toggles weapon")]
    [SerializeField, Tooltip("if it can make any attack")]
    private bool _canAttack = true;
    [SerializeField, Tooltip("if the weapon is attacking every frame")] 
    private bool _alwayActive;
    [SerializeField, Tooltip("if it is attacking with the weapon")] 
    private bool _attacking = false;

    [Header("Data")]
    [SerializeField, Tooltip("all statistic for the weapon")] 
    private S_WeaponData _data;
    [SerializeField, Tooltip("the current of weapon, if it is OK it work else it not work")] 
    private State _state = State.ok;
    [SerializeField, Tooltip("the percent for set the state break on the weapon")]
    private int _lifeBrakePoint = 15;
    [SerializeField, Tooltip("the current health point")] 
    private float _life;

    /// <summary>
    /// Return <b>True</b> if it can make any attack and it state is OK
    /// </summary>
    public bool CanAttack
    {
        get => ((_canAttack && !_attacking) || _alwayActive || (_attacking && !CanTakeAnyDamage)) && _state == State.ok;
    }
    /// <summary>
    /// return <b>True</b> if the hit zone touch any object who has the iDamageable interface
    /// </summary>
    public bool CanTakeAnyDamage
    {
        get
        {
            // get all collider in hit zone
            Vector3 worldCenter = _hitZone.transform.TransformPoint(_hitZone.center);
            Vector3 worldHalfExtents = Vector3.Scale(_hitZone.size, _hitZone.transform.lossyScale) * 0.5f;
            var collide = Physics.OverlapBox(worldCenter, worldHalfExtents).Where(x => x.gameObject != this.gameObject).ToList();
            
            // sort all collide if it is a collider contain the idamageable interface
            var damagable = collide
                .Select(x => x.transform.gameObject)
                .Where(x => GetIDamageable(x, out _))
                .ToArray();

            return damagable.Length > 0;
        }
    }
    /// <summary>
    /// the zone who the weapon make her attack
    /// </summary>
    public BoxCollider HitZone
    {
        get => _hitZone;
    }
    /// <summary>
    /// if it is attacking with the weapon
    /// </summary>
    public bool Attacking
    {
        get => _attacking;
    }
    /// <summary>
    /// if the weapon is attacking every frame
    /// </summary>
    public bool AlwaysActive
    {
        get => _alwayActive;
    }
    /// <summary>
    /// the current of weapon, if it is OK it work else it not work
    /// </summary>
    public State CurrentState
    {
        get => _state;
    }

    public enum State
    {
        ok,
        broken,
        destroy
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _life = _data.MaxLife;
        _damage = _data.Damage;
        _brakePoint = (_lifeBrakePoint * _data.MaxLife) / 100;
        _alwayActive = _data.AlwayActive;
        _attackOneTime = _data.AttackOneTime;

        if (_alwayActive)
            _animator.SetBool("_playAttack", true);
    }
    private void Update()
    {
        if (_state != State.ok)
            return;

        foreach (var hitZone in _damageZones)
        {
            // get all collider in damageZone
            Vector3 worldCenter = hitZone.transform.TransformPoint(hitZone.center);
            Vector3 worldHalfExtents = Vector3.Scale(hitZone.size, hitZone.transform.lossyScale) * 0.5f;
            var collide = Physics.OverlapBox(worldCenter, worldHalfExtents, hitZone.transform.rotation);

            // sort the collider if he is not him self weapon
            var hitObject = collide
                .Select(x => x.gameObject)
                .Where(x => x != gameObject)
                .ToList();

            if (hitObject.Any())
            {
                foreach (var col in hitObject)
                {
                    if (!col)
                        continue;

                    bool succesAttack = AttackCollide(col);
                    if (succesAttack)
                        return;
                }
            }
        }
    }
    /// <summary>
    /// make an attack 
    /// </summary>
    /// <param name="collision">the object who take the damage</param>
    /// <returns>return True if it take damage</returns>
    private bool AttackCollide(GameObject collision)
    {
        // if the current weapon touch it
        if (IsCurrentBot(collision))
            return false;

        // if it always attack one time
        if (!_attackOneTime && _data.AttackOneTime)
            return false;

        if (_attacking || _alwayActive)
        {
            // try get the Idamageable interface
            if (GetIDamageable(collision, out var damageable))
            {
                bool succesToApplyDamage = TryApplyDamage(damageable);
                if (succesToApplyDamage)
                    return true;
            }
        }

        return false;
    }
    /// <summary>
    /// try to get the iDamageable interface in the entity and in the parent of the entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="iDamageable"></param>
    /// <returns>return True if it succes to get the idamageable interface</returns>
    private bool GetIDamageable(GameObject entity, out I_Damageable iDamageable)
    {
        if (entity == null)
        {
            iDamageable = null;
            return false;
        }

        // try to get iDamageable interface in the current entity
        if (entity.TryGetComponent<I_Damageable>(out var id))
        {
            iDamageable = id;
            return true;
        }

        // try to get iDamageable interface in all parent of entity
        var damageableParentComponent = entity.GetComponentInParent<I_Damageable>(true);
        if (damageableParentComponent != null)
        {
            iDamageable = damageableParentComponent;
            return true;
        }

        iDamageable = null;
        return false;
    }
    /// <summary>
    /// try to apply any damage in an object
    /// </summary>
    /// <param name="damagable">object to take any damage</param>
    /// <returns>return True if it succes to take damage</returns>
    private bool TryApplyDamage(I_Damageable damagable)
    {
        if (damagable != null)
        {
            // take damage with scale if it attack only one time scale is 1 else scale is the delta time
            var scaleDamage = _data.AttackOneTime ? 1f : Time.deltaTime;
            damagable.TakeDamage(_damage * scaleDamage);

            if (_data.AttackOneTime)
                _attackOneTime = false;

            return true;
        }

        return false;
    }
    /// <summary>
    /// detect if the target is the current bot
    /// </summary>
    /// <param name="targetCollide">object to verif</param>
    /// <returns>return True if it the current bot</returns>
    private bool IsCurrentBot(GameObject targetCollide)
    {
        // get the wheel controller in the current bot and in the target
        var currentBot = transform.GetComponentInParent<S_WheelsController>(true);
        var hitTarget = targetCollide.GetComponentInParent<S_WheelsController>(true);

        // if one of the wheel controller is null
        if (!hitTarget || !currentBot)
            return false;

        // if the two wheel controllers are the same return true
        if (currentBot.name.Equals(hitTarget.name))
            return true;

        return false;
    }
    /// <summary>
    /// apply damage on it self
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        // if it always destroy
        if (_state.Equals(State.destroy))
            return;

        _life -= amount;
        // if the life is down stop any attack, or if the life is lower or equal at 0 then die
        if (_life <= _brakePoint && _state == State.ok)
        {
            _state = State.broken;
            _animator.SetBool("_playAttack", false);
        }
        if (_life <= 0)
        {
            Die();
        }
    }
    /// <summary>
    /// methode start when this weapon is dead
    /// </summary>
    public void Die()
    {
        // detach the weapon on the bot
        transform.parent.gameObject.transform.parent = null;
        _state = State.destroy;
        _animator.SetBool("_playAttack", false);
        _animator.enabled = false;

        // add rigidbody for fall the weapon
        _rb = this.AddComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.angularDrag = 0f;
        _rb.drag = 2f;

        OnDie?.Invoke();
    }
    /// <summary>
    /// repear the current weapon
    /// </summary>
    public void Repear()
    {
        _life = _data.MaxLife;
        _state = State.ok;
    }
    /// <summary>
    /// Start the attack with cooldown
    /// </summary>
    public void LaunchAttack()
    {
        if (_alwayActive)
            return;

        // if it can attack
        if (CanAttack)
        {
            // set the attack animation at true
            _animator.SetBool("_playAttack", true);

            // make the attack cooldown
            AttackON();
            StartCoroutine(AttackOFF(() =>
            {
                StartCoroutine(AttackCooldown());
            }));
        }
    }
    /// <summary>
    /// Start the attack
    /// </summary>
    private void AttackON()
    {
        _attacking = true;
        _canAttack = false;
    }
    /// <summary>
    /// stop the attack after the attack time cooldown
    /// </summary>
    /// <param name="attackCooldown">launch when it stop attacking</param>
    private IEnumerator AttackOFF(System.Action attackCooldown)
    {
        yield return new WaitForSeconds(_data.AttackTime);
        _animator.SetBool("_playAttack", false);
        _attacking = false;
        _attackOneTime = _data.AttackOneTime;
        attackCooldown?.Invoke();
    }
    /// <summary>
    /// reset the can attack after the attack cooldown in the data
    /// </summary>
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_data.AttackCooldown);
        _canAttack = true;
    }
}
