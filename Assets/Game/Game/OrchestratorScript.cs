using System;
using System.Collections.Generic;
using UnityEngine;

public class OrchestratorScript : MonoBehaviour
{
    public event Action<List<ResourceAmount>> OnResourceChange;

    [Header("References")]
    [SerializeField]
    PlayerController playerController;
    [SerializeField]
    RTSCameraController cameraController;

    [SerializeField]
    List<ResourceAmount> _resources;

    public List<ResourceAmount> resources => _resources;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckBuildingCost(BuidlingSO buidlingSO)
    {
        foreach(var resourceCost in buidlingSO.resourceCosts)
        {
            var resource = _resources.Find(r => r.resource == resourceCost.resource);
            if (resource == null || resource.amount < resourceCost.amount)
            {
                return false; 
            }
        }
        return true;
    }

    public bool CheckResourceAvailability(ResourceSO resourceType, int amount)
    {
        var resource = _resources.Find(r => r.resource == resourceType);
        return resource != null && resource.amount >= amount;
    }

    public bool ApplyBuildingCost(BuidlingSO buidlingSO)
    {
        CheckBuildingCost(buidlingSO);

        foreach (var resourceCost in buidlingSO.resourceCosts)
        {
            var resource = _resources.Find(r => r.resource == resourceCost.resource);
            if (resource != null)
            {
                resource.amount -= resourceCost.amount;
            }
        }

        OnResourceChange.Invoke(resources);
        return true;
    }
}
