using UnityEngine;
using UnityEngine.UIElements;

public class HouseUI : MonoBehaviour
{

    public static VisualElement buildUI(VisualTreeAsset template, HouseComp house)
    {
        var ui = template.Instantiate().contentContainer;
            
        Label lbl = ui.Q<Label>("PopNum");
        if(lbl != null) lbl.text = house.population + "/" + house.house.maxPopulation;
        
        return ui;
    }
}
