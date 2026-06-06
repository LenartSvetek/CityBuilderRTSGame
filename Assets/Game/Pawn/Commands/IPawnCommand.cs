using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public interface IPawnCommand
{
    void Execute(GameObject pawn, [CanBeNull] Action<NavMeshAgent,bool> callback);
}
