using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class RankUp : Popup
{
	public Transform rankMount;

	private MainMenu mainMenu;

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	protected override void Start()
	{
		base.Start();
		mainMenu = UnityEngine.Object.FindObjectOfType(typeof(MainMenu)) as MainMenu;
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
		}
	}

	private IEnumerator delayedStatusMessage(string status)
	{
		yield return new WaitForSeconds(1.5f);
	}
}
