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

    public event Action<string, string> ServerOnPlayerKilled;

    //public bool IsDead() => currentHealth <= 0f;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount, ValulrantNetworkPlayer killerPlayer)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth > 0) return;

        string killedName = GetComponent<Player>().GetNetworkPlayer().GetDisplayName();
        string killerName = killerPlayer.GetDisplayName();

        // DIE
        ServerOnPlayerKilled?.Invoke(killedName, killerName);
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

    #endregion

}
