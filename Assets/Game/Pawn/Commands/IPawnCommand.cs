using System;
using JetBrains.Annotations;
using UnityEngine;

public interface IPawnCommand
{
    void Execute(GameObject pawn, [CanBeNull] Action<bool> callback);
}
