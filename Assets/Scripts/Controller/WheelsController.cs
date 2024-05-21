using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WheelsController : MonoBehaviour
{
    [SerializeField] private List<Wheel> _wheels;
    [SerializeField] private List<WheelCollider> _directionWheels;
    [SerializeField] private float _maxAngle = 30;
    [SerializeField] private List<Transform> _directionWheelsTransforms;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxMotorTorque;
    [SerializeField] private float _minMotorTorque;

    public enum Move
    {
        toward,
        neutral,
        backward
    }

    private float _direction;
    private Move _move = Move.neutral;
    private Rigidbody rb;

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
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _move = Move.neutral;
        if(Input.GetKey(KeyCode.W))
            _move = Move.toward;
        if (Input.GetKey(KeyCode.S))
            _move = Move.backward;

        //Direction
        _direction = 0;

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


    }

    /// <summary>
    /// Apply torque on all wheels
    /// </summary>
    void ApplyMotorTorque()
    {
        //Torque and speed

        float currentSpeed = rb.velocity.magnitude;

        float motorTorque = _minMotorTorque + (_maxMotorTorque - _minMotorTorque);

        Debug.Log("Speed " + currentSpeed);

        if (currentSpeed > _maxSpeed)
        {
            motorTorque = (currentSpeed - _maxSpeed) * -30;
        }

        foreach (Wheel wheel in _wheels)
        {
            switch (_move)
            {
                case Move.toward:

                    wheel.Torque = motorTorque;
                    break;
                case Move.backward:
                    wheel.Torque = -motorTorque;
                    break;
                default:
                    break;
            }
            
            wheel.Accelerate = _move != Move.neutral;
        }

        //clamp torque

        

        
    }

    /// <summary>
    /// Apply Direction on the directinal wheels
    /// </summary>
    private void DirectionUpdate()
    {
        foreach(WheelCollider wheel in _directionWheels)
        {
            wheel.steerAngle = _maxAngle * _direction;
        }
    }


}
