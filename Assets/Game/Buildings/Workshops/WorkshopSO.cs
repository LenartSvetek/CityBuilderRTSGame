using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkshopSO", menuName = "Building/SO/WorkshopSO")]
public class WorkshopSO : ScriptableObject
{
    [SerializeField]
    List<ResourceCost> resources;

    [SerializeField]
    List<ResourceCost> production;

    [SerializeField]
    [Tooltip("Time it takes to produce the item in seconds")]
    int productionCost; // in seconds

    [SerializeField] public GameObject[] prefabs;
}
