using UnityEngine;

[CreateAssetMenu(fileName = "HouseSO", menuName = "Building/SO/HouseSO")]
public class HouseSO : ScriptableObject
{
    [Header("Info")] 
    [SerializeField] public int maxPopulation = 5;
    [SerializeField] public int population = 0;
    [SerializeField] public int minPopulation = 0;
}
