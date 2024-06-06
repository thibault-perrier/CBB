using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class S_WeaponManager : MonoBehaviour, I_Damageable
{
    [SerializeField] private float _life;
    private float _damage;
    private float _brakePoint;
    private Rigidbody _rb;
    private bool _attackOneTime = true;

    [SerializeField] private BoxCollider _hitZone;
    [SerializeField] private BoxCollider _damageZone;
    [SerializeField] private bool _canAttack = true;
    [SerializeField] private bool _alwayActive;
    [SerializeField] private bool _attacking = false;
    [SerializeField] private S_WeaponData _data;
    [SerializeField] private State _state = State.ok;
    [SerializeField] private int _lifeBrakePoint = 15;    //In pourcent life level
    [SerializeField] private Animator _animator;

    public bool CanAttack
    {
        get => ((_canAttack && !_attacking) || _alwayActive) && _state == State.ok;
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
        _rb = this.GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _rb.isKinematic = true;
        _rb.centerOfMass = -transform.forward + (-Vector3.up / 2f);
        
        _rb.mass = _data.Mass;
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
        Vector3 worldCenter = _damageZone.transform.TransformPoint(_damageZone.center);
        Vector3 worldHalfExtents = Vector3.Scale(_damageZone.size, _damageZone.transform.lossyScale) * 0.5f;
        var collide = Physics.OverlapBox(worldCenter, worldHalfExtents, _damageZone.transform.rotation);
        
        var hitObject = collide.Select(x => x.gameObject).Where(x => x != gameObject).ToList();
        if (hitObject.Any())
        {
            foreach (var item in hitObject)
            {
                AttackCollide(item);
            }
        }
    }
    
    private void AttackCollide(GameObject collision)
    {
        if (!_attackOneTime && _data.AttackOneTime)
            return;

        if (_attacking || _alwayActive)
        {
            if (GetIDamageable(collision, out var damageable))
                TryApplyDamage(damageable);
        }
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

    public void TakeDamage(float amount)
    {
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
        _rb.isKinematic = false;
        _state = State.destroy;
        _animator.SetBool("_playAttack", false);

        if (_damageZone.isTrigger)
            _damageZone.isTrigger = false;
    }
    public void Repear()
    {
        _life = _data.MaxLife;
        _state = State.ok;
    }

    public void LaunchAttack()
    {
        if (_alwayActive)
            return;

        if (_state == State.ok && !_attacking)
        {
            _animator.SetBool("_playAttack", true);
            AttackON();
            StartCoroutine(AttackCooldown());
            StartCoroutine(AttackOFF());
        }
    }
    private void AttackON()
    {
        _attacking = true;
    }
    private IEnumerator AttackOFF()
    {
        yield return new WaitForSeconds(_data.AttackTime);
        _animator.SetBool("_playAttack", false);
        _attacking = false;
        _attackOneTime = _data.AttackOneTime;
    }
    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_data.AttackCooldown);
        _canAttack = true;
    }
}
