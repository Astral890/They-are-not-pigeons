using UnityEngine;

public class SpeechManager : MonoBehaviour, InteractableE
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    private bool isPlayerInside = false;
    private marcoMovement playerRef;
    private bool isSpeechActive = false;

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
        Debug.Log("Interacting with SpeechManager");
        if(isPlayerInside && !isSpeechActive)
        {
            isSpeechActive = true;
            gameManager.StartSpeech();
            uiManager.showPromptText("Discurso iniciado");
        }
    }
    public string GetPromptMsg()
    {
        return "Presiona E para empezar con el discurso";
    }

    
}
