using UnityEngine;
using System.Collections;

public class KillFeed : MonoBehaviour
{

	[SerializeField] private GameObject killfeedItemPrefab;

	[SerializeField] private Transform killfeedItemParent;

	[SerializeField] private PlayerHealth playerHealth;

	// Use this for initialization
	private void Start()
	{
		playerHealth.ClientOnPlayerKilled += HandleOnPlayerKilled;
	}

	private void OnDestroy()
    {
		playerHealth.ClientOnPlayerKilled -= HandleOnPlayerKilled;
	}

	public void HandleOnPlayerKilled(string killedPlayer, string killerPlayer)
	{
		Debug.Log($"Handle on player killed:: {killedPlayer}/{killerPlayer}");

		GameObject go = Instantiate(killfeedItemPrefab, killfeedItemParent);
		go.GetComponent<KillFeedItem>().Setup(killedPlayer, killerPlayer);

		Destroy(go, 4f);
	}

}
