using TMPro;
using UnityEngine;

public class FountainController : MonoBehaviour, InteractableE
{
    [SerializeField] private int availableCoins = 3;
    [SerializeField] private string promptMessage = "Presiona E para recoger moneda";
    private bool isPlayerInside = false;
    private marcoMovement playerRef;
    public GameManager gameManager;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colisoooon!!!!!");
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerRef = other.GetComponent<marcoMovement>();
            playerRef.SetCurrentInteractable(this);
            playerRef.UpdatePromptUI();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (playerRef != null)
            {
                playerRef.SetCurrentInteractable(null);
                playerRef.UpdatePromptUI();
                playerRef = null;
            }    
        }
    }

    public void Interact()
    {
        if (isPlayerInside && availableCoins > 0)
        {
            availableCoins--;
            gameManager.giveMoney(1);

            if (playerRef != null)
            {
                playerRef.UpdatePromptUI();
            }
        }
    }

    public string GetPromptMsg()
    {
        if (availableCoins > 0)
        {
            return promptMessage;
        }
        return "La fuente ya no tiene monedas";
    }
}