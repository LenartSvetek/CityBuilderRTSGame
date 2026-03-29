using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class PawnMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Pawn pawn;
    private bool _isMoving = false;
    
    Action<bool> callback;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        pawn = GetComponent<Pawn>();
        agent.speed = pawn.data.moveSpeed;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    private void LateUpdate()
    {
        if (_isMoving && IsAgentFinished())
        {
            _isMoving = false;
            OnDestinationReached();
        }
    }

    public void MoveTo(Vector3 position, [CanBeNull] Action<bool> callback)
    {
        _isMoving = true;
        agent.ResetPath();
        agent.SetDestination(position);
        this.callback = callback;
    }
    
    bool IsAgentFinished()
    {
        // 1. Is the agent still thinking?
        if (agent.pathPending) 
            return false;

        // 2. Is the agent close enough to the target?
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // 3. Does it have a path at all, or is it stationary?
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }
    
    void OnDestinationReached()
    {
        Debug.Log("OnDestinationReached");
        if (callback != null)
        {
            Debug.Log("Callback called");
            callback.Invoke(true);
            callback = null;
        }
    }
}
