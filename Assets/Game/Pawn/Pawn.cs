using UnityEngine;

public class Pawn : MonoBehaviour
{
    public PawnSO data;
    public int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = data.maxHealth;
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
