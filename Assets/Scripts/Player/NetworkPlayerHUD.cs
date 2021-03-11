using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NetworkPlayerHUD : MonoBehaviour
{
    [Header("Escape Menu")]
    [SerializeField] private GameObject escapeMenuPanel = null;
    [SerializeField] private GameObject settingsPanel = null;
    [SerializeField] private Scrollbar mouseSensitivityScrollBar = null;
    [SerializeField] private TMP_InputField mouseSensitivityInput = null;
    [SerializeField] private TMP_Dropdown graphicsSettingDropdown = null;
    [SerializeField] private RenderPipelineAsset[] qualityLevels;

    private SettingsManager settingsManager;
    [SerializeField] private ValulrantNetworkPlayer networkPlayer;

    #region Client

    private void Start()
    {
        settingsManager = FindObjectOfType<SettingsManager>();

        settingsManager.ClientOnSensitivityChanged += HandleSensitivityUpdated;
        settingsManager.ClientOnGraphicsQualityLevelChanged += HandleGraphicsQualityUpdated;

        HandleSensitivityUpdated(settingsManager.GetMouseSensitivity());
        HandleGraphicsQualityUpdated(settingsManager.GetGraphicsQualityLevel());
    }

    private void OnDestroy()
    {
        if (settingsManager != null)
        {
            settingsManager.ClientOnSensitivityChanged -= HandleSensitivityUpdated;
            settingsManager.ClientOnGraphicsQualityLevelChanged -= HandleGraphicsQualityUpdated;
        }
    }

    #region EscapeMenu

    private void HandleSensitivityUpdated(float sens)
    {
        mouseSensitivityInput.text = sens.ToString();
        mouseSensitivityScrollBar.value = sens;
    }

    private void HandleGraphicsQualityUpdated(int level)
    {
        graphicsSettingDropdown.value = level;
    }

    public void ToggleEscapeMenu()
    {
        escapeMenuPanel.SetActive(!escapeMenuPanel.activeSelf);

        Cursor.lockState = escapeMenuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = escapeMenuPanel.activeSelf;
    }

    public void OnDisconnectButton()
    {
        networkPlayer.DisconnectFromServer();
    }

    public bool GetIsEscapeMenuOpen() => escapeMenuPanel.activeSelf;

    public bool GetIsSettingsMenuOpen() => settingsPanel.activeSelf;

    public void ToggleSettingsMenu(bool active)
    {
        if (settingsPanel.activeSelf && !active
            || !settingsPanel.activeSelf && active) ToggleSettingsMenu();
    }

    public void ToggleSettingsMenu()
    {
        bool newState = !settingsPanel.activeSelf;
        settingsPanel.SetActive(newState);

        if (newState) settingsManager.SavePlayerPrefs();
    }

    // Called from UI
    public void OnMouseSensitivityChangedInput()
    {
        if (mouseSensitivityInput.text.EndsWith(".")) return;

        if (float.TryParse(mouseSensitivityInput.text, out float sens))
        {
            OnMouseSensitivityChanged(sens);
            return;
        }

        Debug.LogWarning($"Incorrect mouse sensitivity input: {mouseSensitivityInput.text}");
    }

    // Called from UI
    public void OnMouseSensitivityChangedScroll() => OnMouseSensitivityChanged(mouseSensitivityScrollBar.value);

    private void OnMouseSensitivityChanged(float newSens)
    {
        newSens = Mathf.Clamp(newSens, 0, 1);

        settingsManager.ChangeSensitivity(newSens);
        settingsManager.SaveCurrentSensitivity();

        if (networkPlayer.GetPlayerInstance() != null)
            networkPlayer.GetPlayerInstance().GetComponent<Player>().ChangeSensitivity(newSens);
    }

    // Called from UI
    public void ChangeGraphicsQuality(int value)
    {
        settingsManager.ChangeGraphicsQuality(value);
    }

    #endregion

    #endregion

}
