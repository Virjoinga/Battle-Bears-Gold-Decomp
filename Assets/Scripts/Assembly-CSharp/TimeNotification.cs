using System.Collections;
using UnityEngine;

public class TimeNotification : MonoBehaviour
{
	public TextMesh textTime;

	private void Start()
	{
		StartCoroutine(updateTime());
	}

	private IEnumerator updateTime()
	{
		while (true)
		{
			if (GameManager.Instance != null)
			{
				int timeLeft = GameManager.Instance.TimeLeft;
				int minutes = timeLeft / 60;
				int seconds = timeLeft % 60;
				if (minutes >= 0 && seconds >= 0 && timeLeft > 0)
				{
					if (seconds < 10)
					{
						textTime.text = minutes + ":0" + seconds;
					}
					else
					{
						textTime.text = minutes + ":" + seconds;
					}
				}
				else
				{
					textTime.text = "0:00";
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
}
