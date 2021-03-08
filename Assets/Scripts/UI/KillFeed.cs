using UnityEngine;
using System.Collections;

public class KillFeed : MonoBehaviour
{

	[SerializeField] private GameObject killfeedItemPrefab;

	[SerializeField] private Transform killfeedItemParent;

    #region Client

	public void HandleOnPlayerKilled(string killedPlayer, string killerPlayer)
	{
		GameObject go = (GameObject)Instantiate(killfeedItemPrefab, killfeedItemParent);
		go.GetComponent<KillFeedItem>().Setup(killedPlayer, killerPlayer);

		Destroy(go, 4f);
	}

    #endregion

}
