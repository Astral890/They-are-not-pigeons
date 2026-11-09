using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
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
    private Vector3 nightRotation = new Vector3(-14.251f, -19.366f, 28.804f);
                                                //-27.863, -21.318, 34.195
    private Coroutine nightTransitionCoroutine;

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
    }
}
