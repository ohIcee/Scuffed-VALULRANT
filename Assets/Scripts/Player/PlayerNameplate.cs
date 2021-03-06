using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameplate : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI usernameText;

    [SerializeField]
    private PlayerHealth playerHealth;

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private Player player;

    private void Start()
    {
        playerHealth.ClientOnHealthUpdated += HandleHealthUpdated;

        //usernameText.text = player.username;
        usernameText.text = transform.name;
    }

    private void OnDestroy()
    {
        playerHealth.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    public void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

}
