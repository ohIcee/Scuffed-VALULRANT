using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

public class Player : NetworkBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    [SerializeField] private Camera playerCamera;
    [Tooltip("Audio source for footsteps, jump, etc...")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource jumpAudioSource;
    [SerializeField] private AudioSource landAudioSource;
    [SerializeField] private AudioSource fallDamageAudioSource;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerFiring playerFiring;
    [SerializeField] private CharacterController m_Controller;
    [SerializeField] private PlayerProceduralWeaponWalkAnimation playerProceduralWeaponWalkAnimation;
    [SerializeField] private PlayerAnimationManager animationManager;

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
    [SerializeField] private float shiftSpeedModifier = 0.50f;
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
    [SerializeField] private float landCheckRayLength = 1f;

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
    [SerializeField] private List<AudioClip> footstepSFXs;
    [Tooltip("Sound played when jumping")]
    [SerializeField] private List<AudioClip> jumpSFXs;
    [Tooltip("Sound played when landing")]
    [SerializeField] private List<AudioClip> landSFXs;
    [Tooltip("Sound played when taking damage froma fall")]
    [SerializeField] private List<AudioClip> fallDamageSFXs;

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

    [SyncVar] private ValulrantNetworkPlayer networkPlayer;

    public NetworkIdentity GetNetworkIdentity() => netIdentity;

    [Server]
    public void SetNetworkPlayer(ValulrantNetworkPlayer player)
    {
        networkPlayer = player;
    }
    public ValulrantNetworkPlayer GetNetworkPlayer() => networkPlayer;

    public UnityAction<bool> onStanceChanged;
    public Vector3 characterVelocity { get; set; }

    private bool isGrounded = true;
    public bool GetIsGrounded() => isGrounded;

    private bool isCrouching { get; set; }
    public bool GetIsCrouching() => isCrouching;

    private bool isPressingJump = false;
    [HideInInspector] public bool isPressingShift = false;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 mouseInput = Vector2.zero;

    public float RotationMultiplier
    {
        get
        {
            return 1f;
        }
    }

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

    private void Start()
    {
        SetCrouchingState(false, false);
        ChangeSensitivity(FindObjectOfType<SettingsManager>()?.GetMouseSensitivity() ?? 0.25f);
    }

    [Command]
    public void CmdDealDamage(int damageAmount)
    {
        playerHealth.DealDamage(damageAmount, networkPlayer);
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) return;

        // Kill player if he falls below the killheight (falls off the map)
        if (transform.position.y < killHeight)
        {
            CmdDealDamage(9999);
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
                if (fallDamageAudioSource) CmdPlayFallDamageSound();
                //fallDamageAudioSource.PlayOneShot(fallDamageSFXs);
            }
            else
            {
                // land SFX
                if (landAudioSource) CmdPlayLandSound();
                //landAudioSource.PlayOneShot(landSFX);
            }
        }

        UpdateCharacterHeight(false);

        CheckPreLand();

        HandleCharacterMovement();
    }

    // Raycasts a ray below the player to decent when
    // he will land to play the landing animation before
    // he lands. Makes it look more natural
    private void CheckPreLand()
    {
        Debug.DrawRay(transform.position, -transform.up * landCheckRayLength, Color.red, .1f);

        if (Physics.Raycast(transform.position, -transform.up, landCheckRayLength, groundCheckLayers))
        {
            animationManager.UpdateIsGrounded(true);
        }
        else {
            animationManager.UpdateIsGrounded(false);
        }
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

    public void ChangeSensitivity(float sens)
    {
        rotationSpeed = sens;
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
        bool isShifting = isPressingShift;
        {
            float xVel = 0f, zVel = 0f;
            if (movementInput.x > 0)
            {
                if (isShifting)
                    xVel = .5f;
                else
                    xVel = 1f;
            }
            else if (movementInput.x < 0)
            {
                if (isShifting)
                    xVel = -.5f;
                else
                    xVel = -1f;
            }

            if (movementInput.y > 0)
            {
                if (isShifting)
                    zVel = .5f;
                else
                    zVel = 1f;
            }
            else if (movementInput.y < 0)
            {
                if (isShifting)
                    zVel = -.5f;
                else
                    zVel = -1f;
            }
            animationManager.UpdateVelocities(xVel, zVel);

            if (isShifting)
            {
                isShifting = SetCrouchingState(false, false);
            }

            float speedModifier = isShifting ? shiftSpeedModifier : 1f;

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
                    playerProceduralWeaponWalkAnimation.HasJumped();

                    // force the crouch state to false
                    if (SetCrouchingState(false, false))
                    {
                        // start by canceling out the vertical component of our velocity
                        characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

                        // then, add the jumpSpeed value upwards
                        characterVelocity += Vector3.up * jumpForce;


                        // reduce velocity if Shifting -> Jump
                        characterVelocity = isShifting ? characterVelocity * 0.80f : characterVelocity;

                        // play sound
                        if (jumpAudioSource) CmdPlayJumpSound();

                        // remember last time we jumped because we need to prevent snapping to ground for a short time
                        m_LastTimeJumped = Time.time;

                        // Force grounding to false
                        isGrounded = false;
                        m_GroundNormal = Vector3.up;
                    }
                }

                // footsteps sound
                float chosenFootstepSFXFrequency = (isShifting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency);
                if (m_footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
                {
                    m_footstepDistanceCounter = 0f;

                    if (!isCrouching)
                    {
                        if (footstepAudioSource)
                            CmdPlayFootstepSound();
                    }
                }

                // keep track of distance traveled for footsteps sound
                m_footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
            }
            // handle air movement
            else
            {
                // lower A D effect by /2
                worldspaceMoveInput = transform.TransformVector(new Vector3(movementInput.x / 2, 0f, movementInput.y));

                // add air acceleration
                characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

                // limit air speed to a maximum, but only horizontally
                float verticalVelocity = characterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir);
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

    #region Networking Audio Sources

    [Command]
    private void CmdPlayFootstepSound()
    {
        RpcPlayFootstepSound();
    }

    [ClientRpc]
    private void RpcPlayFootstepSound()
    {
        footstepAudioSource.PlayOneShot(footstepSFXs[Random.Range(0, footstepSFXs.Count - 1)]);
    }

    [Command]
    private void CmdPlayJumpSound()
    {
        RpcPlayJumpSound();
    }

    [ClientRpc]
    private void RpcPlayJumpSound()
    {
        jumpAudioSource.PlayOneShot(jumpSFXs[Random.Range(0, jumpSFXs.Count - 1)]);
    }

    [Command]
    private void CmdPlayLandSound()
    {
        RpcPlayLandSound();
    }

    [ClientRpc]
    private void RpcPlayLandSound()
    {
        landAudioSource.PlayOneShot(landSFXs[Random.Range(0, landSFXs.Count - 1)]);
    }

    [Command]
    private void CmdPlayFallDamageSound()
    {
        RpcPlayFallDamageSound();
    }

    [ClientRpc]
    private void RpcPlayFallDamageSound()
    {
        fallDamageAudioSource.PlayOneShot(fallDamageSFXs[Random.Range(0, fallDamageSFXs.Count - 1)]);
    }

    #endregion

    #region Helper Functions

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

    #endregion

    #region Crouching Functions

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
        //playerBodyAnimator.SetBool(CROUCHPARAM, crouched);
        animationManager.UpdateCrouch(crouched);

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

    #endregion

    #region Inputs

    public void OnJumpPressed() => isPressingJump = true;
    public void OnJumpReleased() => isPressingJump = false;
    public void OnShiftPressed() => isPressingShift = true;
    public void OnShiftReleased() => isPressingShift = false;
    public void OnCrouchPressed() => SetCrouchingState(true, false);
    public void OnCrouchReleased() => SetCrouchingState(false, false);
    public void ReceiveMovementInput(Vector2 movementInput) => this.movementInput = movementInput;
    public void ReceiveMouseInput(Vector2 mouseInput) => this.mouseInput = mouseInput;

    public bool IsPressingMovementInputs() => movementInput != Vector2.zero;

    #endregion

}