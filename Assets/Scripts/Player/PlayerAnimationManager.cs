using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAnimationManager : NetworkBehaviour
{

    [Header("PLAYER ANIMATIONS")]
    [SerializeField] private Animator playerBodyAnimator;
    [SerializeField] private float movementDirectionDampTime;
    [SerializeField] private string XVELOCITYPARAM = "VelocityX";
    [SerializeField] private string ZVELOCITYPARAM = "VelocityZ";
    [SerializeField] private string CROUCHPARAM = "IsCrouching";
    [SerializeField] private string ISGROUNDEDPARAM = "IsGrounded";

    #region Client

    public void UpdateVelocities(float xVel, float zVel) => CmdUpdateVelocities(xVel, zVel);
    public void UpdateCrouch(bool crouch) => CmdUpdateCrouch(crouch);
    public void UpdateIsGrounded(bool grounded) { 
        CmdUpdateIsGrounded(grounded);
    }

    [ClientRpc]
    private void RpcUpdateVelocities(float xVel, float zVel) 
    {
        if (hasAuthority) return;

        playerBodyAnimator.SetFloat(XVELOCITYPARAM, xVel, movementDirectionDampTime, Time.deltaTime);
        playerBodyAnimator.SetFloat(ZVELOCITYPARAM, zVel, movementDirectionDampTime, Time.deltaTime);
    }

    [ClientRpc]
    private void RpcUpdateCrouch(bool crouch)
    {
        if (hasAuthority) return;

        playerBodyAnimator.SetBool(CROUCHPARAM, crouch);
    }

    [ClientRpc]
    private void RpcUpdateIsGrounded(bool grounded)
    {
        if (hasAuthority) return;

        playerBodyAnimator.SetBool(ISGROUNDEDPARAM, grounded);
    }

    #endregion

    #region Server

    [Command] private void CmdUpdateVelocities(float xVel, float zVel) => RpcUpdateVelocities(xVel, zVel);
    [Command] private void CmdUpdateCrouch(bool crouch) => RpcUpdateCrouch(crouch);
    [Command] private void CmdUpdateIsGrounded(bool grounded) => RpcUpdateIsGrounded(grounded);

    #endregion

}
