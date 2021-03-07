using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;

    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private TMP_InputField usernameInput = null;
    [SerializeField] private TMP_InputField mouseSensInput = null;
    [SerializeField] private Scrollbar mouseSensScrollbar = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        ValulrantNetworkManager.ClientOnConnected += HandleClientConnected;
        ValulrantNetworkManager.ClientOnDisconnected += HandleClientDisconnected;

        if (PlayerPrefs.HasKey("MOUSE_SENS"))
        {
            float sens = PlayerPrefs.GetFloat("MOUSE_SENS");
            OnMouseSensitivityChanged(sens);
        }    
    }

    private void OnDisable()
    {
        ValulrantNetworkManager.ClientOnConnected -= HandleClientConnected;
        ValulrantNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        SetUsername();

        NetworkManager.singleton.StartHost();
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
        mouseSensInput.text = newSens.ToString();
        mouseSensScrollbar.value = newSens;

        PlayerPrefs.SetFloat("MOUSE_SENS", newSens);
    }

    private void SetUsername()
    {
        PlayerPrefs.SetString("USERNAME", usernameInput.text.Length > 0 ? usernameInput.text : "XDDD POG");
    }

    public void Join()
    {
        string address = addressInput.text;

        if (address.Length <= 0) return;

        SetUsername();

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    public void OnSettingsClose()
    {
        PlayerPrefs.Save();
        Debug.Log("Saved prefs");
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }

}
