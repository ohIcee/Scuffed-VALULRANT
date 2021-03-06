using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

    private void Start()
    {
        ValulrantNetworkManager.ClientOnConnected += HandleClientConnected;
        ValulrantNetworkPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        ValulrantNetworkPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void ClientHandleInfoUpdated()
    {
        ValulrantNetworkManager networkManager = ((ValulrantNetworkManager)NetworkManager.singleton);
        List<ValulrantNetworkPlayer> players = networkManager.Players;

        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].GetDisplayName();
        }

        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
        }

        startGameButton.interactable = players.Count >= networkManager.GetMinPlayerCountToStart();
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    public void StartGame()
    {
        NetworkIdentity identity = NetworkClient.connection.identity;
        ValulrantNetworkPlayer player = identity.GetComponent<ValulrantNetworkPlayer>();
        player.CmdStartGame();
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // is host
            NetworkManager.singleton.StopHost();
        }
        else
        {
            // is client
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }

    private void OnDestroy()
    {
        ValulrantNetworkManager.ClientOnConnected -= HandleClientConnected;
        ValulrantNetworkPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        ValulrantNetworkPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }
}
