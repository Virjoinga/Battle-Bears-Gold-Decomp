using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using UnityEngine;

public class OutOfEnergy : Popup
{
	private bool tryingToRefill;

	public AudioClip[] clickSounds;

	protected override void OnClose()
	{
		EventTracker.TrackEvent(new OutOfEnergyPopupClosedSchema());
		base.OnClose();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		EventTracker.TrackEvent(new OutOfEnergyPopupOpenedSchema());
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (!tryingToRefill)
		{
			if (clickSounds.Length > 0)
			{
				AudioSource.PlayClipAtPoint(clickSounds[Random.Range(0, clickSounds.Length)], Vector3.zero);
			}
			switch (b.name)
			{
			case "backBtn":
				OnClose();
				break;
			case "refillBtn":
				tryingToRefill = true;
				EventTracker.TrackEvent(new EnergyRefillAttemptedSchema(new VirtualCurrencyAmountParameter(ServiceManager.Instance.GetStats().gas)));
				ServiceManager.Instance.RefillEnergy(OnRefillSuccess, OnRefillFailure);
				break;
			case "playWithoutEnergy":
				OnClose();
				callingObject.SendMessage("OnContinueWithoutEnergy", SendMessageOptions.DontRequireReceiver);
				break;
			}
		}
	}

	private void OnRefillSuccess()
	{
		EventTracker.TrackEvent(new EnergyRefilledSchema());
		OnClose();
		CumulativeStats.Instance.numRefills++;
		if (CumulativeStats.Instance.numRefills == 1)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["THE_LEAKY_FAUCET"]);
		}
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["ENDLESS_REFILLS"], (float)CumulativeStats.Instance.numRefills / 25f * 100f);
		if (callingObject != null)
		{
			callingObject.SendMessage("OnRefillSuccess", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnRefillFailure()
	{
		OnClose();
		if (callingObject != null)
		{
			MainMenu component = callingObject.GetComponent<MainMenu>();
			if (component != null)
			{
				component.OnCreateBuyGasPopup();
			}
		}
	}
}
