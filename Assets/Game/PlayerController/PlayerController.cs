using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerState
{
    Idle,
    Controlling,
    PlacingBuilding,
    DragSelecting,
}

public enum ObjectType
{
    Null,
    Pawn,
    Resource,
    Building,
}

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    
    [SerializeField]
    private CinemachineCamera _cam;
    private CinemachineOrbitalFollow _orbitalCamera;

    [SerializeField]
    private CinemachineBrain _brain;
    private Camera _acCam;
    [SerializeField]
    private GameObject Placer;
    
    [SerializeField]
    private GameObject DefualtPlacer;
    
    PlayerSettings _playerSettings;

    [SerializeField]
    private GameObject _obj;

    [SerializeField]
    private float _gridSize = 5;
    
    public event Action<PlayerState> OnStateChanged;
    private PlayerState _playerState = PlayerState.Idle;

    public PlayerState playerState
    {
        get { return _playerState; }
        set
        {
            if (_playerState == value) return;

            _playerState = value;
            OnStateChanged?.Invoke(_playerState);
        }
    }

    public ObjectType objectType = ObjectType.Null;
    public GameObject hoveredObject = null;
    
    [SerializeField]
    private List<GameObject> selectedObjects = new List<GameObject>();

    public List<GameObject> SelectedObjects
    {
        get { return selectedObjects; }
    }
    
    [SerializeField]
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _orbitalCamera = _cam.GetComponent<CinemachineOrbitalFollow>();
        _playerSettings = GetComponent<PlayerSettings>();

        _acCam = _brain.GetComponent<Camera>();
    }

    public void OnPlayerMove(Vector2 moveInput)
    {
        // Use moveInput to move the player
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y).normalized;

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

        _rigidbody.MovePosition(_rigidbody.position + Time.deltaTime * _playerSettings.PlayerSpeed * direction);
    }

    public void OnPlayerLook(Vector2 lookInput)
    {
        _orbitalCamera.HorizontalAxis.Value += lookInput.x * Time.deltaTime * 10f;
    }

    void LateUpdate() 
    {
        // Moved from FixedUpdate to Update so it perfectly syncs with Cinemachine!
        Ray ray = _acCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Added a LayerMask so hover doesn't get confused by random invisible objects
        int hoverMask = LayerMask.GetMask("Units", "Building", "Resource", "Terrain");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hoverMask)) 
        {
            GameObject hitObj = hit.transform.root.gameObject; // Look at the root parent!

            if (hitObj.CompareTag("Pawn"))
            {
                if (hoveredObject != hitObj)
                {
                    if (hoveredObject != null) hoveredObject.GetComponent<Selectable>()?.StopHover();
                    hoveredObject = hitObj;
                    hoveredObject.GetComponent<Selectable>()?.Hover();
                }
            }
            else if (hoveredObject != null)
            {
                hoveredObject.GetComponent<Selectable>()?.StopHover();
                hoveredObject = null;
            }
            
            // Place object at the hit point
            if (Placer != null)
            {
                Vector3 position = hit.point;
                position.x = Mathf.Floor(position.x / _gridSize) * _gridSize + _gridSize / 2.0f;
                position.z = Mathf.Floor(position.z / _gridSize) * _gridSize + _gridSize / 2.0f;
                Placer.transform.position = position;
            }
        }
    }

    public void OnPlayerClick()
    {  
        _brain.ManualUpdate();
        Ray ray = _brain.OutputCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        

        int layersToHit = LayerMask.GetMask("Units", "Building", "Resource");
        
        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Controlling:
                
                if (!Physics.SphereCast(ray, 0.5f, out hit, Mathf.Infinity, layersToHit)) return;
                
                // Always check the root object in case you hit a child mesh!
                GameObject hitObj = hit.transform.root.gameObject;
                Debug.Log($"Hit: {hitObj.name}");

                switch (hitObj.tag)
                {
                    case "Pawn":
                        if(objectType != ObjectType.Pawn) deselectAll();
                        objectType = ObjectType.Pawn;
                        selectObject(hitObj);
                        break;
                    case "Resource":
                        if(objectType != ObjectType.Resource) deselectAll();
                        objectType = ObjectType.Resource;
                        selectObject(hitObj);
                        break;
                }
                break;

            case PlayerState.PlacingBuilding:
                
                int terrainMask = LayerMask.GetMask("Terrain");
                if (!Physics.Raycast(ray, out hit, Mathf.Infinity, terrainMask)) return;
                
                placeBuilding(hit);
                playerState = PlayerState.Idle;
                break;

            case PlayerState.DragSelecting:
                break;
        }
    }
    
    public void selectObject(GameObject pawn)
    {
        // Removed the hoveredObject hijack! Now it strictly selects exactly what you clicked.
        Selectable p = pawn.GetComponent<Selectable>();
        
        if(p != null)
        {
            p.SelectDeselect();

            if (p.isSelected)
            {
                if(!selectedObjects.Contains(p.gameObject)) 
                {
                    selectedObjects.Add(p.gameObject);
                }
                playerState = PlayerState.Controlling;
            }
            else
            {
                if(selectedObjects.Contains(p.gameObject))
                {
                    selectedObjects.Remove(p.gameObject);
                }
                
                if(selectedObjects.Count == 0) playerState = PlayerState.Idle;
            }
        }
    }
    public void deselectAll()
    {
        selectedObjects.ForEach(p => p.GetComponent<Selectable>().Deselect());
        selectedObjects.Clear();
    }
    
    public void placeBuilding(RaycastHit hit)
    {
        // Place object at the hit point
        Vector3 position = hit.point;
        position.x = Mathf.Floor(position.x / _gridSize) * _gridSize + _gridSize / 2.0f;
        position.z = Mathf.Floor(position.z / _gridSize) * _gridSize + _gridSize / 2.0f;
        ObjectPlacer.PlaceObject(position, _obj);

        Placer.transform.parent = null;
        Destroy(Placer);
        
        Placer = Instantiate(DefualtPlacer, position, Quaternion.identity);
        Placer.transform.parent = transform;
        Placer.SetActive(false);
    }
    
    public void OnBuildingUI() {
        Vector3 position = Placer.transform.position;
        Placer.transform.parent = null;
        Destroy(Placer);
        
        Placer = Instantiate(_obj, position, Quaternion.identity);
        Placer.transform.parent = this.transform;
        Placer.SetActive(true);
        
        playerState = PlayerState.PlacingBuilding;
    }
    
    public void OnPlayerSecondaryClick()
    {
        Ray ray = _acCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(!Physics.Raycast(ray, out hit)) return;
        
        switch (playerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Controlling:
                switch (hit.transform.tag)
                {
                    case "Terrain":
                        CommandPawnTo(hit);
                        break;
                }
                break;
        }
    }

    public void CommandPawnTo(RaycastHit hit)
    {
        Vector3 initPos = hit.point;
        NavMeshHit navHit;

        float pawnOffset = 5;
        
        
        int rows = Mathf.CeilToInt(Mathf.Sqrt(selectedObjects.Count));
        int cols = Mathf.CeilToInt(selectedObjects.Count / (float)rows);
        
        initPos.z -= cols / 2.0f * pawnOffset;
        initPos.x -= rows / 2.0f * pawnOffset;
        
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            int row = Mathf.FloorToInt(i / (float)cols);
            int col = i % cols;
            
            Vector3 pos = new Vector3(initPos.x + row * pawnOffset, 0, initPos.z - col * pawnOffset);
            
            PawnController pawn = selectedObjects[i].GetComponent<PawnController>();
            
            
            MoveCommand moveCommand = new MoveCommand(pos);
            Debug.Log(pos);
            
            pawn.IssueCommand(moveCommand);
        }
        
        
    }
}
