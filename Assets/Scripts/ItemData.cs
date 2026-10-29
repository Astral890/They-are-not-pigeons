using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/New Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public int price;
}
