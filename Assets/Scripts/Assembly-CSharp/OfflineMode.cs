using System.Collections;
using UnityEngine;

public class OfflineMode : MonoBehaviour
{
	public GameObject loadingScreen;

	public AudioClip[] clickSounds;

	public AudioClip transitionSound;

	private void Start()
	{
		if (Application.internetReachability != 0)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		if (b.name == "offline_btn")
		{
			StartCoroutine(playOffline());
		}
	}

	private IEnumerator playOffline()
	{
		if (transitionSound != null)
		{
			AudioSource.PlayClipAtPoint(transitionSound, Vector3.zero);
		}
		base.GetComponent<Animation>().Play("out");
		yield return new WaitForSeconds(base.GetComponent<Animation>()["out"].length);
		loadingScreen.SetActive(true);
		yield return Application.LoadLevelAsync("Tutorial");
	}
}
