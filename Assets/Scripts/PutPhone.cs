using UnityEngine;

public class PutPhone : MonoBehaviour, InteractableE
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject phoneOnTripod;
    private bool isPlayerInside = false;
    private marcoMovement playerRef;

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
        string tmp = "";
        if (gameManager.playerHasPhone)
        {
            tmp = "Pulsa E para colocar el teléfono";
        }
        else
        {
            tmp = "Recoge el teléfono del arbol";
        }
        return tmp;
    }

    public void Interact()
    {
        if (!gameManager.playerHasPhone) return;
        phoneOnTripod.SetActive(true);
        playerRef.SetCurrentInteractable(null);
        playerRef.UpdatePromptUI();
        Destroy(this, 2f);
    }
}
