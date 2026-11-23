using UnityEngine;
using UnityEngine.UI;

public class CohesionUI : MonoBehaviour
{
    [SerializeField] private Image cohesionBar;
    [SerializeField] private GameManager gameManager;
    private Sprite[] subSprites;

    private void Awake()
    {
        subSprites = Resources.LoadAll<Sprite>("Ui/Sprites/CohesionBar");
    }

    public void UpdateBar(int cohesion)
    {
        if (cohesion <= 0) {
            cohesionBar.sprite = subSprites[3];
            return;
        }
        if (cohesion <= 25) {
            cohesionBar.sprite = subSprites[0];
            return;
        }
        if (cohesion <= 50) {
            cohesionBar.sprite = subSprites[1];
            return;
        }
        if (cohesion <= 75) {
            cohesionBar.sprite = subSprites[2];
            return;
        }
        cohesionBar.sprite = subSprites[4];
    }
}
