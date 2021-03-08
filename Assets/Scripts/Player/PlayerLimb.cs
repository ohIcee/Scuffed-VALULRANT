using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLimb : MonoBehaviour
{
    [SerializeField] private Player owningPlayer;
    [SerializeField] private float damageMultiplier;

    public Player GetOwningPlayer() => owningPlayer;
    public float GetDamageMultiplier() => damageMultiplier;
}
