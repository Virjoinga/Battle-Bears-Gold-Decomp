using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using Prime31;
using UnityEngine;

public class ExtrasMenu : Popup
{
	private const string FAQ_URL = "https://battlebears.zendesk.com/forums/22083262-FAQ";

	private const string FORUM_URL = "https://discord.gg/bb";

	private const string PRIVACY_URL = "http://sky.vu/szprivacy";

	private const string TERMS_URL = "http://sky.vu/szterms";

	public GUIButton otherAppButton;

	public GameObject otherAppComingSoon;

	public AudioClip inSound;

	public AudioClip outSound;

	public AudioClip[] clickSounds;

	private string _otherAppiOSURL;

	private string _otherAppAndroidURL;

	private string _otherAppWin8URL;

	private string _merchUrl;

	protected override void Start()
	{
		base.Start();
		if (inSound != null)
		{
			AudioSource.PlayClipAtPoint(inSound, Vector3.zero);
		}
		string val = string.Empty;
		ServiceManager.Instance.UpdateProperty("other_app_available", ref val);
		if (otherAppButton != null && otherAppComingSoon != null)
		{
			otherAppButton.inactive = val.ToLower() != "true";
			otherAppComingSoon.SetActive(val.ToLower() != "true");
		}
		EventTracker.TrackEvent(new ExtrasMenuOpenedSchema());
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
			if (outSound != null)
			{
				AudioSource.PlayClipAtPoint(outSound, Vector3.zero);
			}
			OnClose();
			break;
		case "FAQ_btn":
			EtceteraAndroid.showWebView("https://battlebears.zendesk.com/forums/22083262-FAQ");
			EventTracker.TrackEvent(new FAQClickedSchema());
			break;
		case "forum_btn":
			Application.OpenURL("https://discord.gg/bb");
			EventTracker.TrackEvent(new SmallExtrasBannerClickedSchema(new SmallExtrasBannerURLParameter("https://discord.gg/bb")));
			break;
		case "terms_btn":
			EtceteraAndroid.showWebView("http://sky.vu/szterms");
			EventTracker.TrackEvent(new TermsClickedSchema());
			break;
		case "privacy_btn":
			EtceteraAndroid.showWebView("http://sky.vu/szprivacy");
			EventTracker.TrackEvent(new PrivacyClickedSchema());
			break;
		case "store_btn":
			ServiceManager.Instance.UpdateProperty("android_merch_url", ref _merchUrl);
			EtceteraAndroid.showWebView(_merchUrl);
			EventTracker.TrackEvent(new LargeExtrasBannerClickedSchema(new LargeExtrasBannerURLParameter(_merchUrl)));
			break;
		case "bugreport_btn":
		{
			string val = "https://battlebears.zendesk.com/hc/en-us/requests/new";
			ServiceManager.Instance.UpdateProperty("bugreport_url", ref val);
			EtceteraAndroid.showWebView(val);
			EventTracker.TrackEvent(new SupportClickedSchema());
			break;
		}
		case "MoreGames_btn":
			ServiceManager.Instance.UpdateProperty("droid_other_app_url", ref _otherAppAndroidURL);
			Application.OpenURL(_otherAppAndroidURL);
			EventTracker.TrackEvent(new ExtrasAdIconClickedSchema(new ExtrasAdIconURLParameter(_otherAppAndroidURL)));
			break;
		}
	}

	protected override void OnClose()
	{
		EventTracker.TrackEvent(new ExtrasMenuClosedSchema());
		base.OnClose();
	}
}
