using System.Collections;
using UnityEngine;

public class ReportDisplay : MonoBehaviour
{
	public GameObject fetchingSpinner;

	public TextMesh fetchingStatsText;

	private void Start()
	{
		StartCoroutine("WaitForStats");
	}

	private IEnumerator WaitForStats()
	{
		yield return new WaitForSeconds(0.1f);
		BroadcastMessage("OnGameReport", ServiceManager.Instance.LastGameReport, SendMessageOptions.DontRequireReceiver);
		while (ServiceManager.Instance.WaitingForResults)
		{
			yield return new WaitForSeconds(0.25f);
		}
		fetchingSpinner.gameObject.SetActive(false);
		fetchingStatsText.text = "PLAYER STATS";
		BroadcastMessage("OnGameResults", ServiceManager.Instance.LastGameResults, SendMessageOptions.DontRequireReceiver);
	}

	private void OnGUIButtonPressed(GUIButton b)
	{
	}
}
