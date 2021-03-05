using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    [SerializeField] private float mouseSensitivityMultiplier = .5f;

    [Header("Scripts")]
    [SerializeField] private FirstPersonMovement fpMov;
    [SerializeField] private PlayerShooting playerShooting;

    PlayerControls controls;
    PlayerControls.GroundMovementActions playerMovement;

    Vector2 movementInput;
    Vector2 mouseInput;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        Initialize();
    }

    public void Initialize()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controls = new PlayerControls();
        controls.Enable();
        playerMovement = controls.GroundMovement;

        playerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();

        playerMovement.Jump.performed += _ => fpMov.OnJumpPressed();
        playerMovement.Jump.canceled += _ => fpMov.OnJumpReleased();

        playerMovement.Crouch.started += _ => fpMov.OnCrouchPressed();
        playerMovement.Crouch.canceled += _ => fpMov.OnCrouchReleased();

        playerMovement.ShiftWalk.started += _ => fpMov.OnShiftPressed();
        playerMovement.ShiftWalk.canceled += _ => fpMov.OnShiftReleased();

        playerMovement.Shoot.started += _ => playerShooting.OnStartShooting();
        playerMovement.Shoot.canceled += _ => playerShooting.OnStopShooting();

        playerMovement.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        playerMovement.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
    }

    private void Update()
    {
        if (!hasAuthority) return;

        fpMov.ReceiveMovementInput(movementInput);
        fpMov.ReceiveMouseInput(mouseInput * mouseSensitivityMultiplier);
    }

    private void OnDestroy()
    {
        if (!hasAuthority) return;

        controls.Disable();
    }
}