using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ValulrantNetworkManager : NetworkManager
{

    [Header("Game settings")]
    [SerializeField] private GameMode selectedGameMode;
    [SerializeField] private int minPlayerCountToStart = 2;

    public GameMode GetGameMode() => selectedGameMode;

    public int GetMinPlayerCountToStart() => minPlayerCountToStart;
    [Space()]

    public GameObject ClientPlayerPrefab;
    public GameObject GetClientPlayerPrefab() => ClientPlayerPrefab;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private bool isGameInProgress = false;

    public List<ValulrantNetworkPlayer> Players { get; } = new List<ValulrantNetworkPlayer>();

    #region Server

    public void SelectGamemode(GameMode gameMode) => selectedGameMode = gameMode;

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) return;

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity == null) return;

        ValulrantNetworkPlayer player = conn.identity.GetComponent<ValulrantNetworkPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (Players.Count < minPlayerCountToStart) return;

        isGameInProgress = true;

        ServerChangeScene("Scene_Map_01");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // conn.identity grabs the Network Identity component of connected player
        ValulrantNetworkPlayer player = conn.identity.GetComponent<ValulrantNetworkPlayer>();

        Players.Add(player);

        player.SetTeamColor(new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)
            ));

        player.SetPlayerColor(new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)
            ));

        // If it's the first player, set him as party owner
        player.SetPartyOwner(Players.Count == 1);

        Debug.Log($"Someone connected to the server! There are now {numPlayers} players!");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            selectedGameMode.OnBeginGame();
        }
    }

    #endregion

    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion

}