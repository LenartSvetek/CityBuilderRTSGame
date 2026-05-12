using JetBrains.Annotations;
using System;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using Random = UnityEngine.Random;

public class HouseComp : MonoBehaviour
{
    public event Action<HouseComp> OnHouseUpdate;

    [SerializeField] HouseSO _house;

    [SerializeField]
    private int _population = 0;

    public HouseSO house => _house;

    private void Awake()
    {
        int rInd = Mathf.FloorToInt(Random.value * _house.prefabs.Length) % _house.prefabs.Length;

        GameObject obj = Instantiate(_house.prefabs[rInd], transform);
        obj.tag = "House";
        obj.transform.localPosition = Vector3.zero;
        obj.name = "Model";
    }

    

    private void OnMouseOver()
    {
        SetPopulation(_population + 1);
    }

    [ContextMenu("Set population")]
    public void SetPopulation(int newPopulation)
    {
        _population = Mathf.Clamp(newPopulation, 0, _house.maxPopulation);
        OnHouseUpdate?.Invoke(this);
    }

    [ContextMenu("Set population to 1")]
    public void SetPopulation()
    {
        _population = Mathf.Clamp(1, 0, _house.maxPopulation);
        OnHouseUpdate?.Invoke(this);
    }

    public int Population => _population;
}
