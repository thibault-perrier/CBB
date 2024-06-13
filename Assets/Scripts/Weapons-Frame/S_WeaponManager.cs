using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class S_WeaponManager : MonoBehaviour, I_Damageable
{
    private float _damage;
    private float _brakePoint;
    private Rigidbody _rb;
    private bool _attackOneTime = true;
    private Animator _animator;

    [Header("Hit zones data")]
    [SerializeField] private BoxCollider _hitZone;
    [SerializeField] private BoxCollider[] _damageZones;

    [Header("Toggles weapon")]
    [SerializeField] private bool _canAttack = true;
    [SerializeField] private bool _alwayActive;
    [SerializeField] private bool _attacking = false;

    [Header("Data")]
    [SerializeField] private S_WeaponData _data;
    [SerializeField] private State _state = State.ok;
    [SerializeField] private int _lifeBrakePoint = 15;    //In pourcent life level
    [SerializeField] private float _life;

    public S_WeaponData Data
    {
        get => _data;
    }
    public bool CanAttack
    {
        get => ((_canAttack && !_attacking) || _alwayActive || (_attacking && !CanTakeAnyDamage)) && _state == State.ok;
    }
    public bool CanTakeAnyDamage
    {
        get
        {
            Vector3 worldCenter = _hitZone.transform.TransformPoint(_hitZone.center);
            Vector3 worldHalfExtents = Vector3.Scale(_hitZone.size, _hitZone.transform.lossyScale) * 0.5f;
            var collide = Physics.OverlapBox(worldCenter, worldHalfExtents).Where(x => x.gameObject != this.gameObject).ToList();
            var damagable = collide
                .Select(x => x.transform.gameObject)
                .Where(x => GetIDamageable(x, out _))
                .ToArray();

            return damagable.Length > 0;
        }
    }
    public BoxCollider HitZone
    {
        get => _hitZone;
    }
    public bool Attacking
    {
        get => _attacking;
    }
    public bool AlwaysActive
    {
        get => _alwayActive;
    }
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
        if (!_attacking || _state != State.ok)
            return;

        foreach (var hitZone in _damageZones)
        {
            // get all collider in damageZone
            Vector3 worldCenter = hitZone.transform.TransformPoint(hitZone.center);
            Vector3 worldHalfExtents = Vector3.Scale(hitZone.size, hitZone.transform.lossyScale) * 0.5f;
            var collide = Physics.OverlapBox(worldCenter, worldHalfExtents, hitZone.transform.rotation);

            // sort the collider if he is not him self weapon or if he not hit the him self bot
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
    
    private bool AttackCollide(GameObject collision)
    {
        if (IsCurrentBot(collision))
            return false;

        if (!_attackOneTime && _data.AttackOneTime)
            return false;

        if (_attacking || _alwayActive)
        {
            if (GetIDamageable(collision, out var damageable))
            {
                bool succesToApplyDamage = TryApplyDamage(damageable);
                if (succesToApplyDamage)
                    return true;
            }
        }

        return false;
    }
    private bool GetIDamageable(GameObject entity, out I_Damageable iDamageable)
    {
        if (entity == null)
        {
            iDamageable = null;
            return false;
        }

        if (entity.TryGetComponent<I_Damageable>(out var id))
        {
            iDamageable = id;
            return true;
        }

        var damageableParentComponent = entity.GetComponentInParent<I_Damageable>(true);
        if (damageableParentComponent != null)
        {
            iDamageable = damageableParentComponent;
            return true;
        }

        iDamageable = null;
        return false;
    }
    private bool TryApplyDamage(I_Damageable damagable)
    {
        if (damagable != null)
        {
            var scaleDamage = _data.AttackOneTime ? 1f : Time.deltaTime;
            damagable.TakeDamage(_damage * scaleDamage);

            if (_data.AttackOneTime)
                _attackOneTime = false;

            return true;
        }

        return false;
    }
    private bool IsCurrentBot(GameObject targetCollide)
    {
        var currentBot = transform.GetComponentInParent<S_WheelsController>(true);
        var hitTarget = targetCollide.GetComponentInParent<S_WheelsController>(true);

        if (!hitTarget)
            return false;

        if (currentBot.name.Equals(hitTarget.name))
            return true;

        return false;
    }

    public void TakeDamage(float amount)
    {
        if (_state.Equals(State.destroy))
            return;

        _life -= amount;
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
    public void Die()
    {
        transform.parent.gameObject.transform.parent = null;
        _state = State.destroy;
        _animator.SetBool("_playAttack", false);
        _animator.enabled = false;

        _rb = this.AddComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.angularDrag = 0f;
        _rb.drag = 2f;
    }
    public void Repair()
    {
        _life = _data.MaxLife;
        _state = State.ok;
    }

    public void LaunchAttack()
    {
        if (_alwayActive)
            return;

        if (CanAttack)
        {
            _animator.SetBool("_playAttack", true);
            AttackON();
            StartCoroutine(AttackOFF(() =>
            {
                StartCoroutine(AttackCooldown());
            }));
        }
    }
    private void AttackON()
    {
        _attacking = true;
        _canAttack = false;
    }
    private IEnumerator AttackOFF(System.Action attackCooldown)
    {
        yield return new WaitForSeconds(_data.AttackTime);
        _animator.SetBool("_playAttack", false);
        _attacking = false;
        _attackOneTime = _data.AttackOneTime;
        attackCooldown?.Invoke();
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_data.AttackCooldown);
        _canAttack = true;
    }
}
