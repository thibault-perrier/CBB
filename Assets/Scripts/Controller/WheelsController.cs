using System.Collections.Generic;
using UnityEngine;

public class WheelsController : MonoBehaviour
{
    [SerializeField] private List<WheelCollider> _wheels;
    [SerializeField] private List<WheelCollider> _directionWheelsCollider;
    [SerializeField] private float _maxAngle = 30;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxMotorTorque;

    private float _direction;
    private Move _move = Move.neutral;
    private Rigidbody _rb;
    private float _mass;

    private float _wheelRadius;     // in meter
    [SerializeField] private float _timeToMaxSpeed = 5f;    // in second


    public enum Move
    {
        toward,
        neutral,
        backward
    }

    /// <summary>
    /// Delta value internvale [1 , -1], 1 for right and -1 for left;
    /// </summary>
    public float Direction
    {
        set { _direction = value; }
    }

    /// <summary>
    /// defined direction move between toward, backward and neutral.
    /// </summary>
    public Move Movement
    {
        set { this._move = value; }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _mass = _rb.mass + 4 * _wheels[0].mass;
        _wheelRadius = _wheels[0].radius;
    }

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.W))
            _move = Move.toward;
        if (Input.GetKey(KeyCode.S))
            _move = Move.backward;

        // _direction = 0;

        if (Input.GetKey(KeyCode.A))
        {
            _direction = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _direction = 1;
        }
        

        DirectionUpdate();
        ApplyMotorTorque();

        _move = Move.neutral;
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

        foreach (WheelCollider wheel in _wheels)
        {
            switch (_move)
            {
                case Move.toward:
                    wheel.brakeTorque = 0;
                    wheel.motorTorque = motorTorque;
                    break;
                case Move.backward:
                    wheel.brakeTorque = 0;
                    wheel.motorTorque = -motorTorque;
                    break;
                default:
                    wheel.motorTorque = 0;
                    wheel.brakeTorque = 20;
                    break;
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
