using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DebugMonitor : MonoBehaviour
{
    [SerializeField] private FirstPersonMovement fpMov;
    [SerializeField] private ValulrantNetworkPlayer networkPlayer;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerIDText;
    [SerializeField] private TextMeshProUGUI playerMovementSpeedText;
    [SerializeField] private TextMeshProUGUI playerHeightText;
    [SerializeField] private TextMeshProUGUI isShiftingText;
    [SerializeField] private TextMeshProUGUI isCrouchingText;
    [SerializeField] private TextMeshProUGUI isGroundedText;
    [SerializeField] private TextMeshProUGUI isDeadText;



    /* isgrounded
     * playedid
     * position X,y
     * 
     */

    public void Initialize(FirstPersonMovement fpMov)
    {
        this.fpMov = fpMov;
        networkPlayer = fpMov.GetComponent<ValulrantNetworkPlayer>();
    }

    void Update()
    {
        playerNameText.text = $" {networkPlayer.getDisplayName()}";
        playerIDText.text = $"Player ID: {networkPlayer.netId}";
        playerMovementSpeedText.text = $"{fpMov.characterVelocity}";
        playerHeightText.text = $"Height: {fpMov.currentHeight}";
        isShiftingText.text = $"Is Shifting: {fpMov.isPressingShift}";
        isCrouchingText.text = $"Is Crouching: {fpMov.isCrouching}";
        isGroundedText.text = $"Is Grounded: {fpMov.isGrounded}";
        isDeadText.text = $"Is Dead: {fpMov.isDead}";
    }
}