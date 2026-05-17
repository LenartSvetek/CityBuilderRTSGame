using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;

[System.Serializable]
public enum WorkerState
{
    waitingForWork = 0,
    working = 1,
    walking = 2,
    home = 3
}

public class WorkerComp : MonoBehaviour
{
    private Pawn _pawn;
    public WorkerState state = WorkerState.home;

    private WorkshopComp _workshop;

    [SerializeField]
    private float productionProg = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pawn = GetComponent<Pawn>();
        _workshop = _pawn.WorkingPlace;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_pawn.isWorking) return;

        switch (state)
        {
            case WorkerState.home:
                GoToWork();
                break;
            case WorkerState.waitingForWork:
                CheckForWork();
                break;
            case WorkerState.working:
                Work();
                break;
            default:
                break;
        }
    }

    void GoToWork()
    {
        Transform HouseGatherSpot = _pawn.CurrentHouse.gatherSpot;
        Transform WorkGatherSpot = _pawn.WorkingPlace.gatherSpot;
        if (!_pawn.CheckPosition(HouseGatherSpot)) return;


        _pawn.transform.position = HouseGatherSpot.position;

        _pawn.MovementComp.MoveTo(WorkGatherSpot.position, (bool b) => { Debug.Log("I arrived"); EnterWorkPlace(); });

        state = WorkerState.walking;
    }

    void EnterWorkPlace()
    {
        _pawn.CapsuleCollider.enabled = false;
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }


        productionProg = 0;

        state = WorkerState.waitingForWork;
    }

    void CheckForWork()
    {
        if(!_workshop.StartProduction())
        {
            return;
        }

        productionProg = 0;
        state = WorkerState.working;
    }

    void Work()
    {
        float prodPerSec = _pawn.data.gatherPoints * _pawn.data.gatherPerMin / 60f;

        productionProg += prodPerSec * Time.unscaledDeltaTime;

        Debug.Log($"Production progress: {productionProg}/{_workshop.production.ProductionCost}");

        if (productionProg >= _workshop.production.ProductionCost)
        {
            _workshop.StopProduction(true);
            productionProg = 0;
            state = WorkerState.waitingForWork;

        }
    }
}
