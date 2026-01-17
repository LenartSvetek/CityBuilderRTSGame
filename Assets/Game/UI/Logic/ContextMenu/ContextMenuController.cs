using System;
using UnityEngine;

public class ContextMenuController : MonoBehaviour
{
    public ResourceUI resourceUI;
    public PlayerController player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.OnStateChanged += OnPlayerStateChanged;
    }

    void OnPlayerStateChanged(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Controlling:
                ShowUI();
                break;
            default:
                break;
        }
    }

    void ShowUI()
    {
        ObjectType objectType = player.objectType;

        switch (objectType)
        {
            case ObjectType.Pawn:
                break;
            case ObjectType.Building:
                break;
            case ObjectType.Resource:
                resourceUI.show(player.SelectedObjects[0]);
                break;
            case ObjectType.Null:
                resourceUI.hide();
                break;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
