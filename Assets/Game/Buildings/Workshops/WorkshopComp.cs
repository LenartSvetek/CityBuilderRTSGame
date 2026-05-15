using UnityEngine;

public class WorkshopComp : MonoBehaviour
{
    [SerializeField]
    PopulationScript _population;

    [SerializeField]
    WorkshopSO _production;

    [SerializeField]
    float prodProgress = 0f;

    public PopulationScript population => _population;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _population = GetComponent<PopulationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
