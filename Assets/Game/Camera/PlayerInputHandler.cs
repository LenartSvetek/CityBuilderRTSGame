using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    private PlayerInput inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Rigidbody rb;

    private void Awake() {
        inputActions = new PlayerInput();

        // Subscribe to Move action
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove; // To detect when movement stops

        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;

        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context) {
        lookInput = context.ReadValue<Vector2>();
    }
    private void FixedUpdate() {
        // Use moveInput to move the player
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        movement.x *= rb.transform.forward.x;
        movement.z *= rb.transform.forward.z;
        
        rb.MovePosition(rb.position + Time.deltaTime * 5f * movement);

        Vector3 euler = rb.rotation.eulerAngles + new Vector3(0, lookInput.y * Time.deltaTime * 10f, 0);
        rb.MoveRotation(Quaternion.Euler(euler));
    }
}
