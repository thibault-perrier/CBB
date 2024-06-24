using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_WheelsController : MonoBehaviour
{
    [SerializeField] private List<WheelCollider> _wheels;
    [SerializeField] private List<WheelCollider> _directionWheelsCollider;
    [SerializeField] private float _maxAngle = 30;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxMotorTorque;

    private float _direction;
    private float _move;
    private Rigidbody _rb;
    private float _mass;

    private PlayerInput _playerInput;

    private float _wheelRadius;     // in meter
    [SerializeField] private float _timeToMaxSpeed = 5f;    // in second

    /// <summary>
    /// Delta value internvale [1 , -1], 1 for right and -1 for left;
    /// </summary>
    public float Direction
    {
        set { _direction = value; }
        get { return _direction; }
    }

    /// <summary>
    /// defined direction move between toward, backward and neutral.
    /// </summary>
    public float Movement
    {
        set { this._move = value; }
        get { return this._move; }
    }

    public Rigidbody RigidBody
    {
        get => _rb;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //_rb.centerOfMass = -Vector3.up * 3f;

        _mass = _rb.mass + 4 * _wheels[0].mass;
        _wheelRadius = _wheels[0].radius;
        _playerInput = new PlayerInput();
    }
    
    private void FixedUpdate()
    {
        DirectionUpdate();
        ApplyMotorTorque();
    }

    /// <summary>
    /// Apply torque on all wheels
    /// </summary>
    void ApplyMotorTorque()
    {
        float currentSpeed = _rb.velocity.magnitude;

        float desiredAcceleration = (_maxSpeed - currentSpeed) / _timeToMaxSpeed;
        float requiredForce = _mass * desiredAcceleration;
        float motorTorque = requiredForce * _wheelRadius;

        // limit max torque
        motorTorque = Mathf.Clamp(motorTorque, -_maxMotorTorque, _maxMotorTorque);
        Debug.Log(_move);
        foreach (WheelCollider wheel in _wheels)
        {
            if(Mathf.Abs(_move) >= 0.1f)
            {
                wheel.brakeTorque = 0;
                wheel.motorTorque = motorTorque * _move;
            }
            else
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = 20;
            }
        }
    }

    /// <summary>
    /// Apply Direction on the directinal wheels
    /// </summary>
    private void DirectionUpdate()
    {
        foreach(WheelCollider wheel in _directionWheelsCollider)
        {
            wheel.steerAngle = _maxAngle * _direction;
        }
    }


}
