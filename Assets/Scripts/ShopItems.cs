using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Items")]
public class ShopItems : ScriptableObject
{
    public string itemId;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public int price;
}
