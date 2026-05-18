using NaughtyAttributes;
using System.Linq;
using UnityEngine;

public class WorkshopComp : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    PopulationScript _population;

    [SerializeField]
    WorkshopSO _production;
    public WorkshopSO production => _production;

    [SerializeField]
    [ReadOnly]
    private Transform _GatherSpot;
    public Transform gatherSpot => _GatherSpot;

    public PopulationScript population => _population;

    [ReadOnly]
    private OrchestratorScript _orchestrator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _population = GetComponent<PopulationScript>();

        _GatherSpot = transform.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GatherSpot")).FirstOrDefault();

        _orchestrator = FindFirstObjectByType<OrchestratorScript>();
    }

    public bool StartProduction()
    {
        return _orchestrator.ApplyResourceCost(_production.Resources);
    }

    public void StopProduction(bool success) {
        if (!success) _orchestrator.AddResources(_production.Resources);
        _orchestrator.AddResources(_production.Production);
    }
}
