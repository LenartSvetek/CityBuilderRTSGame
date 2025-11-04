using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public RectTransform cursorUI;

    public float smoothSpeed = 1000f;       // Higher = faster follow

    private Vector3 targetPos;


    public Vector3 offset = new Vector3(8, -10, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = Input.mousePosition + offset;

        // Smoothly interpolate toward the real mouse position
        Vector3 currentPos = cursorUI.position;
        cursorUI.position = Vector3.Lerp(currentPos, targetPos, Time.unscaledDeltaTime * smoothSpeed);


    }
}
