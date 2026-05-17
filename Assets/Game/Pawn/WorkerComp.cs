using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;

[System.Serializable]
public enum WorkerState
{
    working = 0,
    walking = 1,
    home = 2
}

public class WorkerComp : MonoBehaviour
{
    private Pawn _pawn;
    public WorkerState state = WorkerState.home;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pawn = GetComponent<Pawn>();
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

    }
}
