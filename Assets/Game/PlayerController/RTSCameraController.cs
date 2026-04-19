using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class RTSCameraController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    InputActionReference moveAction;
    [SerializeField]
    string sprintActionName = "Sprint";
    [SerializeField]
    float inputDeadZone = 0.1f;

    private bool hasMoveInput = false;
    private Vector3 moveInput3D;
    private bool sprintInput;

    Vector3 currentVelocity = Vector3.zero;
    float currentSpeedMultiplier = 1f;
    
    float decelTimer = 0.0f;
    Vector3 decelStartVelocity = Vector3.zero;
    
    private InputAction sprintAction;
    
    Vector3 cursorPositon = Vector3.zero;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float sprintSpeedMultiplier = 4f;
    [SerializeField] float acceleration = 50f;
    [SerializeField] float decelerateDuration = 1f;
    [SerializeField] AnimationCurve decelerateCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    
    [Header("Height sampling")]
    [SerializeField] LayerMask GroundLayerMask;
    [SerializeField] float GroundSensorRadious = 10f;
    [SerializeField] float HeightMovementSmoothing = 2f;
    
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
        GroundLayerMask = LayerMask.GetMask("Terrain");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCursorPosition();
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
    
        hasMoveInput = moveInput.sqrMagnitude > inputDeadZone * inputDeadZone;
        
        sprintInput = sprintAction.IsPressed();

        float targetMultiplier = sprintInput ? sprintSpeedMultiplier : 1f;
        
        currentSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetMultiplier, Time.unscaledDeltaTime * 10);
    }

    void UpdateMovement()
    {
        if (hasMoveInput)
        {
            Vector3 targetVelocity = currentSpeedMultiplier * moveSpeed * moveInput3D;
            float maxDelta = acceleration * Time.unscaledDeltaTime * currentSpeedMultiplier;
            
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, maxDelta);

            decelTimer = 0f;
            decelStartVelocity = currentVelocity;
        }
        else
        {
            float normalizedTime = 1f;

            if (decelerateDuration >= 0.01f)
            {
                normalizedTime = decelTimer / decelerateDuration;
            }
            
            float curveValue = 1f - decelerateCurve.Evaluate(normalizedTime);
            
            currentVelocity = Vector3.Lerp(decelStartVelocity, Vector3.zero, curveValue);
            
            decelTimer += Time.unscaledDeltaTime;
        }

        Vector3 newPosition = ComputeGroundHeight(cameraTarget.position + currentVelocity * Time.unscaledDeltaTime);

        cameraTarget.position = newPosition;
    }

    Vector3 ComputeGroundHeight(Vector3 position)
    {
        float groundHeight = position.y;
        
        float h1 = SampleHeight(position);
        float h2 = SampleHeight(position + GroundSensorRadious * Vector3.right);
        float h3 = SampleHeight(position + GroundSensorRadious * Vector3.left);
        float h4 = SampleHeight(position + GroundSensorRadious * Vector3.forward);
        float h5 = SampleHeight(position + GroundSensorRadious * Vector3.back);
        
        groundHeight = (h1 + h2 + h3 + h4 + h5) / 5f;
        
        
        
        position.y = Mathf.Clamp(position.y, groundHeight, HeightMovementSmoothing * Time.unscaledDeltaTime);
        
        return position;
    }
    float SampleHeight(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 500f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, GroundLayerMask))
        {
            return hit.point.y;
        }

        return 0f;
    }

    void UpdateCursorPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, GroundLayerMask))
        {
            cursorPositon = hit.point;
        }
    }
    
    public Vector3 GetCursorPosition => cursorPositon; 
}
