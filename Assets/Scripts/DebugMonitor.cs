using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DebugMonitor : MonoBehaviour
{
    public FirstPersonMovement fpMov;

    [SerializeField] private TextMeshProUGUI isGroundedText;



    /* isgrounded
     * playedid
     * position X,y
     * height
     * iscrouched, isshifted, isjumping
     * crouched height,
     * speed?
     */

    void Update()
    {
        isGroundedText.text = $"IsGrounded : {fpMov.isGrounded}";
    }
}