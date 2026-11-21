using UnityEngine;
using System.Collections.Generic;

public class StoreTheories : MonoBehaviour, InteractableE
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] public List<ConceptInfo> shopConcepts;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemConceptPrefab;
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
        PopulateStore(shopConcepts);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerRef.SetCurrentInteractable(null);
            playerRef.UpdatePromptUI();
            if (isStoreOpen) playerRef.SetMenuState(false);
            playerRef.canOpenInventory = true;
            playerRef = null;
            isStoreOpen = false;
            uiManager.SetStoreThoeriesVisibility(isStoreOpen);
        }
    }

    private void PopulateStore(List<ConceptInfo> items)
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in shopConcepts)
        {
            GameObject card = Instantiate(itemConceptPrefab, itemsContainer);
            card.GetComponent<ShopTheoryUI>().Setup(item, this);
        }
    }

    public void TryBuyItem(ConceptInfo concept)
    {
        if(gameManager.player.GetMoney() >= concept.cost)
        {
            gameManager.takeMoney(concept.cost);
            gameManager.player.conceptInfos.Add(concept);
            shopConcepts.Remove(concept);
            PopulateStore(shopConcepts);
        }
        else
        {
            Debug.Log("No hay dineros para el concepto");
        }
    }

    public void Interact()
    {
        Debug.Log("Interacting with the store");
        if (isPlayerInside && !isStoreOpen)
        {
            isStoreOpen = true;
            uiManager.SetStoreThoeriesVisibility(isStoreOpen);
            playerRef.SetMenuState(isStoreOpen);
            playerRef.canOpenInventory = false;
        }
        else
        {
            isStoreOpen = false;
            uiManager.SetStoreThoeriesVisibility(isStoreOpen);
            playerRef.SetMenuState(isStoreOpen);
            playerRef.canOpenInventory = true;
        }
    }

    public string GetPromptMsg()
    {
        return "Presiona E para abrir la tienda";
    }
}
