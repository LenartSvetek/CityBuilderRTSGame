using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] Image image;
    public TMP_Text name;
    public TMP_Text quantity;

    public void show(GameObject obj)
    {
        ResourceAmount amount = obj.GetComponent<ResourceNode>().amount;
        image.GetComponent<Image>().sprite = amount.resource.icon;
        name.text = amount.resource.name;
        quantity.text = $"{amount.amount}/{amount.maxAmount}";
        
        gameObject.SetActive(true);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }
}
