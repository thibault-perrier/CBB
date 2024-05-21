using System.Collections;
using System.Collections.Generic;
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



    private float _direction;
    private bool _accelerate;
    private Rigidbody rb;

    /// <summary>
    /// Delta value internvale [1 , -1], 1 for right and -1 for left;
    /// </summary>
    public float Direction
    {
        set { _direction = value; }
    }
    public bool Accelerate
    {
        set { this._accelerate = value; }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
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

        //Torque and speed

        float currentSpeed = rb.velocity.magnitude;

        Debug.Log("Speed " + currentSpeed);

        //clamp torque
        if (currentSpeed < _maxSpeed)
        {
            float motorTorque = _minMotorTorque + (_maxMotorTorque - _minMotorTorque);
            ApplyMotorTorque(motorTorque);
        }
        else
        {
            ApplyMotorTorque(-(currentSpeed - _maxSpeed) * 10);
        }
    }

    /// <summary>
    /// Apply torque on all wheels
    /// </summary>
    /// <param name="torque">Torque value to set</param>
    void ApplyMotorTorque(float torque)
    {
        foreach (Wheel wheel in _wheels)
        {
            wheel.Torque = torque;
            wheel.Accelerate = this._accelerate;
        }
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
