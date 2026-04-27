using UnityEngine;
using UnityEngine.UIElements;

public class HouseUI : MonoBehaviour
{

    public static VisualElement buildUI(VisualTreeAsset template, HouseSO house)
    {
        var ui = template.Instantiate().contentContainer;
        
        
        return ui;
    }
}
