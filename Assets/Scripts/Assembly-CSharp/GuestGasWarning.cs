using Analytics;
using Analytics.Schemas;
using UnityEngine;

public class GuestGasWarning : Popup
{
	public AudioClip[] clickSounds;

	public string returnStatus = string.Empty;

	protected override void Start()
	{
		base.Start();
		EventTracker.TrackEvent(new GuestPurchaseConfirmationOpenedSchema());
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		switch (b.name)
		{
		case "backBtn":
			EventTracker.TrackEvent(new GuestPurchaseConfirmationClosedSchema());
			returnStatus = "cancel";
			OnClose();
			break;
		case "continueBtn":
			returnStatus = "continue";
			OnClose();
			break;
		case "upgradeAccountBtn":
			returnStatus = "upgrade";
			OnClose();
			break;
		}
	}
}
