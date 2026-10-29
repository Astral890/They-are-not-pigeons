using UnityEngine;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private List<ItemData> shopItems;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemCardPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.showStore();
            PopulateStore(shopItems);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.hideStore();
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
}
