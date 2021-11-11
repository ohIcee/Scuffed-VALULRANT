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

    public event Action<ValulrantNetworkPlayer, ValulrantNetworkPlayer> ServerOnPlayerKilled;

    public bool IsDead() => currentHealth <= 0;

    #region Server

    [Server]
    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, 100);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        currentHealth = maxHealth;
    }

    /// <summary>
    /// Deal damage to player
    /// </summary>
    /// <param name="damageAmount">Amount of damage dealt</param>
    /// <param name="killerPlayer">Player who dealt the damage</param>
    [Server]
    public void DealDamage(int damageAmount, ValulrantNetworkPlayer killerPlayer)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth > 0) return;

        // DIE
        ServerOnPlayerKilled?.Invoke(GetComponent<Player>().GetNetworkPlayer(), killerPlayer);

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
