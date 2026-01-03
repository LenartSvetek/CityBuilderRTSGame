using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[System.Serializable]
public class ResourceType
{
    public ResourceAmount resource;
    public int level;
}

public class Resources : MonoBehaviour
{
    public List<ResourceType> types = new List<ResourceType>();
    public GameObject resourcePrefab;

    public int place = 10;
    
    public void PlaceResources(ProceduralTerrain terrain)
    {
        for (int i = 0; i < place; i++)
        {
            float x = Random.value * (terrain.size - 2) + 1;
            float y = Random.value * (terrain.size - 2) + 1;
            ResourceType type = types[Random.Range(1, types.Count - 1)];
            
            GameObject resource = Instantiate(resourcePrefab, new Vector3(x, 0, y), Quaternion.identity);
            BuildingScript buildingScript = resource.GetComponent<BuildingScript>();
            buildingScript.resource = type.resource.resource;
            buildingScript.level = type.level;
            
            ResourceNode resourceNode = resource.GetComponent<ResourceNode>();
            resourceNode.amount = type.resource;
            
            
        }
    }
}
