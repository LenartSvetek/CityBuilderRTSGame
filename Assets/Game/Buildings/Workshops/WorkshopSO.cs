using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkshopSO", menuName = "Building/SO/WorkshopSO")]
public class WorkshopSO : ScriptableObject
{
    [SerializeField]
    ResourceSO _resource;
    public ResourceSO Resource => _resource;

    [SerializeField]
    List<ResourceCost> _resources;
    public List<ResourceCost> Resources => _resources;

    [SerializeField]
    List<ResourceCost> _production;
    public List<ResourceCost> Production => _production;

    [SerializeField]
    [Tooltip("Time it takes to produce the item in seconds")]
    int _productionCost; // in seconds
    public int ProductionCost => _productionCost;

    [SerializeField] public GameObject[] prefabs;
}
