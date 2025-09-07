using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class LevelUp : Popup
{
	public TextMesh levelText;

	public Transform[] unlockItems;

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
		mainMenu = Object.FindObjectOfType(typeof(MainMenu)) as MainMenu;
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
		}
	}

	private IEnumerator delayedStatusMessage(string status)
	{
		yield return new WaitForSeconds(1.5f);
	}
}
