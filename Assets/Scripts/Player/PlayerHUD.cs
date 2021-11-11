using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private GameObject playerHUDCanvas = null;

    [Header("Health")]
    [SerializeField] private PlayerHealth playerHealth = null;
    [SerializeField] private Image healthBarImage = null;
    [SerializeField] private TextMeshProUGUI healthText = null;

    [Header("Shooting & Ammo")]
    [SerializeField] private PlayerFiring playerFiring = null;

    [SerializeField] private WeaponManager weaponManager = null;
    [SerializeField] private TextMeshProUGUI ammoText = null;

    [Header("Kevlar")]
    [SerializeField] private PlayerEquipment playerEquipment = null;
    [SerializeField] private TextMeshProUGUI kevlarText = null;

    [Header("Money & Buy Menu")]
    [SerializeField] private TextMeshProUGUI moneyText = null;
    [SerializeField] private GameObject buyMenuPanel = null;

    [Header("Scoreboard")]
    [SerializeField] private GameObject scoreBoardPanel = null;

    [Header("Player Killed Popup")]
    [SerializeField] private GameObject playerKilledPopupPrefab = null;
    
    [SerializeField] private Player player;
    

#region BuyMenu
 
    public bool GetIsBuyMenuOpen() => buyMenuPanel.activeSelf;

    public void ToggleBuyMenu(bool active)
    {
        if (buyMenuPanel.activeSelf && !active
            || !buyMenuPanel.activeSelf && active) ToggleBuyMenu();
    }

    public void ToggleBuyMenu()
    {
        buyMenuPanel.SetActive(!buyMenuPanel.activeSelf);

        Cursor.lockState = buyMenuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = buyMenuPanel.activeSelf;
    }

#endregion

    private void Start()
    {
        if (player.GetNetworkPlayer() != null)
        {
            player.GetNetworkPlayer().ClientOnMoneyUpdated += HandleMoneyUpdated;
        }
        else Debug.LogWarning("NetworkPlayer is NULL");

        playerHealth.ClientOnHealthUpdated += HandleHealthUpdated;
        playerFiring.ClientOnAmmoUpdated += HandleAmmoUpdated;
        weaponManager.ClientOnAmmoUpdated += HandleAmmoUpdated;

        playerEquipment.ClientOnKevlarDurabilityUpdated += HandleKevlarDurabilityUpdated;
    }

    private void OnDestroy()
    {
        playerHealth.ClientOnHealthUpdated -= HandleHealthUpdated;
        playerFiring.ClientOnAmmoUpdated -= HandleAmmoUpdated;
        weaponManager.ClientOnAmmoUpdated -= HandleAmmoUpdated;

        if (player != null && player.GetNetworkPlayer() != null)
            player.GetNetworkPlayer().ClientOnMoneyUpdated -= HandleMoneyUpdated;

        playerEquipment.ClientOnKevlarDurabilityUpdated -= HandleKevlarDurabilityUpdated;
    }

    public void BuyItem(string itemToBuy)
    {
        switch (itemToBuy)
        {
            case "LIGHTKEVLAR":
                playerEquipment.TryBuyLightKevlar();
                break;
            case "HEAVYKEVLAR":
                playerEquipment.TryBuyHeavyKevlar();
                break;
            default:
                Debug.LogWarning($"Incorrect item to purchase: {itemToBuy}");
                break;
        }
    }

    public void OnPlayerKilledPopup()
    {
        GameObject popup = Instantiate(playerKilledPopupPrefab, playerHUDCanvas.transform, false);
        Destroy(popup, 1f);
    }

    public void HandleAmmoUpdated(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }

    public void UpdateKevlarText(int amount)
    {
        kevlarText.text = amount.ToString();
    }

    public void HandleHealthUpdated(int currentHealth, int maxHealth) {
        healthText.text = currentHealth.ToString();
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    private void HandleKevlarDurabilityUpdated(int durability)
    {
        UpdateKevlarText(durability);
    }

    private void HandleMoneyUpdated(int oldMoney, int newMoney)
    {
        moneyText.text = $"${newMoney}";
    }
}
