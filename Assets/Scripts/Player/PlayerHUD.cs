using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth = null;
    [SerializeField] private Image healthBarImage = null;
    [SerializeField] private TextMeshProUGUI healthText = null;
    
    [SerializeField] private GameObject escapeMenuPanel = null;

    [SerializeField] private PlayerShooting playerShooting = null;
    [SerializeField] private TextMeshProUGUI ammoText = null;

    private bool isEscapeMenuOpen = false;
    public bool GetIsEscapeMenuOpen() => isEscapeMenuOpen;

    private Player player;
    private WeaponManager weaponManager;

    // NO CHECKMARK IF DELETED - LEAVE IT IN
    private void Start()
    {

    }

    private void Awake()
    {
        playerHealth.ClientOnHealthUpdated += HandleHealthUpdated;
        playerShooting.ClientOnAmmoUpdated += HandleAmmoUpdated;
    }

    private void OnDestroy()
    {
        playerHealth.ClientOnHealthUpdated -= HandleHealthUpdated;
        playerShooting.ClientOnAmmoUpdated -= HandleAmmoUpdated;
    }

    public void ToggleEscapeMenu()
    {
        escapeMenuPanel.SetActive(!escapeMenuPanel.activeSelf);

        Cursor.lockState = escapeMenuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = escapeMenuPanel.activeSelf;
        isEscapeMenuOpen = escapeMenuPanel.activeSelf;
    }

    public void OnDisconnectButton()
    {
        Player player = GetComponent<Player>();
        ValulrantNetworkPlayer networkPlayer = player.GetNetworkPlayer();
        Debug.Log($"networkplayer: {networkPlayer == null}");
        networkPlayer.DisconnectFromServer();
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
        weaponManager = player.GetComponent<WeaponManager>();
    }

    public void HandleAmmoUpdated(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    public void HandleHealthUpdated(int currentHealth, int maxHealth) {
        healthText.text = currentHealth.ToString();
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
