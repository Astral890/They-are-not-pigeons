using UnityEngine;
using UnityEngine.Video;

public class MainMenuCredits : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuContent;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private VideoPlayer creditsVideo;

    private void Awake()
    {
        creditsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        creditsVideo.loopPointReached += OnCreditsFinished;
    }

    private void OnDisable()
    {
        creditsVideo.loopPointReached -= OnCreditsFinished;
    }

    public void ShowCredits()
    {
        mainMenuContent.SetActive(false);
        creditsPanel.SetActive(true);
        creditsVideo.Play();
    }

    public void BackToMenu()
    {
        creditsVideo.Stop();
        creditsPanel.SetActive(false);
        mainMenuContent.SetActive(true);
    }

    private void OnCreditsFinished(VideoPlayer player)
    {
        BackToMenu();
    }
    
}