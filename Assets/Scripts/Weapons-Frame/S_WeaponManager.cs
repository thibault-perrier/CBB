using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private bool _isTrap = false;

    [Header("Data")]
    [SerializeField] private S_WeaponData _data;
    [SerializeField] private State _state = State.ok;
    [SerializeField] private int _lifeBrakePoint = 15;    //In pourcent life level
    public float _life;

    [Header("Event")]
    [SerializeField, Tooltip("call when he start Attack")]
    private UnityEvent _attackingStart;
    [SerializeField, Tooltip("call when he stop attack after attacking time")]
    private UnityEvent _attackingEnd;
    [SerializeField, Tooltip("call when he take any damage")]
    private UnityEvent _onTakeDamage;
    [SerializeField, Tooltip("call when the weapon is broken")]
    private UnityEvent _onWeaponUnuseable;
    [SerializeField, Tooltip("Call when the weapon is destroy and detached to bot")]
    private UnityEvent _onWeaponDestroy;
    [SerializeField, Tooltip("call when he begin to tuch a target who can take damage")]
    private UnityEvent _onBeginTouchTarget;
    [SerializeField, Tooltip("call when he stop to tuch an target who can take any damage")]
    private UnityEvent _onEndTouchTarget;

    private GameObject _vfxSmoke;
    private bool _tuchDamageable = false;
    private bool _touchEvent = true;
    private bool _canLaunchVfx = true;

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
                .Where(x => CanRecieveDamage(x))
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
    public UnityEvent AttackingEnd
    {
        get => _attackingEnd;
    }
    public UnityEvent WeaponDestroy
    {
        get => _onWeaponDestroy;
    }
    public UnityEvent AttackingStart
    {
        get => _attackingStart;
    }
    public UnityEvent WeaponUnuseable
    {
        get => _onWeaponUnuseable;
    }
    private float CooldownVfx
    {
        get
        {
            GameObject vfx = _data.VfxHitContact;
            if (!vfx)
                return 0f;

            ParticleSystem particule = vfx.GetComponent<ParticleSystem>();

            if (!particule)
                return 0f;

            return particule.duration / 5f;
        }
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
        _animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

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
        if ((!_attacking && !_alwayActive) || _state != State.ok)
            return;

        List<GameObject> ignoreObjects = new() { gameObject };

        foreach (var hitZone in _damageZones)
        {
            // get all collider in damageZone
            Vector3 worldCenter = hitZone.transform.TransformPoint(hitZone.center);
            Vector3 worldHalfExtents = Vector3.Scale(hitZone.size, hitZone.transform.lossyScale) * 0.5f;
            var collide = Physics.OverlapBox(worldCenter, worldHalfExtents, hitZone.transform.rotation);

            // sort the collider if he is not him self weapon or if he not hit the him self bot
            var hitObject = collide
                .Where(x => !ignoreObjects.Contains(x.gameObject))
                .ToList();

            if (hitObject.Any())
            {
                foreach (var col in hitObject.Select(x => x.gameObject))
                {
                    if (!col)
                        continue;

                    if (!_isTrap)
                    {
                        if (IsCurrentBot(col))
                            continue;

                        MakeAttackEvent(hitObject, col);
                    }
                    else
                    {
                        MakeAttackEvent(hitObject, col);
                    }
                }
            }
        }
    }

    private void MakeAttackEvent(List<Collider> hitObject, GameObject col)
    {
        bool succesAttack = AttackCollide(col);

        if (!succesAttack && _tuchDamageable)
        {
            if (_touchEvent)
            {
                _onEndTouchTarget?.Invoke();
                _tuchDamageable = false;
                _touchEvent = false;
            }
        }

        if (succesAttack)
        {
            if (!_tuchDamageable)
            {
                _onBeginTouchTarget?.Invoke();
            }

            StartCoroutine(Delay(.5f, () =>
            {
                _tuchDamageable = true;
                _touchEvent = true;
            }));

            if (_canLaunchVfx)
            {
                InstanceVFX(hitObject[0]);
                _canLaunchVfx = false;

                StartCoroutine(Delay(CooldownVfx, () =>
                {
                    _canLaunchVfx = true;
                }));
            }

            return;
        }
    }

    private void InstanceVFX(Collider hitObject)
    {
        if (!_data.VfxHitContact)
            return;

        if (hitObject.gameObject != gameObject)
        {
            Vector3 vfxPosition = _damageZones[0].transform.TransformPoint(_damageZones[0].center);
            var hitVfx = Instantiate(_data.VfxHitContact, vfxPosition, Quaternion.identity) as GameObject;
            hitVfx.transform.localScale = _data.ScaleVfxHitContact;
        }
    }
    private bool AttackCollide(GameObject collision)
    {
        if (!_attackOneTime && _data.AttackOneTime)
            return false;

        if (_attacking || _alwayActive)
        {
            if (GetIDamageable(collision, out var damageable))
            {
                if (!damageable.CanRecieveDamage())
                    return false;

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
            if (!damagable.CanRecieveDamage())
                return false;

            var scaleDamage = _data.AttackOneTime ? 1f : Time.deltaTime;
            damagable.TakeDamage(_damage * scaleDamage);
            _onTakeDamage?.Invoke();

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

        if (currentBot.tag.Equals(hitTarget.tag))
            return true;

        return false;
    }

    public void TakeDamage(float amount)
    {
        if (_state.Equals(State.destroy) || _isTrap)
            return;

        _life -= amount;
        if (_life <= _brakePoint && _state == State.ok)
        {
            _state = State.broken;
            _vfxSmoke = Instantiate(_data.VfxLowUp, transform.position, Quaternion.identity, transform.parent);
            _animator.SetBool("_playAttack", false);
            _onWeaponUnuseable?.Invoke();
        }
        if (_life <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(_vfxSmoke);
        Instantiate(_data.VfxDestroy, transform.position, Quaternion.identity);
        _onWeaponDestroy?.Invoke();
        DetachWeapon();
    }

    private void RemoveWeaponInInventory()
    {
        var player = transform.GetComponentInParent<S_PlayerController>(true);
        if (player)
        {
            if (player.enabled)
            {
                if (S_DataGame.Instance)
                    S_DataGame.Instance.inventory.RemoveWeapon(_data);
            }
        }
    }

    public void DetachWeapon()
    {
        transform.parent.gameObject.transform.parent = null;
        _state = State.destroy;
        _animator.SetBool("_playAttack", false);
        _animator.enabled = false;
        _onWeaponUnuseable?.Invoke();

        _rb = this.AddComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.angularDrag = 0f;
        _rb.drag = 2f;
        _rb.AddTorque(Vector3.one);
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

        if (_canAttack && _state == State.ok)
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
        _attackingStart?.Invoke();
    }
    private IEnumerator AttackOFF(System.Action attackCooldown)
    {
        yield return new WaitForSeconds(_data.AttackTime);
        _animator.SetBool("_playAttack", false);
        _attacking = false;
        _attackOneTime = _data.AttackOneTime;
        attackCooldown?.Invoke();
        _attackingEnd?.Invoke();
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_data.AttackCooldown);
        _canAttack = true;
    }
    private IEnumerator Delay(float time, System.Action callBack)
    {
        yield return new WaitForSeconds(time);
        callBack?.Invoke();
    }

    public bool CanRecieveDamage()
    {
        if (_isTrap)
            return false;

        if (_state == State.destroy)
            return false;

        return true;
    }
    public bool CanRecieveDamage(GameObject hit)
    {
        if (!hit)
            return false;

        if (GetIDamageable(hit, out var component))
            return component.CanRecieveDamage();

        return false;
    }

    public int GetRepairPrice()
    {
        int price = 0;
        price += (int)(((_data.Cost * 0.8) / _data.MaxLife) * (_data.MaxLife - _life));
        return price;
    }

}
