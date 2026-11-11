using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SleepUI : MonoBehaviour
{
    [SerializeField] private Button nextDayButton;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI followersText;
    [SerializeField] private GameManager gameManager;

    public void SetUpPanelSleep()
    {
        moneyText.text = gameManager.player.GetMoney().ToString();
        followersText.text = gameManager.player.GetFollowers().ToString();
        nextDayButton.onClick.RemoveAllListeners();
        nextDayButton.onClick.AddListener(() => GoToNextDay());
    }

    private void GoToNextDay()
    {
        gameManager.uiManager.SetDay();
        gameManager.uiManager.HideSleepPanel();
    }
}
