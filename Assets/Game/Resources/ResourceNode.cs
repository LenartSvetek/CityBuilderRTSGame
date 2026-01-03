using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public ResourceAmount amount;
    public int gatherPerTick = 10;
    
    public int Gather()
    {
        if (amount.amount <= 0)
            return 0;

        int gathered = Mathf.Min(gatherPerTick, amount.amount);
        amount.amount -= gathered;

        if (amount.amount <= 0)
            Destroy(gameObject);

        return gathered;
    }
}
