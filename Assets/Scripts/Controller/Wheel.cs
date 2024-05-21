using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private bool _accelerate;
    [SerializeField] private WheelCollider _wheelCollider;
    [SerializeField] private float _torque;

    public bool Accelerate
    {
        set { _accelerate = value; }
    }

    public float Torque
    {
        set { _torque = value; }
    }

    private void FixedUpdate()
    {
        if (_accelerate)
        {
            _wheelCollider.motorTorque = _torque;
            _wheelCollider.brakeTorque = 0;
        }
        else
        {
            _wheelCollider.motorTorque = 0;
            _wheelCollider.brakeTorque = 10;
        }
    }
}
