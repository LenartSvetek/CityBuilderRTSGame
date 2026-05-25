using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacingComp : MonoBehaviour
{
    [SerializeField]
    RTSCameraController _cameraController;

    private PlayerInputHandler _inputHandler;
    private PlayerInput _inputActions;

    private BoxCollider _myBox;
    private bool _canPlace = true;

    private Vector3 checkPosition;
    public bool canPlace => _canPlace;

    #region Rotate
    private bool _isRotating = false;
    private float _rotDir = 0;
    private float _rotSpeed = 90; // degrees per second
    #endregion

    Outline outline;

    #region Workshop
    WorkshopComp workshopComp;

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

        workshopComp = GetComponent<WorkshopComp>();
        outline = GetComponentInChildren<Outline>();

        _cameraController = FindFirstObjectByType<RTSCameraController>();
    }

    void Update()
    {
        if (_myBox == null) return;

        UpdateCheckPosition();
        Vector3 pos = checkPosition;

        Vector3 center = checkPosition;

        Vector3 halfExtents = Vector3.Scale(_myBox.size, transform.lossyScale) * 0.5f;
        
        var colliders = Physics.OverlapBox(center, halfExtents, transform.rotation, LayerMask.GetMask("Building"));
        Debug.Log("Colliders overlapping: " + colliders.Length + " center: " + center);
        if (colliders.Length == 1)
        {
            Debug.Log("Something is overlapping my Box Collider!");
            var hitObj = colliders[0].gameObject;
            BuildingScript bScript;

            if (workshopComp == null) IsNotPlacable();
            else if ((bScript = hitObj.transform.root.GetComponent<BuildingScript>()) && bScript.resource != null && workshopComp.CanPlace(bScript.resource)) { IsPlacable(); pos = hitObj.transform.position; }
            else IsNotPlacable();
        }
        else if(colliders.Length > 1)
        {
            IsNotPlacable();
        }
        else
        {
            if(workshopComp == null || workshopComp.CanPlace(null))
                IsPlacable();
            else
            {
                IsNotPlacable();
            }
        }

        UpdatePosition(pos);

        #region Rotate

        if (_isRotating)
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

    void UpdateCheckPosition()
    {
        Vector3 mousePos = _cameraController.GetCursorPosition;

        Ray ray = new Ray(mousePos + Vector3.up * 100f, Vector3.down);
        RaycastHit hit;

        var layers = new List<String> { };
        layers.Add("Terrain");
        int hoverMask = LayerMask.GetMask(layers.ToArray());

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hoverMask))
        {
            GameObject hitObj = hit.transform.root.gameObject;

            Vector3 position = hit.point;
            
            checkPosition = position;
        }
    }

    void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
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

    void IsPlacable() {
        outline.enabled = false;
        outline.OutlineColor = Color.black;
        _canPlace = true;
    }

    void IsNotPlacable()
    {
        outline.enabled = true;
        outline.OutlineColor = Color.red;
        _canPlace = false;
    }

    void PositionOnObject(Transform target)
    {
        transform.position = target.position;
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
