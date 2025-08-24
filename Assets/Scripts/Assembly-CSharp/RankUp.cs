using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class RankUp : Popup
{
	public Transform rankMount;

	public TextMesh facebookRewardText;

	public GUIButton facebookButton;

	public GameObject facebookStatusOverlay;

	public TextMesh facebookStatus;

	private MainMenu mainMenu;

	private string _contactingFacebook;

	private string _facebookPostTitle;

	private string _facebookPostCaption;

	private string _facebookRankFormatString;

	private string _facebookLoginFailed;

	private string _facebookPostSuccess;

	private string _facebookPostFailed;

	protected override void OnEnable()
	{
		base.OnEnable();
		FacebookManager.sessionOpenedEvent += facebookLogin;
		FacebookManager.loginFailedEvent += facebookLoginFailed;
		FacebookManager.dialogCompletedWithUrlEvent += facebookPost;
		FacebookManager.dialogFailedEvent += facebookPostFailed;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		FacebookManager.sessionOpenedEvent -= facebookLogin;
		FacebookManager.loginFailedEvent -= facebookLoginFailed;
		FacebookManager.dialogCompletedWithUrlEvent -= facebookPost;
		FacebookManager.dialogFailedEvent -= facebookPostFailed;
	}

	protected void UpdateLocalizedText()
	{
		_contactingFacebook = Language.Get("FACEBOOK_CONTACTING_STATUS");
		_facebookPostTitle = Language.Get("FACEBOOK_POST_TITLE");
		_facebookPostCaption = Language.Get("FACEBOOK_POST_ICON_CAPTION");
		_facebookRankFormatString = Language.Get("FACEBOOK_POST_RANK_FORMAT_STRING");
		_facebookLoginFailed = Language.Get("FACEBOOK_LOGIN_FAILED");
		_facebookPostSuccess = Language.Get("FACEBOOK_POST_SUCCESSFULL");
		_facebookPostFailed = Language.Get("FACEBOOK_POST_FAILED");
	}

	protected override void Start()
	{
		base.Start();
		UpdateLocalizedText();
		mainMenu = UnityEngine.Object.FindObjectOfType(typeof(MainMenu)) as MainMenu;
		facebookStatusOverlay.SetActiveRecursively(false);
		Reward reward = ServiceManager.Instance.GetReward("rank_fb_reward");
		if (reward != null && facebookRewardText != null)
		{
			facebookRewardText.text = "+" + string.Format("{0:#,0}", reward.joules);
		}
		double skill = ServiceManager.Instance.GetStats().skill;
		Rank rank = ServiceManager.GetRank(skill);
		UnityEngine.Object @object = Resources.Load("Icons/Rank/" + Enum.GetName(typeof(Rank), (int)rank) + "_big");
		if (@object != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
			gameObject.transform.parent = rankMount;
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.layer = LayerMask.NameToLayer("HUD");
		}
		switch (rank)
		{
		case Rank.bronze:
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BRONZE"]);
			break;
		case Rank.silver:
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BRONZE"]);
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["SILVER"]);
			break;
		case Rank.gold:
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BRONZE"]);
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["SILVER"]);
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["GOLD"]);
			break;
		case Rank.diamond:
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BRONZE"]);
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["SILVER"]);
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["GOLD"]);
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["DIAMOND"]);
			break;
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		switch (b.name)
		{
		case "backBtn":
			OnClose();
			break;
		case "shareBtn":
			facebookButton.disable();
			StartCoroutine(delayedStatusMessage(_contactingFacebook));
			if (!FacebookAndroid.isSessionValid())
			{
				FacebookAndroid.login();
			}
			else
			{
				postToFacebook();
			}
			break;
		}
	}

	private void facebookLogin()
	{
		postToFacebook();
	}

	private void MetroFacebookLogin(string error)
	{
		if (error == null)
		{
			postToFacebook();
		}
	}

	private void postToFacebook()
	{
		double skill = ServiceManager.Instance.GetStats().skill;
		Rank rank = ServiceManager.GetRank(skill);
		string value = string.Format(_facebookRankFormatString, Enum.GetName(typeof(Rank), (int)rank).ToUpper());
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("message", value);
		dictionary.Add("link", "https://play.google.com/store/apps/details?id=net.skyvu.battlebearsgold&hl=en");
		dictionary.Add("name", _facebookPostTitle);
		dictionary.Add("picture", "https://battlebears.com/wp-content/uploads/2013/04/BBG_Icon.png");
		dictionary.Add("caption", _facebookPostCaption);
		Dictionary<string, string> parameters = dictionary;
		FacebookAndroid.showDialog("stream.publish", parameters);
	}

	private void facebookLoginFailed(P31Error error)
	{
		Debug.LogError("Facebook login failed from rank up: " + error.message);
		StartCoroutine(delayedStatusMessage(_facebookLoginFailed));
	}

	private void facebookPost()
	{
		Debug.Log("Successfully posted to Facebook from rank up");
		StartCoroutine(delayedStatusMessage(_facebookPostSuccess));
		ServiceManager.Instance.RequestReward("rank_fb_reward", mainMenu.OnGetRewardSuccess, mainMenu.OnGetRewardFail);
	}

	private void facebookPost(string url)
	{
		facebookPost();
	}

	private void facebookPost(string result, object obj)
	{
		Debug.Log("Successfully posted to Facebook from levelup game with string: " + result + " and object " + obj);
		facebookPost();
	}

	private void facebookPostFailed(P31Error error)
	{
		Debug.Log("Facebook post failed from rank up: " + error.message);
		StartCoroutine(delayedStatusMessage(_facebookPostFailed));
	}

	private IEnumerator delayedStatusMessage(string status)
	{
		facebookStatusOverlay.SetActiveRecursively(true);
		facebookStatus.text = status;
		yield return new WaitForSeconds(1.5f);
		facebookStatusOverlay.SetActiveRecursively(false);
	}
}
