using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    [SerializeField]
    List<HouseComp> houses;

    #region Population
    [Foldout("Population")]
    [SerializeField]
    [Tooltip("Population growth per second")]
    [Label("Pop growth rate")]
    float populationGrowthRate = 30f; // population growth per minute
    float populationGrowthTimer = 0f;
    float populationGrowthCutoff = 0f;
    [Foldout("Population")]
    [SerializeField]
    int populationFree = 0;
    #endregion

    public List<ResourceAmount> resources => _resources;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        populationGrowthCutoff = 1f / (populationGrowthRate / 60);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePopulation();
    }

    private void UpdatePopulation()
    {
        populationGrowthTimer += Time.unscaledDeltaTime;

        Debug.Log($"Population growth timer: {populationGrowthTimer}, cutoff: {populationGrowthCutoff}");

        if (populationGrowthTimer < populationGrowthCutoff) return;

        
        var populationResource = _resources.Find(r => r.resource.resourceName == "Population");
        if (populationResource != null)
        {
            var newPop = Mathf.FloorToInt(populationGrowthTimer / populationGrowthCutoff);
            newPop = Mathf.Min(populationResource.amount + newPop, populationResource.maxAmount);
            populationFree += newPop - populationResource.amount;
            populationResource.amount = newPop;
            OnResourceChange.Invoke(resources);
        }

        populationGrowthTimer = 0f;
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

    #region Registering Buildings

    public void RegisterBuilding(HouseComp house)
    {
        if (houses.Contains(house))
        {
            return;
        }

        houses.Add(house);

        var maxPop = houses.Select(house => house.house.maxPopulation).Sum();
        Debug.Log("Max population updated: " + maxPop);
        resources.Find(r => r.resource.resourceName == "Population").maxAmount = maxPop;

        OnResourceChange.Invoke(resources);
    }

    #endregion
}
