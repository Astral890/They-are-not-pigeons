using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private int initialSlots = 20;
    [SerializeField] private GameManager gameManager;
    private List<InventorySlot> activeSlots = new List<InventorySlot>();

    private void Start()
    {
        for (int i = 0; i < initialSlots; i++)
        {
            CreateNewSlot();
        }

        gameManager.playerInventory.OnInventoryUpdate += RefreshUI;
        RefreshUI();
    }

    private void CreateNewSlot()
    {
        GameObject newSlotObj = Instantiate(slotPrefab, content);
        InventorySlot slot = newSlotObj.GetComponent<InventorySlot>();
        slot.ClearSlot();
        activeSlots.Add(slot);
    }

    public void RefreshUI()
    {
        List<ItemData> dataSlots = gameManager.playerInventory.items;

        while(activeSlots.Count < dataSlots.Count)
        {
            CreateNewSlot();
        }

        for(int i = 0; i < activeSlots.Count; i++)
        {
            if(i < dataSlots.Count && dataSlots[i].id != null)
            {
                activeSlots[i].SetItem(dataSlots[i]);
            }
            else
            {
                activeSlots[i].ClearSlot();
            }
        }
    }
}
