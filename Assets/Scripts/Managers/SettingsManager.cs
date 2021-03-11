using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
    
    private static string MOUSE_SENSITIVITY_PREFS_KEY = "MOUSE_SENS";
    private static string GRAPHICS_QUALITY_PREFS_KEY = "QUALITY_LEVEL";

    private float mouseSensitivity = 1f;
    public event Action<float> ClientOnSensitivityChanged;

    private int graphicsQualityLevel = 0;
    [SerializeField] private RenderPipelineAsset[] qualityLevels;
    public event Action<int> ClientOnGraphicsQualityLevelChanged;

    public static SettingsManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        mouseSensitivity = PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_PREFS_KEY);
        graphicsQualityLevel = PlayerPrefs.GetInt(GRAPHICS_QUALITY_PREFS_KEY);
    }

#region Mouse Sensitivity

    public float GetMouseSensitivity() => mouseSensitivity;

    public void ChangeSensitivity(float newSens)
    {
        mouseSensitivity = newSens;

        ClientOnSensitivityChanged?.Invoke(newSens);
    }

    public void SaveCurrentSensitivity() => PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_PREFS_KEY, mouseSensitivity);

#endregion

#region Graphics Quality Level

    public int GetGraphicsQualityLevel() => graphicsQualityLevel;

    public void ChangeGraphicsQuality(int level)
    {
        graphicsQualityLevel = level;

        QualitySettings.SetQualityLevel(level);
        QualitySettings.renderPipeline = qualityLevels[level];

        ClientOnGraphicsQualityLevelChanged?.Invoke(level);
    }

    public void SaveCurrentGraphicsQuality() => PlayerPrefs.SetInt(GRAPHICS_QUALITY_PREFS_KEY, graphicsQualityLevel);

#endregion

    public void SavePlayerPrefs() => PlayerPrefs.Save();

}
