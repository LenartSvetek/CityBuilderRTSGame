using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerInputState
{
    Idle,
    Commanding,
    PlacingBuilding,
    DragSelecting
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
    
    private PlayerInputState _inputState = PlayerInputState.Idle;
    
    private Selectable howeredPawn = null;
    
    [SerializeField]
    private List<PawnController> controlledPawns = new List<PawnController>();
    
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

    void FixedUpdate() {
        
        // When you left-click
        Ray ray = _acCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits the terrain (or anything with a collider)
        if (Physics.Raycast(ray, out hit)) {
            if ( hit.transform.CompareTag("Pawn") && howeredPawn is null)
            {
                howeredPawn = hit.transform.GetComponent<Selectable>();
                howeredPawn.Hover();
            }
            else if (!hit.transform.CompareTag("Pawn") && howeredPawn is not null)
            {
                howeredPawn.StopHover();
                howeredPawn = null;
            }
            
            // Place object at the hit point
            Vector3 position = hit.point;
            position.x = Mathf.Floor(position.x / _gridSize) * _gridSize + _gridSize / 2.0f;
            position.z = Mathf.Floor(position.z / _gridSize) * _gridSize + _gridSize / 2.0f;
            Placer.transform.position = position;

            
        }
    }

    public void OnPlayerClick()
    {
        Ray ray = _acCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(!Physics.Raycast(ray, out hit)) return;
        
        switch (_inputState)
        {
            case PlayerInputState.Idle:
                switch (hit.transform.tag)
                {
                    case "Pawn":
                        selectPawn(hit.transform.gameObject);
                        break;
                }
                break;
            case PlayerInputState.Commanding:
                switch (hit.transform.tag)
                {
                    case "Pawn":
                        selectPawn(hit.transform.gameObject);
                        break;
                }
                break;
            case PlayerInputState.PlacingBuilding:
                placeBuilding(hit);
                _inputState = PlayerInputState.Idle;
                break;
            case PlayerInputState.DragSelecting:
                break;
        }

    }
    
    public void selectPawn(GameObject pawn)
    {
        Selectable p = null;
        if(howeredPawn != null) p = howeredPawn;
        else p = pawn.GetComponent<Selectable>();
        
        p.SelectDeselect();

        if (p.isSelected)
        {
            if(controlledPawns.Exists(x => x.gameObject == p.gameObject)) return;
            controlledPawns.Add(p.gameObject.GetComponent<PawnController>());
            _inputState = PlayerInputState.Commanding;
        }
        else
        {
            if(!controlledPawns.Exists(x => x.gameObject == p.gameObject)) return;
            controlledPawns.Remove(p.gameObject.GetComponent<PawnController>());
            
            if(controlledPawns.Count == 0) _inputState = PlayerInputState.Idle;
        }
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
        
        _inputState = PlayerInputState.PlacingBuilding;
    }
    
    public void OnPlayerSecondaryClick()
    {
        Ray ray = _acCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(!Physics.Raycast(ray, out hit)) return;
        
        switch (_inputState)
        {
            case PlayerInputState.Idle:
                break;
            case PlayerInputState.Commanding:
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
        Vector3 position = hit.point;
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(position, out navHit, 2f, NavMesh.AllAreas))
        {
            position = navHit.position;
        }
        
        foreach (PawnController pawn in controlledPawns)
        {
            MoveCommand moveCommand = new MoveCommand(position);
            Debug.Log(position);
            
            pawn.IssueCommand(moveCommand);
        }
    }
}
