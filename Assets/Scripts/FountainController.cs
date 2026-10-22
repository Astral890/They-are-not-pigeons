using TMPro;
using UnityEngine;

public class FountainController : MonoBehaviour
{
    [SerializeField] private int availableCoins = 3;
    [SerializeField] private string promptMessage = "Presiona E para recoger moneda";
    private bool isPlayerInside = false;
    private PlayerMovement playerRef;
    public GameManager gameManager;
    public UIManager uiManager;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colisoooon!!!!!");
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerRef = other.GetComponent<PlayerMovement>();
            playerRef.setCurrentFountain(this);

            if (availableCoins > 0)
            {
                uiManager.showPromptText(promptMessage);
            }
            else
            {
                uiManager.showPromptText("La fuente ya no tiene monedas");
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (playerRef != null)
            {
                playerRef.setCurrentFountain(null);
                playerRef = null;
            }
            uiManager.hidePromptText();
        }
    }

    public void Interact()
    {
        if(isPlayerInside&& availableCoins > 0)
        {
            availableCoins--;
            gameManager.addMoney(1);
            if (availableCoins <= 0)
            {
                uiManager.showPromptText("La fuente ha quedado vacia");
            }
        }
    }
}