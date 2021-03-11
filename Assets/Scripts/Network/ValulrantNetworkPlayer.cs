using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
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
    [SyncVar] [SerializeField] private Color playerColor = Color.red;

    [SyncVar(hook = nameof(ClientHandleKillCountUpdated))] private int kills;
    [SyncVar(hook = nameof(ClientHandleDeathCountUpdated))] private int deaths;
    [SyncVar(hook = nameof(ClientHandleMoneyUpdated))] private int money;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] private bool isPartyOwner = false;

    [SerializeField] private List<ScoreboardScoreEntry> scoreboardScoreEntries = null;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    public event Action<int, int> ClientOnMoneyUpdated;

    private Color teamColor = new Color();
    [SyncVar] private GameObject playerInstance = null;

    public int GetMoney() => money;
    public int GetKills() => kills;
    public int GetDeaths() => deaths;
    public string GetDisplayName() => displayName;
    public bool GetIsPartyOwner() => isPartyOwner;
    public Color GetPlayerColor() => playerColor;

    #region Server

    [Server]
    public void UpdateKillCount(int killsToAdd)
    {
        kills += killsToAdd;
    }

    [Server]
    private void AddMoney(int moneyToAdd)
    {
        money += moneyToAdd;

        //Debug.Log($"Player {displayName} now has {money} money!");
    }

    [Server]
    public void SubtractMoney(int moneyToSubtract)
    {
        money -= moneyToSubtract;

        //Debug.Log($"Player {displayName} now has {money} money!");
    }

    [Server]
    public void SetPlayerInstance(GameObject player)
    {
        playerInstance = player;

        playerInstance.transform.name = displayName;

        Player playerPlayerScript = playerInstance.GetComponent<Player>();
        playerPlayerScript.SetNetworkPlayer(this);

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
    private void ServerHandlePlayerKilled(ValulrantNetworkPlayer killedPlayer, ValulrantNetworkPlayer killerPlayer)
    {
        if (killerPlayer == killedPlayer) return;

        killerPlayer.UpdateKillCount(1);
        killerPlayer.AddMoney(500); // replace with money reward
    }

    [Server]
    private void ServerHandleDie()
    {
        RpcDoPlayerDeathEffect(playerInstance.transform.position);

        deaths++;
        //Debug.Log($"Player {displayName} now has {deaths} deaths!");

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
        SetDisplayName(newName);
    }

    #endregion

    #region Client

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

        playerInstance.GetComponent<PlayerFiring>().OnKilledPlayer();

        playerInstance.GetComponent<PlayerHUD>().OnPlayerKilledPopup();
    }

    private void ClientHandleDeathCountUpdated(int oldDeaths, int newDeaths)
    { 
        // update HUD
    }

    private void ClientHandleMoneyUpdated(int oldMoney, int newMoney)
    {
        // update HUD

        ClientOnMoneyUpdated?.Invoke(oldMoney, newMoney);
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
