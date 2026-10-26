using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptMsg;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private HungerUI hungerUI;

    public void showPromptText(string text)
    {
        promptMsg.text = text;
        promptMsg.gameObject.SetActive(true);
    }

    public void hidePromptText()
    {
        promptMsg.gameObject.SetActive(false);
    }

    public void updateMoney(int value)
    {
        money.text =value.ToString();
    }

    public void BindPlayer(PlayerStats player)
    {
        player.OnHungerChanged += hungerUI.UpdateBar;
        hungerUI.UpdateBar(player.GetHunger());
    }
}
