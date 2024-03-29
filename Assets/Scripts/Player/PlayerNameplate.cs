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

        if (player.GetNetworkPlayer() != null)
            usernameText.text = player.GetNetworkPlayer().GetDisplayName();
    }

    //private void UpdateUsername(string username)
    //{
    //    usernameText.text = username;
    //}

    private void OnDestroy()
    {
        playerHealth.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    public void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

}
