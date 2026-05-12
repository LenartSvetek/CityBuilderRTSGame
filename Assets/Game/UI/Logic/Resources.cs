using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Logic
{
    public class Resources : MonoBehaviour
    {
        UIDocument ui;
        OrchestratorScript orchestrator;
        public VisualTreeAsset resourceTemplate;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            orchestrator = Object.FindFirstObjectByType<OrchestratorScript>();

            ui = GetComponent<UIDocument>();

            orchestrator.OnResourceChange += UpdateUI;
            
            UpdateUI(orchestrator.resources);
        }

        private void UpdateUI(List<ResourceAmount> resources)
        {
            var res = ui.rootVisualElement.Q("Resources");
            res.Clear();

            foreach (ResourceAmount resource in resources)
            {
                var resourceElement = resourceTemplate.Instantiate();

                resourceElement.Q<VisualElement>("ResourcePic").style.backgroundImage = new StyleBackground(resource.resource.icon);
                resourceElement.Q<Label>("ResourceAmount").text = resource.amount + "/" + resource.maxAmount;

                res.Add(resourceElement);
            }
        }
    }
}
