using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum BUILDING_TYPE {
    House,
    Utility,
    Resource
}

public class BuildingScript : MonoBehaviour {

    [SerializeField]
    private BUILDING_TYPE _type = BUILDING_TYPE.House;
    
    [SerializeField]
    private ResourceSO _resource;

    public ResourceSO resource
    {
        get => _resource;
        set => _resource = value;
    }
    
    public BUILDING_TYPE type {
        get => _type;
        set {
            _type = value;
        }
    }

    [SerializeField]
    private int _level = 1;

    [Range(1, 2)]
    public int level {
        get => _level;
        set {
            _level = value;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        updateBuilding();
    }

    public void DestroyAllChildren(GameObject parent) {
        // Use a temporary list to avoid modifying the collection while iterating
        var children = new List<Transform>();

        foreach (Transform child in parent.transform) {
            children.Add(child);
        }


        foreach (Transform child in children) {
            DestroyAllChildren(child.gameObject); 
        }
    }

    public void updateBuilding() {
        DestroyAllChildren(gameObject);


        if (_type == BUILDING_TYPE.Resource)
        {
            if(_resource == null) return;

            Addressables
                .LoadAssetsAsync<GameObject>(
                    new List<string>() { "Building", _type.ToString(), _resource.resourceName, $"level{_level}" },
                    null, Addressables.MergeMode.Intersection).Completed += OnComplete;
        }
        else
        {
            Addressables
                .LoadAssetsAsync<GameObject>(new List<string>() { "Building", _type.ToString(), $"level{_level}" },
                    null, Addressables.MergeMode.Intersection).Completed += OnComplete;
        }
    }

    private void OnComplete(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (transform.childCount != 0) return;
        
        if (handle.Status == AsyncOperationStatus.Succeeded) {

            if (handle.Result.Count > 0) {
                int rInd = Mathf.FloorToInt(Random.value * handle.Result.Count);

                GameObject obj = Instantiate(handle.Result[rInd], transform);
                obj.tag = _type.ToString();
                obj.transform.localPosition = Vector3.zero;
                obj.name = "Model";
            }

        }
    }



    // Update is called once per frame
    void Update() {

    }
}
