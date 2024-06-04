
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class S_BrustBotController : MonoBehaviour
{
    [Header("Bot Imobility")]
    [SerializeField, Min(0f), Tooltip("the velocity requirement for he is immobile")]
    private float _requirementVelocityImmobile = 0.01f;
    [SerializeField, Min(1f), Tooltip("the time requirement for he is immobile")]
    private float _requirementTimeImmobile = 5f;

    [Header("Physic")]
    [SerializeField, Min(0f), Tooltip("the force used for apply physic for realse the wheels")]
    private float _realeaseWheelForce = 1f;

    private float _currentImmobileTime;
    private Rigidbody _rb;
    private WheelCollider[] _wheelColliders;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _wheelColliders = GetComponentsInChildren<WheelCollider>(true);
    }
    private void Update()
    {
        bool isImmobile = ImmobilityCalcul();

        if (isImmobile)
        {
            var wheelPairs = GetAllWheelPairs();

            if (wheelPairs.Any())
            {
                wheelPairs = GetBlockedWheels(wheelPairs);
                ApplyPhysicsBot(wheelPairs);
            }
        }
    }

    /// <summary>
    /// calcul if the current bot is immobile
    /// </summary>
    /// <returns>Return <b>True</b> if he is immobile else return <b>False</b></returns>
    private bool ImmobilityCalcul()
    {
        // if he is in movement
        if (_rb.velocity.magnitude > _requirementVelocityImmobile)
        {
            _currentImmobileTime = 0f;
            return false;
        }

        // if he is immobile a long time
        _currentImmobileTime += Time.deltaTime;
        if (_currentImmobileTime >= _requirementTimeImmobile)
            return true;

        return false;
    }
    /// <summary>
    /// Get all pair of wheel touching
    /// </summary>
    /// <returns>return the pair of wheel item1 is current wheel and item2 is enemy wheel</returns>
    private List<(WheelCollider, WheelCollider)> GetAllWheelPairs()
    {
        List<(WheelCollider, WheelCollider)> pair = new();

        foreach (var currentWheel in _wheelColliders)
        {
            // get all collider arount the current wheen and sort with the object has WheelCollider
            var hitObjects = Physics.OverlapSphere(currentWheel.transform.position, currentWheel.radius)
                .Select(x => x.gameObject)
                .Where(x => x.TryGetComponent<WheelCollider>(out _) && !_wheelColliders.Contains(x.GetComponent<WheelCollider>()))
                .ToList();

            // if there is any alement
            if (hitObjects.Any())
            {
                WheelCollider wheelHit = new();
                // if we hit multiples target
                if (hitObjects.Count() > 1)
                {
                    // find the nearest wheel
                    var nearWheelObject = hitObjects
                        .OrderBy(x => Vector3.Distance(x.transform.position, currentWheel.transform.position))
                        .ToList()[0];

                    wheelHit = nearWheelObject.GetComponent<WheelCollider>();
                }
                else
                {
                    wheelHit = hitObjects[0].GetComponent<WheelCollider>();
                }

                // add the pair with the current wheel and the wheel who was hit by the current wheel
                pair.Add((currentWheel, wheelHit));
            }
        }

        return pair;
    }
    /// <summary>
    /// get all wheel who have blocked
    /// </summary>
    /// <param name="wheelPair">all pair of wheel who have collide</param>
    /// <returns>return the wheel who have blocked</returns>
    private List<(WheelCollider, WheelCollider)> GetBlockedWheels(List<(WheelCollider, WheelCollider)> wheelPair)
    {
        List<(WheelCollider, WheelCollider)> blockedPair = new();

        foreach (var pair in wheelPair)
        {
            // calcul the distance for current wheel and enemy wheel
            float distanceToCurrentWheel = (pair.Item1.transform.position - transform.position).magnitude;
            float distanceToEnemyWheel   = (pair.Item2.transform.position - transform.position).magnitude;
        
            // if the enemy wheel is more nearer than the current wheel so add the pair
            if (distanceToCurrentWheel > distanceToEnemyWheel) 
                blockedPair.Add((pair.Item1, pair.Item2));
        }

        return blockedPair;
    }
    /// <summary>
    /// Apply the physics for realase the wheel
    /// </summary>
    /// <param name="wheelBlockPair">each pair of wheel who are block with other wheel</param>
    private void ApplyPhysicsBot(List<(WheelCollider, WheelCollider)> wheelBlockPair)
    {
        // if the wheel pair is empty so go back
        if (!wheelBlockPair.Any())
            return;

        Vector3 resultRealseForce = Vector3.zero;
        wheelBlockPair.ForEach(pair =>
        {
            // increment the resultRealase force for each pair of wheel
            Vector3 dirToPairWheel = (pair.Item1.transform.position - pair.Item2.transform.position).normalized;
            resultRealseForce += dirToPairWheel * _realeaseWheelForce;
        });

        Debug.Log(resultRealseForce);
        // apply for for each wheel who are blcok
        _rb.AddForce(resultRealseForce, ForceMode.Impulse);
    }
}
