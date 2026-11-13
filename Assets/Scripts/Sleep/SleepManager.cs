using UnityEngine;

public class SleepManager : MonoBehaviour, InteractableE
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private marcoMovement playerRef;
    [SerializeField] private SleepUI sleepUI;
    private bool isPlayerInside = false;
    private bool isSleepPanelOpen = false;

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
            playerRef.canOpenInventory = true;
            playerRef = null;
            isSleepPanelOpen = false;
            gameManager.uiManager.HideSleepPanel();
        }
    }

    public void Interact()
    {
        Debug.Log("Interacting with the store");
        if (isPlayerInside && !isSleepPanelOpen)
        {
            if (!gameManager.isNight)
            {
                gameManager.uiManager.showPromptText("No puedes dormir durante el día");
                return;
            }
            isSleepPanelOpen = true;
            sleepUI.SetUpPanelSleep();
            gameManager.uiManager.ShowSleepPanel();
            playerRef.SetMenuState(isSleepPanelOpen);
            playerRef.canOpenInventory = false;
        }
        else
        {
            isSleepPanelOpen = false;
            gameManager.uiManager.HideSleepPanel();
            playerRef.SetMenuState(isSleepPanelOpen);
            playerRef.canOpenInventory = true;
        }
    }

    public string GetPromptMsg()
    {
        return "Pulsa E para ir a dormir";
    }
}
