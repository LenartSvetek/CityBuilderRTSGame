using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Buildings : MonoBehaviour
{
    [SerializeField] OrchestratorScript orchestrator;
    
    [System.Serializable]
    public struct BuildingData {
        public string buttonName;
        public GameObject prefab;
    }

    public List<BuildingData> buildings;
    
    public VisualTreeAsset buttonTemplate;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var container = root.Q<VisualElement>("Buildings");

        foreach (var item in buildings)
        {
            VisualElement instance = buttonTemplate.Instantiate();
            Button btn = instance.Q<Button>();

            btn.text = item.prefab.name;
            btn.clicked += () => orchestrator.OnBuildingBTN(item.prefab);

            container.Add(instance);
        }
    }
}
