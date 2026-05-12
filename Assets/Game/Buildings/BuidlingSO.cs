using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ResourceCost
{
    public ResourceSO resource;
    public int amount;
}

[CreateAssetMenu(fileName = "BuidlingSO", menuName = "RTS/BuidlingSO")]
public class BuidlingSO : ScriptableObject
{
    [SerializeField]
    public GameObject prefab;

    [SerializeField]
    public List<ResourceCost> resourceCosts;
}
