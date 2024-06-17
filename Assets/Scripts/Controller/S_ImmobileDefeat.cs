using System;
using UnityEngine;

public class S_ImmobileDefeat : MonoBehaviour
{
    [SerializeField, Tooltip("the velocity who need for be immobile")]
    private float _immobileVelocity;
    [SerializeField, Tooltip("the time requirement for be immobile")]
    private float _immobileTimeRequirement;

    private float _immobileCurrentTime;
    private Rigidbody _rigidbody;

    public event Action IsImmobile;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float straight = Vector3.Dot(transform.up, Vector3.up);

        if (_rigidbody.velocity.magnitude <= _immobileVelocity && straight < .2f)
        {
            _immobileCurrentTime += Time.deltaTime;

            if (_immobileCurrentTime >= _immobileTimeRequirement)
                IsImmobile?.Invoke();

            return;
        }

        _immobileCurrentTime = 0f;

    }
}
