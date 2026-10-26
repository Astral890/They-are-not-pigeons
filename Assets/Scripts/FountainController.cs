using TMPro;
using UnityEngine;

public class FountainController : MonoBehaviour
{
    [SerializeField] private int availableCoins = 3;
    [SerializeField] private string promptMessage = "Presiona E para recoger moneda";
    private bool isPlayerInside = false;
    private marcoMovement playerRef;
    public GameManager gameManager;
    public UIManager uiManager;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colisoooon!!!!!");
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerRef = other.GetComponent<marcoMovement>();
            playerRef.SetCurrentFountain(this);

            if (availableCoins > 0)
            {
                uiManager.showPromptText(promptMessage);
            }
            else
            {
                uiManager.showPromptText("La fuente ya no tiene monedas");
            }
        }
        else
        {
            Debug.Log("No es la tag");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (playerRef != null)
            {
                playerRef.SetCurrentFountain(null);
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
            gameManager.giveMoney(1);
            if (availableCoins <= 0)
            {
                uiManager.showPromptText("La fuente ha quedado vacia");
            }
        }
    }
}