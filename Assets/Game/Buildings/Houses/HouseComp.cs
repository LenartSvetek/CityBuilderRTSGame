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

    [SerializeField]
    private Transform _GatherSpot;
    public Transform gatherSpot => _GatherSpot;

    public HouseSO house => _house;

    private void Awake()
    {
        int rInd = Mathf.FloorToInt(Random.value * _house.prefabs.Length) % _house.prefabs.Length;

        GameObject obj = Instantiate(_house.prefabs[rInd], transform);
        obj.tag = "House";
        obj.transform.localPosition = Vector3.zero;
        obj.name = "Model";

        _GatherSpot = transform.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GatherSpot")).FirstOrDefault();
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
