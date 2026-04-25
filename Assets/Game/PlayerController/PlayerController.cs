using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

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
    RTSCameraController _cameraController;

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

    private bool playerClicked = false;
    
    public List<GameObject> SelectedObjects
    {
        get { return selectedObjects; }
    }
    
    [SerializeField]
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerSettings = GetComponent<PlayerSettings>();
    }

    void LateUpdate() 
    {
        if(playerClicked) handlePlayerClick();
        
        Vector3 mousePos = _cameraController.GetCursorPosition;
        
        Ray ray = new Ray(mousePos + Vector3.up * 100f, Vector3.down);
        RaycastHit hit;

        
        
        int hoverMask = LayerMask.GetMask("Units", "Building", "Resource", "Terrain");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hoverMask)) 
        {
            GameObject hitObj = hit.transform.root.gameObject;

            if (Placer != null)
            {
                Vector3 position = hit.point;
                //position.x = Mathf.Floor(position.x / _gridSize) * _gridSize + _gridSize / 2.0f;
                //position.z = Mathf.Floor(position.z / _gridSize) * _gridSize + _gridSize / 2.0f;
                Placer.transform.position = position;
            }
        }
        
        playerClicked = false;
    }
    
    public void OnPlayerClick()
    {  
        playerClicked = true;
    }

    void handlePlayerClick()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Clicked on UI, ignoring.");
            return;
        }
        
        
        
        
        Ray ray = _cameraController.GetRay;
        RaycastHit hit;
        // Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.red, 1000);

        int layersToHit = LayerMask.GetMask("Units", "Building", "Resource");
        
        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Controlling:
                
                if (!Physics.SphereCast(ray, 0.5f, out hit, Mathf.Infinity, layersToHit)) return;
                
                // Always check the root object in case you hit a child mesh!
                GameObject hitObj = hit.transform.root.gameObject;
                Debug.Log($"Hit: {hitObj.name} | {hitObj.tag}");

                switch (hitObj.tag)
                { 
                    case "Terrain":
                        deselectAll();
                        break;
                    case "Pawn":
                        if(objectType != ObjectType.Pawn) deselectAll();
                        selectObject(hitObj);
                        break;
                    case "Resource":
                        if(objectType != ObjectType.Resource) deselectAll();
                        
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
        
        if(!Input.GetKey(KeyCode.LeftShift)) deselectAll();
        
        if(p != null)
        {
            SetObjectType(pawn);
            
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

    void SetObjectType(GameObject p)
    {
        switch (p.tag)
        {
            case "Pawn":
                objectType = ObjectType.Pawn;
                break;
            case "Resource":
                objectType = ObjectType.Resource;
                break;
            case "Building":
                objectType = ObjectType.Building;
                break; 
            default:
                objectType = ObjectType.Null;
                break;
        }
    }
    
    public void deselectAll()
    {
        selectedObjects.ForEach(p => p.GetComponent<Selectable>().Deselect());
        selectedObjects.Clear();
        objectType = ObjectType.Null;
        playerState = PlayerState.Idle;
    }
    
    public void placeBuilding(RaycastHit hit)
    {
        // Place object at the hit point
        Vector3 position = hit.point;
        //position.x = Mathf.Floor(position.x / _gridSize) * _gridSize + _gridSize / 2.0f;
        //position.z = Mathf.Floor(position.z / _gridSize) * _gridSize + _gridSize / 2.0f;
        //ObjectPlacer.PlaceObject(position, Placer);

        Placer.transform.parent = null;
        Placer.transform.position = position;
        Placer.GetComponentInChildren<BoxCollider>().enabled = true;
        Placer = null;
        Debug.Log("placing building at: " + position);
        
        Placer = Instantiate(DefualtPlacer, position, Quaternion.identity);
        Placer.transform.parent = transform;
        Placer.SetActive(false);
    }
    
    public void OnBuildingUI(GameObject buildingPrefab) {
        Vector3 position = Placer.transform.position;
        Placer.transform.parent = null;
        Destroy(Placer);
        
        Debug.Log(buildingPrefab.name);

        Placer = Instantiate(buildingPrefab, position, Quaternion.identity);
        Placer.transform.parent = this.transform;
        Placer.SetActive(true);
        
        Debug.Log("Placer set to: " + Placer.name);

        playerState = PlayerState.PlacingBuilding;
    }
    
    public void OnPlayerSecondaryClick()
    {
        Ray ray = _cameraController.GetRay;
        
        RaycastHit hit;
        
        if(!Physics.Raycast(ray, out hit)) return;
        
        switch (playerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Controlling:
                Debug.Log(hit.transform.tag);
                switch (hit.transform.tag)
                {
                    case "Terrain":
                        if(objectType == ObjectType.Pawn)
                            CommandPawnTo(hit);
                        break;
                    case "Resource":
                        if(objectType == ObjectType.Pawn)
                            CommandPawnGather(hit);
                        break;
                }
                break;
        }
    }

    void CommandPawnGather(RaycastHit hit)
    {
        Vector3 goTo = hit.point;

        float pawnOffset = 5;
        
        
        int rows = Mathf.CeilToInt(Mathf.Sqrt(selectedObjects.Count));
        int cols = Mathf.CeilToInt(selectedObjects.Count / (float)rows);
        
        goTo.z -= cols / 2.0f * pawnOffset;
        goTo.x -= rows / 2.0f * pawnOffset;
        
        for (var i = 0; i < selectedObjects.Count; i++)
        {
            int row = Mathf.FloorToInt(i / (float)cols);
            int col = i % cols;
            
            Vector3 pos = new Vector3(goTo.x + row * pawnOffset, 0, goTo.z - col * pawnOffset);
            
            PawnController pawn = selectedObjects[i].GetComponent<PawnController>();
            
            
            MoveCommand goToCommand = new MoveCommand(pos);
            MoveCommand backCommand = new MoveCommand(pawn.gameObject.transform.position);
            Debug.Log(pos);

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                pawn.ClearCommands();
                pawn.CyclicCommands(true);
                pawn.IssueCommands(new List<IPawnCommand>() { goToCommand, backCommand });
            }
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
            
            
            MoveCommand goToCommand = new MoveCommand(pos);
            
            Debug.Log(pos);

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                pawn.ClearCommands();
                pawn.IssueCommands(new List<IPawnCommand>() { goToCommand });
            }
        }
        
        
    }
}
