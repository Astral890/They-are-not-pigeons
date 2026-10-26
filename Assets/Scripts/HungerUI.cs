using UnityEngine;
using UnityEngine.UI;

public class HungerUI : MonoBehaviour
{
    [SerializeField] private Image hungerBar;
    [SerializeField] private GameManager gameManager;
    private Sprite[] subSprites;

    private void Awake()
    {
        subSprites = Resources.LoadAll<Sprite>("Ui/Sprites/HungerBar");
    }

    public void UpdateBar(float hunger)
    {
        if (hunger <= 0f) {
            hungerBar.sprite = subSprites[3];
            return;
        }
        if (hunger <= 25f) {
            hungerBar.sprite = subSprites[0];
            return;
        }
        if (hunger <= 50f) {
            hungerBar.sprite = subSprites[1];
            return;
        }
        if (hunger <= 75f) {
            hungerBar.sprite = subSprites[2];
            return;
        }
        hungerBar.sprite = subSprites[4];
    }
}
