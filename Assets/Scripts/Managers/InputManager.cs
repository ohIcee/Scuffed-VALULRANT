using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    [SerializeField] private float mouseSensitivityMultiplier = .5f;

    [Header("Scripts")]
    [SerializeField] private FirstPersonMovement fpMov;
    [SerializeField] private PlayerShooting playerShooting;

    private UIManager uIManager;

    PlayerControls controls;
    PlayerControls.PlayerInputActions playerInput;

    Vector2 movementInput;
    Vector2 mouseInput;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        Initialize();
    }

    public void Initialize()
    {
        uIManager = FindObjectOfType<UIManager>();

        controls = new PlayerControls();
        controls.Enable();
        playerInput = controls.PlayerInput;

        playerInput.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();

        playerInput.DebugMonitor.performed += _ => uIManager.toggleDebugMonitor();
        playerInput.EscapeMenu.performed += _ =>
        {
            uIManager.toggleEscapeMenu();
            toggleCursorLock(false);
        };

        playerInput.Jump.performed += _ => fpMov.OnJumpPressed();
        playerInput.Jump.canceled += _ => fpMov.OnJumpReleased();

        playerInput.Crouch.started += _ => fpMov.OnCrouchPressed();
        playerInput.Crouch.canceled += _ => fpMov.OnCrouchReleased();

        playerInput.ShiftWalk.started += _ => fpMov.OnShiftPressed();
        playerInput.ShiftWalk.canceled += _ => fpMov.OnShiftReleased();

        playerInput.Shoot.started += _ =>
        {
            playerShooting.OnStartShooting();
            toggleCursorLock(true);
        };
        playerInput.Shoot.canceled += _ => playerShooting.OnStopShooting();

        playerInput.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        playerInput.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
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
    private void toggleCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}