using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerSetup : NetworkBehaviour
{
    public List<MonoBehaviour> ScriptsToEnable = new List<MonoBehaviour>();
    public CharacterController characterController = null;
    public CinemachineVirtualCamera playerVirtualCamera = null;
    public Camera playerCamera = null;
    public Camera playerWeaponCamera = null;
    public AudioListener audioListener = null;
    public GameObject playerHUD = null;
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

        characterController.enabled = true;
        bodyMeshRenderer.enabled = true;
        playerVirtualCamera.enabled = true;
        playerCamera.gameObject.SetActive(true);
        playerCamera.enabled = true;
        playerHUD.SetActive(true);
        playerWeaponCamera.gameObject.SetActive(true);
        playerWeaponCamera.enabled = true;
        audioListener.enabled = true;

        foreach (MonoBehaviour script in ScriptsToEnable)
        {
            script.enabled = true;
        }
    }

    #endregion

}
