using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{

    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))] private int currentHealth;
    [SyncVar] private bool isDead;

    public bool IsDead => isDead;

    public event Action<string> ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount, string _sourceID)
    {
        if (currentHealth == 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth != 0) return;

        Debug.Log(_sourceID);

        ServerOnDie?.Invoke(_sourceID);

        Debug.Log("We Died!");
        isDead = true;
    }

    [Server]

    public void ResetPlayer()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    #endregion

    #region Client

    public int GetCurrentHealth() => currentHealth;

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion

}
