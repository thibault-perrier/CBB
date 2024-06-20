using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_BotVFXController : MonoBehaviour
{
    private Rigidbody _rb;

    [Header("VFX Hit other bot")]
    [SerializeField] private GameObject _vfxHitRobot;
    [SerializeField] private float _velocityRequireForVfxHitRobot = 2f;
    [SerializeField] private UnityEvent _onHitRobot;

    private void Start()
    {
        _rb = GetComponentInParent<Rigidbody>(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject gHit = collision.gameObject;

        if (!VFXBotHitBot(collision, gHit))
            return;
    }

    private bool VFXBotHitBot(Collision collision, GameObject gHit)
    {
        if (gHit.TryGetComponent<S_WheelsController>(out _))
        {
            if (_rb.velocity.magnitude >= _velocityRequireForVfxHitRobot)
            {
                Instantiate(_vfxHitRobot, collision.contacts[0].point, Quaternion.identity);
                _onHitRobot?.Invoke();
                return true;
            }
        }

        return false;
    }
}
