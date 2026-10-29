using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerStats player;
    public UIManager uiManager;

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
}