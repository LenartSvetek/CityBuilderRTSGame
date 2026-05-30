using UnityEngine;

public class RangedWeaponComp : WeaponBaseComp
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        Debug.Log("Ranged weapon start");    
    }

    
}
