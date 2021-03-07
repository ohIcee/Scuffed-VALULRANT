using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    [SerializeField] private float weaponSwaySensitivityMultiplier = .5f;

    [Header("Scripts")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] WeaponSway weaponSway;

    private PlayerHUD playerHUD;

    private PlayerControls controls;
    private PlayerControls.PlayerInputActions playerInput;

    private Vector2 movementInput;
    private Vector2 mouseInput;

    private float mouseSensitivity = 1f;

    public override void OnStartAuthority()
    {
        Initialize();
    }

    public void Initialize()
    {
        playerHUD = GetComponent<PlayerHUD>();
        mouseSensitivity = PlayerPrefs.GetFloat("MOUSE_SENS");

        controls = new PlayerControls();
        controls.Enable();
        playerInput = controls.PlayerInput;

        playerInput.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();

        playerInput.EscapeMenu.performed += _ =>
        {
            playerHUD.ToggleEscapeMenu();
        };

        if (player != null)
        {
            playerInput.Jump.performed += _ => {
                if (playerHUD.GetIsEscapeMenuOpen()) return;

                player.OnJumpPressed();
            };
            playerInput.Jump.canceled += _ => player.OnJumpReleased();

            playerInput.Crouch.started += _ => {
                if (playerHUD.GetIsEscapeMenuOpen()) return;

                player.OnCrouchPressed();
            };
            playerInput.Crouch.canceled += _ => player.OnCrouchReleased();

            playerInput.ShiftWalk.started += _ => {
                if (playerHUD.GetIsEscapeMenuOpen()) return;

                player.OnShiftPressed();
            };
            playerInput.ShiftWalk.canceled += _ => player.OnShiftReleased();

            playerInput.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            playerInput.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        }

        if (playerShooting != null)
        {
            playerInput.Aim.started += _ => playerShooting.OnStartAiming();
            playerInput.Aim.canceled += _ => playerShooting.OnStopAiming();

            playerInput.Shoot.started += _ =>
            {
                if (playerHUD.GetIsEscapeMenuOpen()) return;

                playerShooting.OnStartShooting();
                toggleCursorLock(true);
            };
            playerInput.Shoot.canceled += _ => playerShooting.OnStopShooting();

            playerInput.Reload.performed += _ =>
            {
                if (playerHUD.GetIsEscapeMenuOpen()) return;

                playerShooting.OnPressReload();
            };
        }

        playerInput.Suicide.performed += _ => {
            if (playerHUD.GetIsEscapeMenuOpen()) return;

            player.CmdDealDamage(9999);
        };
        
    }

    private void Update()
    {
        if (!hasAuthority) return;

        // disable input if escape menu is open
        if (playerHUD.GetIsEscapeMenuOpen())
        {
            OnEscapeMenu();
            return;
        }

        player.ReceiveMovementInput(movementInput);
        player.ReceiveMouseInput(mouseInput);
        weaponSway.ReceiveInput(mouseInput * mouseSensitivity * weaponSwaySensitivityMultiplier);
    }

    private void OnEscapeMenu()
    {
        player.ReceiveMovementInput(Vector2.zero);
        player.ReceiveMouseInput(Vector2.zero);
        weaponSway.ReceiveInput(Vector2.zero);
        playerShooting.OnStopShooting();
        playerShooting.OnStopAiming();
    }

    private void OnDestroy()
    {
        if (!hasAuthority) return;

        controls.Disable();
    }

    private void toggleCursorLock(bool locked)
    {
        if (playerHUD.GetIsEscapeMenuOpen()) return;

        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}