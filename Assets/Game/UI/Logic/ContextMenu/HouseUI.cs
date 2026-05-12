using UnityEngine;
using UnityEngine.UIElements;

public class HouseUI : MonoBehaviour
{
    static VisualElement ui;
    static HouseComp PrevHouse = null;

    public static VisualElement buildUI(VisualTreeAsset template, HouseComp house)
    {
        ui = template.Instantiate().contentContainer;
            
        Label lbl = ui.Q<Label>("PopNum");
        if(lbl != null) lbl.text = house.Population + "/" + house.house.maxPopulation;
        
        if(PrevHouse != null) PrevHouse.OnHouseUpdate -= OnHouseChanged;
        house.OnHouseUpdate += OnHouseChanged;
        PrevHouse = house;

        return ui;
    }

    public static void OnHouseChanged(HouseComp house)
    {
        Label lbl = ui.Q<Label>("PopNum");
        if (lbl != null) lbl.text = house.Population + "/" + house.house.maxPopulation;
    }
}
