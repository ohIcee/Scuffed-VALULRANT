using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : GameMode
{

    #region GameMode overrides
    [Header("GameMode settings")]
    [SerializeField] private bool hasRespawning = true;
    public override bool HasRespawning
    {
        get { return hasRespawning; }
    }

    [SerializeField] private float respawnTime = 2f;
    public override float RespawnTime
    {
        get { return respawnTime; }
    }

    private GameModes selectedGameMode = GameModes.FreeForAll;
    public override GameModes SelectedGameMode
    {
        get { return selectedGameMode; }
    }

    [SerializeField] private string gamemodeSpawnLocationsParentName = "SPAWNS_FFA";
    public override string GamemodeSpawnLocationsParentName
    {
        get { return gamemodeSpawnLocationsParentName; }
    }
    #endregion

    [Header("References")]
    [SerializeField] private ValulrantNetworkManager networkManager;

    public override void OnBeginGame()
    {
        networkManager.playerSpawnMethod = PlayerSpawnMethod.RoundRobin;

        // Enable the correct spawns for the selected gamemode
        Transform spawnParentTransform = GameObject.Find(ALLSpawnLocationsParentName).transform;
        foreach (Transform obj in spawnParentTransform)
        {
            if (obj.name.Equals(gamemodeSpawnLocationsParentName))
                continue;

            obj.gameObject.SetActive(false);
        }

        foreach (ValulrantNetworkPlayer player in networkManager.Players)
        {
            GameObject playerInstance = Instantiate(
                    networkManager.ClientPlayerPrefab,
                    networkManager.GetStartPosition().position,
                    Quaternion.identity);

            NetworkServer.Spawn(playerInstance, player.connectionToClient);
            player.SetPlayerInstance(playerInstance);
        }
    }

    public override void OnDeath(ValulrantNetworkPlayer player) { }

    public override void OnKilledPlayer(ValulrantNetworkPlayer player) { }

    public override void OnAllPlayersDead() {

        foreach (ValulrantNetworkPlayer player in networkManager.Players)
        {
            GameObject playerInstance = Instantiate(
                    networkManager.ClientPlayerPrefab,
                    networkManager.GetStartPosition().position,
                    Quaternion.identity);

            NetworkServer.Spawn(playerInstance, player.connectionToClient);
            player.SetPlayerInstance(playerInstance);
        }

    }

}
