using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TakePhone : MonoBehaviour, InteractableE
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject phoneOnTree;
    private bool isPlayerInside = false;
    private marcoMovement playerRef;
    private bool hasPhone = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerRef = other.GetComponent<marcoMovement>();
            playerRef.SetCurrentInteractable(this);
            playerRef.UpdatePromptUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerRef.SetCurrentInteractable(null);
            playerRef.UpdatePromptUI();
            playerRef = null;
            
        }
    }

    public string GetPromptMsg()
    {

        return "Pulsa E para tomar el teléfono";
    }

    public void Interact()
    {
        if(isPlayerInside && hasPhone)
        {
            gameManager.playerHasPhone = true;
            phoneOnTree.SetActive(false);
            playerRef.SetCurrentInteractable(null);
            playerRef.UpdatePromptUI();
            Destroy(this, 2f);
        }
    }
}
