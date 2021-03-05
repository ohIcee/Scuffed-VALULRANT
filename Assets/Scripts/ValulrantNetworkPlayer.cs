using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*

    [SyncVar]                       -> Synchronised variable between clients | Only if value is changed on server it is transmitted to all clients
    [Server]                        -> Can be run ONLY on the SERVER 

    [Command]   (CmdMethodName)     -> Clients calling a method on the server
    [ServerRpc] (RpcMethodName)     -> Server calling a method on ALL clients
    [TargetRpc] (TargetMethodName)  -> Server calling a method on a SPECIFIC client

 */
 
public class ValulrantNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer playerColorRenderer = null;

    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] private string displayName = "Missing Name";
    [SyncVar(hook = nameof(HandlePlayerColorUpdated))] [SerializeField] private Color playerColor = Color.red;

    #region Server

    [Server]
    public void SetDisplayName(string newName) => displayName = newName;

    [Server]
    public void SetPlayerColor(Color color) => playerColor = color;

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
        displayNameText.text = newName;
    }

    private void HandlePlayerColorUpdated(Color oldColor, Color newColor)
    {
        playerColorRenderer.material.SetColor("_Color", newColor);
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

    #endregion

}
