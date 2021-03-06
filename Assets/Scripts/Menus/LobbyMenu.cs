using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;

    private void Start()
    {
        ValulrantNetworkManager.ClientOnConnected += HandleClientConnected;
        ValulrantNetworkPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
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
    }
}
