using UnityEngine;

public class PlacingComp : MonoBehaviour
{
    private BoxCollider _myBox;

    private bool _canPlace = true;

    public bool canPlace => _canPlace;

    void Start()
    {
        // Get the reference to your existing collider
        _myBox = GetComponentInChildren<BoxCollider>();
    }

    void Update()
    {
        if (_myBox == null) return;

        Vector3 center = transform.TransformPoint(_myBox.center);

        Vector3 halfExtents = Vector3.Scale(_myBox.size, transform.lossyScale) * 0.5f;

        bool isHit = Physics.CheckBox(center, halfExtents, transform.rotation, LayerMask.GetMask("Building"));

        if (isHit)
        {
            Debug.Log("Something is overlapping my Box Collider!");
            var outline = GetComponentInChildren<Outline>();
            outline.enabled = true;
            outline.OutlineColor = Color.red;
            _canPlace = false;
        }
        else
        {
            var outline = GetComponentInChildren<Outline>();
            outline.enabled = false;
            outline.OutlineColor = Color.black;
            _canPlace = true;
        }
    }

    void OnDestroy()
    {
        var outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
        outline.OutlineColor = Color.black;
    }

    // Optional: Draw it in the Scene view so you can verify it matches
    void OnDrawGizmos()
    {
        BoxCollider box = GetComponentInChildren<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
