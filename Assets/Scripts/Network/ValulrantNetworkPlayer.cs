using Mirror;
using System;
using TMPro;
using UnityEngine;

/*

    [SyncVar]                       -> Synchronised variable between clients | Only if value is changed on server it is transmitted to all clients
    [Server]                        -> Can be run ONLY on the SERVER 

    [Command]   (CmdMethodName)     -> Clients calling a method on the server
    [ClientRpc] (RpcMethodName)     -> Server calling a method on ALL clients
    [TargetRpc] (TargetMethodName)  -> Server calling a method on a SPECIFIC client

 */

public class ValulrantNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer playerColorRenderer = null;

    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] private string displayName = "Missing Name";
    [SyncVar(hook = nameof(HandlePlayerColorUpdated))] [SerializeField] private Color playerColor = Color.red;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] private bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    private Color teamColor = new Color();

    public bool GetIsPartyOwner() => isPartyOwner;

    #region Server

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
        if (newName.Length < 2 || newName.Length > 20) return;

        // LMAO XDD

        // All players will debug log the new name
        RpcLogNewName(newName);

        SetDisplayName(newName);
    }

    #endregion

    #region Client

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        //displayNameText.text = newName;
    }

    private void HandlePlayerColorUpdated(Color oldColor, Color newColor)
    {
        //playerColorRenderer.material.SetColor("_Color", newColor);
    }

    [ContextMenu("SetMyName")]
    private void SetMyName()
    {
        CmdSetDisplayName("M");
    }

    [ClientRpc]
    private void RpcLogNewName(string newName)
    {
        Debug.Log(newName);
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        // If this check is not present, it would update for EVERYONE.
        if (!hasAuthority) return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    public override void OnStartClient()
    {
        // Don't run on server
        if (NetworkServer.active) return;

        ((ValulrantNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        // Don't run on server
        if (!isClientOnly) return;

        ((ValulrantNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) return;

        // Subscribe to events
    }

    #endregion
}
