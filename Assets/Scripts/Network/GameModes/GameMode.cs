using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameModes
{ 
    FreeForAll,
    SearchAndDestroy
}

[System.Serializable]
[CreateAssetMenu(menuName = "GameModes/Create GameMode", order = 0)]
public class GameMode : ScriptableObject
{

    public GameModes gameMode;

    public float RespawnTime;

}
