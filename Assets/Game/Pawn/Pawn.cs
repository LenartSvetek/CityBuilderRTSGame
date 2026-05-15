using UnityEngine;

public class Pawn : MonoBehaviour
{
    public PawnSO data;
    public int currentHealth;

    [SerializeField]
    private HouseComp _currentHouse;

    [SerializeField]
    private WorkshopComp _workingAt;

    public bool isWorking => _workingAt != null;

    public HouseComp CurrentHouse => _currentHouse;

    protected virtual void Awake()
    {
        currentHealth = data.maxHealth;
    }

    public void SetHome(HouseComp house)
    {
        _currentHouse = house;
    }

    public void SetWorkshop(WorkshopComp workshop)
    {
        _workingAt = workshop;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
