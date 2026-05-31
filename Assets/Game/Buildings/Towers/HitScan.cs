using UnityEngine;

public class HitScan : MonoBehaviour
{
    public GameObject line;

    public void CreateLine(Vector3 startPoint, Vector3 endPoint, float duration = 0f)
    {
        GameObject lineInstance = Instantiate(line);

        LineRenderer lren = lineInstance.GetComponent<LineRenderer>();

        lren.positionCount = 2;
        lren.SetPosition(0, startPoint);
        lren.SetPosition(1, endPoint);
        
        if (duration > 0f)
        {
            Destroy(lineInstance, duration);
        }
    }
}
