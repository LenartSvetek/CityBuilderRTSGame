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

        private Dictionary<string, int[]> resourceHistory = new Dictionary<string, int[]>();
        private int currentIndex = 0;
        private int totalSamples = 0;
        private float timer = 0f;
        private float refreshUITime = 0f;

        private Dictionary<string, float> resourceChange = new Dictionary<string, float>();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            orchestrator = Object.FindFirstObjectByType<OrchestratorScript>();

            ui = GetComponent<UIDocument>();

            orchestrator.OnResourceChange += UpdateUI;
            
            UpdateUI(orchestrator.resources);
        }

        private void Update()
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= 1f)
            {
                timer = 0f;

                // Loop through whatever resources exist right now
                foreach (var resource in orchestrator.cleanResources)
                {
                    if (!resourceHistory.ContainsKey(resource.resource.name))
                    {
                        resourceHistory[resource.resource.name] = new int[60];

                        for (int i = 0; i < 60; i++) resourceHistory[resource.resource.name][i] = 0;
                    }

                    resourceHistory[resource.resource.name][currentIndex] = resource.amount;
                }

                currentIndex = (currentIndex + 1) % 60;
                if (totalSamples < 60) totalSamples++;
            }

            refreshUITime += Time.unscaledDeltaTime;
            if(refreshUITime >= 60f)
            {
                UpdateUI(orchestrator.resources);
                refreshUITime = 0f;
            }
        }

        public float GetResourcesPerMinute(ResourceAmount resource)
        {
            if (totalSamples < 2) return 0f;

            int oldestIndex = (totalSamples == 60) ? currentIndex : 0;
            int latestIndex = (currentIndex - 1 + 60) % 60;

            float change = resourceHistory[resource.resource.name][latestIndex] - resourceHistory[resource.resource.name][oldestIndex];

            return (change / totalSamples) * 60f;
        }

        private void UpdateUI(List<ResourceAmount> resources)
        {
            var res = ui.rootVisualElement.Q("Resources");
            res.Clear();

            //foreach(var resource in orchestrator.cleanResources)
            //{
            //    Debug.Log($"Resource: {resource.resource.name}, Amount: {resource.amount}");
            //}

            foreach (ResourceAmount resource in resources)
            {
                var resChange = GetResourcesPerMinute(resource);

                var resourceElement = resourceTemplate.Instantiate();

                resourceElement.Q<VisualElement>("ResourcePic").style.backgroundImage = new StyleBackground(resource.resource.icon);
                resourceElement.Q<Label>("ResourceAmount").text = $"{resource.amount}/{resource.maxAmount} ({resChange:+0.##;-0.##;})";

                res.Add(resourceElement);
            }
        }
    }
}
