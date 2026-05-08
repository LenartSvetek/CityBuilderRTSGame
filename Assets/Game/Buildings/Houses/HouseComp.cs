using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class HouseComp : MonoBehaviour
{
    [SerializeField] HouseSO _house;

    private int _population = 0;

    public int population => _population;
    public HouseSO house => _house;

    private void Awake()
    {
        int rInd = Mathf.FloorToInt(Random.value * _house.prefabs.Length) % _house.prefabs.Length;

        GameObject obj = Instantiate(_house.prefabs[rInd], transform);
        obj.tag = "House";
        obj.transform.localPosition = Vector3.zero;
        obj.name = "Model";
    }
}
