using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private PlayerHealth playerHealth = null;
    [SerializeField] private Image healthBarImage = null;
    [SerializeField] private TextMeshProUGUI healthText = null;
    
    [Header("Escape Menu")]
    [SerializeField] private GameObject escapeMenuPanel = null;

    [Header("Shooting & Ammo")]
    [SerializeField] private PlayerFiring playerFiring = null;
    [SerializeField] private TextMeshProUGUI ammoText = null;

    [Header("Money & Buy Menu")]
    [SerializeField] private PlayerEquipment playerEquipment = null;
    [SerializeField] private TextMeshProUGUI moneyText = null;
    [SerializeField] private GameObject buyMenuPanel = null;

    [Header("Scoreboard")]
    [SerializeField] private GameObject scoreBoardPanel = null;

    public bool GetIsEscapeMenuOpen() => escapeMenuPanel.activeSelf;
    public bool GetIsBuyMenuOpen() => buyMenuPanel.activeSelf;

    private Player player;
    private WeaponManager weaponManager;

    // NO CHECKMARK IF DELETED - LEAVE IT IN
    private void Start()
    {

    }

    private void Awake()
    {
        playerHealth.ClientOnHealthUpdated += HandleHealthUpdated;
        playerFiring.ClientOnAmmoUpdated += HandleAmmoUpdated;

        playerEquipment.ClientOnHelmetDurabilityUpdated += HandleHelmetDurabilityUpdated;
        playerEquipment.ClientOnKevlarDurabilityUpdated += HandleKevlarDurabilityUpdated;

    }

    private void OnDestroy()
    {
        playerHealth.ClientOnHealthUpdated -= HandleHealthUpdated;
        playerFiring.ClientOnAmmoUpdated -= HandleAmmoUpdated;
    }

    public void ToggleEscapeMenu()
    {
        escapeMenuPanel.SetActive(!escapeMenuPanel.activeSelf);

        Cursor.lockState = escapeMenuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = escapeMenuPanel.activeSelf;
    }

    public void OnDisconnectButton()
    {
        Player player = GetComponent<Player>();
        ValulrantNetworkPlayer networkPlayer = player.GetNetworkPlayer();
        networkPlayer.DisconnectFromServer();
    }

    public void ToggleBuyMenu()
    {
        buyMenuPanel.SetActive(!buyMenuPanel.activeSelf);

        Cursor.lockState = buyMenuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = buyMenuPanel.activeSelf;
    }

    public void BuyItem(string itemToBuy)
    {
        switch (itemToBuy)
        {
            case "HELMET":
                playerEquipment.TryBuyHelmet();
                break;
            case "KEVLAR":
                playerEquipment.TryBuyKevlar();
                break;
            case "HELMETKEVLAR":
                playerEquipment.TryBuyKevlarAndHelmet();
                break;
            default:
                Debug.LogWarning($"Incorrect item to purchase: {itemToBuy}");
                break;
        }
    }

    public void HandleAmmoUpdated(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = $"${money}";
    }

    public void HandleHealthUpdated(int currentHealth, int maxHealth) {
        healthText.text = currentHealth.ToString();
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    private void HandleHelmetDurabilityUpdated(int durability)
    { 
    
    }

    private void HandleKevlarDurabilityUpdated(int durability)
    { 
        
    }
}
