using UnityEngine;

public class PawnController : MonoBehaviour
{
    private Pawn pawn;

    void Awake()
    {
        pawn = GetComponent<Pawn>();
    }

    public void IssueCommand(IPawnCommand command)
    {
        command.Execute(pawn);
    }
}
