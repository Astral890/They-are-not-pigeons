using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class PlazaPauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Behaviour playerMovement;
    [SerializeField] private string mainMenuScene = "MenuPrincipal";

    [Header("Optional settings UI")]
    [SerializeField] private GameObject menuContent;
    [SerializeField] private GameObject optionsPanel;

    private bool isPaused;
    private float previousTimeScale;
    private bool movementWasEnabled;

    private void Awake()
    {
        if (pausePanel == null)
        {
            Debug.LogError("Assign Pause Panel on PlazaPauseMenu.", this);
            enabled = false;
            return;
        }

        ShowPauseHome();
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current?.escapeKey.wasPressedThisFrame != true)
            return;

        if (!isPaused)
            PauseGame();
        else if (optionsPanel != null && optionsPanel.activeSelf)
            ShowPauseHome();
        else
            ResumeGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (playerMovement != null)
        {
            movementWasEnabled = playerMovement.enabled;
            playerMovement.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;
        Time.timeScale = previousTimeScale;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = movementWasEnabled;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenOptions()
    {
        if (!isPaused || menuContent == null || optionsPanel == null)
            return;

        menuContent.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ShowPauseHome()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (menuContent != null)
            menuContent.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        if (!isPaused)
            return;

        if (string.IsNullOrWhiteSpace(mainMenuScene) ||
            !Application.CanStreamedLevelBeLoaded(mainMenuScene))
        {
            Debug.LogError($"Add the scene '{mainMenuScene}' to the build scene list.", this);
            return;
        }

        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    private void OnDisable()
    {
        if (isPaused)
            ResumeGame();
    }
}