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

    public override void OnStartAuthority()
    {
        Debug.Log($"imam avtoriteto igralca {transform.name}?: {hasAuthority}");
        if (!hasAuthority) return;

        characterController.enabled = true;
        playerCamera.enabled = true;
        playerWeaponCamera.enabled = true;

        foreach (MonoBehaviour script in ScriptsToEnable)
        {
            script.enabled = true;
        }

        Debug.Log($"Player {transform.name} has been setup!");
    }
}
