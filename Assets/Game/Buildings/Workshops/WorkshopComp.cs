using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorkshopComp : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    PopulationScript _population;

    [SerializeField]
    WorkshopSO _production;
    public WorkshopSO production => _production;

    BuildingComp _building;
    public BuildingComp building => _building;

    public PopulationScript population => _population;

    [ReadOnly]
    private OrchestratorScript _orchestrator;

    private void Awake()
    {
        _building = GetComponent<BuildingComp>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _population = GetComponent<PopulationScript>();

        

        _orchestrator = FindFirstObjectByType<OrchestratorScript>();
    }

    public bool StartProduction()
    {
        return _orchestrator.ApplyResourceCost(_production.Resources, true);
    }

    public void StopProduction(bool success) {
        if (!success) _orchestrator.AddResources(_production.Resources, true);
        _orchestrator.AddResources(_production.Production, true);
    }

    #region placing

    public bool CanPlace(ResourceSO resource)
    {
        Debug.Log($"Can place {resource?.resourceName} in {name} with production resource req: {_production.Resource}");
        if (_production.Resource == null) return true;
        if(resource == null) return false;
        return resource.resourceName == _production.Resource.resourceName;
    }

    #endregion
}
