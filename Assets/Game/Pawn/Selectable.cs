using Game.Libraries.Hoverable;
using UnityEngine;

public class Selectable : MonoBehaviour, IHoverable
{
    public GameObject selectionRing;
    public bool isSelected = false;

    public void SelectDeselect()
    {
        isSelected = !isSelected;
        if(selectionRing != null)
            selectionRing.SetActive(isSelected);
    }
    
    public void Select()
    {
        isSelected = true;
        if (selectionRing != null)
            selectionRing.SetActive(true);
        // highlight
    }

    public void Deselect()
    {
        if (selectionRing != null)
            selectionRing.SetActive(false);
        isSelected = false;
    }

    public void OnHoverEnter()
    {
        selectionRing.SetActive(true);
    }
    public void OnHoverExit()
    {
        if (isSelected) return;
        selectionRing.SetActive(false);
    }
}
