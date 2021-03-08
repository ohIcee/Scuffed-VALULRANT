using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/*

    [SyncVar]                       -> Synchronised variable between clients | Only if value is changed on server it is transmitted to all clients | Can only be changed on server!!!
    [Server]                        -> Can be run ONLY on the SERVER 

    [Command]   (CmdMethodName)     -> Clients calling a method on the server
    [ClientRpc] (RpcMethodName)     -> Server calling a method on ALL clients
    [TargetRpc] (TargetMethodName)  -> Server calling a method on a SPECIFIC client

 */

public class ValulrantNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Renderer playerColorRenderer = null;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private KillFeed killFeed;
    private PlayerHealth playerHealth = null;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] [SerializeField] private string displayName = "Missing Name";
    [SyncVar(hook = nameof(HandlePlayerColorUpdated))] [SerializeField] private Color playerColor = Color.red;

    [SyncVar(hook = nameof(ClientHandleKillCountUpdated))] private int kills;
    [SyncVar(hook = nameof(ClientHandleDeathCountUpdated))] private int deaths;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] private bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    private Color teamColor = new Color();
    [SyncVar] private GameObject playerInstance = null;

    public string GetDisplayName() => displayName;
    public bool GetIsPartyOwner() => isPartyOwner;

    [ClientRpc]
    private void RpcUpdatePlayerName(string name)
    {
        playerInstance.transform.name = name;
    }

    #region Server

    [Server]
    public void SetPlayerInstance(GameObject player)
    {
        playerInstance = player;

        RpcUpdatePlayerName(displayName);

        playerInstance.transform.name = displayName;

        playerInstance.GetComponent<Player>().SetNetworkPlayer(this);
        
        playerHealth = playerInstance.GetComponent<PlayerHealth>();
        playerHealth.ServerOnDie += ServerHandleDie;
        playerHealth.ServerOnPlayerKilled += ServerHandlePlayerKilled;

        if (PlayerPrefs.HasKey("MOUSE_SENS")) {
            float sens = PlayerPrefs.GetFloat("MOUSE_SENS");
            playerInstance.GetComponent<Player>().ChangeSensitivity(sens);
        }
    }

    public override void OnStopServer()
    {
        if (playerHealth != null) {
            playerHealth.ServerOnDie -= ServerHandleDie;
            playerHealth.ServerOnPlayerKilled -= ServerHandlePlayerKilled;
        }
    }

    [Server]
    private void ServerHandlePlayerKilled(string killedName, string killerName)
    {
        RpcPlayerKilled(killedName, killerName);
    }

    [Server]
    private void ServerHandleDie()
    {
        RpcDoPlayerDeathEffect(playerInstance.transform.position);

        deaths++;
        Debug.Log($"Player {displayName} now has {deaths} deaths!");

        // DESTROY PLAYER
        playerHealth.ServerOnDie -= ServerHandleDie;
        playerHealth.ServerOnPlayerKilled -= ServerHandlePlayerKilled;

        NetworkServer.Destroy(playerInstance);

        ValulrantNetworkManager networkManager = (ValulrantNetworkManager)NetworkManager.singleton;
        float respawnTime = networkManager.GetRespawnTime();

        if (respawnTime < 0f) return;

        StartCoroutine(RespawnPlayerTimer(networkManager));
    }

    IEnumerator RespawnPlayerTimer(ValulrantNetworkManager networkManager)
    {
        yield return new WaitForSeconds(networkManager.GetRespawnTime());

        GameObject playerInstance = Instantiate(
            networkManager.GetClientPlayerPrefab(),
            networkManager.GetStartPosition().position,
            Quaternion.identity
            );
        playerInstance.GetComponent<Player>().SetNetworkPlayer(this);
        NetworkServer.Spawn(playerInstance, connectionToClient);

        GameObject spawnFx = Instantiate(spawnEffect, playerInstance.transform.position, Quaternion.identity);
        Destroy(spawnFx, 3f);

        SetPlayerInstance(playerInstance);
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
        //RpcLogNewName(newName);

        SetDisplayName(newName);
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcPlayerKilled(string killedName, string killerName)
    {
        killFeed.HandleOnPlayerKilled(killedName, killerName);
    }

    [ClientRpc]
    private void RpcDoPlayerDeathEffect(Vector3 position)
    {
        GameObject fx = (GameObject)Instantiate(deathEffect, position, Quaternion.identity);

        Destroy(fx, 3f);
    }

    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    private void ClientHandleKillCountUpdated(int oldKills, int newKills)
    { 
        // update HUD
    }

    private void ClientHandleDeathCountUpdated(int oldDeaths, int newDeaths)
    { 
        // update HUD
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

    public void DisconnectFromServer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (isServer)
        {
            ((ValulrantNetworkManager)NetworkManager.singleton).StopHost();
            return;
        }

        ((ValulrantNetworkManager)NetworkManager.singleton).StopClient();
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
    }

    #endregion
}
