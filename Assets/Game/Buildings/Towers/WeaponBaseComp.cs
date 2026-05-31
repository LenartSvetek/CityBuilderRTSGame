using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponBaseComp : MonoBehaviour
{
    protected Transform attackOrigin;

    protected HitScan hitscanComp;

    [SerializeField]
    List<WarriorComp> _targets;

    [SerializeField]
    float attackInterval = 250;
    [SerializeField]
    protected float attackDamage = 10;

    float attackTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        SensorComp sensor = GetComponentInChildren<SensorComp>();
        if (sensor == null) return;

        sensor.OnEnemyPawnEnter += OnEnemyPawnEnter;
        sensor.OnEnemyPawnLeave += OnEnemyPawnLeave;

        hitscanComp = GetComponent<HitScan>();

        attackOrigin = transform.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("AttackOrigin")).FirstOrDefault();
    }

    void OnEnemyPawnEnter(WarriorComp pawn) {
        if (_targets.Contains(pawn)) return;

        if(_targets.Count == 0 && attackTimer > attackInterval)
        {
            attackTimer = 0;
        }
        _targets.Add(pawn);
        pawn.healthComp.OnDeath += OnEnemyPawnDeath;

        pawn.Attacked(GetComponent<BuildingComp>());
    }

    void OnEnemyPawnLeave(WarriorComp pawn)
    {

        pawn.healthComp.OnDeath -= OnEnemyPawnDeath;
        _targets.Remove(pawn);
    }

    void OnEnemyPawnDeath(HealthComp pawn)
    {
        _targets.Remove(pawn.GetComponent<WarriorComp>());
    }

    void Update()
    {
        attackTimer -= Time.unscaledDeltaTime * 1000;

        if (attackTimer <= 0 && _targets.Count > 0) {
            attackTimer = attackInterval;
            Attack(_targets[0]);
        }
    }

    protected virtual void Attack(WarriorComp warrior) {
        Debug.Log("Base weapon attacking");
    }
}
