using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RTSCameraController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    InputActionReference moveAction;
    [SerializeField]
    string sprintActionName = "Sprint";

    private Vector3 moveInput3D;
    private bool sprintInput;

    private InputAction sprintAction;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float sprintSpeedMultiplier = 4f;

    [Header("Components")]
    [SerializeField] new CinemachineCamera camera;
    [SerializeField] CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] Transform cameraTarget;

    private void OnValidate()
    {
        if (camera == null) camera = GetComponent<CinemachineCamera>();
        if(orbitalFollow == null) orbitalFollow = camera.GetComponent<CinemachineOrbitalFollow>();
    }

    private void Awake()
    {
        sprintAction = moveAction.action.actionMap.FindAction(sprintActionName);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateMovement();
    }

    void HandleInput()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        
        Vector3 forward = camera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = camera.transform.right;
        right.y = 0;
        right.Normalize();

        moveInput3D = forward * moveInput.y + right * moveInput.x;
    
        sprintInput = sprintAction.IsPressed();
    }

    void UpdateMovement()
    {
        Vector3 velocity = moveInput3D * moveSpeed * (sprintInput ? sprintSpeedMultiplier : 1f);

        Vector3 newPosition = cameraTarget.position + velocity * Time.unscaledDeltaTime;

        cameraTarget.position = newPosition;
    }
}
