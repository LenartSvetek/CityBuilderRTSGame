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

    [SerializeField]
    private AudioClip _startProductionSound;

    [SerializeField]
    private AudioClip _endProductionSound;

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
        var success = _orchestrator.ApplyResourceCost(_production.Resources, true);

        if(success)
            AudioSource.PlayClipAtPoint(_startProductionSound, transform.position);

        return success;
    }

    public void StopProduction(bool success) {
        if (!success) _orchestrator.AddResources(_production.Resources, true);
        else
            _orchestrator.AddResources(_production.Production, true);

        AudioSource.PlayClipAtPoint(_endProductionSound, transform.position);
    }

    #region placing

    public bool CanPlace(ResourceSO resource)
    {
        if (_production.Resource == null) return true;
        if(resource == null) return false;
        return resource.resourceName == _production.Resource.resourceName;
    }

    #endregion
}
