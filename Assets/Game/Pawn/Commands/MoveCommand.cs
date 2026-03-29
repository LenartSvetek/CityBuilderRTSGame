using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : IPawnCommand
{
    private Vector3 target;

    public MoveCommand(Vector3 target)
    {
        this.target = target;
    }

    public void Execute(GameObject pawn, [CanBeNull] Action<bool> callback)
    {
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(target, out navHit, 2f, NavMesh.AllAreas))
        {
            target = navHit.position;
        }

        pawn.GetComponent<PawnMovement>().MoveTo(target, callback);
    }
}
