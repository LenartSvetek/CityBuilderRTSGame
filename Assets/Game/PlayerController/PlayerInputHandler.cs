using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    private PlayerInput _inputActions;
    private Vector2 _moveInput;

    private bool _isLooking;
    private Vector2 _lookInput;

    [SerializeField]
    private CinemachineCamera cam;
    [SerializeField]
    private CinemachineBrain brain;
    private CinemachineOrbitalFollow _orbitalCamera;

    private PlayerController _playerController;
    
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

        _inputActions.Player.Action.canceled += OnAction;

        _inputActions.Player.Action2.canceled += OnSecondaryAction;

        _orbitalCamera = cam.GetComponent<CinemachineOrbitalFollow>();
        
        _playerController = GetComponent<PlayerController>();

        Cursor.lockState = CursorLockMode.Confined;
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

    private void OnAction(InputAction.CallbackContext context) {
        if(!context.ReadValueAsButton()) {
            _playerController.OnPlayerClick();
        }
    }
    
    private void OnSecondaryAction(InputAction.CallbackContext context) {
        if(!context.ReadValueAsButton()) {
            _playerController.OnPlayerSecondaryClick();
        }
    }

    private void FixedUpdate()
    {
        if (_moveInput != Vector2.zero)
        {
            _playerController.OnPlayerMove(_moveInput);
        }
        
        if (_isLooking)
        {
            _playerController.OnPlayerLook(_lookInput);
        }

        // Vector3 euler = rb.rotation.eulerAngles + new Vector3(0, lookInput.y * Time.deltaTime * 10f, 0);
        // rb.MoveRotation(Quaternion.Euler(euler));
    }
}
