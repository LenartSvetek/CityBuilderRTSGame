using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    static public void PlaceObject(Camera camera, GameObject obj) {
        // When you left-click
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits the terrain (or anything with a collider)
        if (Physics.Raycast(ray, out hit)) {
            Debug.Log(hit.collider.gameObject.name);
            // Place object at the hit point
            Vector3 position = hit.point;
            Instantiate(obj, position, Quaternion.identity);
            Debug.Log($"Place position: {position}");
        }
    }

    static public void PlaceObject(Vector3 position, GameObject obj) {
        Instantiate(obj, position, Quaternion.identity);
    }
}
