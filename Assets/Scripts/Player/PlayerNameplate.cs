using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameplate : MonoBehaviour
{

	[SerializeField]
	private TextMeshProUGUI usernameText;

	[SerializeField]
	private Image healthBar;

	[SerializeField] private Health health = null;

	[SerializeField]
	private Player player;

	private void Awake()
	{
		health.ClientOnHealthUpdated += HandleHealthUpdated;
		usernameText.text = player.username;
	}

	private void OnDestroy()
	{
		health.ClientOnHealthUpdated -= HandleHealthUpdated;
	}

	private void HandleHealthUpdated(int currentHealth, int maxHealth)
	{
		healthBar.fillAmount = (float)currentHealth / maxHealth;
	}

}
