using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject sleepPanel;
    [SerializeField] private TextMeshProUGUI followersText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI promptMsg;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private HungerUI hungerUI;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private marcoMovement player;
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private GameObject theoryPanel;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private float transitionDuration = 10f;
    private Vector3 dayRotation = new Vector3(50f, -30f, 0f);
    private Vector3 nightRotation = new Vector3(-27.863f, -21.318f, 34.195f);
                                                //nuevos -27.863, -21.318, 34.195
                                                //viejos -14.251, -19.366, 28.804
    private Coroutine nightTransitionCoroutine;
    private Coroutine dayTransitionCorutine;

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

    public void updateFollowers(int value)
    {
        followersText.text = "F: " + value.ToString();
    }

    public void BindPlayer(PlayerStats player)
    {
        player.OnHungerChanged += hungerUI.UpdateBar;
        hungerUI.UpdateBar(player.GetHunger());
    }

    public void ShowSleepPanel()
    {
        sleepPanel.SetActive(true);
        player.SetMenuState(true);
    }

    public void HideSleepPanel()
    {
        sleepPanel.SetActive(false);
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

    public void SetTheoryVisibility(bool isVisible)
    {
        theoryPanel.SetActive(isVisible);
    }

    public void SetNight()
    {
        if (nightTransitionCoroutine != null)
        {
            StopCoroutine(nightTransitionCoroutine);
        }

        nightTransitionCoroutine = StartCoroutine(TransitionToNightRoutine());
    }

    public void SetDay()
    {
        if (dayTransitionCorutine != null)
        {
            StopCoroutine(dayTransitionCorutine);
        }

        dayTransitionCorutine = StartCoroutine(TransitionToDayRoutine());
    }

    private IEnumerator TransitionToDayRoutine()
    {
        Transform lightTransform = directionalLight.transform;
        Quaternion startRotation = lightTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(dayRotation);

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            lightTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }

        lightTransform.rotation = targetRotation;
        dayTransitionCorutine = null;
    }

    private IEnumerator TransitionToNightRoutine()
    {
        Transform lightTransform = directionalLight.transform;
        Quaternion startRotation = lightTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(nightRotation);

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            // Interpolación esférica con suavizado al inicio y al final
            lightTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }

        lightTransform.rotation = targetRotation;
        nightTransitionCoroutine = null;
        gameManager.EndSpeech(); // Llamada a EndSpeech() después de la transición
    }
}
