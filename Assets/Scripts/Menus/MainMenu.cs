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

        usernameInput.text = PlayerPrefs.GetString("USERNAME");
    }

    private void OnDisable()
    {
        ValulrantNetworkManager.ClientOnConnected -= HandleClientConnected;
        ValulrantNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;

        settingsManager.ClientOnSensitivityChanged -= HandleSensitivityUpdated;
        settingsManager.ClientOnGraphicsQualityLevelChanged -= HandleGraphicsQualityUpdated;
    }

    // Receive a string for the gamemode and find the component on the
    // NetworkManager, because stopping hosting for some reason reloads
    // the scene and thus unbinds our UI actions...
    public void SelectGamemode(string mode)
    {
        ValulrantNetworkManager manager = (ValulrantNetworkManager)ValulrantNetworkManager.singleton;

        switch (mode)
        {
            case "FFA":
                manager.SelectGamemode(FindObjectOfType<FFA>());
                break;
            case "BOMB":
                manager.SelectGamemode(FindObjectOfType<Bomb>());
                break;
            default:
                Debug.LogWarning($"Invalid gamemode selected! {mode}");
                break;
        }
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
        PlayerPrefs.Save();
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

    public void ExitGame() => Application.Quit();

}
