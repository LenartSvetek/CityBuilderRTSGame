using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    private Dictionary<ResourceSO, int> resources = new Dictionary<ResourceSO, int>();
    
    public void Add(ResourceSO resource, int amount)
    {
        if (!resources.ContainsKey(resource))
            resources[resource] = 0;

        resources[resource] += amount;
        // Fire UI event here later
    }

    public bool CanAfford(List<ResourceAmount> cost)
    {
        foreach (var c in cost)
        {
            if (!resources.ContainsKey(c.resource) ||
                resources[c.resource] < c.amount)
                return false;
        }
        return true;
    }

    public void Spend(List<ResourceAmount> cost)
    {
        foreach (var c in cost)
            resources[c.resource] -= c.amount;
    }

    public int Get(ResourceSO resource)
    {
        return resources.TryGetValue(resource, out int value) ? value : 0;
    }
}
