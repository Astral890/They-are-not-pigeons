using UnityEngine;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour, InteractableE
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private List<ItemData> shopItems;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemCardPrefab;
    private bool isPlayerInside = false;
    private marcoMovement playerRef;
    private bool isStoreOpen = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerRef = other.GetComponent<marcoMovement>();
            playerRef.SetCurrentInteractable(this);
            playerRef.UpdatePromptUI();
        }
        PopulateStore(shopItems);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerRef.SetCurrentInteractable(null);
            playerRef.UpdatePromptUI();
            playerRef = null;
            isStoreOpen = false;
            uiManager.SetStoreVisibility(isStoreOpen);
        }
    }

    private void PopulateStore(List<ItemData> items)
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in shopItems)
        {
            GameObject card = Instantiate(itemCardPrefab, itemsContainer);
            card.GetComponent<ShopItemUI>().Setup(item, this);
        }
    }

    public void TryBuyItem(ItemData item)
    {
        if(gameManager.player.GetMoney() >= item.price)
        {
            gameManager.takeMoney(item.price);
            gameManager.playerInventory.AddItem(item);
        }
        else
        {
            Debug.Log("No hay dineros " + item.itemName);
        }
    }

    public void Interact()
    {
        Debug.Log("Interacting with the store");
        if (isPlayerInside && !isStoreOpen)
        {
            isStoreOpen = true;
            uiManager.SetStoreVisibility(isStoreOpen);
            playerRef.SetMenuState(isStoreOpen);
        }
        else
        {
            isStoreOpen = false;
            uiManager.SetStoreVisibility(isStoreOpen);
            playerRef.SetMenuState(isStoreOpen);
        }
    }

    public string GetPromptMsg()
    {
        return "Presiona E para abrir la tienda";
    }
}
