using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] public List<ItemData> items = new List<ItemData>();
    public event Action OnInventoryUpdate;

    public void AddItem(ItemData newItem)
    {
        items.Add(newItem);
        OnInventoryUpdate?.Invoke();
    }

    public void RemoveItem(ItemData removeItem)
    {
        items.Remove(removeItem);
        OnInventoryUpdate?.Invoke();
    }
}
