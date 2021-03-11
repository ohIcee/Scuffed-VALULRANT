using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private GameObject playerHUDCanvas = null;

    [Header("Health")]
    [SerializeField] private PlayerHealth playerHealth = null;
    [SerializeField] private Image healthBarImage = null;
    [SerializeField] private TextMeshProUGUI healthText = null;
    
    [Header("Escape Menu")]
    [SerializeField] private GameObject escapeMenuPanel = null;
    [SerializeField] private GameObject settingsPanel = null;
    [SerializeField] private Scrollbar mouseSensitivityScrollBar = null;
    [SerializeField] private TMP_InputField mouseSensitivityInput = null;
    [SerializeField] private TMP_Dropdown graphicsSettingDropdown = null;
    [SerializeField] private RenderPipelineAsset[] qualityLevels;

    [Header("Shooting & Ammo")]
    [SerializeField] private PlayerFiring playerFiring = null;
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

#region EscapeMenu

    public bool GetIsEscapeMenuOpen() => escapeMenuPanel.activeSelf;
    public bool GetIsBuyMenuOpen() => buyMenuPanel.activeSelf;
    public bool GetIsSettingsMenuOpen() => settingsPanel.activeSelf;

    public void ToggleSettingsMenu(bool active)
    {
        if (settingsPanel.activeSelf && !active
            || !settingsPanel.activeSelf && active) ToggleSettingsMenu();
    }

    public void ToggleSettingsMenu()
    {
        bool newState = !settingsPanel.activeSelf;
        settingsPanel.SetActive(newState);

        if (newState) PlayerPrefs.Save();
    }

    public void OnMouseSensitivityChangedInput()
    {
        if (mouseSensitivityInput.text.EndsWith(".")) return;

        if (float.TryParse(mouseSensitivityInput.text, out float sens))
        {
            OnMouseSensitivityChanged(sens);
            return;
        }

        Debug.LogWarning($"Incorrect mouse sensitivity input: {mouseSensitivityInput.text}");
    }

    public void OnMouseSensitivityChangedScroll() => OnMouseSensitivityChanged(mouseSensitivityScrollBar.value);
    
    private void SetInitialMouseSensitivityValue() 
    {
        float sensitivity = PlayerPrefs.GetFloat("MOUSE_SENS");

        mouseSensitivityInput.text = sensitivity.ToString();
        mouseSensitivityScrollBar.value = sensitivity;
    }

    public void SetInitialGraphicsQualityValue()
    {
        graphicsSettingDropdown.value = PlayerPrefs.GetInt("QUALITY_LEVEL");
    }

    private void OnMouseSensitivityChanged(float newSens)
    {
        newSens = Mathf.Clamp(newSens, 0, 1);

        mouseSensitivityInput.text = newSens.ToString();
        mouseSensitivityScrollBar.value = newSens;

        player.ChangeSensitivity( newSens );

        Debug.Log($"CHANGED SENSITIVITY TO {newSens}");

        PlayerPrefs.SetFloat("MOUSE_SENS", newSens);
    }

    public void ChangeGraphicsQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];

        Debug.Log($"saved quality level {value}");

        PlayerPrefs.SetInt("QUALITY_LEVEL", value);
    }

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

    // NO CHECKMARK IF DELETED - LEAVE IT IN
    private void Start()
    {
        player.GetNetworkPlayer().ClientOnMoneyUpdated += HandleMoneyUpdated;
    }

    private void Awake()
    {
        playerHealth.ClientOnHealthUpdated += HandleHealthUpdated;
        playerFiring.ClientOnAmmoUpdated += HandleAmmoUpdated;

        playerEquipment.ClientOnKevlarDurabilityUpdated += HandleKevlarDurabilityUpdated;

        SetInitialMouseSensitivityValue();
        SetInitialGraphicsQualityValue();
    }

    private void OnDestroy()
    {
        playerHealth.ClientOnHealthUpdated -= HandleHealthUpdated;
        playerFiring.ClientOnAmmoUpdated -= HandleAmmoUpdated;

        if (player != null && player.GetNetworkPlayer() != null)
            player.GetNetworkPlayer().ClientOnMoneyUpdated -= HandleMoneyUpdated;

        playerEquipment.ClientOnKevlarDurabilityUpdated -= HandleKevlarDurabilityUpdated;
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
