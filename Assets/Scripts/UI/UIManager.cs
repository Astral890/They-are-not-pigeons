using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptMsg;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private HungerUI hungerUI;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private marcoMovement player;
    [SerializeField] private GameObject InventoryPanel;

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

    public void showStore()
    {
        shopPanel.SetActive(true);
        player.SetMenuState(true);
    }

    public void hideStore()
    {
        shopPanel.SetActive(false);
        player.SetMenuState(false);
    }

    public void SetStoreVisibility(bool isVisible)
    {
        shopPanel.SetActive(isVisible);
    }

    public void OpenInventory(bool flag)
    {
        InventoryPanel.SetActive(flag);
    }
}
