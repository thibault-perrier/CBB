using Unity.VisualScripting;
using UnityEngine;

public class S_WeaponManager : MonoBehaviour, I_Damageable
{
    private int _life;
    private int _damage;
    private float _brakePoint;
    private Rigidbody _rb;
    [SerializeField] private bool _attacking = false;

    [SerializeField] private S_WeaponData _data;
    [SerializeField] private State _state = State.ok;
    [SerializeField] private int _lifeBrakePoint = 15;    //In pourcent life level
    [SerializeField] private bool _shotWeapon = false;
    [SerializeField] private bool _active;
    [SerializeField] private Animator _animator;

    public enum State
    {
        ok,
        broken,
        destroy
    }

    private void Awake()
    {
        _rb = this.AddComponent<Rigidbody>();
        _rb.isKinematic = true;
        _animator = GetComponent<Animator>();
        
        _animator.updateMode = AnimatorUpdateMode.AnimatePhysics;       //TO DO: check
        
        _rb.mass = _data.Mass;
        _life = _data.MaxLife;
        _damage = _data.Damage;
        _brakePoint = (_lifeBrakePoint * _data.MaxLife) / 100;
        _active = _data.Active;
        _attacking = !_active;
        if (_active)
        {
            _animator.SetBool("_playAttack", _attacking);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void TakeDamage(int amount)
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
        transform.parent = null;
        _rb.isKinematic = false;
        _state = State.destroy;
    }

    public void Repair()
    {
        _life = _data.MaxLife;
        _state = State.ok;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(_attacking && collision.gameObject.TryGetComponent(out I_Damageable damageable))
        {
            damageable.TakeDamage(_damage);
        }
    }

    public void LaunchAttack()
    {
        if (_state == State.ok && !_attacking)
        {
            _animator.SetBool("_playAttack", true);
            AttackON();
        }
    }

    private void AttackON()
    {
        _attacking = true;
    }

    private void AttackOFF()
    {
        _attacking = false;
        _animator.SetBool("_playAttack", false);
    }

}
