using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject selectionRing;
    public bool isSelected;

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
}
