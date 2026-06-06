using UnityEngine;
using UnityEngine.UI;

public class HealthBarComp : MonoBehaviour
{
    private Transform camera;

    [SerializeField]
    HealthComp healthComp;
    [SerializeField ]
    Sprite healthSprite;

    Canvas canvas;
    Image healthBar;

    float verticalOffset = 0.5f;

    [SerializeField]
    float hideAfterAttack = 1;
    float hideAfterAttackTimer = 0;

    bool isVisible = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main.transform;

        healthComp = transform.root.GetComponent<HealthComp>();
        healthComp.OnTakingDamage += UpdateHealthBar;
        healthComp.OnHeal += UpdateHealthBar;

        GameObject canvasObj = new GameObject("BuildingCanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.enabled = false;

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
        healthBar.sprite = healthSprite;
        healthBar.color = Color.green; 

        healthBar.type = Image.Type.Filled;
        healthBar.fillMethod = Image.FillMethod.Horizontal;
        healthBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthBar.fillAmount = 1.0f; 

        
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        StretchUIElement(fillRect);

        UpdatePosition();
        UpdateHealthBar(healthComp);
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
        hideAfterAttackTimer -= Time.unscaledDeltaTime;
        if (hideAfterAttackTimer < 0) {
            canvas.enabled = false;    
            hideAfterAttackTimer = 0; 
        }

        if(healthComp.health < healthComp.maxHealth)
        {
            canvas.enabled = true;
        }

        if (camera != null)
        {
            canvas.transform.rotation = camera.rotation; 
        }
    }

    private void OnMouseOver()
    {
        if (!isVisible)
        {
            canvas.enabled = true;
        }
        hideAfterAttackTimer = hideAfterAttack;
    }

    void UpdatePosition()
    {
        Vector3 targetPosition = transform.position;

        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            targetPosition.y = collider.bounds.max.y + verticalOffset;
        }

        canvas.transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
    }

    void UpdateHealthBar(HealthComp comp)
    {
        if (!isVisible)
        {
            canvas.enabled = true;
        }
        hideAfterAttackTimer = hideAfterAttack;

        if (healthBar != null)
            healthBar.fillAmount = comp.health / comp.maxHealth;
    }
}
