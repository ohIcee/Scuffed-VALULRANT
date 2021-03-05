using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : NetworkBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform headCamera;  // Head to lower when we crouch
    [SerializeField] private Transform body;

    [Header("Values")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -30f; // Default: -9.81

    private Vector3 lastPosition;
    private Vector3 playerVelocity;

    [SerializeField] private float goingDownSlopesForce = 1f;
    [SerializeField] private float slopeCheckOffsetMultiplier = .5f;
    [SerializeField] private Vector3 slopeCheckOffset;
    [SerializeField] private float slopeCheckDistance = 1f;

    [Header("Scripts")]
    [SerializeField] private GroundCheck groundCheck;

    #region movement

    private float movingSpeed;
    Vector2 movementInput;
    Vector3 verticalVelocity = Vector3.zero;

    #endregion

    #region shifting

    [Header("Values")]
    public float shiftSpeed = 3f;

    [HideInInspector] public bool IsShifting;
    [HideInInspector] public bool CanShift = true;

    public event System.Action ShiftWalking;

    #endregion

    #region jump

    [Header("Values")]
    public float JumpHeight = 2f;

    [HideInInspector] public bool IsJumping;
    [HideInInspector] public bool CanJump = true;

    public event System.Action Jumped;

    #endregion

    #region crouching
    public float crouchSpeed = 2;
    [SerializeField] private float crouchYLocalPosition = 1;
    [SerializeField] private float crouchedCapsuleHeight;

    [HideInInspector] public bool CanCrouch = true; // Cant crouch if frozen? example
    [HideInInspector] public bool IsCrouchPressed;
    [HideInInspector] public bool IsCrouched;

    private float defaultHeadYLocalPosition;
    private float defaultHeight;
    private float defaultCapsuleHeight;

    public event System.Action CrouchStart, CrouchEnd;

    #endregion

    void Start()
    {
        defaultHeadYLocalPosition = headCamera.localPosition.y;
        defaultHeight = characterController.height;
        defaultCapsuleHeight = body.localScale.y;

        lastPosition = transform.position;
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) return;

        playerVelocity = transform.position - lastPosition;
        lastPosition = transform.position;

        Crouching();

        if (IsShifting)
        {
            movingSpeed = shiftSpeed;
        }
        else
        {
            if (IsCrouched)
            {
                movingSpeed = crouchSpeed;
            }
            else
            {
                movingSpeed = speed;
            }
        }

        Vector3 horizontalVelocity = (transform.right * movementInput.x + transform.forward * movementInput.y) * movingSpeed;
        //controller.Move(horizontalVelocity * Time.deltaTime);

        if (groundCheck.isGrounded)
        {
            verticalVelocity.y = 0;
        }

        Jumping();

        if (groundCheck.isGrounded)
        {
            CheckSlope();
        }

        //if (groundCheck.isGrounded)
        //{
        //    // force down
        //    verticalVelocity.y -= goingDownSlopesForce;
        //}

        verticalVelocity.y += gravity;
        //controller.Move(verticalVelocity * Time.deltaTime);

        characterController.Move(new Vector3(horizontalVelocity.x, verticalVelocity.y, horizontalVelocity.z) * Time.deltaTime);
    }

    private void CheckSlope() {
        Vector3 rayOrigin = transform.position + slopeCheckOffset + characterController.velocity * slopeCheckOffsetMultiplier;
        Debug.DrawRay(rayOrigin, -transform.up * slopeCheckDistance);

        //Debug.Log();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + slopeCheckOffset + characterController.velocity * slopeCheckOffsetMultiplier, .1f);
    }

    private void Crouching() {
        if (!IsCrouched && IsCrouchPressed)
        {
            // Enforce crouched y local position of the head.
            headCamera.localPosition = new Vector3(headCamera.localPosition.x, crouchYLocalPosition, headCamera.localPosition.z);

            // Lower the character controller.
            characterController.height = defaultHeight - (defaultHeadYLocalPosition - crouchYLocalPosition);
            characterController.center = Vector3.up * characterController.height * .5f;

            // Lower the capsule body.
            body.localPosition += new Vector3(0f, 1 - crouchedCapsuleHeight, 0f);
            body.localScale = new Vector3(body.localScale.x, crouchedCapsuleHeight, body.localScale.z);

            // Set state.
            if (!IsCrouched)
            {
                IsCrouched = true;
                CrouchStart?.Invoke();
            }
        }
        else if (!IsCrouchPressed && IsCrouched)
        {
            // Reset the head to its default y local position.
            headCamera.localPosition = new Vector3(headCamera.localPosition.x, defaultHeadYLocalPosition, headCamera.localPosition.z);

            // Reset the character controller's position.
            characterController.height = defaultHeight;
            characterController.center = Vector3.up * characterController.height * .5f;

            // Reset the capsule body.
            body.localPosition -= new Vector3(0f, 1 - crouchedCapsuleHeight, 0f);
            body.localScale = new Vector3(body.localScale.x, defaultCapsuleHeight, body.localScale.z);

            // Reset state.
            IsCrouched = false;
            CrouchEnd?.Invoke();
        }
    }

    private void Jumping() {
        // Jump: v = sqrt(-2 * jumpHeight * gravity)
        if (IsJumping)
        {
            if (groundCheck.isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(-2f * JumpHeight * gravity);
                //controller.Move(verticalVelocity * Time.deltaTime);
            }
            IsJumping = false;
        }
    }

    #region Inputs

    public void OnJumpPressed()
    {
        if (CanJump)
        {
            IsJumping = true;
            Jumped?.Invoke();
        }
    }
    public void OnShiftPressed()
    {
        if (CanShift)
        {
            IsShifting = true;
            ShiftWalking?.Invoke();
        }
    }

    public void OnShiftReleased() => IsShifting = false;
    public void OnCrouchPressed() => IsCrouchPressed = true;
    public void OnCrouchReleased() => IsCrouchPressed = false;
    public void ReceiveMovementInput(Vector2 movementInput) => this.movementInput = movementInput;

    #endregion

}