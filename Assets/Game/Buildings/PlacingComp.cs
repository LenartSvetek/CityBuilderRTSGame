using UnityEngine;
using UnityEngine.InputSystem;

public class PlacingComp : MonoBehaviour
{
    private PlayerInputHandler _inputHandler;
    private PlayerInput _inputActions;

    private BoxCollider _myBox;
    private bool _canPlace = true;

    public bool canPlace => _canPlace;

    #region Rotate
    private bool _isRotating = false;
    private float _rotDir = 0;
    private float _rotSpeed = 90; // degrees per second
    #endregion

    void Start()
    {
        _myBox = GetComponentInChildren<BoxCollider>();

        _myBox.enabled = false; 

        Debug.Log("Input actions number: " + FindObjectsByType<PlayerInputHandler>(FindObjectsSortMode.InstanceID).Length);

        _inputHandler = FindFirstObjectByType<PlayerInputHandler>();
        _inputActions = _inputHandler.inputActions;

        _inputActions.Building.Enable();
        
        
        _inputActions.Building.Rotate.performed += OnRotateStart;
        _inputActions.Building.Rotate.canceled += OnRotateStop;
    }

    void Update()
    {
        if (_myBox == null) return;

        Vector3 center = transform.TransformPoint(_myBox.center);

        Vector3 halfExtents = Vector3.Scale(_myBox.size, transform.lossyScale) * 0.5f;

        bool isHit = Physics.CheckBox(center, halfExtents, transform.rotation, LayerMask.GetMask("Building"));

        if (isHit)
        {
            Debug.Log("Something is overlapping my Box Collider!");
            var outline = GetComponentInChildren<Outline>();
            outline.enabled = true;
            outline.OutlineColor = Color.red;
            _canPlace = false;
        }
        else
        {
            var outline = GetComponentInChildren<Outline>();
            outline.enabled = false;
            outline.OutlineColor = Color.black;
            _canPlace = true;
        }

        #region Rotate

        if(_isRotating)
        {
            transform.Rotate(0, _rotDir * _rotSpeed * Time.unscaledDeltaTime, 0);
        }

        #endregion
    }

    void OnDestroy()
    {
        var outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
        outline.OutlineColor = Color.black;

        _myBox.enabled = true;
    }

    void OnRotateStart(InputAction.CallbackContext context)
    {
        _isRotating = true;
        _rotDir = context.ReadValue<float>();
    }

    void OnRotateStop(InputAction.CallbackContext context)
    {
        _isRotating = false;
        _rotDir = 0;
    }

    // Optional: Draw it in the Scene view so you can verify it matches
    void OnDrawGizmos()
    {
        BoxCollider box = GetComponentInChildren<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
