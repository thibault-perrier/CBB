using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
    public GameObject _target;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag(_playerTag);
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
        _target.transform.position = target;
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
