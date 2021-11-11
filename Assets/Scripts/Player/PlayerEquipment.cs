using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerEquipment : NetworkBehaviour
{

    [SerializeField] private Player player;

    [SerializeField] private int lightKevlarPrice;
    [SerializeField] private int heavyKevlarPrice;

    [SerializeField] private int lightKevlarValue;
    [SerializeField] private int heavyKevlarValue;

    [SerializeField] private float kevlarDamageDecreaseMultiplier;

    [SyncVar(hook = nameof(ClientHandleKevlarDurabilityUpdated))] 
    private int kevlarDurability;

    public event Action<int> ClientOnKevlarDurabilityUpdated;

    public int GetKevlarDurability() => kevlarDurability;
    public float GetDamageDecreaseMultiplier() => kevlarDamageDecreaseMultiplier;

    #region Client

    public void TryBuyLightKevlar() {
        CmdBuyLightKevlar();
    }

    public void TryBuyHeavyKevlar() {
        CmdBuyHeavyKevlar();
    }
    
    [Client]
    private void ClientHandleKevlarDurabilityUpdated(int oldDur, int newDur)
    {
        ClientOnKevlarDurabilityUpdated?.Invoke(newDur);
    }

    #endregion

    #region Server

    public void UpdateKevlarDurability(int durability)
    {
        kevlarDurability = durability;
    }

    [Command]
    private void CmdBuyLightKevlar()
    {
        ValulrantNetworkPlayer networkPlayer = player.GetNetworkPlayer();
        int money = networkPlayer.GetMoney();

        if (kevlarDurability < lightKevlarValue && money >= lightKevlarPrice)
        {
            networkPlayer.SubtractMoney(lightKevlarPrice);
            kevlarDurability = lightKevlarValue;
        }
    }

    [Command]
    private void CmdBuyHeavyKevlar()
    {
        ValulrantNetworkPlayer networkPlayer = player.GetNetworkPlayer();
        int money = networkPlayer.GetMoney();

        if (kevlarDurability < heavyKevlarValue && money >= heavyKevlarPrice)
        {
            networkPlayer.SubtractMoney(heavyKevlarPrice);
            kevlarDurability = heavyKevlarValue;
        }
    }

    #endregion

}
