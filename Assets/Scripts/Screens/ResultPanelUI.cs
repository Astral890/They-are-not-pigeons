using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MatchOutcome
{
    HungerDefeat,
    FollowersDefeat,
    Victory
}

public sealed class ResultPanelUI : MonoBehaviour
{
    [Header("Fondo")]
    [SerializeField] private Image background;
    [SerializeField] private Sprite hungerBackground;
    [SerializeField] private Sprite followersBackground;
    [SerializeField] private Sprite victoryBackground;

    [Header("Textos")]
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text finalMoney;
    [SerializeField] private TMP_Text finalFollowers;
    [SerializeField] private TMP_Text daysPlayed;

    public void Show(MatchOutcome outcome, int money, int followers, int days)
    {
        switch (outcome)
        {
            case MatchOutcome.HungerDefeat:
                background.sprite = hungerBackground;
                title.text = "DERROTA";
                description.text = "Te quedaste sin dinero y el hambre acabó con tu partida.";
                break;

            case MatchOutcome.FollowersDefeat:
                background.sprite = followersBackground;
                title.text = "DERROTA";
                description.text = "La plaza dejó de escucharte. Perdiste el apoyo de tus seguidores.";
                break;

            case MatchOutcome.Victory:
                background.sprite = victoryBackground;
                title.text = "VICTORIA";
                description.text = "Tus teorías conquistaron la plaza.";
                break;

            default:
                return;
        }

        finalMoney.text = $"${money:N0}";
        finalFollowers.text = followers.ToString("N0");
        daysPlayed.text = days.ToString("N0");

        gameObject.SetActive(true);
    }

    [ContextMenu("Probar/Derrota por hambre")]
    private void PreviewHungerDefeat()
    {
        Show(MatchOutcome.HungerDefeat, 0, 38, 4);
    }

    [ContextMenu("Probar/Derrota por falta de cohesión")]
    private void PreviewFollowersDefeat()
    {
        Show(MatchOutcome.FollowersDefeat, 125, 0, 5);
    }

    [ContextMenu("Probar/Victoria")]
    private void PreviewVictory()
    {
        Show(MatchOutcome.Victory, 350, 120, 7);
    }
}