using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private TextMeshProUGUI cohesion;
    [SerializeField] private TextMeshProUGUI coherence;

    public void showPromptText(string text)
    {
        promptText.text = text;
        promptText.gameObject.SetActive(true);
    }

    public void hidePromptText()
    {
        promptText.gameObject.SetActive(false);
    }

    public void updateMoney(int value)
    {
        money.text = "$" + value.ToString();
    }

    public void updateCohesion(int value)
    {
        cohesion.text = "Cohesion: " + value.ToString();
    }

    public void updateCoherence(int value)
    {
        coherence.text = "Coherence: " + value.ToString();
    }
}
