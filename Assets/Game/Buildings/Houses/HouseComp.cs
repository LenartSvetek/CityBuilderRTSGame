using JetBrains.Annotations;
using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using Random = UnityEngine.Random;

public class HouseComp : MonoBehaviour
{
    public event Action<HouseComp> OnHouseUpdate;
    

    [SerializeField] HouseSO _house;

    [SerializeField]
    private List<Pawn> population;

    

    public HouseSO house => _house;

    BuildingComp _building;
    public BuildingComp building => _building;
    private void Awake()
    {
        _building = GetComponent<BuildingComp>();
    }


    public bool AddPawn(Pawn pawn)
    {
        if (population.Count >= _house.maxPopulation)
            return false;
        population.Add(pawn);
        
        OnHouseUpdate?.Invoke(this);
        return true;
    }

    public int Population => population.Count;
    public int space => _house.maxPopulation - population.Count;
}
