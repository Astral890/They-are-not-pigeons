using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    public void Setup(ShopItems item, StoreManager storeManager)
    {
        itemIcon.sprite = item.icon;
        titleText.text = item.itemName;
        priceText.text = item.price.ToString();
        //buyButton.onClick.AddListener(() => storeManager.BuyItem(item));
    }
}
