using UnityEngine;

public class HouseComponent : MonoBehaviour
{
    [Header("Info")] 
    [SerializeField] public int maxPopulation = 5;
    [SerializeField] public int population = 0;
    [SerializeField] public int minPopulation = 0;
}
