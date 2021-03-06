using UnityEngine;
using System.Collections;

public class KillFeed : MonoBehaviour
{

	[SerializeField]
	GameObject killfeedItemPrefab;

	// Use this for initialization
	void Start()
	{
		//GameManager.instance.onPlayerKilledCallback += OnKill;
	}

	public void OnKill(string player, string source)
	{
		GameObject go = (GameObject)Instantiate(killfeedItemPrefab, this.transform);
		go.GetComponent<KillFeedItem>().Setup(player, source);

		Destroy(go, 4f);
	}

}
