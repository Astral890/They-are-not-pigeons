using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerStats player;
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
}