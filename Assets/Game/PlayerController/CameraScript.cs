using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private void LateUpdate() {
        Vector3 angles = transform.eulerAngles;
        angles.y = transform.parent.eulerAngles.y;
        transform.eulerAngles = angles;
    }
}
