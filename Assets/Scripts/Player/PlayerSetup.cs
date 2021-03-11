using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerSetup : NetworkBehaviour
{
    public Player player;
    public List<MonoBehaviour> ScriptsToEnable = new List<MonoBehaviour>();
    public CharacterController characterController = null;
    public Camera playerCamera = null;
    public Camera playerWeaponCamera = null;
    public AudioListener audioListener = null;
    public GameObject playerHUDCanvas = null;
    public PlayerHUD playerHUD = null;
    public PlayerHealth playerHealth = null;
    public MeshRenderer bodyMeshRenderer = null;
    public GameObject playerNamePlate;

    [Header("Weapon shit")]
    public GameObject weaponHolder = null;
    public string weaponLayerName = "Weapon";

    public bool IsDisabled = false;

    #region Server

    [Command]
    private void CmdResetHealth()
    {
        playerHealth.ResetHealth();
    }

    [Command]
    private void CmdSetPlayerColor() => RpcSetPlayerColor();

    #endregion

    #region Client

    [ClientRpc]
    private void RpcSetPlayerColor()
    {
        player.UpdateRendererColor(player.GetNetworkPlayer().GetPlayerColor());
    }

    public override void OnStartAuthority()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        if (!hasAuthority) return;

        //- Get players and update scoreboard
        //List<ValulrantNetworkPlayer> players = ((ValulrantNetworkManager)NetworkManager.singleton).Players;
        //foreach (ValulrantNetworkPlayer player in players)
        //{
        //    playerHUD.Ge
        //}

        CmdResetHealth();
        IsDisabled = false;

        playerNamePlate.SetActive(false);

        characterController.enabled = true;
        bodyMeshRenderer.enabled = true;
        playerCamera.enabled = true;
        playerHUDCanvas.SetActive(true);
        playerWeaponCamera.gameObject.SetActive(true);
        playerWeaponCamera.enabled = true;
        audioListener.enabled = true;

        playerHUD.UpdateMoneyText( GetComponent<Player>().GetNetworkPlayer().GetMoney() );

        CmdSetPlayerColor();

        foreach (MonoBehaviour script in ScriptsToEnable)
        {
            script.enabled = true;
        }
    }

    #endregion

}
