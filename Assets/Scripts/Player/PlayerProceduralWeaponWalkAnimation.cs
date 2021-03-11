using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class PlayerProceduralWeaponWalkAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool enableBobbing = true;
    [SerializeField] private float walkBobbingSpeed = 10f;
    [SerializeField] private float shiftBobbingSpeed = 10f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float shiftBobAmount = 0.05f;

    [SerializeField] private float jumpingYOffset = -.1f;
    [SerializeField] private float landResetTime = .5f;
 
    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private Player player = null;
 
    private float defaultYPos;
    private float currBobAmount = 0f;

    private bool previouslyGrounded = false;
    private float currentLandResetTime = 0f;

    private void Start()
    {
        defaultYPos = transform.localPosition.y;
    }

    public void HasJumped()
    {
        Debug.Log($"Setting land reset time to {landResetTime}");
        currentLandResetTime = landResetTime;        
    }

    private void Update()
    {

        if (!enableBobbing) return;

        if (!player.IsGrounded)
        {
            currBobAmount = 0;
            transform.localPosition = new Vector3(
                transform.localPosition.x, 
                transform.localPosition.y,
                Mathf.Lerp(transform.localPosition.z, defaultYPos + jumpingYOffset, Time.deltaTime * walkBobbingSpeed)
                );

            return;
        }

        if (currentLandResetTime > 0f && player.IsGrounded)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x, 
                transform.localPosition.y,
                Mathf.Lerp(transform.localPosition.z, defaultYPos, Time.deltaTime * walkBobbingSpeed)
                );

            currentLandResetTime -= Time.deltaTime;
            return;
        }

        if (!player.IsPressingMovementInputs()) {
            currBobAmount = 0;
            transform.localPosition = new Vector3(
                transform.localPosition.x, 
                transform.localPosition.y,
                Mathf.Lerp(transform.localPosition.z, defaultYPos, Time.deltaTime * (player.isPressingShift || player.IsCrouching ? shiftBobbingSpeed : walkBobbingSpeed))
                );
        }
        else if (player.IsPressingMovementInputs() && player.IsGrounded)
        {
            currBobAmount += Time.deltaTime * (player.isPressingShift || player.IsCrouching ? shiftBobbingSpeed : walkBobbingSpeed);
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                defaultYPos + Mathf.Sin(currBobAmount) * (player.isPressingShift || player.IsCrouching ? shiftBobAmount : walkBobAmount)
                );
        }

    }
}
