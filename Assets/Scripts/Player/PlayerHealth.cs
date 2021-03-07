using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))] private int currentHealth;

    // Server raised event
    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    public event Action<string, string> ClientOnPlayerKilled;

    //public bool IsDead() => currentHealth <= 0f;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        currentHealth = maxHealth;
    }

    [Server]    // killerPlayer is NULL when it's a suicide
    public void DealDamage(int damageAmount, ValulrantNetworkPlayer killerPlayer)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth > 0) return;

        Debug.Log($"This: {GetComponent<Player>().GetNetworkPlayer() == null} ; That: {killerPlayer == null}");

        RpcPlayerKilled(GetComponent<Player>().GetNetworkPlayer().GetDisplayName(), killerPlayer.GetDisplayName());
        ClientOnPlayerKilled?.Invoke(GetComponent<Player>().GetNetworkPlayer().GetDisplayName(), killerPlayer.GetDisplayName());

        // DIE
        ServerOnDie?.Invoke();
    }

    [Server]
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    #endregion

    #region Client

    // Gets called when the player recieves the updated SyncVar variable "currentHealth"
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    [ClientRpc]
    private void RpcPlayerKilled(string killed, string killer)
    {
        //ClientOnPlayerKilled?.Invoke(killed, killer);
    }

    #endregion

}
