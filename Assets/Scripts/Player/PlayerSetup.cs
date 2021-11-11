using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using UnityEngine.SceneManagement;

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
    public GameObject playerNamePlate = null;
    public GameObject playerBody = null;

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

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        if (!hasAuthority) return;

        CmdResetHealth();
        IsDisabled = false;

        playerNamePlate.SetActive(false);
        playerBody.SetActive(false);

        characterController.enabled = true;
        playerCamera.enabled = true;
        playerHUDCanvas.SetActive(true);
        playerWeaponCamera.gameObject.SetActive(true);
        playerWeaponCamera.enabled = true;
        audioListener.enabled = true;

        if (GetComponent<Player>().GetNetworkPlayer() != null)
            playerHUD.UpdateMoneyText( GetComponent<Player>().GetNetworkPlayer().GetMoney() );

        foreach (MonoBehaviour script in ScriptsToEnable)
        {
            script.enabled = true;
        }
    }

    #endregion

}
