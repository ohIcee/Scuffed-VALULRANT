using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerEquipment : NetworkBehaviour
{

    [SerializeField] private Player player;

    [SerializeField] private int kevlarPrice;
    [SerializeField] private int helmetPrice;

    [SyncVar(hook = nameof(ClientHandleHelmetDurabilityUpdated))] private int helmetDurability;
    [SyncVar(hook = nameof(ClientHandleKevlarDurabilityUpdated))] private int kevlarDurability;

    public event Action<int> ClientOnHelmetDurabilityUpdated;
    public event Action<int> ClientOnKevlarDurabilityUpdated;

    #region Client

    public void TryBuyKevlarAndHelmet() {
        TryBuyKevlar();
        TryBuyHelmet();
    }

    public void TryBuyKevlar() {
        CmdBuyKevlar();
    }

    public void TryBuyHelmet() {
        CmdBuyHelmet();
    }

    private void ClientHandleHelmetDurabilityUpdated(int oldDur, int newDur)
    {
        if (!hasAuthority) return;

        ClientOnHelmetDurabilityUpdated?.Invoke(newDur);
    }

    private void ClientHandleKevlarDurabilityUpdated(int oldDur, int newDur)
    {
        if (!hasAuthority) return;

        ClientOnKevlarDurabilityUpdated?.Invoke(newDur);
    }

    #endregion

    #region Server

    private void CmdBuyKevlar()
    {
        ValulrantNetworkPlayer networkPlayer = player.GetNetworkPlayer();
        int money = networkPlayer.GetMoney();

        if (kevlarDurability < 100 && money >= kevlarPrice)
        {
            networkPlayer.SubtractMoney(kevlarPrice);
            kevlarDurability = 100;
        }
    }

    private void CmdBuyHelmet()
    {
        ValulrantNetworkPlayer networkPlayer = player.GetNetworkPlayer();
        int money = networkPlayer.GetMoney();

        if (helmetDurability < 100 && money >= helmetPrice)
        {
            networkPlayer.SubtractMoney(helmetPrice);
            helmetDurability = 100;
        }
    }

    #endregion

}
