using UnityEngine;
using UnityEngine.AI;

public class PawnMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Pawn pawn;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        pawn = GetComponent<Pawn>();
        agent.speed = pawn.data.moveSpeed;
    }

    public void MoveTo(Vector3 position)
    {
        agent.SetDestination(position);
    }
}
