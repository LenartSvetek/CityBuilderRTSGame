using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseComp : MonoBehaviour
{
    [SerializeField]
    List<WarriorComp> _targets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        SensorComp sensor = GetComponentInChildren<SensorComp>();
        if (sensor == null) return;

        sensor.OnEnemyPawnEnter += OnEnemyPawnEnter;
        sensor.OnEnemyPawnLeave += OnEnemyPawnLeave;
    }

    void OnEnemyPawnEnter(WarriorComp pawn) {
        if (_targets.Contains(pawn)) return;
        _targets.Add(pawn);

        pawn.Attacked(GetComponent<BuildingComp>());
    }

    void OnEnemyPawnLeave(WarriorComp pawn)
    {
        _targets.Remove(pawn);
    }
}
