using UnityEngine;

[System.Serializable]
public class ResourceAmount
{
    public ResourceSO resource;
    public int amount;
    public int maxAmount;

    public ResourceAmount Clone()
    {
        return new ResourceAmount
        {
            resource = resource,
            amount = amount,
            maxAmount = maxAmount
        };
    }
}
