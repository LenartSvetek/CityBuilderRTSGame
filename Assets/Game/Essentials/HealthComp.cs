using System;
using UnityEngine;

public class HealthComp : MonoBehaviour
{
    public event Action<HealthComp> OnTakingDamage;
    public event Action<HealthComp> OnHeal;
    public event Action<HealthComp> OnDeath;

    [SerializeField]
    float _maxHealth = 100;
    public float maxHealth => _maxHealth;

    [SerializeField]
    float _health = 100;
    public float health => _health;

    [SerializeField]
    float _selfHealAmount = 10;
    [SerializeField]
    float _selfHealInterval = 100;
    [SerializeField]
    float _selfhealAttackStopTimeout = 1000;

    float _selfHealTimer = 100;

    void Update()
    {
        _selfHealTimer -= Time.deltaTime * 1000;
        if(_selfHealTimer < 0)
        {
            Heal(_selfHealAmount);
            _selfHealTimer = _selfHealInterval;
        }
    }

    public void TakeDamage(float damage)
    {
        if (_health <= 0) return;
        OnTakingDamage?.Invoke(this);
        _health -= damage;
        _selfHealTimer = _selfhealAttackStopTimeout;
        if (_health <= 0)
        {
            OnDeath?.Invoke(this);
            //_freeSpaces.Where(s => s != null).ToList().ForEach(s => s.GoHome());
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        if(_health >= maxHealth) return;

        _health += amount;
        if(_health > maxHealth) _health = maxHealth;
        OnHeal?.Invoke(this);
    }
}
