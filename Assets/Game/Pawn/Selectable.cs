using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject selectionRing;
    public bool isSelected = false;

    public void SelectDeselect()
    {
        isSelected = !isSelected;
        if(selectionRing is not null)
            selectionRing.SetActive(isSelected);
    }
    
    public void Select()
    {
        isSelected = true;
        if (selectionRing is not null)
            selectionRing.SetActive(true);
        // highlight
    }

    public void Deselect()
    {
        if (selectionRing is not null)
            selectionRing.SetActive(false);
        isSelected = false;
    }
    
    public void Hover()
    {
        if(selectionRing is not null)
            selectionRing.SetActive(true);
    }

    public void StopHover()
    {
        if (isSelected) return;
        if(selectionRing is not null)
            selectionRing.SetActive(false);
    }
}
