using UnityEngine;

public class SensorController : MonoBehaviour
{
    SensorComp sensor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sensor = transform.root.GetComponentInChildren<SensorComp>();
        if (sensor == null) enabled = false;
    }

    void OnMouseEnter()
    {
        sensor.ShowRange();
    }

    void OnMouseExit()
    {
        sensor.HideRange();
    }
}
