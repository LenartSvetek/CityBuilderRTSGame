using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _inputActions;
    private Vector2 _moveInput;

    private bool _isLooking;
    private Vector2 _lookInput;

    private Rigidbody _rb;

    [SerializeField]
    private CinemachineCamera cam;
    private CinemachineOrbitalFollow _orbitalCamera;

    private void Awake()
    {
        _inputActions = new PlayerInput();

        // Subscribe to Move action
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove; // To detect when movement stops

        _inputActions.Player.Look.performed += OnLook;
        _inputActions.Player.Look.canceled += OnLook;

        _inputActions.Player.LookActivation.performed += OnLookActivation;
        _inputActions.Player.LookActivation.canceled += OnLookActivation;

        _rb = GetComponent<Rigidbody>();
        _orbitalCamera = cam.GetComponent<CinemachineOrbitalFollow>();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void OnLookActivation(InputAction.CallbackContext context)
    {
        _isLooking = context.ReadValueAsButton();

        if (_isLooking)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        _orbitalCamera.HorizontalAxis.Recentering.Enabled = !_isLooking;

    }

    private void FixedUpdate()
    {
        // Use moveInput to move the player
        Vector3 movement = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;

        // Flatten camera forward and right vectors (ignore X rotation)
        Vector3 forward = _orbitalCamera.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = _orbitalCamera.transform.right;
        right.y = 0f;
        right.Normalize();

        // Movement direction
        Vector3 direction = forward * movement.z + right * movement.x;
        direction.Normalize();

        _rb.MovePosition(_rb.position + Time.deltaTime * 5f * direction);

        if (_isLooking)
        {
            _orbitalCamera.HorizontalAxis.Value += _lookInput.x * Time.deltaTime * 10f;

        }

        // Vector3 euler = rb.rotation.eulerAngles + new Vector3(0, lookInput.y * Time.deltaTime * 10f, 0);
        // rb.MoveRotation(Quaternion.Euler(euler));
    }
}
