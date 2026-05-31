using System;
using System.Linq;
using UnityEngine;

[Serializable]
public enum BulletType
{
    Hitscan = 0
};

public class RangedWeaponComp : WeaponBaseComp
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack(WarriorComp warrior)
    {
        hitscanComp.CreateLine(attackOrigin.position, warrior.transform.position, 0.1f);

        warrior.healthComp.TakeDamage(attackDamage);
    }
}
