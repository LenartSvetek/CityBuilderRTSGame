using System;
using Unity.Cinemachine;
using UnityEditor.SceneManagement;
using UnityEngine;

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
    
    private bool _isPlacingBuilding = false;
    
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
            // Place object at the hit point
            Vector3 position = hit.point;
            position.x = Mathf.Floor(position.x / _gridSize) * _gridSize + _gridSize / 2.0f;
            position.z = Mathf.Floor(position.z / _gridSize) * _gridSize + _gridSize / 2.0f;
            Placer.transform.position = position;
            
            
        }
    }

    public void OnPlayerClick()
    {
        if (!_isPlacingBuilding) return;

        Ray ray = _acCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits the terrain (or anything with a collider)
        if (Physics.Raycast(ray, out hit)) {
            // Place object at the hit point
            Vector3 position = hit.point;
            position.x = Mathf.Floor(position.x / _gridSize) * _gridSize + _gridSize / 2.0f;
            position.z = Mathf.Floor(position.z / _gridSize) * _gridSize + _gridSize / 2.0f;
            ObjectPlacer.PlaceObject(position, _obj);

            Placer.transform.parent = null;
            Destroy(Placer);
            
            Placer = Instantiate(DefualtPlacer, position, Quaternion.identity);
            Placer.transform.parent = transform;
        }
        _isPlacingBuilding = false;
    }
    
    public void OnBuildingUI() {
        Vector3 position = Placer.transform.position;
        Placer.transform.parent = null;
        Destroy(Placer);
        
        Placer = Instantiate(_obj, position, Quaternion.identity);
        Placer.transform.parent = this.transform;
        
        _isPlacingBuilding = true;
    }
}
