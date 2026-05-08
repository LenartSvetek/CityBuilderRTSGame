using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenuController : MonoBehaviour
{
    public UIDocument HUD;
    public PlayerController player;
    
    [Header("Components")]
    public VisualTreeAsset HouseUI;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.OnStateChanged += OnPlayerStateChanged;
        HUD = GetComponent<UIDocument>();
    }

    void OnPlayerStateChanged(PlayerState state)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        ObjectType objectType = player.objectType;
        Debug.Log("Yooo");
        Debug.Log(objectType.ToString());
        switch (objectType)
        {
            case ObjectType.Pawn:
                break;
            case ObjectType.Building:
                HouseComp obj = player.SelectedObjects[0].GetComponent<HouseComp>();
                if (obj.CompareTag("House"))
                {
                    Debug.Log(obj.name);
                    var ui = global::HouseUI.buildUI(HouseUI, obj);
                    HUD.rootVisualElement.Q("ContexMenu").Add(ui);
                }
                break;
            case ObjectType.Resource:
                break;
            case ObjectType.Null:
                HUD.rootVisualElement.Q("ContexMenu").Clear();
                break;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
