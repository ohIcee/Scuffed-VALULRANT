using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardScoreEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName_text;
    [SerializeField] private TextMeshProUGUI kills_text;
    [SerializeField] private TextMeshProUGUI deaths_text;
    [SerializeField] private TextMeshProUGUI kdr_text;

    public ValulrantNetworkPlayer assignedPlayer { get; private set; }

    public void AssignPlayer(ValulrantNetworkPlayer player)
    {
        if (assignedPlayer != null) return;

        assignedPlayer = player;

        playerName_text.text = assignedPlayer.GetDisplayName();

        UpdateInfo(assignedPlayer.GetKills(), assignedPlayer.GetDeaths());
    }

    public void UnAssignPlayer() {
        assignedPlayer = null;

        kills_text.text = "";
        deaths_text.text = "";
        kdr_text.text = "";
    }

    public void UpdateInfo(int kills, int deaths)
    {
        kills_text.text = kills.ToString();
        deaths_text.text = deaths.ToString();
        kdr_text.text = (kills / deaths).ToString();
    }
}
