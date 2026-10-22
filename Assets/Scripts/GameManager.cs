using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int currentMoney = 0;
    public UIManager uiManager;

    public void addMoney(int amount)
    {
        currentMoney += amount;
        uiManager.updateMoney(currentMoney);
    }
}