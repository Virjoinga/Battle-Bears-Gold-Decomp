using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class LevelUp : Popup
{
	public TextMesh levelText;

	public Transform[] unlockItems;

	public TextMesh facebookRewardText;

	public GUIButton facebookButton;

	public GameObject facebookStatusOverlay;

	public TextMesh facebookStatus;

	private MainMenu mainMenu;

	private string _contactingFacebook;

	private string _facebookPostTitle;

	private string _facebookPostCaption;

	private string _facebookLevelFormatString;

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
		_facebookLevelFormatString = Language.Get("FACEBOOK_POST_LEVEL_FORMAT_STRING");
		_facebookLoginFailed = Language.Get("FACEBOOK_LOGIN_FAILED");
		_facebookPostSuccess = Language.Get("FACEBOOK_POST_SUCCESSFULL");
		_facebookPostFailed = Language.Get("FACEBOOK_POST_FAILED");
	}

	protected override void Start()
	{
		base.Start();
		UpdateLocalizedText();
		mainMenu = Object.FindObjectOfType(typeof(MainMenu)) as MainMenu;
		facebookStatusOverlay.SetActive(false);
		Reward reward = ServiceManager.Instance.GetReward("level_fb_reward");
		if (reward != null && facebookRewardText != null)
		{
			facebookRewardText.text = "+" + string.Format("{0:#,0}", reward.joules);
		}
		int num = (int)ServiceManager.Instance.GetStats().level;
		levelText.text = num.ToString();
		if (num >= 25)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL25"]);
		}
		if (num >= 10)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL10"]);
		}
		if (num >= 9)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL9"]);
		}
		if (num >= 8)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL8"]);
		}
		if (num >= 7)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL7"]);
		}
		if (num >= 6)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL6"]);
		}
		if (num >= 5)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL5"]);
		}
		if (num >= 4)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL4"]);
		}
		if (num >= 3)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL3"]);
		}
		if (num >= 2)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEVEL2"]);
		}
		int num2 = 0;
		for (int i = 0; i < unlockItems.Length; i++)
		{
			unlockItems[i].gameObject.SetActive(false);
		}
		foreach (KeyValuePair<string, Character> character in Store.Instance.characters)
		{
			if (character.Value.characterData.level == num)
			{
				Object @object = null;
				@object = Resources.Load("Icons/Characters/" + character.Value.characterData.name + "/" + character.Value.skins[0].name + "_red");
				mountIcon(character.Value.characterData.title, string.Empty, @object, num2++, 0f);
				if (num2 >= unlockItems.Length)
				{
					return;
				}
			}
			foreach (Item skin in character.Value.skins)
			{
				if (skin.level == num)
				{
					Object icon = Resources.Load("Icons/Characters/" + character.Value.characterData.name + "/" + skin.name + "_red");
					mountIcon(character.Value.characterData.title, skin.title, icon, num2++, 0f);
					if (num2 >= unlockItems.Length)
					{
						return;
					}
				}
			}
			foreach (Item primaryWeapon in character.Value.primaryWeapons)
			{
				if (primaryWeapon.level == num)
				{
					Object icon2 = Resources.Load("Icons/Weapons/" + character.Value.characterData.name + "/" + primaryWeapon.name);
					mountIcon(character.Value.characterData.title, primaryWeapon.title, icon2, num2++, 0f);
					if (num2 >= unlockItems.Length)
					{
						return;
					}
				}
			}
			foreach (Item secondaryWeapon in character.Value.secondaryWeapons)
			{
				if (secondaryWeapon.level == num)
				{
					Object icon3 = Resources.Load("Icons/Weapons/" + character.Value.characterData.name + "/" + secondaryWeapon.name);
					mountIcon(character.Value.characterData.title, secondaryWeapon.title, icon3, num2++, 0f);
					if (num2 >= unlockItems.Length)
					{
						return;
					}
				}
			}
			foreach (Item meleeWeapon in character.Value.meleeWeapons)
			{
				if (meleeWeapon.level == num)
				{
					Object icon4 = Resources.Load("Icons/Weapons/" + character.Value.characterData.name + "/" + meleeWeapon.name);
					mountIcon(character.Value.characterData.title, meleeWeapon.title, icon4, num2++, 0f);
					if (num2 >= unlockItems.Length)
					{
						return;
					}
				}
			}
		}
		foreach (Item special in Store.Instance.specials)
		{
			if (special.level == num)
			{
				Object icon5 = Resources.Load("Icons/Specials/" + special.name);
				mountIcon(special.title, string.Empty, icon5, num2++, 0f);
				if (num2 >= unlockItems.Length)
				{
					return;
				}
			}
		}
		foreach (Item item in Store.Instance.equipment)
		{
			if (item.level == num)
			{
				Object icon6 = Resources.Load("Icons/Equipment/" + item.name);
				mountIcon(item.title, string.Empty, icon6, num2++, 0f);
				if (num2 >= unlockItems.Length)
				{
					break;
				}
			}
		}
	}

	private void mountIcon(string itemClass, string title, Object icon, int index, float offset = 0f)
	{
		if (icon != null)
		{
			GameObject gameObject = Object.Instantiate(icon) as GameObject;
			unlockItems[index].gameObject.SetActive(true);
			gameObject.transform.parent = unlockItems[index].Find("iconMount");
			gameObject.transform.localPosition = new Vector3(offset, 0f, 0f);
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.layer = LayerMask.NameToLayer("HUD");
		}
		unlockItems[index].Find("itemClass").GetComponent<TextMesh>().text = itemClass;
		unlockItems[index].Find("itemTitle").GetComponent<TextMesh>().text = title;
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
		string value = string.Format(_facebookLevelFormatString, (int)ServiceManager.Instance.GetStats().level);
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
		Debug.LogError("Facebook login failed from level up: " + error.message);
		StartCoroutine(delayedStatusMessage(_facebookLoginFailed));
	}

	private void facebookPost(string result, object obj)
	{
		facebookPost();
	}

	private void facebookPost(string url)
	{
		facebookPost();
	}

	private void facebookPost()
	{
		StartCoroutine(delayedStatusMessage(_facebookPostSuccess));
		ServiceManager.Instance.RequestReward("level_fb_reward", mainMenu.OnGetRewardSuccess, mainMenu.OnGetRewardFail);
	}

	private void facebookPostFailed(P31Error error)
	{
		Debug.LogError("Facebook post failed from level up: " + error.message);
		StartCoroutine(delayedStatusMessage(_facebookPostFailed));
	}

	private IEnumerator delayedStatusMessage(string status)
	{
		facebookStatusOverlay.SetActive(true);
		facebookStatus.text = status;
		yield return new WaitForSeconds(1.5f);
		facebookStatusOverlay.SetActive(false);
	}
}
