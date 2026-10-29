using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemQtyText;

    public void Setup(ItemData item)
    {
        itemIcon.sprite = item.icon;
        itemIcon.enabled = true;
        itemQtyText.text = "1";
        itemQtyText.enabled = true;
    }
}
