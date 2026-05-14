using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Buildings : MonoBehaviour
{
    [SerializeField] OrchestratorScript orchestrator;
    [SerializeField] PlayerController playerController;

    [System.Serializable]
    public struct BuildingData {
        public string buttonName;
        public BuidlingSO SO;
    }

    public List<BuildingData> buildings;
    
    public VisualTreeAsset buttonTemplate;

    #region Tooltip
    public VisualTreeAsset tooltipTemplate;
    public VisualTreeAsset resourceTemplate;

    VisualElement tooltipInstance;
    BuidlingSO tooltipData;
    #endregion
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var container = root.Q<VisualElement>("Buildings");

        foreach (var item in buildings)
        {
            VisualElement instance = buttonTemplate.Instantiate();
            Button btn = instance.Q<Button>();

            btn.text = item.buttonName;
           
            btn.clicked += () => playerController.OnBuildingUI(item.SO);

            btn.RegisterCallback<MouseEnterEvent>(evt => {
                ShowTooltip(item.SO, btn);
            });
            
            btn.RegisterCallback<MouseLeaveEvent>(evt => {
                HideTooltip();
            });

            container.Add(instance);
        }

        orchestrator.OnResourceChange += OnResourceChange;
    }

    void ShowTooltip(BuidlingSO buidling, Button btn) {
        if (tooltipInstance != null) {
            tooltipInstance.RemoveFromHierarchy();
            tooltipData = null;
        }

        tooltipData = buidling;
        tooltipInstance = tooltipTemplate.Instantiate();
        tooltipInstance.Q<Label>("Name").text = buidling.name;
        tooltipInstance.style.position = Position.Absolute;
        tooltipInstance.style.left = btn.worldBound.x;
        tooltipInstance.style.bottom = Screen.height - btn.worldBound.y;

        var costContainer = tooltipInstance.Q<VisualElement>("Resources");

        foreach (var item in buidling.resourceCosts) { 
            var costInstance = resourceTemplate.Instantiate();

            costInstance.Q<VisualElement>("ResourcePic").style.backgroundImage = new StyleBackground(item.resource.icon);
            costInstance.Q<Label>("ResourceAmount").text = item.amount.ToString();
            if(!orchestrator.CheckResourceAvailability(item.resource, item.amount)) {
                costInstance.Q<Label>("ResourceAmount").style.color = Color.red;
            }

            costContainer.Add(costInstance);
        }

        GetComponent<UIDocument>().rootVisualElement.Add(tooltipInstance);
    }

    void HideTooltip() {
        if (tooltipInstance != null) {
            tooltipInstance.RemoveFromHierarchy();
            tooltipInstance = null;
            tooltipData = null;
        }
    }

    void OnResourceChange(List<ResourceAmount> resources)
    {
        if (tooltipInstance == null) return;

        var costContainer = tooltipInstance.Q<VisualElement>("Resources");
        costContainer.Clear();

        foreach (var item in tooltipData.resourceCosts)
        {
            var costInstance = resourceTemplate.Instantiate();

            costInstance.Q<VisualElement>("ResourcePic").style.backgroundImage = new StyleBackground(item.resource.icon);
            costInstance.Q<Label>("ResourceAmount").text = item.amount.ToString();
            if (!orchestrator.CheckResourceAvailability(item.resource, item.amount))
            {
                costInstance.Q<Label>("ResourceAmount").style.color = Color.red;
            }

            costContainer.Add(costInstance);
        }
    }
}
