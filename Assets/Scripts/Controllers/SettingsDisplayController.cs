using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDisplayController : MonoBehaviour
{
    [Header("Controles de la pantalla")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button resetButton;

    private const string FullscreenKey = "Settings.Fullscreen";
    private const string QualityKey = "Settings.QualityName";

    private string[] qualityNames;
    private int defaultQuality;

    private void Start()
    {
        if (fullscreenToggle == null ||
            qualityDropdown == null ||
            resetButton == null)
        {
            Debug.LogError(
                "Asigna los tres controles de Settings en el Inspector.",
                this
            );

            enabled = false;
            return;
        }

        qualityNames = QualitySettings.names;
        defaultQuality = QualitySettings.GetQualityLevel();

        // Muestra los niveles que realmente existen en el proyecto.
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(
            new List<string>(qualityNames)
        );

        bool fullscreen = PlayerPrefs.GetInt(
            FullscreenKey,
            Screen.fullScreen ? 1 : 0
        ) == 1;

        string savedQuality = PlayerPrefs.GetString(
            QualityKey,
            qualityNames[defaultQuality]
        );

        int qualityIndex = System.Array.IndexOf(
            qualityNames,
            savedQuality
        );

        if (qualityIndex < 0)
            qualityIndex = defaultQuality;

        fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
        qualityDropdown.SetValueWithoutNotify(qualityIndex);
        qualityDropdown.RefreshShownValue();

        Screen.fullScreen = fullscreen;
        QualitySettings.SetQualityLevel(qualityIndex, true);

        fullscreenToggle.onValueChanged.AddListener(
            ChangeFullscreen
        );

        qualityDropdown.onValueChanged.AddListener(
            ChangeQuality
        );

        resetButton.onClick.AddListener(ResetDisplaySettings);
    }

    private void ChangeFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;

        PlayerPrefs.SetInt(
            FullscreenKey,
            fullscreen ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    private void ChangeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);

        PlayerPrefs.SetString(
            QualityKey,
            qualityNames[index]
        );

        PlayerPrefs.Save();
    }

    private void ResetDisplaySettings()
    {
        fullscreenToggle.SetIsOnWithoutNotify(true);

        qualityDropdown.SetValueWithoutNotify(defaultQuality);
        qualityDropdown.RefreshShownValue();

        ChangeFullscreen(true);
        ChangeQuality(defaultQuality);
    }

    private void OnDestroy()
    {
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(
                ChangeFullscreen
            );

        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.RemoveListener(
                ChangeQuality
            );

        if (resetButton != null)
            resetButton.onClick.RemoveListener(
                ResetDisplaySettings
            );
    }
}