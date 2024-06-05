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
    private BoxCollider _collider;

    [SerializeField] private bool _canAttack = true;
    [SerializeField] private bool _alwayActive;
    [SerializeField] private bool _attacking = false;
    [SerializeField] private BoxCollider _hitZone;
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
            var collide = Physics.OverlapBox(worldCenter, worldHalfExtents);
            var damagable = collide
                .Select(x => x.transform.gameObject)
                .Where(x => x.TryGetComponent<I_Damageable>(out _))
                .Where(x => x != this.gameObject)
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

    public enum State
    {
        ok,
        broken,
        destroy
    }

    private void Awake()
    {
        _rb = this.GetComponentInParent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider>();

        _rb.isKinematic = true;
        _rb.centerOfMass = -transform.forward + (-Vector3.up / 2f);
        
        _rb.mass = _data.Mass;
        _life = _data.MaxLife;
        _damage = _data.Damage;
        _brakePoint = (_lifeBrakePoint * _data.MaxLife) / 100;
        _alwayActive = _data.AlwayActive;

        if (_alwayActive)
            _animator.SetBool("_playAttack", true);
    }
    private void Update()
    {
        Vector3 worldCenter = _collider.transform.TransformPoint(_collider.center);
        Vector3 worldHalfExtents = Vector3.Scale(_collider.size, _collider.transform.lossyScale) * 0.5f;
        var collide = Physics.OverlapBox(worldCenter, worldHalfExtents, _collider.transform.rotation);
        
        if (collide != null)
        {
            var hitObject = collide.Select(x => x.gameObject).Where(x => x != gameObject).ToList();
            if (hitObject.Any())
            {
                foreach (var item in hitObject)
                {
                    AttackCollide(item);
                }
            }
        }
    }

    public void TakeDamage(float amount)
    {
        _life -= amount;
        if (_life <= _brakePoint && _state == State.ok)
        {
            _state = State.broken;
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
    }
    public void Repear()
    {
        _life = _data.MaxLife;
        _state = State.ok;
    }

    private void AttackCollide(GameObject collision)
    {
        if (!_attackOneTime)
            return;

        if (_attacking || _alwayActive)
        {
            I_Damageable iDamagable = collision.GetComponent<I_Damageable>();
            bool succes = TryApplyDamage(iDamagable);

            if (!succes)
            {
                iDamagable = collision.GetComponentInParent<I_Damageable>();
                TryApplyDamage(iDamagable);
            }
        }
    }
    private bool TryApplyDamage(I_Damageable damagable)
    {
        if (damagable != null)
        {
            var scaleDamage = AlwaysActive ? Time.deltaTime : 1f;
            damagable.TakeDamage(_damage * scaleDamage);

            if (_data.AttackTime.Equals(0f))
                _attackOneTime = false;

            return true;
        }

        return false;
    }

    public void LaunchAttack()
    {
        if (_alwayActive)
            return;

        if (_state == State.ok && !_attacking)
        {
            _animator.SetBool("_playAttack", true);
            AttackON();
            StartCoroutine(AttackOFF());
            StartCoroutine(AttackCooldown());
        }
    }
    private void AttackON()
    {
        _attacking = true;
    }
    private IEnumerator AttackOFF()
    {
        yield return new WaitForSeconds(_data.AttackTime);
        // _animator.SetBool("_playAttack", false);
        _attacking = false;
        _attackOneTime = true;
    }
    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_data.AttackCooldown);
        _canAttack = true;
    }
}
