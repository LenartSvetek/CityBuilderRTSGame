using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerState
{
    Idle,
    Controlling,
    PlacingBuilding,
    DragSelecting
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
    
    OrchestratorScript _orchestrator;

    [SerializeField]
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerSettings = GetComponent<PlayerSettings>();
        _orchestrator = UnityEngine.Object.FindFirstObjectByType<OrchestratorScript>();
    }

    void LateUpdate() 
    {
        if(playerClicked) handlePlayerClick();
        
        Vector3 mousePos = _cameraController.GetCursorPosition;
        
        Ray ray = new Ray(mousePos + Vector3.up * 100f, Vector3.down);
        RaycastHit hit;

        var layersAll = new List<String>() { "Units", "Building", "Resource", "Terrain" };
        var layers = new List<String>{};
        if(Placer == null) layers.AddRange(layersAll);
        else layers.Add("Terrain");
        int hoverMask = LayerMask.GetMask(layers.ToArray());

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

        int layersToHit = LayerMask.GetMask("Units", "Building", "Resource", "Terrain");
        
        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Controlling:
                
                if (!Physics.SphereCast(ray, 0.5f, out hit, Mathf.Infinity, layersToHit)) return;
                
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
                    case "Building":
                    case "House":
                        if(objectType != ObjectType.Building) deselectAll();
                        
                        selectObject(hitObj);
                        playerState = PlayerState.Controlling;
                        break;
                }
                break;

            case PlayerState.PlacingBuilding:
                
                int terrainMask = LayerMask.GetMask("Terrain");
                if (!Physics.Raycast(ray, out hit, Mathf.Infinity, terrainMask)) return;
                
                placeBuilding(hit);
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
        Debug.Log($"SetObjectType: {p.tag}");
        switch (p.tag)
        {
            case "Pawn":
                objectType = ObjectType.Pawn;
                break;
            case "Resource":
                objectType = ObjectType.Resource;
                break;
            case "House":
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
        if (Placer.GetComponent<PlacingComp>().canPlace != true) return;
        if(!_orchestrator.ApplyResourceCost(currHoldBuilding.resourceCosts)) return;

        // Place object at the hit point
        Vector3 position = hit.point;
        //position.x = Mathf.Floor(position.x / _gridSize) * _gridSize + _gridSize / 2.0f;
        //position.z = Mathf.Floor(position.z / _gridSize) * _gridSize + _gridSize / 2.0f;
        //ObjectPlacer.PlaceObject(position, Placer);

        Placer.transform.parent = null;
        Placer.transform.position = position;
        

        Destroy(Placer.GetComponent<PlacingComp>());

        Debug.Log("Placed building tag: " + Placer.tag);
        if (Placer.tag == "House") _orchestrator.RegisterBuilding(Placer.GetComponent<HouseComp>());
        if (Placer.tag == "Building") _orchestrator.RegisterBuilding(Placer.GetComponent<WorkshopComp>());

        Placer = null;
        Debug.Log("placing building at: " + position);
        
        Placer = Instantiate(DefualtPlacer, position, Quaternion.identity);
        Placer.transform.parent = transform;
        Placer.SetActive(false);

        currHoldBuilding = null;
        playerState = PlayerState.Idle;
    }

    BuidlingSO currHoldBuilding = null;
    public void OnBuildingUI(BuidlingSO buildingSO) {
        if(!_orchestrator.CheckResourceCost(buildingSO.resourceCosts)) return;

        currHoldBuilding = buildingSO;

        Vector3 position = Placer.transform.position;
        Placer.transform.parent = null;
        Destroy(Placer);
        
        Debug.Log("Prefab: " + buildingSO.name);

        Placer = Instantiate(buildingSO.prefab, position, Quaternion.identity);
        Placer.transform.parent = this.transform;

        Placer.AddComponent<PlacingComp>();

        Placer.SetActive(true);
        
        Debug.Log("Placer set to: " + Placer.name);

        playerState = PlayerState.PlacingBuilding;
    }
    
    public void OnPlayerSecondaryClick()
    {
        //Ray ray = _cameraController.GetRay;
        
        //RaycastHit hit;
        
        //if(!Physics.Raycast(ray, out hit)) return;
        
        //switch (playerState)
        //{
        //    case PlayerState.Idle:
        //        break;
        //    case PlayerState.Controlling:
        //        Debug.Log(hit.transform.tag);
        //        switch (hit.transform.tag)
        //        {
        //            case "Terrain":
        //                if(objectType == ObjectType.Pawn)
        //                    CommandPawnTo(hit);
        //                break;
        //            case "Resource":
        //                if(objectType == ObjectType.Pawn)
        //                    CommandPawnGather(hit);
        //                break;
        //        }
        //        break;
        //}
    }
}
