using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int gainFollowers = 1000;
    public PlayerStats player;
    public PlayerInventory playerInventory;
    public UIManager uiManager;
    [SerializeField] private marcoMovement playerMovement;
    [SerializeField] private SpawAudience spawAudience;

    private void Awake()
    {
        uiManager.BindPlayer(player);
    }

    public void giveMoney(int amount)
    {
        player.AddMoney(amount);
        uiManager.updateMoney(player.GetMoney());
    }

    public void takeMoney(int amount)
    {
        player.RemoveMoney(amount);
        uiManager.updateMoney(player.GetMoney());
    }

    public void StartSpeech()
    {
        playerMovement.SetTransitionState(true);
        uiManager.SetNight();
        spawAudience.StartSpawningAudience();

    }

    public void EndSpeech()
    {
        playerMovement.SetTransitionState(false);
    }

    public void GiveFollowersBySpeech()
    {
        int currentFollowers = player.GetFollowers();
        int followersToAdd = 
    }
}