using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    [SerializeField] private float weaponSwaySensitivityMultiplier = .5f;

    [Header("Scripts")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerFiring playerFiring;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] WeaponSway weaponSway;
    [SerializeField] private WeaponManager weaponManager;

    private PlayerHUD playerHUD;
    private NetworkPlayerHUD networkHUD;

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
        playerHUD = GetComponent<PlayerHUD>();
        
        if (player.GetNetworkPlayer() != null)
            networkHUD = player.GetNetworkPlayer().GetComponent<NetworkPlayerHUD>();

        controls = new PlayerControls();
        controls.Enable();
        playerInput = controls.PlayerInput;

        playerInput.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();

        playerInput.EscapeMenu.performed += _ =>
        {
            if (playerHUD.GetIsBuyMenuOpen())
            {
                playerHUD.ToggleBuyMenu(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
            }

            if (networkHUD != null)
            {
                networkHUD.ToggleSettingsMenu(false);
                networkHUD.ToggleEscapeMenu();
            }
        };

        playerInput.BuyMenu.performed += _ => {
            if (networkHUD != null && networkHUD.GetIsEscapeMenuOpen()) return;

            // is game in buy stage

            playerHUD.ToggleBuyMenu();
        };

        if (player != null)
        {
            playerInput.Jump.performed += _ => {
                if (networkHUD != null && networkHUD.GetIsEscapeMenuOpen() || playerHUD.GetIsBuyMenuOpen()) return;

                player.OnJumpPressed();
            };
            playerInput.Jump.canceled += _ => player.OnJumpReleased();

            playerInput.Crouch.started += _ => {
                if (networkHUD != null && networkHUD.GetIsEscapeMenuOpen() || playerHUD.GetIsBuyMenuOpen()) return;

                player.OnCrouchPressed();
            };
            playerInput.Crouch.canceled += _ => player.OnCrouchReleased();

            playerInput.ShiftWalk.started += _ => {
                if (networkHUD != null && networkHUD.GetIsEscapeMenuOpen() || playerHUD.GetIsBuyMenuOpen()) return;

                player.OnShiftPressed();
            };
            playerInput.ShiftWalk.canceled += _ => player.OnShiftReleased();

            playerInput.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            playerInput.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        }

        if (playerFiring != null)
        {
            playerInput.Fire.started += _ =>
            {
                if (networkHUD != null && networkHUD.GetIsEscapeMenuOpen() || playerHUD.GetIsBuyMenuOpen()) return;

                playerFiring.OnStartFiring();
                toggleCursorLock(true);
            };
            playerInput.Fire.canceled += _ => playerFiring.OnStopFiring();

            playerInput.Reload.performed += _ =>
            {
                if ((networkHUD != null && networkHUD.GetIsEscapeMenuOpen()) || playerHUD.GetIsBuyMenuOpen()) return;

                playerFiring.OnPressReload();
            };
        }

        if (weaponManager != null)
        {
            playerInput.EquipPrimary.performed += _ =>
            {
                weaponManager.EquipWeapon(WeaponSlot.Primary);
            };

            playerInput.EquipSecondary.performed += _ =>
            {
                weaponManager.EquipWeapon(WeaponSlot.Secondary);
            };
        }

        playerInput.Suicide.performed += _ => {
            if ((networkHUD != null && networkHUD.GetIsEscapeMenuOpen()) || playerHUD.GetIsBuyMenuOpen()) return;

            player.CmdDealDamage(9999);
        };
        
    }

    private void Update()
    {
        if (!hasAuthority) return;

        // disable input if escape menu is open
        if (networkHUD != null && networkHUD.GetIsEscapeMenuOpen() || playerHUD.GetIsBuyMenuOpen())
        {
            OnBlockingUIOpen();
            return;
        }

        player.ReceiveMovementInput(movementInput);
        player.ReceiveMouseInput(mouseInput);
        weaponSway.ReceiveInput(mouseInput * weaponSwaySensitivityMultiplier);
    }

    private void OnBlockingUIOpen()
    {
        player.ReceiveMovementInput(Vector2.zero);
        player.ReceiveMouseInput(Vector2.zero);
        weaponSway.ReceiveInput(Vector2.zero);
        playerFiring.OnStopFiring();
    }

    private void OnDestroy()
    {
        if (!hasAuthority) return;

        controls.Disable();
    }

    private void toggleCursorLock(bool locked)
    {
        if ((networkHUD != null && networkHUD.GetIsEscapeMenuOpen()) || playerHUD.GetIsBuyMenuOpen()) return;

        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}