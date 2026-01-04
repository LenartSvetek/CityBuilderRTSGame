using System;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject selectionRing;
    public bool isSelected = false;

    public void SelectDeselect()
    {
        isSelected = !isSelected;
        selectionRing.SetActive(isSelected);
    }
    
    public void Select()
    {
        isSelected = true;
        selectionRing.SetActive(true);
        // highlight
    }

    public void Deselect()
    {
        selectionRing.SetActive(false);
        isSelected = false;
    }
    
    public void Hover()
    {
        selectionRing.SetActive(true);
    }

    public void StopHover()
    {
        if (isSelected) return;
        selectionRing.SetActive(false);
    }
}
