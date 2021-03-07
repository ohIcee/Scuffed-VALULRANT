using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    [SerializeField] private float weaponSwaySensitivityMultiplier = .5f;

    [Header("Scripts")]
    [SerializeField] private Player fpMov;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] WeaponSway weaponSway;

    private UIManager uIManager;

    private PlayerControls controls;
    private PlayerControls.PlayerInputActions playerInput;

    private Vector2 movementInput;
    private Vector2 mouseInput;

    public override void OnStartAuthority()
    {
        Initialize();
    }

    public void Initialize()
    {
        //uIManager = FindObjectOfType<UIManager>();

        controls = new PlayerControls();
        controls.Enable();
        playerInput = controls.PlayerInput;

        playerInput.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();

        //playerInput.DebugMonitor.performed += _ => uIManager.ToggleDebugMonitor();
        playerInput.EscapeMenu.performed += _ =>
        {
            //uIManager.ToggleEscapeMenu();
            toggleCursorLock(false);
        };

        if (fpMov != null)
        {
            playerInput.Jump.performed += _ => fpMov.OnJumpPressed();
            playerInput.Jump.canceled += _ => fpMov.OnJumpReleased();

            playerInput.Crouch.started += _ => fpMov.OnCrouchPressed();
            playerInput.Crouch.canceled += _ => fpMov.OnCrouchReleased();

            playerInput.ShiftWalk.started += _ => fpMov.OnShiftPressed();
            playerInput.ShiftWalk.canceled += _ => fpMov.OnShiftReleased();

            playerInput.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            playerInput.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        }

        if (playerShooting != null)
        {
            playerInput.Aim.started += _ => playerShooting.OnStartAiming();
            playerInput.Aim.canceled += _ => playerShooting.OnStopAiming();

            playerInput.Shoot.started += _ =>
            {
                Debug.Log("SHOOT");
                playerShooting.OnStartShooting();
                toggleCursorLock(true);
            };
            playerInput.Shoot.canceled += _ => playerShooting.OnStopShooting();
        }
        
    }

    private void Update()
    {
        if (!hasAuthority) return;

        fpMov.ReceiveMovementInput(movementInput);
        fpMov.ReceiveMouseInput(mouseInput/6);
        weaponSway.ReceiveInput(mouseInput/6 * weaponSwaySensitivityMultiplier);
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