using System.Collections;
using Prime31;
using UnityEngine;

public class MessageOfTheDay : MonoBehaviour
{
	public string urlToCheck = "http://www.battlebears.com/storage/images/push/push_bbr.xml";

	private void Start()
	{
		StartCoroutine(checkForMessage());
	}

	private IEnumerator checkForMessage()
	{
		string oldMessage = PlayerPrefs.GetString("MessageFromSkyVu", string.Empty);
		WWW www = new WWW(urlToCheck);
		yield return www;
		if (www.error == null && www.text != string.Empty)
		{
			string newMessage = www.text;
			if (newMessage != oldMessage)
			{
				PlayerPrefs.SetString("MessageFromSkyVu", newMessage);
				EtceteraAndroid.showAlert(string.Empty, newMessage, "OK");
			}
		}
		Object.Destroy(this);
	}
}
