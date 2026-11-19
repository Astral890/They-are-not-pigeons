using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SleepUI : MonoBehaviour
{
    [SerializeField] private Button nextDayButton;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI followersText;
    [SerializeField] private TextMeshProUGUI followersGain;
    [SerializeField] private TextMeshProUGUI followersLoss;
    [SerializeField] private GameManager gameManager;

    public void SetUpPanelSleep()
    {
        moneyText.text = gameManager.player.GetMoney().ToString();
        followersText.text = gameManager.player.GetFollowers().ToString();
        changeFollowersQty(gameManager.followersReward);
        nextDayButton.onClick.RemoveAllListeners();
        nextDayButton.onClick.AddListener(() => GoToNextDay());
    }

    private void changeFollowersQty(int amount)
    {
        //Debug.Log(amount.ToString());
        if(amount >= 0)
        {
            followersGain.text = "+ " + amount.ToString();
            followersLoss.text = "0";
        }
        else
        {
            followersLoss.text = amount.ToString();
            followersGain.text = "0";
        }
    }

    public void GoToNextDay()
    {
        Debug.Log("Click en el boton");
        gameManager.uiManager.SetDay();
        gameManager.uiManager.HideSleepPanel();
    }
}
