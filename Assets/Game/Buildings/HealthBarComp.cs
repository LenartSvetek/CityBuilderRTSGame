using UnityEngine;
using UnityEngine.UI;

public class HealthBarComp : MonoBehaviour
{
    private Transform camera;

    BuildingComp buildingComp;

    Canvas canvas;
    Image healthBar;

    float verticalOffset = 0.5f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main.transform;

        buildingComp = GetComponent<BuildingComp>();
        buildingComp.OnBuildingTakeDamage += UpdateHealthBar;

        GameObject canvasObj = new GameObject("BuildingCanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        var rectTransform = canvas.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(10, 1); 

        canvas.renderMode = RenderMode.WorldSpace;

        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        StretchUIElement(bgRect);

        

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(canvasObj.transform, false);

        healthBar = fillObj.AddComponent<Image>();
        healthBar.color = Color.green; 

        healthBar.type = Image.Type.Filled;
        healthBar.fillMethod = Image.FillMethod.Horizontal;
        healthBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthBar.fillAmount = 1.0f; 

        
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        StretchUIElement(fillRect);

        UpdatePosition();
        UpdateHealthBar(buildingComp);
    }

    void StretchUIElement(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (camera != null)
        {
            canvas.transform.rotation = camera.rotation; 
        }
    }

    void UpdatePosition()
    {
        Vector3 targetPosition = transform.position;

        var collider = GetComponentInChildren<Collider>();
        if (collider != null)
        {
            targetPosition.y = collider.bounds.max.y + verticalOffset;
        }

        canvas.transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
    }

    void UpdateHealthBar(BuildingComp comp)
    {
        Debug.Log("Building attacked");
        if (healthBar != null)
            healthBar.fillAmount = comp.health / comp.maxHealth;

        LayoutRebuilder.ForceRebuildLayoutImmediate(healthBar.rectTransform);
        Debug.Log("Fill amount: " + healthBar.fillAmount);
    }
}
