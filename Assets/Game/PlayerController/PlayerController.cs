using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerState
{
    Idle,
    Controlling,
    PlacingBuilding,
    DragSelecting,
    Destroying
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
            case PlayerState.Destroying:
                playerState = PlayerState.Idle;
                int buildingMask = LayerMask.GetMask("Building");
                if (!Physics.Raycast(ray, out hit, Mathf.Infinity, buildingMask)) return;

                if (hit.collider.gameObject.tag.CompareTo("Outpost") == 0) return;

                Destroy(hit.collider.gameObject);
                break;
        }
    }
    
    public void SetDestroySet()
    {
        playerState = PlayerState.Destroying;
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
        if (currHoldBuilding == null) return;
        if (Placer.GetComponent<PlacingComp>().canPlace != true) return;
        if(!_orchestrator.ApplyResourceCost(currHoldBuilding.resourceCosts)) return;

       
        Destroy(Placer.GetComponent<PlacingComp>());

        if (Placer.tag == "House") _orchestrator.RegisterBuilding(Placer.GetComponent<HouseComp>());
        if (Placer.tag == "Building") _orchestrator.RegisterBuilding(Placer.GetComponent<WorkshopComp>());
        if (Placer.tag == "Tower") _orchestrator.RegisterDependent(Placer.GetComponent<HealthComp>());
        Placer = null;
        

        currHoldBuilding = null;
        playerState = PlayerState.Idle;
    }

    BuidlingSO currHoldBuilding = null;
    public void OnBuildingUI(BuidlingSO buildingSO) {
        if (currHoldBuilding) { Destroy(Placer); currHoldBuilding = null; }
        if(!_orchestrator.CheckResourceCost(buildingSO.resourceCosts)) return;

        currHoldBuilding = buildingSO;

        Placer = Instantiate(buildingSO.prefab, transform.position, Quaternion.identity);

        Placer.AddComponent<PlacingComp>();

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
