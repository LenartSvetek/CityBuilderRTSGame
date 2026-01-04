using UnityEngine;

public class MoveCommand : IPawnCommand
{
    private Vector3 target;

    public MoveCommand(Vector3 target)
    {
        this.target = target;
    }

    public void Execute(Pawn pawn)
    {
        pawn.GetComponent<PawnMovement>().MoveTo(target);
    }
}
