using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField inputUsername;
    [SerializeField] private Button saveUsernameButton;
    [SerializeField] private GameManager gameManager;
    public string nombreUsuario = "";

    void Awake()
    {
        saveUsernameButton.onClick.RemoveAllListeners();
        saveUsernameButton.onClick.AddListener(() => SaveUsername());
    }

    private void SaveUsername()
    {
        //Debug.Log("Username: " + inputUsername.text);
        gameManager.player.username = inputUsername.text;
        gameManager.uiManager.SetInputUsernamePanelVisibility(false);
    }
}
