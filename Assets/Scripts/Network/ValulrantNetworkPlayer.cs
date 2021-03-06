using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

/*

    [SyncVar]                       -> Synchronised variable between clients | Only if value is changed on server it is transmitted to all clients | Can only be changed on server!!!
    [Server]                        -> Can be run ONLY on the SERVER 

    [Command]   (CmdMethodName)     -> Clients calling a method on the server
    [ClientRpc] (RpcMethodName)     -> Server calling a method on ALL clients
    [TargetRpc] (TargetMethodName)  -> Server calling a method on a SPECIFIC client

 */

public class ValulrantNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer playerColorRenderer = null;
    private PlayerHealth playerHealth = null;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] [SerializeField] private string displayName = "Missing Name";
    [SyncVar(hook = nameof(HandlePlayerColorUpdated))] [SerializeField] private Color playerColor = Color.red;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] private bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    private Color teamColor = new Color();
    [SyncVar] private GameObject playerInstance = null;

    public string GetDisplayName() => displayName;
    public bool GetIsPartyOwner() => isPartyOwner;

    #region Server

    [Server]
    public void SetPlayerInstance(GameObject player)
    {
        Debug.Log($"Setting player instance {player.transform.name}");
        playerInstance = player;
        playerHealth = playerInstance.GetComponent<PlayerHealth>();
        playerHealth.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        if (playerHealth != null)
            playerHealth.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        PlayerSetup playerSetup = playerInstance.GetComponent<PlayerSetup>();
        playerSetup.DisablePlayer();
        RpcDisablePlayer(playerSetup);
        StartCoroutine(RespawnPlayerTimer(playerSetup));
    }

    IEnumerator RespawnPlayerTimer(PlayerSetup playerSetup)
    {
        ValulrantNetworkManager networkManager = (ValulrantNetworkManager)NetworkManager.singleton;
        float respawnTime = networkManager.GetRespawnTime();
        yield return new WaitForSeconds(respawnTime);
        RpcPlayerRespawn();
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Server]
    public void SetDisplayName(string newName) => displayName = newName;

    [Server]
    public void SetPlayerColor(Color color) => playerColor = color;

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;

        ((ValulrantNetworkManager)NetworkManager.singleton).StartGame();
    }

    [Command]
    private void CmdSetDisplayName(string newName)
    {
        // Do validation
        //if (newName.Length < 2 || newName.Length > 20) return;

        // All players will debug log the new name
        RpcLogNewName(newName);

        SetDisplayName(newName);
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcPlayerRespawn()
    {
        ValulrantNetworkManager networkManager = (ValulrantNetworkManager)NetworkManager.singleton;
        Debug.Log(playerInstance == null);
        if (playerInstance.transform.TryGetComponent<PlayerSetup>(out PlayerSetup playerSetup))
        {
            playerSetup.transform.position = networkManager.GetStartPosition().position;
            playerSetup.SetupPlayer();
        }
        else {
            Debug.LogError("Could not get component PlayerSetup!");
        }
    }

    [ClientRpc]
    private void RpcDisablePlayer(PlayerSetup playerSetup)
    {
        if (isServer) return;

        playerSetup.DisablePlayer();
    }

    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    private void HandlePlayerColorUpdated(Color oldColor, Color newColor)
    {
        playerColorRenderer.material.SetColor("_Color", newColor);
    }

    [ClientRpc]
    private void RpcLogNewName(string newName)
    {
        Debug.Log(newName);
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        // If this check is not present, it would update for EVERYONE.
        if (!hasAuthority) return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerPrefs.GetString("USERNAME"));
    }

    public override void OnStartClient()
    {
        // Don't run on server
        if (NetworkServer.active) return;

        ((ValulrantNetworkManager)NetworkManager.singleton).Players.Add(this);

        if (!hasAuthority) return;
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        // Don't run on server
        if (!isClientOnly) return;

        ((ValulrantNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) return;

        // Subscribe to events
    }

    #endregion
}
