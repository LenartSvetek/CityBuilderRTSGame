using UnityEngine;
using UnityEngine.SceneManagement;

public class OutpostComp : MonoBehaviour
{
    [ContextMenu("Destroy")]
    private void dest()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene("Menu");
    }
}
