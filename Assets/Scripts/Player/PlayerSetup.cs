using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    public List<MonoBehaviour> ScriptsToEnable = new List<MonoBehaviour>();
    public CharacterController characterController = null;
    public Camera playerCamera = null;
    public Camera playerWeaponCamera = null;
    public AudioListener audioListener = null;
    public GameObject playerHUD = null;
    public PlayerHealth playerHealth = null;

    [Header("Weapon shit")]
    public GameObject weaponHolder = null;
    public string weaponLayerName = "Weapon";

    public bool IsDisabled = false;

    #region Server

    [Command]
    private void CmdResetHealth() {
        playerHealth.ResetHealth();
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        SetupPlayer();
    }

    public void SetupPlayer()
    {
        if (!hasAuthority) return;

        IsDisabled = false;

        characterController.enabled = true;
        playerCamera.enabled = true;
        playerWeaponCamera.enabled = true;
        audioListener.enabled = true;
        playerHUD.SetActive(true);

        CmdResetHealth();

        foreach (MonoBehaviour script in ScriptsToEnable)
        {
            script.enabled = true;
        }

        // TODO: Fix weapon camera shit
        //weaponHolder.layer = 11;

        Debug.Log($"Player {transform.name} has been setup!");
    }

    public void DisablePlayer() 
    {
        if (!hasAuthority) return;

        IsDisabled = true;

        characterController.enabled = false;
        playerCamera.enabled = false;
        playerWeaponCamera.enabled = false;
        audioListener.enabled = false;
        playerHUD.SetActive(false);

        foreach (MonoBehaviour script in ScriptsToEnable)
        {
            script.enabled = false;
        }

        Debug.Log($"Player {transform.name} has been disabled!");
    }

    #endregion

}
