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
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        ValulrantNetworkManager.ClientOnConnected += HandleClientConnected;
        ValulrantNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
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
