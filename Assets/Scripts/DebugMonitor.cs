using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DebugMonitor : MonoBehaviour
{
    [SerializeField] private Player fpMov;
    [SerializeField] private ValulrantNetworkPlayer networkPlayer;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerIDText;
    [SerializeField] private TextMeshProUGUI playerMovementSpeedText;
    [SerializeField] private TextMeshProUGUI isShiftingText;
    [SerializeField] private TextMeshProUGUI isCrouchingText;
    [SerializeField] private TextMeshProUGUI isGroundedText;
    [SerializeField] private TextMeshProUGUI isDeadText;

    public void Initialize(Player fpMov)
    {
        this.fpMov = fpMov;
        networkPlayer = fpMov.GetComponent<ValulrantNetworkPlayer>();
    }

    void Update()
    {
        playerNameText.text = $" {networkPlayer.getDisplayName()}";
        playerIDText.text = $"Player ID: {networkPlayer.netId}";
        playerMovementSpeedText.text = $"{fpMov.characterVelocity}";
        isShiftingText.text = $"Is Shifting: {fpMov.isPressingShift}";
        isCrouchingText.text = $"Is Crouching: {fpMov.IsCrouching}";
        isGroundedText.text = $"Is Grounded: {fpMov.IsGrounded}";
        isDeadText.text = $"Is Dead: {fpMov.GetHealth().IsDead}";
    }
}