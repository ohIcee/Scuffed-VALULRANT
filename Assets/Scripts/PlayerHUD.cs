using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private Image healthBarImage = null;
    [SerializeField] private TextMeshProUGUI healthText = null;

    private Player player;
    private WeaponManager weaponManager;

    public void SetPlayer(Player _player)
    {
        player = _player;
        weaponManager = player.GetComponent<WeaponManager>();

        health = player.GetComponent<Health>();

        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth) {
        healthText.text = currentHealth.ToString();
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
