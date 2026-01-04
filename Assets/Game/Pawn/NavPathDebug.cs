using UnityEngine;
using UnityEngine.AI;

public class NavPathDebug : MonoBehaviour
{
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnDrawGizmos()
    {
        if (agent == null || !agent.hasPath) return;

        Gizmos.color = Color.green;

        Vector3 prev = transform.position;
        foreach (var corner in agent.path.corners)
        {
            Gizmos.DrawLine(prev, corner);
            Gizmos.DrawSphere(corner, 0.2f);
            prev = corner;
        }
    }
}