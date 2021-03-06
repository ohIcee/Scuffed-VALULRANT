using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Image healthBarImage = null;
    [SerializeField] private TextMeshProUGUI healthText = null;

    private Player player;
    private WeaponManager weaponManager;

    public void SetPlayer(Player _player)
    {
        player = _player;
        weaponManager = player.GetComponent<WeaponManager>();
    }

    public void UpdateHealth(int currentHealth, int maxHealth) {
        healthText.text = currentHealth.ToString();
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
