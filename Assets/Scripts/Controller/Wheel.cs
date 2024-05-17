using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private bool _accelerate;
    [SerializeField] private WheelCollider _wheelCollider;
    [SerializeField] private float _speed;

    private void Update()
    {
        if (_accelerate)
        {
            _wheelCollider.motorTorque = 30;
        }
        else
        {
            _wheelCollider.brakeTorque = 0;
        }
    }
}
