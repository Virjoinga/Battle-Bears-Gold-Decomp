using System.Collections;
using UnityEngine;

public class ReportMenuController : MonoBehaviour
{
	[SerializeField]
	private GUIButton[] reportButtons;

	[SerializeField]
	private GameObject errorPopup;

	private GUIButton _lastReportPopupButtonPressed;

	private int _lastPlayerToBeReported = -1;

	private bool _requestInProgress;

	public void ShowMenu(int playerToBeReported, GUIButton reportPopupButton)
	{
		_lastPlayerToBeReported = playerToBeReported;
		_lastReportPopupButtonPressed = reportPopupButton;
		base.gameObject.SetActive(true);
		base.animation.Play("in");
	}

	private void OnGUIButtonClicked(GUIButton button)
	{
		if (!_requestInProgress)
		{
			switch (button.name)
			{
			case "backBtn":
				DismissMenu();
				break;
			case "friendly":
				SubmitReport(ReportType.friendly);
				break;
			case "helpful":
				SubmitReport(ReportType.helpful);
				break;
			case "teamwork":
				SubmitReport(ReportType.teamwork);
				break;
			case "afk":
				SubmitReport(ReportType.afk);
				break;
			case "hacker":
				SubmitReport(ReportType.hacker);
				break;
			case "harassment":
				SubmitReport(ReportType.harassment);
				break;
			case "offensiveName":
				SubmitReport(ReportType.offensiveName);
				break;
			}
		}
	}

	private void SubmitReport(ReportType reportType)
	{
		_requestInProgress = true;
		GUIButton[] array = reportButtons;
		foreach (GUIButton gUIButton in array)
		{
			gUIButton.inactive = true;
		}
		ServiceManager.Instance.ReportPlayer(_lastPlayerToBeReported, (int)reportType, ReportSuccess, ReportFailure);
	}

	private void ReportSuccess()
	{
		Debug.Log("Reported that guy successfully!");
		DismissMenu();
		DisableLastGUIButton();
	}

	private void ReportFailure()
	{
		Debug.Log("Couldnt report that guy!");
		DismissMenu();
		OpenErrorPopup();
	}

	private void DisableLastGUIButton()
	{
		_lastReportPopupButtonPressed.gameObject.SetActive(false);
	}

	private void DismissMenu()
	{
		_requestInProgress = false;
		base.animation.Play("out");
		StartCoroutine(DelayedSetActive(false, base.animation["out"].length));
	}

	private IEnumerator DelayedSetActive(bool active, float delay)
	{
		yield return new WaitForSeconds(delay);
		base.gameObject.SetActive(false);
	}

	private void OpenErrorPopup()
	{
		errorPopup.SetActive(true);
		errorPopup.animation.Play("in");
		Transform transform = errorPopup.transform.FindChild("text");
		if (transform != null)
		{
			TextMesh component = transform.gameObject.GetComponent<TextMesh>();
			if (component != null)
			{
				component.text = ServiceManager.Instance.LastError;
			}
		}
	}
}
