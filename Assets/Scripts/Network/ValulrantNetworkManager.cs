using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValulrantNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // conn.identity grabs the Network Identity component of connected player
        ValulrantNetworkPlayer player = conn.identity.GetComponent<ValulrantNetworkPlayer>();
        player.SetDisplayName($"Player {numPlayers}");
        player.SetPlayerColor(new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
            ));

        Debug.Log($"Someone connected to the server! There are now {numPlayers} players!");
    }
}