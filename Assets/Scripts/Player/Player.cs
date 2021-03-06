using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using System.Collections;

public class Player : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private UIManager uiManager;

    [Tooltip("Reference to the main camera used for the player")]
    [SerializeField] private Camera playerCamera;
    [Tooltip("Audio source for footsteps, jump, etc...")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource jumpAudioSource;
    [SerializeField] private AudioSource landAudioSource;
    [SerializeField] private AudioSource fallDamageAudioSource;

    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    [SerializeField] private float gravityDownForce = 20f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    [SerializeField] private LayerMask groundCheckLayers = -1;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    [SerializeField] private float groundCheckDistance = 0.05f;

    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    [SerializeField] private float maxSpeedOnGround = 10f;
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    [SerializeField] private float movementSharpnessOnGround = 15;
    [Tooltip("Max movement speed when crouching")]
    [Range(0, 1)]
    [SerializeField] private float maxSpeedCrouchedRatio = 0.5f;
    [Tooltip("Max movement speed when not grounded")]
    [SerializeField] private float maxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when in the air")]
    [SerializeField] private float accelerationSpeedInAir = 25f;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    [SerializeField] private float sprintSpeedModifier = 2f;
    [Tooltip("Height at which the player dies instantly when falling off the map")]
    [SerializeField] private float killHeight = -50f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    [SerializeField] private float rotationSpeed = 200f;
    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed multiplier when aiming")]
#pragma warning disable 0414 // don't warn about unused variable
    [SerializeField] private float aimingRotationMultiplier = 0.4f;
#pragma warning restore 0414

    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    [SerializeField] private float jumpForce = 9f;

    [Header("Stance")]
    [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    [SerializeField] private float cameraHeightRatio = 0.9f;
    [Tooltip("Height of character when standing")]
    [SerializeField] private float capsuleHeightStanding = 1.8f;
    [Tooltip("Height of character when crouching")]
    [SerializeField] private float capsuleHeightCrouching = 0.9f;
    [Tooltip("Speed of crouching transitions")]
    [SerializeField] private float crouchingSharpness = 10f;

    [Header("Audio")]
    [Tooltip("Amount of footstep sounds played when moving one meter")]
    [SerializeField] private float footstepSFXFrequency = 1f;
    [Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
    [SerializeField] private float footstepSFXFrequencyWhileSprinting = 1f;
    [Tooltip("Sound played for footsteps")]
    [SerializeField] private AudioClip footstepSFX;
    [Tooltip("Sound played when jumping")]
    [SerializeField] private AudioClip jumpSFX;
    [Tooltip("Sound played when landing")]
    [SerializeField] private AudioClip landSFX;
    [Tooltip("Sound played when taking damage froma fall")]
    [SerializeField] private AudioClip fallDamageSFX;

    [Header("Fall Damage")]
    [Tooltip("Whether the player will recieve damage when hitting the ground at high speed")]
    [SerializeField] private bool recievesFallDamage;
    [Tooltip("Minimun fall speed for recieving fall damage")]
    [SerializeField] private float minSpeedForFallDamage = 10f;
    [Tooltip("Fall speed for recieving th emaximum amount of fall damage")]
    [SerializeField] private float maxSpeedForFallDamage = 30f;
    [Tooltip("Damage recieved when falling at the mimimum speed")]
    [SerializeField] private float fallDamageAtMinSpeed = 10f;
    [Tooltip("Damage recieved when falling at the maximum speed")]
    [SerializeField] private float fallDamageAtMaxSpeed = 50f;

    [SerializeField] private PlayerNameplate playerNameplate;

    public UnityAction<bool> onStanceChanged;
    public Vector3 characterVelocity { get; set; }

    private bool isGrounded { get; set; }
    public bool IsGrounded => isGrounded;

    private bool isCrouching { get; set; }
    public bool IsCrouching => isCrouching;

    private bool isPressingJump = false;
    [HideInInspector] public bool isPressingShift = false;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 mouseInput = Vector2.zero;

    public float RotationMultiplier
    {
        get
        {
            //if (m_WeaponsManager.isAiming)
            //{
            //    return aimingRotationMultiplier;
            //}

            return 1f;
        }
    }

    [SerializeField] private CharacterController m_Controller;
    Vector3 m_GroundNormal;
    Vector3 m_CharacterVelocity;
    Vector3 m_LatestImpactSpeed;
    float m_LastTimeJumped = 0f;
    float m_CameraVerticalAngle = 0f;
    float m_footstepDistanceCounter;
    float m_TargetCharacterHeight;
    [HideInInspector] public float currentHeight;

    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private PlayerHUD playerHUD;

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) return;

        // check for Y kill
        //if (!isDead && transform.position.y < killHeight)
        if (transform.position.y < killHeight)
        {
            //RpcTakeDamage(9999, transform.name);
        }

        bool wasGrounded = isGrounded;
        GroundCheck();

        // landing
        if (isGrounded && !wasGrounded)
        {
            // Fall damage
            float fallSpeed = -Mathf.Min(characterVelocity.y, m_LatestImpactSpeed.y);
            float fallSpeedRatio = (fallSpeed - minSpeedForFallDamage) / (maxSpeedForFallDamage - minSpeedForFallDamage);
            if (recievesFallDamage && fallSpeedRatio > 0f)
            {
                float dmgFromFall = Mathf.Lerp(fallDamageAtMinSpeed, fallDamageAtMaxSpeed, fallSpeedRatio);
                //RpcTakeDamage((int)dmgFromFall, transform.name);

                // fall damage SFX
                if (fallDamageAudioSource)
                    fallDamageAudioSource.PlayOneShot(fallDamageSFX);
            }
            else
            {
                // land SFX
                if (landAudioSource)
                    landAudioSource.PlayOneShot(landSFX);
            }
        }

        UpdateCharacterHeight(false);

        HandleCharacterMovement();
    }

    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

        // reset values before the ground check
        isGrounded = false;
        m_GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height), m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                m_GroundNormal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    isGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > m_Controller.skinWidth)
                    {
                        m_Controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    void HandleCharacterMovement()
    {
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(new Vector3(0f, (mouseInput.x * rotationSpeed * RotationMultiplier), 0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += -mouseInput.y * rotationSpeed * RotationMultiplier;

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
        }

        // character movement handling
        bool isSprinting = isPressingShift;
        {
            if (isSprinting)
            {
                isSprinting = SetCrouchingState(false, false);
            }

            float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(new Vector3(movementInput.x, 0f, movementInput.y));

            // handle grounded movement
            if (isGrounded)
            {
                // calculate the desired velocity from inputs, max speed, and current slope
                Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround * speedModifier;
                // reduce speed if crouching by crouch speed ratio
                if (isCrouching)
                    targetVelocity *= maxSpeedCrouchedRatio;
                targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) * targetVelocity.magnitude;

                // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

                // jumping
                if (isGrounded && isPressingJump)
                {
                    isPressingJump = false;

                    // force the crouch state to false
                    if (SetCrouchingState(false, false))
                    {
                        // start by canceling out the vertical component of our velocity
                        characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

                        // then, add the jumpSpeed value upwards
                        characterVelocity += Vector3.up * jumpForce;

                        // play sound
                        if (jumpAudioSource)
                            jumpAudioSource.PlayOneShot(jumpSFX);

                        // remember last time we jumped because we need to prevent snapping to ground for a short time
                        m_LastTimeJumped = Time.time;

                        // Force grounding to false
                        isGrounded = false;
                        m_GroundNormal = Vector3.up;
                    }
                }

                // footsteps sound
                float chosenFootstepSFXFrequency = (isSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency);
                if (m_footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
                {
                    m_footstepDistanceCounter = 0f;

                    if (!isCrouching)
                    {
                        if (footstepAudioSource)
                            footstepAudioSource.PlayOneShot(footstepSFX);
                    }
                }

                // keep track of distance traveled for footsteps sound
                m_footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
            }
            // handle air movement
            else
            {
                // add air acceleration
                characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

                // limit air speed to a maximum, but only horizontally
                float verticalVelocity = characterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
                characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
            }
        }

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);
        m_Controller.Move(characterVelocity * Time.deltaTime);

        // detect obstructions to adjust velocity accordingly
        m_LatestImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            // We remember the last impact speed because the fall damage logic might need it
            m_LatestImpactSpeed = characterVelocity;

            characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
        }
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    void UpdateCharacterHeight(bool force)
    {
        // Update height instantly
        if (force)
        {
            m_Controller.height = m_TargetCharacterHeight;
            currentHeight = m_Controller.height;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * cameraHeightRatio;
            //m_Actor.aimPoint.transform.localPosition = m_Controller.center;
        }
        // Update smooth height
        else if (m_Controller.height != m_TargetCharacterHeight)
        {
            // resize the capsule and adjust camera position
            m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight, crouchingSharpness * Time.deltaTime);
            currentHeight = m_Controller.height;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * m_TargetCharacterHeight * cameraHeightRatio, crouchingSharpness * Time.deltaTime);
            //m_Actor.aimPoint.transform.localPosition = m_Controller.center;
        }
    }

    // returns false if there was an obstruction
    bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        // set appropriate heights
        if (crouched)
        {
            m_TargetCharacterHeight = capsuleHeightCrouching;
        }
        else
        {
            // Detect obstructions
            if (!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(capsuleHeightStanding),
                    m_Controller.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);
                foreach (Collider c in standingOverlaps)
                {
                    if (c != m_Controller)
                    {
                        return false;
                    }
                }
            }

            m_TargetCharacterHeight = capsuleHeightStanding;
        }

        if (onStanceChanged != null)
        {
            onStanceChanged.Invoke(crouched);
        }

        isCrouching = crouched;
        return true;
    }

    #region Inputs

    public void OnJumpPressed() => isPressingJump = true;
    public void OnJumpReleased() => isPressingJump = false;
    public void OnShiftPressed() => isPressingShift = true;
    public void OnShiftReleased() => isPressingShift = false;
    public void OnCrouchPressed() => SetCrouchingState(isCrouching, false);
    public void OnCrouchReleased() => SetCrouchingState(!isCrouching, false);
    public void ReceiveMovementInput(Vector2 movementInput) => this.movementInput = movementInput;
    public void ReceiveMouseInput(Vector2 mouseInput) {
        this.mouseInput = mouseInput; 
    }

    #endregion

}