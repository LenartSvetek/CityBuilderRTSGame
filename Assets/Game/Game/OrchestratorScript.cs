using UnityEngine;

public class OrchestratorScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    PlayerController playerController;
    [SerializeField]
    RTSCameraController cameraController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuildingBTN(GameObject buildingPrefab)
    {
        playerController.OnBuildingUI(buildingPrefab);
    }
}
