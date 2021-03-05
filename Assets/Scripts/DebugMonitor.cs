using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DebugMonitor : MonoBehaviour
{
    private FirstPersonMovement fpMov;
    private ValulrantNetworkPlayer networkPlayer;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerIDText;
    [SerializeField] private TextMeshProUGUI isGroundedText;



    /* isgrounded
     * playedid
     * position X,y
     * height
     * iscrouched, isshifted, isjumping
     * crouched height,
     * speed?
     */

    public void Initialize(FirstPersonMovement fpMov)
    {
        this.fpMov = fpMov;
        networkPlayer = fpMov.GetComponent<ValulrantNetworkPlayer>();
    }

    void Update()
    {
       playerNameText.text = $" {networkPlayer.getDisplayName()}";
       playerIDText.text = $"Player ID : {networkPlayer.netId}";
       isGroundedText.text = $"IsGrounded : {fpMov.isGrounded}";
    }
}