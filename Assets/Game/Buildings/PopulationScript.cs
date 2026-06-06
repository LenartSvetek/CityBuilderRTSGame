using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PopulationScript : MonoBehaviour
{
    WorkshopComp _workshop;

    [SerializeField]
    int maxWorkers;

    [SerializeField]
    [ReadOnly]
    List<Pawn> workers;

    public int space => maxWorkers - workers.Count;
    public bool hasSpace => space > 0;

    void Start()
    {
        _workshop = GetComponent<WorkshopComp>();
    }

    public void AddWorker(Pawn pawn)
    {
        if (workers.Count >= maxWorkers)
        {

            return;
        }
        workers.Add(pawn);
        pawn.SetWorkshop(_workshop);
    }

    public void AddWorkers(List<Pawn> pawns)
    {
        if (workers.Count >= maxWorkers)
        {

            return;
        }
        workers.AddRange(pawns);
        foreach (Pawn pawn in pawns) {
            pawn.SetWorkshop(_workshop);
        }
    }


    void OnDestroy()
    {
        var orch = Object.FindFirstObjectByType<OrchestratorScript>();

        if(orch)
        {
            workers.ForEach(p => p.SentHome());
            workers.ForEach(orch.PawnFreed);
        }
    }
}
