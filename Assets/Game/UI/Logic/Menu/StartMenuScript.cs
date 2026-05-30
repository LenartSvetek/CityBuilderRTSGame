using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenuScript : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        var root = GetComponent<UIDocument>().rootVisualElement;

        Button startBtn = root.Q<Button>("Start");
        Button exitBtn = root.Q<Button>("Exit");

        startBtn.clicked += StartGame;
        exitBtn.clicked += EndGame;
    }

    void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void EndGame()
    {
        Application.Quit();

        // 2. Stops play mode if you are testing inside the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
