using UnityEngine;

public class OutpostComp : MonoBehaviour
{
    [ContextMenu("Destroy")]
    private void dest()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Debug.Log("the game is lost");
    }
}
