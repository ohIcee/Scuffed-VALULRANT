using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameModes
{ 
    FreeForAll,
    SearchAndDestroy
}

[System.Serializable]
public abstract class GameMode : MonoBehaviour
{
    public string ALLSpawnLocationsParentName = "SPAWNS";

    public abstract GameModes SelectedGameMode { get; }
    public abstract bool HasRespawning { get; }
    public abstract float RespawnTime { get; }
    public abstract string GamemodeSpawnLocationsParentName { get; }

    public abstract void OnBeginGame();

    public abstract void OnKilledPlayer(ValulrantNetworkPlayer player);

    public abstract void OnDeath(ValulrantNetworkPlayer player);

    public abstract void OnAllPlayersDead();

}
