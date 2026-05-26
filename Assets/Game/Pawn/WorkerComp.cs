using UnityEngine;


public class WorkerComp : MonoBehaviour
{
    private Pawn _pawn;
    

    private WorkshopComp _workshop;

    [SerializeField]
    private float productionProg = 0f;

    public PawnState state { get => _pawn.state; set => _pawn.state = value; }

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
            case PawnState.home:
                GoToWork();
                break;
            case PawnState.waitingForWork:
                CheckForWork();
                break;
            case PawnState.working:
                Work();
                break;
            default:
                break;
        }
    }

    void GoToWork()
    {
        Transform HouseGatherSpot = _pawn.CurrentHouse.building.gatherSpot;
        Transform WorkGatherSpot = _pawn.WorkingPlace.building.gatherSpot;
        if (!_pawn.CheckPosition(HouseGatherSpot)) return;


        _pawn.transform.position = HouseGatherSpot.position;

        state = PawnState.walking;
        _pawn.MovementComp.MoveTo(WorkGatherSpot.position, (bool b) => { Debug.Log("I arrived"); EnterWorkPlace(); });
    }

    void EnterWorkPlace()
    {
        _pawn.CapsuleCollider.enabled = false;
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }


        productionProg = 0;

        state = PawnState.waitingForWork;
    }

    void CheckForWork()
    {
        if(!_workshop.StartProduction())
        {
            return;
        }

        productionProg = 0;
        state = PawnState.working;
    }

    void Work()
    {
        float prodPerSec = _pawn.data.gatherPoints * _pawn.data.gatherPerMin / 60f;

        productionProg += prodPerSec * Time.unscaledDeltaTime;


        if (productionProg >= _workshop.production.ProductionCost)
        {
            _workshop.StopProduction(true);
            productionProg = 0;
            state = PawnState.waitingForWork;

        }
    }
}
