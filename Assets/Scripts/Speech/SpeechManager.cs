using UnityEngine;

public class SpeechManager : MonoBehaviour, InteractableE
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    public bool speachAlready = false;
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
        if(isPlayerInside && !isSpeechActive && !speachAlready)
        {
            isSpeechActive = true;
            gameManager.uiManager.showPromptText("");
            gameManager.StartSpeech();
        }
    }
    public string GetPromptMsg()
    {
        string tmp = "";
        if (!speachAlready)
        {
            tmp = "Presiona E para empezar con el discurso";
        }
        else
        {
            tmp = "Ya diste un discurso";
        }
        return tmp;
    }

    public void ResetStatus()
    {
        isSpeechActive = false;
    }

    
}
