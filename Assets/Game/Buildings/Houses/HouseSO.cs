using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HouseSO", menuName = "Building/SO/HouseSO")]
public class HouseSO : ScriptableObject
{
    [Header("Info")] 
    [SerializeField] public int maxPopulation = 5;
    public int level = 1;
    [Header("Prefabs")] [SerializeField] public GameObject[] prefabs;
}
