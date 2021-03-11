using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject joiningPagePanel = null;

    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private TMP_InputField usernameInput = null;
    [SerializeField] private TMP_InputField mouseSensInput = null;
    [SerializeField] private Scrollbar mouseSensScrollbar = null;
    [SerializeField] private Button joinButton = null;

    [SerializeField] private RenderPipelineAsset[] qualityLevels;
    [SerializeField] private TMP_Dropdown graphicsSettingDropdown = null;

    private SettingsManager settingsManager = null;

    private void OnEnable()
    {
        settingsManager = FindObjectOfType<SettingsManager>();

        ValulrantNetworkManager.ClientOnConnected += HandleClientConnected;
        ValulrantNetworkManager.ClientOnDisconnected += HandleClientDisconnected;

        settingsManager.ClientOnSensitivityChanged += HandleSensitivityUpdated;
        settingsManager.ClientOnGraphicsQualityLevelChanged += HandleGraphicsQualityUpdated;

        HandleSensitivityUpdated(settingsManager.GetMouseSensitivity());
        HandleGraphicsQualityUpdated(settingsManager.GetGraphicsQualityLevel());
    }

    private void OnDisable()
    {
        ValulrantNetworkManager.ClientOnConnected -= HandleClientConnected;
        ValulrantNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;

        settingsManager.ClientOnSensitivityChanged -= HandleSensitivityUpdated;
        settingsManager.ClientOnGraphicsQualityLevelChanged -= HandleGraphicsQualityUpdated;
    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        SetUsername();

        NetworkManager.singleton.StartHost();
    }

    #region SettingsPage

    private void HandleSensitivityUpdated(float sens)
    {
        mouseSensInput.text = sens.ToString();
        mouseSensScrollbar.value = sens;
    }

    private void HandleGraphicsQualityUpdated(int level)
    {
        Debug.Log("Graphics Quality Updated!");

        graphicsSettingDropdown.value = level;
    }

    public void ChangeGraphicsQuality(int value)
    {
        settingsManager.ChangeGraphicsQuality(value);
        settingsManager.SaveCurrentGraphicsQuality();
    }

    public void OnMouseSensitivityChangedInput()
    {
        if (mouseSensInput.text.EndsWith(".")) return;

        if (float.TryParse(mouseSensInput.text, out float sens))
        {
            OnMouseSensitivityChanged(sens);
            return;
        }

        Debug.LogWarning($"Incorrect mouse sensitivity input: {mouseSensInput.text}");
    }

    public void OnMouseSensitivityChangedScroll() => OnMouseSensitivityChanged(mouseSensScrollbar.value);

    private void OnMouseSensitivityChanged(float newSens)
    {
        newSens = Mathf.Clamp(newSens, 0, 1);

        settingsManager.ChangeSensitivity(newSens);
        settingsManager.SaveCurrentSensitivity();
    }

    #endregion

    private void SetUsername()
    {
        PlayerPrefs.SetString("USERNAME", usernameInput.text.Length > 0 ? usernameInput.text : "XDDD POG");
    }

    public void Join()
    {
        string address = addressInput.text.Trim();

        if (address.Length <= 0) address = "localhost";

        SetUsername();

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
        joiningPagePanel.SetActive(true);
        landingPagePanel.SetActive(false);
    }

    public void OnSettingsClose()
    {
        settingsManager.SavePlayerPrefs();
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        landingPagePanel.SetActive(false);
        joiningPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
        landingPagePanel.SetActive(true);
        joiningPagePanel.SetActive(false);
    }

}
