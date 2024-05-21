using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class S_AIController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField, Tooltip("Tag for find player")] 
    private string _playerTag;

    [Header("Agents")]
    [SerializeField, Tooltip("component for create path from player")]
    private NavMeshAgent _agents;

    [Header("Togles Actions")]
    [SerializeField, Tooltip("if he focus the plaeyr weapon")]
    private bool _attackPlayerWeapon;
    [SerializeField, Tooltip("if he dont touch the traps")]
    private bool _dodgeTrap;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100)] private float _attackProbability = 100f;

    private GameObject _player;

    private WheelsController _wheelsController;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag(_playerTag);
        _wheelsController = GetComponent<WheelsController>();
    }
    private void FixedUpdate()
    {
        UpdateAIMovement();
    }

    /// <summary>
    /// update the target for AI movement
    /// </summary>
    private void UpdateAIMovement()
    {
        Vector3 position = new();
        bool succes = GetFirstPathPosition(ref position);
        if (succes)
        {
            MoveAI(position);
        }
        else
        {
            Vector3 playerDirection = (_player.transform.position - transform.position).normalized;
            MoveAI(transform.position + playerDirection * 2f);
        }
    }
    /// <summary>
    /// set wheel velocity from the target
    /// </summary>
    /// <param name="target"></param>
    private void MoveAI(Vector3 target)
    {
        float forwardAmout = 0f;
        float TurnAmount = 0f;

        Vector3 dir = (target - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);
        float angleToDir = Vector3.SignedAngle(transform.forward, dir, Vector3.up);


        if (dot > 0f)
        {
            // go forward
            forwardAmout = 1f;
        }
        else
        {
            // go backward
            forwardAmout = -1f;
        }

        if (angleToDir > 0f)
        {
            // turn right
            TurnAmount = 1f;
        }
        else
        {
            // turn left
            TurnAmount = -1f;
        }
        Debug.Log(forwardAmout);
        // _wheelsController.Direction = TurnAmount;
        if (forwardAmout > 0f)
        {
            _wheelsController.Movement = WheelsController.Move.toward;
        }
        else
        {
            _wheelsController.Movement = WheelsController.Move.backward;
        }
    }
    /// <summary>
    /// get the position to the seconds point for a path from player
    /// </summary>
    /// <returns>return the secondes point of path</returns>
    private bool GetFirstPathPosition(ref Vector3 result)
    {
        if (_agents == null)
        {
            result = Vector3.zero;
            return false;
        }

        // create and calcute path
        var path = new NavMeshPath();
        _agents.CalculatePath(_player.transform.position, path);

        // return the seconds point of path
        if (path.corners.Length > 1)
        {
            result = path.corners[1];
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
