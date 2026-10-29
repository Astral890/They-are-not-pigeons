using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemQtyText;
    public ItemData currentItem;

    public void SetItem(ItemData item)
    {
        currentItem = item;

        itemIcon.sprite = item.icon;
        itemIcon.enabled = true;

        itemQtyText.text = "x1";
        itemQtyText.enabled = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemIcon.enabled = false;
        itemQtyText.enabled = false;
    }
}
