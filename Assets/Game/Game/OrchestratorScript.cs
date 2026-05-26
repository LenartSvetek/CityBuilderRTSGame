using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
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
    List<ResourceAmount> _cleanResources; // tracks only the production/consumption of resources, not the total amount. Used for UI display and calculating per minute changes.
    public List<ResourceAmount> cleanResources => _cleanResources;

    [SerializeField]
    List<HouseComp> houses;

    [SerializeField]
    List<WorkshopComp> workshops;
    List<WorkshopComp> workshopsWSpace => workshops.Where(w => w.population.hasSpace).ToList();


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
    GameObject pawnPrefab;
    [Foldout("Population")]
    [SerializeField]
    List<Pawn> workers;
    [Foldout("Population")]
    [SerializeField]
    List<Pawn> freeWorkers;
    [SerializeField]
    int populationFree => freeWorkers.Count;
    #endregion

    public List<ResourceAmount> resources => _resources;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        populationGrowthCutoff = 1f / (populationGrowthRate / 60);
        _cleanResources = _resources.Select(item => item.Clone()).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePopulation();
        DistributeFreeWorkforce();
    }

    private void UpdatePopulation()
    {
        populationGrowthTimer += Time.unscaledDeltaTime;

        if (populationGrowthTimer < populationGrowthCutoff) return;

        var populationResource = _resources.Find(r => r.resource.resourceName == "Population");
        if (populationResource != null)
        {
            var popChange = Mathf.FloorToInt(populationGrowthTimer / populationGrowthCutoff);
            

            var newPop = Mathf.Min(populationResource.amount + popChange, populationResource.maxAmount);
            popChange = newPop - populationResource.amount;

            populationResource.amount = newPop;
            OnResourceChange.Invoke(resources);

            int i = 0;
            while (popChange > 0 && i < houses.Count)
            {
                HouseComp house = houses[i];
                int take = Mathf.Min(popChange, house.space);
                popChange -= take;
                
                for(int j = 0; j < take; j++)
                {
                    var pawn = CreatePawn(house).GetComponent<Pawn>();
                    house.AddPawn(pawn);
                    freeWorkers.Add(pawn);
                }


                i++;
            }
        }

        populationGrowthTimer = 0f;
    }

    private void DistributeFreeWorkforce()
    {
        if(populationFree <= 0 || workshopsWSpace.Count <= 0) return;

        int popToAssign = Mathf.Min(1, Mathf.FloorToInt(populationFree / workshopsWSpace.Count));
        int workshopsAssigned = workshopsWSpace.Count;
        for (int w_i = 0; populationFree > 0 && w_i < workshopsAssigned; w_i++)
        {
            WorkshopComp workshop = workshopsWSpace[w_i];
            
            int actualMoveCount = Mathf.Min(popToAssign, populationFree, freeWorkers.Count);
            int startIndex = freeWorkers.Count - actualMoveCount;
            List<Pawn> pawnsToMove = freeWorkers.GetRange(startIndex, actualMoveCount);

            workers.AddRange(pawnsToMove);
            freeWorkers.RemoveRange(startIndex, actualMoveCount);

            workshop.population.AddWorkers(pawnsToMove);
        }
    }

    public GameObject CreatePawn(HouseComp house)
    {
        GameObject obj = Instantiate(pawnPrefab);
        Pawn pawnComp = obj.GetComponent<Pawn>();
        pawnComp.SetHome(house);

        return obj;
    }

    public bool CheckResourceCost(List<ResourceCost> cost)
    {
        foreach(var resourceCost in cost)
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

    public bool ApplyResourceCost(List<ResourceCost> cost, bool bProduction = false)
    {
        if(!CheckResourceCost(cost)) return false;

        foreach (var resourceCost in cost)
        {
            var resource = _resources.Find(r => r.resource == resourceCost.resource);
            if (resource != null)
            {
                resource.amount -= resourceCost.amount;
                if(bProduction)
                {
                    var cleanResource = _cleanResources.Find(r => r.resource == resourceCost.resource);
                    if (cleanResource != null)
                    {
                        cleanResource.amount -= resourceCost.amount;
                    }
                }
            }
        }

        OnResourceChange.Invoke(resources);
        return true;
    }

    public void AddResources(List<ResourceCost> production, bool bProduction = false)
    {
        foreach(var product in production)
        {
            var resource = _resources.Find(r => r.resource == product.resource);
            resource.amount = Mathf.Min(resource.amount + product.amount, resource.maxAmount);
            if(bProduction)
            {
                var cleanResource = _cleanResources.Find(r => r.resource == product.resource);
                if (cleanResource != null)
                {
                    cleanResource.amount += product.amount;
                }
            }
        }

        OnResourceChange.Invoke(resources);
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

    public void RegisterBuilding(WorkshopComp workshop)
    {
        if (workshops.Contains(workshop))
        {
            return;
        }

        workshops.Add(workshop);
    }

    #endregion
}
