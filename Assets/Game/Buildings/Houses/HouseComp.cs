using JetBrains.Annotations;
using UnityEngine;

public class HouseComp : MonoBehaviour
{
    [SerializeField] HouseSO _house;
    
    public HouseSO house => _house;
}
