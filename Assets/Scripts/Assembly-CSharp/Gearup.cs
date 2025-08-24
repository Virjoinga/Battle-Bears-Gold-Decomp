using System;
using System.Collections;
using System.Collections.Generic;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using Prime31;
using UnityEngine;

public class Gearup : MonoBehaviour
{
	public enum GearupSubmenu
	{
		LOADOUT = 0,
		CHARACTER = 1,
		PRIMARY = 2,
		SECONDARY = 3,
		MELEE = 4,
		SPECIAL = 5,
		EQUIPMENT1 = 6,
		EQUIPMENT2 = 7,
		SKIN = 8,
		PROMODE = 9,
		TAUNT = 10,
		NONE = 11
	}

	private const string _jumpProModeName = "jump";

	private const string _radarProModeName = "radar";

	public MainMenu mainMenu;

	public GUIController guiController;

	private GearupSubmenu currentSubmenu = GearupSubmenu.NONE;

	public AnimatedScroller gearupSubmenuScroller;

	public GameObject submenuScrollerButtonPrefab;

	public GameObject submenuScrollerButtonBUYPrefab;

	private List<GameObject> submenuButtons = new List<GameObject>();

	public GameObject availabilityText;

	public TextMesh loadoutText;

	public TextMesh characterNameText;

	public TextMesh skinText;

	public TextMesh tauntText;

	public TextMesh primaryLoadoutText;

	public TextMesh secondaryLoadoutText;

	public TextMesh meleeLoadoutText;

	public TextMesh specialLoadoutText;

	public TextMesh equipment1LoadoutText;

	public TextMesh equipment2LoadoutText;

	public TextMesh proModeLoadoutText;

	private Character currentCharacter;

	public Material redMaterial;

	public Material greenMaterial;

	public Transform equipButtonMount;

	private GameObject currentEquipButton;

	public GameObject equipButton;

	public GameObject equipSkinButton;

	public GameObject buyButton;

	public GameObject equipGoldButton;

	public GameObject buyGoldButton;

	public GameObject greyButton;

	public TextMesh characterTopBarText;

	public Transform characterTopBarIconMount;

	private PlayerLoadout temporaryLoadout;

	private Item currentlySelectedItem;

	public GameObject[] weaponStats;

	public TextMesh submenuText;

	public GameObject gearDescriptionPlate;

	private GameObject currentDescriptionPlate;

	public GameObject accessDenied;

	public TextMesh accessDeniedLevel;

	private Item attemptedItemToPurchase;

	private Purchaseable attemptedPurchaseInfo;

	public Transform[] equipmentDescriptionMounts;

	public Transform[] equipmentModifierMounts;

	private ArrayList equipmentDescriptionIcons = new ArrayList();

	public AudioClip[] clickSounds;

	public AudioClip purchaseSound;

	public AudioClip changeMenuSound;

	public GameObject gearTextDescriptionPopup;

	public GameObject secondEquipmentButton;

	public GameObject secondEquipmentBuyButton;

	public TextMesh descriptionButtonExtraText;

	private string productIDForIAP;

	private string _noneItemLabel;

	private string _moreInfoLabel;

	private string _goldSkinsDescription;

	private string _unaffectedByEquipment;

	private string _equipmentHealthModifier;

	private string _equipmentSpeedModifier;

	private string _equipmentSpecialModifier;

	private int _lastSelectedLoadout;

	private event Action<bool> _descriptionPlateVisibilityChanged;

	public event Action<bool> DescriptionPlateVisibilityChanged
	{
		add
		{
			this._descriptionPlateVisibilityChanged = (Action<bool>)Delegate.Combine(this._descriptionPlateVisibilityChanged, value);
		}
		remove
		{
			this._descriptionPlateVisibilityChanged = (Action<bool>)Delegate.Remove(this._descriptionPlateVisibilityChanged, value);
		}
	}

	private void Start()
	{
		UpdateLocalizedText();
		availabilityText.SetActive(false);
		temporaryLoadout = LoadoutManager.Instance.CurrentLoadout.Clone();
		for (int i = 0; i < weaponStats.Length; i++)
		{
			weaponStats[i].SetActive(false);
		}
		StartCoroutine(delayedRefreshLoadout());
		gearDescriptionPlate.SetActive(false);
		accessDenied.SetActive(false);
		resetEquipmentDescriptions();
		updateEquipmentSlots();
	}

	private IEnumerator delayedRefreshLoadout()
	{
		yield return new WaitForSeconds(0.15f);
		refreshLoadoutDisplay();
	}

	private void updateEquipmentSlots()
	{
		Item itemByName = ServiceManager.Instance.GetItemByName("BonusEquipSlot");
		if (ServiceManager.Instance.IsItemBought(itemByName.id))
		{
			secondEquipmentButton.SetActive(true);
			secondEquipmentButton.transform.Find("press").gameObject.SetActive(false);
			secondEquipmentBuyButton.SetActive(false);
			return;
		}
		secondEquipmentButton.SetActive(false);
		secondEquipmentBuyButton.SetActive(true);
		secondEquipmentBuyButton.transform.Find("press").gameObject.SetActive(false);
		Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(itemByName.id);
		if (purchaseableByID.current_gas > 0)
		{
			secondEquipmentBuyButton.transform.Find("joules_icon").gameObject.SetActive(false);
			(secondEquipmentBuyButton.transform.Find("cost").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:#,0}", purchaseableByID.current_gas);
		}
		else
		{
			secondEquipmentBuyButton.transform.Find("gas_icon").gameObject.SetActive(false);
			(secondEquipmentBuyButton.transform.Find("cost").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:#,0}", purchaseableByID.current_joules);
		}
		if ((double)itemByName.level > ServiceManager.Instance.GetStats().level)
		{
			(secondEquipmentBuyButton.transform.Find("levelNum").GetComponent(typeof(TextMesh)) as TextMesh).text = itemByName.level.ToString();
			return;
		}
		secondEquipmentBuyButton.transform.Find("levelNum").gameObject.SetActive(false);
		secondEquipmentBuyButton.transform.Find("miniLevel_icon").gameObject.SetActive(false);
	}

	private void UpdateLocalizedText()
	{
		_noneItemLabel = Language.Get("GEARUP_NONE");
		_moreInfoLabel = Language.Get("GEARUP_MORE_INFO");
		_goldSkinsDescription = Language.Get("GEARUP_GOLD_SKINS_DESCRIPTION");
		_unaffectedByEquipment = Language.Get("GEARUP_UNAFFECTED_BY_EQUIPMENT");
		_equipmentHealthModifier = Language.Get("GEARUP_EQUIPMENT_HEALTH_MODIFIER");
		_equipmentSpeedModifier = Language.Get("GEARUP_EQUIPMENT_SPEED_MODIFIER");
		_equipmentSpecialModifier = Language.Get("GEARUP_EQUIPMENT_SPECIAL_MODIFIER");
	}

	private void resetEquipmentDescriptions()
	{
		Transform[] array = equipmentDescriptionMounts;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActive(false);
		}
		Transform[] array2 = equipmentModifierMounts;
		foreach (Transform transform2 in array2)
		{
			transform2.gameObject.SetActive(false);
		}
		for (int k = 0; k < equipmentDescriptionIcons.Count; k++)
		{
			UnityEngine.Object.Destroy((GameObject)equipmentDescriptionIcons[k]);
		}
		equipmentDescriptionIcons.Clear();
	}

	private void addMainScreenIcon(GameObject obj, Item item)
	{
		Transform transform = obj.transform.Find("mountPoint");
		foreach (Transform item2 in transform)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		if (currentCharacter != null)
		{
			UnityEngine.Object @object = null;
			@object = ((item == null) ? Resources.Load("Icons/Specials/None") : ((item.type == "special") ? Resources.Load("Icons/Specials/" + item.name) : ((item.type == "equipment") ? Resources.Load("Icons/Equipment/" + item.name) : ((item.type == "skin") ? Resources.Load("Icons/Characters/" + currentCharacter.characterData.name + "/" + item.name + "_red") : ((item.type == "character") ? Resources.Load("Icons/genericCharacter") : ((!(item.type == "taunt")) ? Resources.Load("Icons/Weapons/" + currentCharacter.characterData.name + "/" + item.name) : Resources.Load("Icons/Taunts/" + currentCharacter.characterData.name + "/" + item.name)))))));
			if (@object != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	private void refreshLoadoutDisplay()
	{
		if (LoadoutManager.Instance == null || Store.Instance == null)
		{
			return;
		}
		PlayerLoadout currentLoadout = LoadoutManager.Instance.CurrentLoadout;
		if (currentLoadout != null)
		{
			currentCharacter = Store.Instance.characters[currentLoadout.model.name];
			SetUpCategoryButtons(currentLoadout);
			mainMenu.updateHeadIcons();
			gearDescriptionPlate.SetActive(false);
			accessDenied.SetActive(false);
			if (mainMenu.currentCharacter != null)
			{
				mainMenu.currentCharacter.transform.localPosition = Vector3.zero;
			}
			resetEquipmentDescriptions();
		}
	}

	private void SetUpCategoryButtons(PlayerLoadout loadout)
	{
		loadoutText.text = LoadoutManager.Instance.LoadoutPrefix + " " + loadout.loadoutNumber;
		if (LoadoutManager.Instance.CurrentLoadout.taunt != null && LoadoutManager.Instance.CurrentLoadout.taunt.title != string.Empty)
		{
			tauntText.text = LoadoutManager.Instance.CurrentLoadout.taunt.title;
			addMainScreenIcon(tauntText.transform.parent.gameObject, loadout.taunt);
		}
		else
		{
			tauntText.text = _noneItemLabel;
			addMainScreenIcon(tauntText.transform.parent.gameObject, null);
		}
		characterNameText.text = loadout.model.title;
		addMainScreenIcon(characterNameText.transform.parent.gameObject, loadout.model);
		skinText.text = loadout.skin.title;
		addMainScreenIcon(skinText.transform.parent.gameObject, loadout.skin);
		primaryLoadoutText.text = loadout.primary.title;
		addMainScreenIcon(primaryLoadoutText.transform.parent.gameObject, loadout.primary);
		secondaryLoadoutText.text = loadout.secondary.title;
		addMainScreenIcon(secondaryLoadoutText.transform.parent.gameObject, loadout.secondary);
		meleeLoadoutText.text = loadout.melee.title;
		addMainScreenIcon(meleeLoadoutText.transform.parent.gameObject, loadout.melee);
		if (loadout.special != null && loadout.special.name != string.Empty)
		{
			specialLoadoutText.text = loadout.special.title;
			addMainScreenIcon(specialLoadoutText.transform.parent.gameObject, loadout.special);
		}
		else
		{
			specialLoadoutText.text = _noneItemLabel;
			addMainScreenIcon(specialLoadoutText.transform.parent.gameObject, null);
		}
		if (loadout.equipment1 != null && loadout.equipment1.name != string.Empty)
		{
			equipment1LoadoutText.text = loadout.equipment1.title;
			addMainScreenIcon(equipment1LoadoutText.transform.parent.gameObject, loadout.equipment1);
		}
		else
		{
			equipment1LoadoutText.text = _noneItemLabel;
			addMainScreenIcon(equipment1LoadoutText.transform.parent.gameObject, null);
		}
		if (loadout.equipment2 != null && loadout.equipment2.name != string.Empty)
		{
			equipment2LoadoutText.text = loadout.equipment2.title;
			addMainScreenIcon(equipment2LoadoutText.transform.parent.gameObject, loadout.equipment2);
		}
		else
		{
			equipment2LoadoutText.text = _noneItemLabel;
			addMainScreenIcon(equipment2LoadoutText.transform.parent.gameObject, null);
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[UnityEngine.Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		accessDenied.SetActive(false);
		if (b.name == "buyEquipmentSlot")
		{
			PurchaseSecondaryEquipmentSlotButton();
		}
		if (b.name == "gearDescription_btn")
		{
			mainMenu.OnShowItemDescription(gearTextDescriptionPopup, currentlySelectedItem);
			if (currentlySelectedItem != null)
			{
				EventTracker.TrackEvent(new ItemDescriptionOpenedSchema(new ItemNameParameter(currentlySelectedItem.name), new ItemTypeParameter(currentSubmenu)));
			}
		}
		if (b.name == "back")
		{
			HandleBackButton();
		}
		if (b.name.StartsWith("gearcategory_"))
		{
			HandleGearCategoryButton(b);
		}
		else if (b.name.StartsWith("select#"))
		{
			HandleSelectButton(b);
		}
		else if (b.name == "equip")
		{
			if (currentSubmenu == GearupSubmenu.SKIN && currentlySelectedItem.name.StartsWith("gold"))
			{
				attemptEquipGoldItem();
			}
			else
			{
				attemptEquip();
			}
		}
	}

	private void PurchaseSecondaryEquipmentSlotButton()
	{
		Item itemByName = ServiceManager.Instance.GetItemByName("BonusEquipSlot");
		if (itemByName != null)
		{
			if ((double)itemByName.level > ServiceManager.Instance.GetStats().level)
			{
				accessDenied.SetActive(true);
				accessDeniedLevel.text = itemByName.level.ToString();
			}
			else
			{
				TryBuyItem(itemByName, false);
			}
		}
	}

	private void TryBuyItem(Item item, bool setGuiControllerActive, Purchaseable purchaseInfo = null)
	{
		purchaseInfo = purchaseInfo ?? ServiceManager.Instance.GetPurchaseableByID(item.id);
		if (purchaseInfo.current_gas > 0)
		{
			if (ServiceManager.Instance.GetStats().gas >= purchaseInfo.current_gas)
			{
				BuyItem(item, purchaseInfo);
				return;
			}
			PromptIAP(purchaseInfo.current_gas, true);
			if (setGuiControllerActive)
			{
				guiController.IsActive = true;
			}
		}
		else if (ServiceManager.Instance.GetStats().joules >= purchaseInfo.current_joules)
		{
			BuyItem(item, purchaseInfo);
		}
		else
		{
			PromptIAP(purchaseInfo.current_joules, false);
			if (setGuiControllerActive)
			{
				guiController.IsActive = true;
			}
		}
	}

	private void BuyItem(Item item, Purchaseable purchaseInfo)
	{
		attemptedPurchaseInfo = purchaseInfo;
		attemptedItemToPurchase = item;
		EventTracker.TrackEvent(ItemTransactionEventHelper.Transaction(item, purchaseInfo));
		ServiceManager.Instance.PurchaseItem(purchaseInfo, delegate
		{
			EventTracker.TrackEvent(ItemTransactionEventHelper.PurchaseSucceeded(item, purchaseInfo));
			OnItemBuySuccess();
		}, delegate
		{
			EventTracker.TrackEvent(ItemTransactionEventHelper.PurchaseFailed(item, purchaseInfo, ServiceManager.Instance.LastError));
			OnItemBuyFail();
		});
	}

	private void HandleBackButton()
	{
		if (changeMenuSound != null)
		{
			AudioSource.PlayClipAtPoint(changeMenuSound, Vector3.zero);
		}
		switch (currentSubmenu)
		{
		case GearupSubmenu.LOADOUT:
			if (LoadoutManager.Instance.CurrentLoadout.loadoutNumber != temporaryLoadout.loadoutNumber)
			{
				mainMenu.loadCharacter(LoadoutManager.Instance.CurrentLoadout);
				mainMenu.OnDelayedRotate();
			}
			break;
		case GearupSubmenu.CHARACTER:
			if (LoadoutManager.Instance.CurrentLoadout.model.name != temporaryLoadout.model.name)
			{
				mainMenu.loadCharacter(LoadoutManager.Instance.CurrentLoadout);
				mainMenu.OnDelayedRotate();
			}
			break;
		case GearupSubmenu.SKIN:
			if (LoadoutManager.Instance.CurrentLoadout.skin.name != temporaryLoadout.skin.name)
			{
				mainMenu.loadSkin(LoadoutManager.Instance.CurrentLoadout);
			}
			break;
		case GearupSubmenu.PRIMARY:
			if (LoadoutManager.Instance.CurrentLoadout.primary.name != temporaryLoadout.primary.name)
			{
				mainMenu.handleNewWeapon(LoadoutManager.Instance.CurrentLoadout, currentSubmenu);
			}
			break;
		case GearupSubmenu.SECONDARY:
			if (LoadoutManager.Instance.CurrentLoadout.secondary.name != temporaryLoadout.secondary.name)
			{
				mainMenu.handleNewWeapon(LoadoutManager.Instance.CurrentLoadout, currentSubmenu);
			}
			break;
		case GearupSubmenu.MELEE:
			if (LoadoutManager.Instance.CurrentLoadout.melee.name != temporaryLoadout.melee.name)
			{
				mainMenu.handleNewWeapon(LoadoutManager.Instance.CurrentLoadout, currentSubmenu);
			}
			break;
		}
		temporaryLoadout = LoadoutManager.Instance.CurrentLoadout.Clone();
		refreshLoadoutDisplay();
		EventTracker.TrackEvent(new ItemCategoryClosedSchema(new ItemTypeParameter(currentSubmenu)));
	}

	private void HandleGearCategoryButton(GUIButton b)
	{
		string text = b.name;
		if (changeMenuSound != null)
		{
			AudioSource.PlayClipAtPoint(changeMenuSound, Vector3.zero);
		}
		string[] array = text.Split('_');
		string text2 = array[1];
		OnReset();
		switch (text2)
		{
		case "loadout":
			currentSubmenu = GearupSubmenu.LOADOUT;
			loadLoadouts();
			break;
		case "taunt":
			currentSubmenu = GearupSubmenu.TAUNT;
			loadTaunts();
			break;
		case "character":
			currentSubmenu = GearupSubmenu.CHARACTER;
			loadCharacters();
			break;
		case "primary":
			currentSubmenu = GearupSubmenu.PRIMARY;
			loadPrimaries();
			break;
		case "secondary":
			currentSubmenu = GearupSubmenu.SECONDARY;
			loadSecondaries();
			break;
		case "melee":
			currentSubmenu = GearupSubmenu.MELEE;
			loadMelees();
			break;
		case "special":
			currentSubmenu = GearupSubmenu.SPECIAL;
			loadSpecials();
			if (currentEquipButton != null && (temporaryLoadout.special == null || temporaryLoadout.special.name.Equals(string.Empty)))
			{
				currentEquipButton.SetActive(false);
			}
			break;
		case "equipment1":
			currentSubmenu = GearupSubmenu.EQUIPMENT1;
			loadEquipment1();
			if (currentEquipButton != null && (temporaryLoadout.equipment1 == null || temporaryLoadout.equipment1.name.Equals(string.Empty)))
			{
				currentEquipButton.SetActive(false);
			}
			break;
		case "equipment2":
			currentSubmenu = GearupSubmenu.EQUIPMENT2;
			loadEquipment2();
			if (currentEquipButton != null && (temporaryLoadout.equipment2 == null || temporaryLoadout.equipment2.name.Equals(string.Empty)))
			{
				currentEquipButton.SetActive(false);
			}
			break;
		case "skin":
			currentSubmenu = GearupSubmenu.SKIN;
			loadSkins();
			break;
		case "proMode":
			currentSubmenu = GearupSubmenu.PROMODE;
			loadProModeOptions();
			if (currentEquipButton != null)
			{
				currentEquipButton.SetActive(false);
			}
			if (b.transform.parent.name.Contains("ad"))
			{
				EventTracker.TrackEvent(new ProModeAdClickedSchema());
			}
			break;
		}
		if (currentSubmenu == GearupSubmenu.PRIMARY || currentSubmenu == GearupSubmenu.SECONDARY || currentSubmenu == GearupSubmenu.MELEE)
		{
			mainMenu.ToggleWeapons(true);
		}
		Transform[] array2 = equipmentModifierMounts;
		foreach (Transform transform in array2)
		{
			transform.gameObject.SetActive(false);
		}
		GameObject[] array3 = weaponStats;
		foreach (GameObject gameObject in array3)
		{
			gameObject.SetActive(false);
		}
		EventTracker.TrackEvent(new ItemCategoryOpenedSchema(new ItemTypeParameter(currentSubmenu)));
	}

	private void HandleSelectButton(GUIButton button)
	{
		string[] array = button.name.Split('#');
		string text = array[1];
		gearupSubmenuScroller.clearButtons();
		button.OnButtonPressed();
		temporaryLoadout = LoadoutManager.Instance.CurrentLoadout.Clone();
		switch (currentSubmenu)
		{
		case GearupSubmenu.LOADOUT:
		{
			string s = text.Replace(LoadoutManager.Instance.LoadoutPrefix, string.Empty);
			int num = int.Parse(s);
			if (num != _lastSelectedLoadout)
			{
				_lastSelectedLoadout = num;
				temporaryLoadout = LoadoutManager.Instance.GetLoadoutByNumber(num, string.Empty);
				handleWeapon(Store.Instance.items_by_name[text]);
				mainMenu.loadCharacter(temporaryLoadout);
				mainMenu.OnDelayedRotate();
			}
			break;
		}
		case GearupSubmenu.TAUNT:
			temporaryLoadout.taunt = Store.Instance.items_by_name[text];
			if (temporaryLoadout.taunt != null && mainMenu.CharacterAnimator[temporaryLoadout.taunt.name] != null)
			{
				mainMenu.ToggleWeapons(false);
				mainMenu.CharacterAnimator.Stop();
				mainMenu.CharacterAnimator.Play(temporaryLoadout.taunt.name);
			}
			handleWeapon(temporaryLoadout.taunt);
			break;
		case GearupSubmenu.CHARACTER:
			temporaryLoadout = LoadoutManager.Instance.LoadLoadout(ServiceManager.Instance.GetStats().pid, text, LoadoutManager.Instance.CurrentLoadout.loadoutNumber);
			handleWeapon(temporaryLoadout.model);
			if (temporaryLoadout.model.name != mainMenu.currentCharacter.name)
			{
				mainMenu.loadCharacter(temporaryLoadout);
				mainMenu.OnDelayedRotate();
			}
			break;
		case GearupSubmenu.PRIMARY:
			temporaryLoadout.primary = Store.Instance.items_by_name[text];
			handleWeapon(temporaryLoadout.primary);
			mainMenu.handleNewWeapon(temporaryLoadout, currentSubmenu);
			break;
		case GearupSubmenu.SECONDARY:
			temporaryLoadout.secondary = Store.Instance.items_by_name[text];
			handleWeapon(temporaryLoadout.secondary);
			mainMenu.handleNewWeapon(temporaryLoadout, currentSubmenu);
			break;
		case GearupSubmenu.MELEE:
			temporaryLoadout.melee = Store.Instance.items_by_name[text];
			handleWeapon(temporaryLoadout.melee);
			mainMenu.handleNewWeapon(temporaryLoadout, currentSubmenu);
			break;
		case GearupSubmenu.SKIN:
			temporaryLoadout.skin = Store.Instance.items_by_name[text];
			handleWeapon(temporaryLoadout.skin);
			mainMenu.loadSkin(temporaryLoadout);
			break;
		case GearupSubmenu.SPECIAL:
			temporaryLoadout.special = Store.Instance.items_by_name[text];
			handleWeapon(temporaryLoadout.special);
			break;
		case GearupSubmenu.EQUIPMENT1:
			temporaryLoadout.equipment1 = Store.Instance.items_by_name[text];
			handleWeapon(temporaryLoadout.equipment1);
			break;
		case GearupSubmenu.EQUIPMENT2:
			temporaryLoadout.equipment2 = Store.Instance.items_by_name[text];
			handleWeapon(temporaryLoadout.equipment2);
			break;
		case GearupSubmenu.PROMODE:
			handleWeapon(Store.Instance.items_by_name[text]);
			break;
		}
		EventTracker.TrackEvent(new ItemSelectedSchema(new ItemNameParameter(text), new ItemTypeParameter(currentSubmenu)));
	}

	private void equipSelectedItem()
	{
		if (currentEquipButton != null)
		{
			currentEquipButton.name = "customize_gearUp";
		}
		PlayerLoadout playerLoadout = LoadoutManager.Instance.CurrentLoadout;
		switch (currentSubmenu)
		{
		case GearupSubmenu.LOADOUT:
		{
			string s = currentlySelectedItem.name.Replace(LoadoutManager.Instance.LoadoutPrefix, string.Empty);
			playerLoadout = LoadoutManager.Instance.GetLoadoutByNumber(int.Parse(s), string.Empty);
			break;
		}
		case GearupSubmenu.TAUNT:
			playerLoadout.taunt = currentlySelectedItem;
			break;
		case GearupSubmenu.CHARACTER:
			playerLoadout = LoadoutManager.Instance.LoadLoadout(ServiceManager.Instance.GetStats().pid, currentlySelectedItem.name, playerLoadout.loadoutNumber);
			break;
		case GearupSubmenu.PRIMARY:
			playerLoadout.primary = currentlySelectedItem;
			break;
		case GearupSubmenu.SECONDARY:
			playerLoadout.secondary = currentlySelectedItem;
			break;
		case GearupSubmenu.MELEE:
			playerLoadout.melee = currentlySelectedItem;
			break;
		case GearupSubmenu.SKIN:
			playerLoadout.skin = currentlySelectedItem;
			break;
		case GearupSubmenu.SPECIAL:
			playerLoadout.special = currentlySelectedItem;
			break;
		case GearupSubmenu.EQUIPMENT1:
			playerLoadout.equipment1 = currentlySelectedItem;
			break;
		case GearupSubmenu.EQUIPMENT2:
			playerLoadout.equipment2 = currentlySelectedItem;
			break;
		case GearupSubmenu.PROMODE:
			EquipProModeItem(currentlySelectedItem);
			break;
		}
		LoadoutManager.Instance.CurrentLoadout = playerLoadout;
		temporaryLoadout = playerLoadout;
		refreshLoadoutDisplay();
		guiController.IsActive = true;
		if (currentEquipButton != null)
		{
			SendMessageUpwards("OnGUIButtonClicked", currentEquipButton.GetComponent(typeof(GUIButton)) as GUIButton, SendMessageOptions.DontRequireReceiver);
		}
		EventTracker.TrackEvent(new ItemEquippedSchema(new ItemNameParameter(currentlySelectedItem.name), new ItemTypeParameter(currentSubmenu)));
	}

	private void EquipProModeItem(Item itemToEquip)
	{
		switch (itemToEquip.name)
		{
		case "jump":
		case "radar":
			return;
		}
		Debug.LogError("No way to handle pro-mode item: " + itemToEquip.name);
	}

	private void attemptEquip()
	{
		guiController.IsActive = false;
		Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(currentlySelectedItem.id);
		if (ServiceManager.Instance.IsItemBought(currentlySelectedItem.id))
		{
			if (currentlySelectedItem.type == "equipment")
			{
				PlayerLoadout currentLoadout = LoadoutManager.Instance.CurrentLoadout;
				if (currentSubmenu == GearupSubmenu.EQUIPMENT1)
				{
					if (currentLoadout.equipment2 != null && currentLoadout.equipment2.id == currentlySelectedItem.id)
					{
						guiController.IsActive = true;
						return;
					}
				}
				else if (currentSubmenu == GearupSubmenu.EQUIPMENT2 && currentLoadout.equipment1 != null && currentLoadout.equipment1.id == currentlySelectedItem.id)
				{
					guiController.IsActive = true;
					return;
				}
			}
			equipSelectedItem();
		}
		else
		{
			TryBuyItem(currentlySelectedItem, true, purchaseableByID);
		}
	}

	private void attemptEquipGoldItem()
	{
		guiController.IsActive = false;
		Item itemByName = ServiceManager.Instance.GetItemByName("goldSkinBundle");
		attemptedItemToPurchase = currentlySelectedItem;
		Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(itemByName.id);
		if (ServiceManager.Instance.IsItemBought(currentlySelectedItem.id))
		{
			equipSelectedItem();
		}
		else
		{
			mainMenu.TryCreatePopup(new SetupPopup(mainMenu.buyGoldPackPopup, SetupGoldPopup));
		}
	}

	private void SetupGoldPopup(GameObject goldPopup)
	{
		BuyGoldPack component = goldPopup.GetComponent<BuyGoldPack>();
		component.OnPackPurchaseCancel = OnGoldPackCancel;
		component.OnPackPurchaseFailure = OnGoldPackFail;
		component.OnPackPurchaseSuccess = OnGoldPackSuccess;
		guiController.IsActive = true;
		ReportGoldPackEvent("gold_skin_purchase_viewed", null);
	}

	public GameObject createShopPopup(GameObject popup, int itemCost, bool isGas)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(popup) as GameObject;
		gameObject.transform.parent = mainMenu.popupRoot;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		InsufficientFunds component = gameObject.GetComponent<InsufficientFunds>();
		if (component != null)
		{
			component.OnSetCallingObject(mainMenu.gameObject, mainMenu.popupCamera);
			component.OnSetGUIControllerToDisable(guiController);
			component.OnSetItemCost(itemCost, isGas);
			component.OnSetMainMenu(mainMenu);
		}
		return gameObject;
	}

	private void PromptIAP(int itemCost, bool costIsGas)
	{
		int num = 0;
		num = ((!costIsGas) ? (itemCost - ServiceManager.Instance.GetStats().joules) : (itemCost - ServiceManager.Instance.GetStats().gas));
		Dictionary<string, int> productIds = GetProductIds();
		int num2 = int.MaxValue;
		string text = string.Empty;
		int num3 = int.MinValue;
		string text2 = string.Empty;
		foreach (KeyValuePair<string, int> item in productIds)
		{
			if ((!costIsGas || item.Key.Contains("gascan")) && (costIsGas || item.Key.Contains("joules")))
			{
				int num4 = item.Value - num;
				if (num4 > 0 && num4 < num2)
				{
					text = item.Key;
					num2 = num4;
				}
				if (item.Value > num3)
				{
					num3 = item.Value;
					text2 = item.Key;
				}
			}
		}
		if (text == string.Empty)
		{
			text = text2;
		}
		int num5 = productIds[text];
		string title = ((!costIsGas) ? "Joules" : "Gas Cans");
		string message = string.Format("You need {0:#,#} more {1} to purchase. Buy {2:#,#}?", num, (!costIsGas) ? "Joules" : "Gas Cans", num5);
		productIDForIAP = text;
		EtceteraAndroid.showAlert(title, message, "Buy", "Cancel");
		EtceteraAndroidManager.alertButtonClickedEvent += AlertCallback;
		EventTracker.TrackEvent(new PurchaseAdditionalFundsOpenedSchema(new ItemNameParameter(currentlySelectedItem.name), new ItemTypeParameter(currentSubmenu), new ProductIDParameter(productIDForIAP), IAPTransactionEventHelper.RealCurrencySpent(productIDForIAP), IAPTransactionEventHelper.VirtualCurrencyReceived(productIDForIAP)));
	}

	private Dictionary<string, int> GetProductIds()
	{
		Dictionary<string, int> dictionary = null;
		return ServiceManager.Instance.GetGooglePlayProducts();
	}

	private void AlertCallback(string buttonName)
	{
		EtceteraAndroidManager.alertButtonClickedEvent -= AlertCallback;
		if (buttonName == "Buy")
		{
			PerformIAP(productIDForIAP);
			return;
		}
		Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(currentlySelectedItem.id);
		bool flag = purchaseableByID.current_gas > 0;
		int num = GetProductIds()[productIDForIAP];
		EventTracker.TrackEvent(new PurchaseAdditionalFundsClosedSchema(new ItemNameParameter(currentlySelectedItem.name), new ItemTypeParameter(currentSubmenu), new ProductIDParameter(productIDForIAP), IAPTransactionEventHelper.RealCurrencySpent(productIDForIAP), IAPTransactionEventHelper.VirtualCurrencyReceived(productIDForIAP)));
	}

	private void PerformIAP(string iapProductID)
	{
		ServiceManager.Instance.PurchaseCurrency(iapProductID, 1, delegate
		{
			Debug.Log("Currency purchase success");
			mainMenu.OnUpdatePlayerStats();
		}, delegate
		{
			Debug.Log("Currency purchase failure");
		}, delegate
		{
			Debug.Log("Currency purchase cancelled");
		});
	}

	private void OnGoldPackSuccess(BuyGoldPack goldPopup)
	{
		if (purchaseSound != null)
		{
			AudioSource.PlayClipAtPoint(purchaseSound, Vector3.zero);
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (goldPopup.gasPurchased > 0)
		{
			dictionary.Add("gas_cans_purchased", goldPopup.gasPurchased);
		}
		if (goldPopup.joulesPurchased > 0)
		{
			dictionary.Add("joules_purchased", goldPopup.joulesPurchased);
		}
		ReportGoldPackEvent("gold_skin_purchase_success", dictionary);
		if (attemptedItemToPurchase.type == "skin")
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["QUICK_CHANGE_ARTIST"]);
		}
		CumulativeStats.Instance.OnSaveStats();
		mainMenu.OnUpdatePlayerStats();
		equipSelectedItem();
		updateEquipmentSlots();
	}

	private void OnGoldPackFail(BuyGoldPack goldPopup)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (goldPopup.gasPurchased > 0)
		{
			dictionary.Add("gas_cans_purchased", goldPopup.gasPurchased);
		}
		if (goldPopup.joulesPurchased > 0)
		{
			dictionary.Add("joules_purchased", goldPopup.joulesPurchased);
		}
		ReportGoldPackEvent("gold_skin_purchase_failed", dictionary);
		if (ServiceManager.Instance.LastError == "Invalid session")
		{
			mainMenu.OnSessionError();
		}
	}

	private void OnGoldPackCancel(BuyGoldPack goldPopup)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (goldPopup.gasPurchased > 0)
		{
			dictionary.Add("gas_cans_purchased", goldPopup.gasPurchased);
		}
		if (goldPopup.joulesPurchased > 0)
		{
			dictionary.Add("joules_purchased", goldPopup.joulesPurchased);
		}
		ReportGoldPackEvent("gold_skin_purchase_canceled", dictionary);
	}

	private void ReportGoldPackEvent(string eventName, Dictionary<string, object> otherArgs)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		Stats stats = ServiceManager.Instance.GetStats();
		dictionary.Add("level", (int)stats.level);
		dictionary.Add("skill_color", Enum.GetName(typeof(Rank), (int)ServiceManager.GetRank(stats.skill)));
		dictionary.Add("player_joules", stats.joules);
		dictionary.Add("player_gas", stats.gas);
		dictionary.Add("device_platform", Application.platform.ToString());
		dictionary.Add("selected_skin", attemptedItemToPurchase.name);
		int val = -1;
		ServiceManager.Instance.UpdateProperty("gold_skins_deal", ref val);
		Deal deal = ServiceManager.Instance.GetDeal(val);
		int? gas = deal.gas;
		if (gas.HasValue)
		{
			int? gas2 = deal.gas;
			if (gas2.HasValue && gas2.Value > 0)
			{
				dictionary.Add("bundle_price_gas", deal.gas);
				goto IL_0173;
			}
		}
		int? joules = deal.joules;
		if (joules.HasValue)
		{
			int? joules2 = deal.joules;
			if (joules2.HasValue && joules2.Value > 0)
			{
				dictionary.Add("bundle_price_joules", deal.joules);
			}
		}
		goto IL_0173;
		IL_0173:
		if (otherArgs == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> otherArg in otherArgs)
		{
			dictionary[otherArg.Key] = otherArg.Value;
		}
	}

	private void OnItemBuySuccess()
	{
		if (purchaseSound != null)
		{
			AudioSource.PlayClipAtPoint(purchaseSound, Vector3.zero);
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		Stats stats = ServiceManager.Instance.GetStats();
		dictionary.Add("level", (int)stats.level);
		dictionary.Add("skill_color", Enum.GetName(typeof(Rank), (int)ServiceManager.GetRank(stats.skill)));
		if (attemptedPurchaseInfo.current_joules > 0)
		{
			dictionary.Add("cur_joules", attemptedPurchaseInfo.current_joules);
			dictionary.Add("base_joules", attemptedPurchaseInfo.base_joules);
		}
		if (attemptedPurchaseInfo.current_gas > 0)
		{
			dictionary.Add("cur_gas", attemptedPurchaseInfo.current_gas);
			dictionary.Add("base_gas", attemptedPurchaseInfo.base_gas);
		}
		Debug.Log(attemptedItemToPurchase.type.Substring(0, 4) + "_" + attemptedItemToPurchase.name);
		if (attemptedItemToPurchase.type == "skin")
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["QUICK_CHANGE_ARTIST"]);
		}
		if (attemptedItemToPurchase.type == "character")
		{
			CumulativeStats.Instance.numClassesBought++;
			if (CumulativeStats.Instance.numClassesBought >= 3)
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["GOTTA_COLLECT_THEM_ALL"]);
			}
			if (CumulativeStats.Instance.numClassesBought >= 2)
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["THE_HONEY_POT"]);
			}
			if (CumulativeStats.Instance.numClassesBought >= 1)
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["WE_HAVE_CLASS"]);
			}
		}
		if (attemptedItemToPurchase.type == "equipment")
		{
			CumulativeStats.Instance.numEquipmentsBought++;
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["PAWSABILITY"], (float)CumulativeStats.Instance.numEquipmentsBought / 2f * 100f);
		}
		CumulativeStats.Instance.OnSaveStats();
		mainMenu.OnUpdatePlayerStats();
		equipSelectedItem();
		updateEquipmentSlots();
	}

	private void OnItemBuyFail()
	{
		if (ServiceManager.Instance.LastError == "Invalid session")
		{
			mainMenu.OnSessionError();
		}
	}

	private void OnReset()
	{
		availabilityText.SetActive(false);
		gearupSubmenuScroller.OnReset();
		for (int i = 0; i < submenuButtons.Count; i++)
		{
			UnityEngine.Object.Destroy(submenuButtons[i].gameObject);
		}
		submenuButtons.Clear();
	}

	private GUIButton addGoldSkinEntry(Item item, bool isEquipped)
	{
		Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
		GameObject gameObject = null;
		if (ServiceManager.Instance.IsItemBought(item.id))
		{
			gameObject = UnityEngine.Object.Instantiate(submenuScrollerButtonPrefab) as GameObject;
			gameObject.name = "select#" + item.name;
			gameObject.transform.Find("title").GetComponent<TextMesh>().text = item.title;
			gameObject.transform.Find("subtitle").GetComponent<TextMesh>().text = ((!isEquipped) ? string.Empty : "Equipped");
		}
		else
		{
			gameObject = UnityEngine.Object.Instantiate(submenuScrollerButtonBUYPrefab) as GameObject;
			gameObject.name = "select#" + item.name;
			gameObject.transform.Find("gas_icon").gameObject.SetActive(false);
			gameObject.transform.Find("joules_icon").gameObject.SetActive(false);
			gameObject.transform.Find("cost").GetComponent<TextMesh>().text = _moreInfoLabel;
			(gameObject.transform.Find("subtitle").GetComponent(typeof(TextMesh)) as TextMesh).text = item.title;
			if ((double)item.level > ServiceManager.Instance.GetStats().level)
			{
				(gameObject.transform.Find("levelNum").GetComponent(typeof(TextMesh)) as TextMesh).text = item.level.ToString();
			}
			else
			{
				gameObject.transform.Find("levelNum").gameObject.SetActive(false);
				gameObject.transform.Find("miniLevel_icon").gameObject.SetActive(false);
			}
		}
		UnityEngine.Object @object = (@object = Resources.Load("Icons/Characters/" + currentCharacter.characterData.name + "/" + item.name + "_red"));
		if (@object != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(@object) as GameObject;
			Transform parent = gameObject.transform.Find("mountPoint");
			gameObject2.transform.parent = parent;
			gameObject2.transform.localPosition = new Vector3(0f, 5f, 0f);
			gameObject2.transform.localEulerAngles = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		GUIButton gUIButton = gameObject.GetComponent(typeof(GUIButton)) as GUIButton;
		gUIButton.listener = base.gameObject;
		gearupSubmenuScroller.addButton(gUIButton);
		submenuButtons.Add(gameObject);
		return gUIButton;
	}

	private GUIButton addEntry(Item item, bool isEquipped)
	{
		Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
		GameObject gameObject = null;
		if (ServiceManager.Instance.IsItemBought(item.id))
		{
			gameObject = UnityEngine.Object.Instantiate(submenuScrollerButtonPrefab) as GameObject;
			gameObject.name = "select#" + item.name;
			(gameObject.transform.Find("title").GetComponent(typeof(TextMesh)) as TextMesh).text = item.title;
			if (isEquipped)
			{
				(gameObject.transform.Find("subtitle").GetComponent(typeof(TextMesh)) as TextMesh).text = "Equipped";
			}
			else
			{
				(gameObject.transform.Find("subtitle").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
			}
		}
		else
		{
			gameObject = UnityEngine.Object.Instantiate(submenuScrollerButtonBUYPrefab) as GameObject;
			gameObject.name = "select#" + item.name;
			if (purchaseableByID.current_gas > 0)
			{
				gameObject.transform.Find("joules_icon").gameObject.SetActive(false);
				gameObject.transform.Find("cost").GetComponent<TextMesh>().text = string.Format("{0:#,0}", purchaseableByID.current_gas);
			}
			else
			{
				gameObject.transform.Find("gas_icon").gameObject.SetActive(false);
				gameObject.transform.Find("cost").GetComponent<TextMesh>().text = string.Format("{0:#,0}", purchaseableByID.current_joules);
			}
			(gameObject.transform.Find("subtitle").GetComponent(typeof(TextMesh)) as TextMesh).text = item.title;
			if ((double)item.level > ServiceManager.Instance.GetStats().level)
			{
				(gameObject.transform.Find("levelNum").GetComponent(typeof(TextMesh)) as TextMesh).text = item.level.ToString();
			}
			else
			{
				gameObject.transform.Find("levelNum").gameObject.SetActive(false);
				gameObject.transform.Find("miniLevel_icon").gameObject.SetActive(false);
			}
		}
		UnityEngine.Object @object = null;
		if (item.type == "character")
		{
			Character character = Store.Instance.characters[item.name];
			for (int i = 0; i < character.skins.Count; i++)
			{
				if (character.skins[i].is_default)
				{
					@object = Resources.Load("Icons/Characters/" + item.name + "/" + character.skins[i].name + "_red");
					break;
				}
			}
		}
		else if (item.type == "skin")
		{
			@object = Resources.Load("Icons/Characters/" + currentCharacter.characterData.name + "/" + item.name + "_red");
		}
		else if (item.type == "special")
		{
			@object = Resources.Load("Icons/Specials/" + item.name);
		}
		else if (item.type == "equipment")
		{
			@object = Resources.Load("Icons/Equipment/" + item.name);
		}
		else if (item.type == "proMode")
		{
			@object = Resources.Load("Icons/ProMode/" + item.name);
		}
		else if (!(item.type == "loadout"))
		{
			@object = ((!(item.type == "taunt")) ? Resources.Load("Icons/Weapons/" + currentCharacter.characterData.name + "/" + item.name) : Resources.Load("Icons/Taunts/" + currentCharacter.characterData.name + "/" + item.name));
		}
		else
		{
			int loadoutNumber = int.Parse(item.name.Replace(LoadoutManager.Instance.LoadoutPrefix, string.Empty));
			PlayerLoadout loadoutByNumber = LoadoutManager.Instance.GetLoadoutByNumber(loadoutNumber, string.Empty);
			@object = Resources.Load("Icons/Characters/" + loadoutByNumber.model.name + "/" + loadoutByNumber.skin.name + "_red");
		}
		if (@object != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(@object) as GameObject;
			Transform parent = gameObject.transform.Find("mountPoint");
			gameObject2.transform.parent = parent;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		GUIButton gUIButton = gameObject.GetComponent(typeof(GUIButton)) as GUIButton;
		gUIButton.listener = base.gameObject;
		gearupSubmenuScroller.addButton(gUIButton);
		submenuButtons.Add(gameObject);
		return gUIButton;
	}

	private void SetUpWeaponStatsDisplay(Item item)
	{
		for (int i = 0; i < item.stats.Length; i++)
		{
			weaponStats[i].SetActive(true);
			(weaponStats[i].GetComponent(typeof(TextMesh)) as TextMesh).text = item.stats[i].name;
			Transform transform = weaponStats[i].transform.Find("backer/charge");
			float propertyMax = ServiceManager.Instance.getPropertyMax(item.stats[i].name);
			float num = 0f;
			if (item.properties.ContainsKey(item.stats[i].value))
			{
				num = (float)item.properties[item.stats[i].value] / propertyMax;
			}
			else
			{
				Debug.LogWarning("stat property: " + item.stats[i].name + " is not a property of item " + item.name + " this should not happen!");
			}
			float num2 = 0.2f;
			float num3 = 0f;
			Vector3 localScale = transform.localScale;
			localScale.x = num2 + 0.95f * num * (1f - num2 - num3);
			transform.localScale = localScale;
		}
	}

	private void SetUpEquipOrPurchaseButton(Item item, Purchaseable purchaseInfo)
	{
		if (item.type.Equals("skin") && item.name.StartsWith("gold"))
		{
			if (ServiceManager.Instance.IsItemBought(item.id))
			{
				currentEquipButton = UnityEngine.Object.Instantiate(equipGoldButton) as GameObject;
			}
			else
			{
				currentEquipButton = UnityEngine.Object.Instantiate(buyGoldButton) as GameObject;
			}
			currentEquipButton.transform.parent = equipButtonMount;
			currentEquipButton.transform.localPosition = Vector3.zero;
			currentEquipButton.transform.localEulerAngles = Vector3.zero;
			currentEquipButton.name = "equip";
			(currentEquipButton.GetComponent(typeof(GUIButton)) as GUIButton).listener = base.gameObject;
		}
		else if (ServiceManager.Instance.IsItemBought(item.id))
		{
			if (item.type == "skin")
			{
				currentEquipButton = UnityEngine.Object.Instantiate(equipSkinButton) as GameObject;
			}
			else if (item.type == "proMode")
			{
				SetUpProModeEquipButtonAlreadyPurchased(item);
			}
			else
			{
				currentEquipButton = UnityEngine.Object.Instantiate(equipButton) as GameObject;
			}
			currentEquipButton.name = "equip";
			currentEquipButton.transform.parent = equipButtonMount;
			currentEquipButton.transform.localPosition = Vector3.zero;
			currentEquipButton.transform.localEulerAngles = Vector3.zero;
			(currentEquipButton.GetComponent(typeof(GUIButton)) as GUIButton).listener = base.gameObject;
		}
		else if ((double)item.level > ServiceManager.Instance.GetStats().level)
		{
			currentEquipButton = UnityEngine.Object.Instantiate(greyButton) as GameObject;
			currentEquipButton.transform.parent = equipButtonMount;
			currentEquipButton.transform.localPosition = Vector3.zero;
			currentEquipButton.transform.localEulerAngles = Vector3.zero;
			if (purchaseInfo.current_gas > 0)
			{
				currentEquipButton.transform.Find("joules_icon").gameObject.SetActive(false);
				(currentEquipButton.transform.Find("cost").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:#,0}", purchaseInfo.current_gas);
			}
			else
			{
				currentEquipButton.transform.Find("gas_icon").gameObject.SetActive(false);
				(currentEquipButton.transform.Find("cost").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:#,0}", purchaseInfo.current_joules);
			}
			(currentEquipButton.transform.Find("level").GetComponent(typeof(TextMesh)) as TextMesh).text = item.level.ToString();
			accessDenied.SetActive(true);
			accessDeniedLevel.text = item.level.ToString();
		}
		else
		{
			currentEquipButton = UnityEngine.Object.Instantiate(buyButton) as GameObject;
			currentEquipButton.transform.parent = equipButtonMount;
			currentEquipButton.transform.localPosition = Vector3.zero;
			currentEquipButton.transform.localEulerAngles = Vector3.zero;
			currentEquipButton.name = "equip";
			if (purchaseInfo.current_gas > 0)
			{
				currentEquipButton.transform.Find("joules_icon").gameObject.SetActive(false);
				(currentEquipButton.transform.Find("cost").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:#,0}", purchaseInfo.current_gas);
			}
			else
			{
				currentEquipButton.transform.Find("gas_icon").gameObject.SetActive(false);
				(currentEquipButton.transform.Find("cost").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:#,0}", purchaseInfo.current_joules);
			}
			(currentEquipButton.GetComponent(typeof(GUIButton)) as GUIButton).listener = base.gameObject;
		}
	}

	private void SetUpProModeEquipButtonAlreadyPurchased(Item item)
	{
		currentEquipButton = UnityEngine.Object.Instantiate(equipButton) as GameObject;
		currentEquipButton.SetActive(false);
	}

	private void handleWeapon(Item item)
	{
		accessDenied.SetActive(false);
		descriptionButtonExtraText.gameObject.SetActive(false);
		for (int i = 0; i < weaponStats.Length; i++)
		{
			weaponStats[i].SetActive(false);
		}
		if (currentEquipButton != null)
		{
			UnityEngine.Object.Destroy(currentEquipButton);
		}
		if (currentDescriptionPlate != null)
		{
			UnityEngine.Object.Destroy(currentDescriptionPlate);
		}
		if (mainMenu.currentCharacter != null)
		{
			mainMenu.currentCharacter.transform.localPosition = Vector3.zero;
		}
		gearDescriptionPlate.SetActive(false);
		if (item != null)
		{
			UnityEngine.Object @object = null;
			if (item.type == "character")
			{
				Character character = Store.Instance.characters[item.name];
				for (int j = 0; j < character.skins.Count; j++)
				{
					if (character.skins[j].is_default)
					{
						@object = Resources.Load("Icons/Characters/" + item.name + "/" + character.skins[j].name + "_red");
						break;
					}
				}
			}
			else if (!(item.type == "skin"))
			{
				@object = ((item.type == "special") ? Resources.Load("Icons/Specials/" + item.name) : ((item.type == "equipment") ? Resources.Load("Icons/Equipment/" + item.name) : ((!(item.type == "taunt")) ? Resources.Load("Icons/Weapons/" + currentCharacter.characterData.name + "/" + item.name) : Resources.Load("Icons/Taunts/" + currentCharacter.characterData.name + "/" + item.name))));
			}
			else
			{
				@object = Resources.Load("Icons/Characters/" + currentCharacter.characterData.name + "/" + item.name + "_red");
				if (item.name.StartsWith("gold") && !ServiceManager.Instance.IsItemBought(item.id))
				{
					descriptionButtonExtraText.text = _goldSkinsDescription;
					descriptionButtonExtraText.gameObject.SetActive(true);
				}
			}
			resetEquipmentDescriptions();
			if (item.type == "equipment")
			{
				if (item.name.StartsWith("speed") || item.name.StartsWith("armour"))
				{
					displayEquipmentDescription(equipmentDescriptionMounts[1], LoadoutManager.Instance.CurrentLoadout.model, item);
				}
				else
				{
					displayEquipmentDescription(equipmentDescriptionMounts[0], LoadoutManager.Instance.CurrentLoadout.primary, item);
					displayEquipmentDescription(equipmentDescriptionMounts[1], LoadoutManager.Instance.CurrentLoadout.secondary, item);
					displayEquipmentDescription(equipmentDescriptionMounts[2], LoadoutManager.Instance.CurrentLoadout.melee, item);
				}
			}
			int num = 0;
			float num2 = 0f;
			if (item.HasBonusProperty("healthPercent"))
			{
				num2 = item.GetBonusProperty("healthPercent");
				if (num2 != 0f)
				{
					displayEquipmentModifiers(equipmentModifierMounts[num], _equipmentHealthModifier, num2);
					num++;
				}
			}
			if (item.HasBonusProperty("speedMultiplier"))
			{
				num2 = item.GetBonusProperty("speedMultiplier");
				if (num2 >= 1f || num2 <= -1f)
				{
					displayEquipmentModifiers(equipmentModifierMounts[num], _equipmentSpeedModifier, num2);
					num++;
				}
			}
			if (item.HasBonusProperty("cooldownMultiplier") && num < 2)
			{
				num2 = item.GetBonusProperty("cooldownMultiplier");
				if (num2 != 0f)
				{
					displayEquipmentModifiers(equipmentModifierMounts[num], _equipmentSpecialModifier, num2);
					num++;
				}
			}
			SetUpWeaponStatsDisplay(item);
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			SetUpEquipOrPurchaseButton(item, purchaseableByID);
			if (@object != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
				Transform parent = currentEquipButton.transform.Find("mountPoint");
				gameObject.transform.parent = parent;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			UnityEngine.Object object2 = Resources.Load("Icons/GearDescriptions/" + item.name);
			if (object2 != null)
			{
				if (mainMenu.currentCharacter != null)
				{
					mainMenu.currentCharacter.transform.localPosition = new Vector3(10000000f, 0f, 0f);
				}
				gearDescriptionPlate.SetActive(true);
				gearDescriptionPlate.GetComponent<Animation>().Play("in");
				currentDescriptionPlate = UnityEngine.Object.Instantiate(object2) as GameObject;
				currentDescriptionPlate.transform.parent = gearDescriptionPlate.transform.Find("gearDesc_mount");
				currentDescriptionPlate.transform.localEulerAngles = Vector3.zero;
				currentDescriptionPlate.transform.localPosition = Vector3.zero;
				(gearDescriptionPlate.transform.Find("weaponName").GetComponent(typeof(TextMesh)) as TextMesh).text = item.title;
			}
			if (this._descriptionPlateVisibilityChanged != null)
			{
				this._descriptionPlateVisibilityChanged(gearDescriptionPlate.activeSelf);
			}
		}
		currentlySelectedItem = item;
	}

	private void displayEquipmentModifiers(Transform mount, string title, float val)
	{
		mount.gameObject.SetActive(true);
		mount.Find("title").GetComponent<TextMesh>().text = title;
		if (val > 0f)
		{
			mount.Find("stat").GetComponent<Renderer>().material.color = Color.green;
			mount.Find("stat").GetComponent<TextMesh>().text = "+" + Mathf.RoundToInt(val) + "%";
		}
		else
		{
			mount.Find("stat").GetComponent<Renderer>().material.color = Color.red;
			mount.Find("stat").GetComponent<TextMesh>().text = Mathf.RoundToInt(val) + "%";
		}
	}

	private void displayEquipmentDescription(Transform mount, Item item, Item equipment)
	{
		UnityEngine.Object @object = null;
		@object = ((!(item.type == "character")) ? Resources.Load("Icons/Weapons/" + currentCharacter.characterData.name + "/" + item.name) : Resources.Load("Icons/Characters/" + item.name + "/" + LoadoutManager.Instance.CurrentLoadout.skin.name + "_red"));
		double num = 0.0;
		mount.gameObject.SetActive(true);
		if (item.properties.ContainsKey(equipment.name))
		{
			num = item.properties[equipment.name];
		}
		string text = string.Empty;
		if (num > 0.0)
		{
			text = "+";
		}
		else if (num < 0.0)
		{
			text = "-";
		}
		string text2 = string.Empty;
		if (equipment.name.StartsWith("ammo"))
		{
			text2 = "bullets";
		}
		else if (equipment.name.StartsWith("damage"))
		{
			text2 = "damage";
		}
		else if (equipment.name.StartsWith("explosion"))
		{
			text2 = "damage";
		}
		else if (equipment.name.StartsWith("melee"))
		{
			text2 = "damage";
		}
		else if (equipment.name.StartsWith("speed"))
		{
			text2 = "speed";
		}
		else if (equipment.name.StartsWith("armour"))
		{
			text2 = "health";
		}
		if (num == 0.0)
		{
			mount.Find("statDescription").GetComponent<TextMesh>().text = _unaffectedByEquipment;
		}
		else if (equipment.name.StartsWith("speed"))
		{
			mount.Find("statDescription").GetComponent<TextMesh>().text = text + num * 100.0 + "% " + text2;
		}
		else
		{
			mount.Find("statDescription").GetComponent<TextMesh>().text = text + " " + num + " " + text2;
		}
		if (@object != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
			gameObject.transform.parent = mount;
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			equipmentDescriptionIcons.Add(gameObject);
		}
	}

	private void loadLoadouts()
	{
		int num = 0;
		Dictionary<string, Item> allItemsByName = ServiceManager.Instance.GetAllItemsByName();
		foreach (Item value in allItemsByName.Values)
		{
			if (value.type.Equals(LoadoutManager.Instance.LoadoutPrefix.ToLower()))
			{
				num++;
			}
		}
		for (int i = 1; i <= num; i++)
		{
			if (LoadoutManager.Instance.CurrentLoadout.loadoutNumber == i)
			{
				Item loadoutItem = LoadoutManager.Instance.GetLoadoutItem(i);
				GUIButton gUIButton = addEntry(loadoutItem, true);
				gUIButton.OnButtonPressed();
				handleWeapon(loadoutItem);
			}
			else
			{
				addEntry(LoadoutManager.Instance.GetLoadoutItem(i), false);
			}
		}
	}

	private void loadTaunts()
	{
		if (currentCharacter.taunts.Count == 0)
		{
			availabilityText.SetActive(true);
			return;
		}
		for (int i = 0; i < currentCharacter.taunts.Count; i++)
		{
			Item item = currentCharacter.taunts[i];
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (!ServiceManager.Instance.IsItemBought(item.id) && !purchaseableByID.is_for_sale)
			{
				continue;
			}
			if (LoadoutManager.Instance.CurrentLoadout.taunt != null && item.name == LoadoutManager.Instance.CurrentLoadout.taunt.name && item.name != string.Empty)
			{
				GUIButton gUIButton = addEntry(item, true);
				gUIButton.OnButtonPressed();
				handleWeapon(item);
				if (item != null && mainMenu.CharacterAnimator[item.name] != null)
				{
					mainMenu.ToggleWeapons(false);
					mainMenu.CharacterAnimator.Stop();
					mainMenu.CharacterAnimator.Play(temporaryLoadout.taunt.name);
				}
			}
			else
			{
				addEntry(item, false);
			}
		}
	}

	private void loadCharacters()
	{
		if (Store.Instance.characters.Count == 0)
		{
			availabilityText.SetActive(true);
			handleWeapon(null);
			return;
		}
		foreach (KeyValuePair<string, Character> character in Store.Instance.characters)
		{
			Character value = character.Value;
			if (currentCharacter.characterData.name == value.characterData.name)
			{
				GUIButton gUIButton = addEntry(value.characterData, true);
				gUIButton.OnButtonPressed();
				handleWeapon(currentCharacter.characterData);
			}
			else
			{
				addEntry(value.characterData, false);
			}
		}
	}

	private void loadPrimaries()
	{
		if (currentCharacter.primaryWeapons.Count == 0)
		{
			availabilityText.SetActive(true);
			handleWeapon(null);
			return;
		}
		for (int i = 0; i < currentCharacter.primaryWeapons.Count; i++)
		{
			Item item = currentCharacter.primaryWeapons[i];
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (ServiceManager.Instance.IsItemBought(item.id) || purchaseableByID.is_for_sale)
			{
				if (item.name == LoadoutManager.Instance.CurrentLoadout.primary.name)
				{
					GUIButton gUIButton = addEntry(item, true);
					gUIButton.OnButtonPressed();
					handleWeapon(item);
					mainMenu.handleNewWeapon(LoadoutManager.Instance.CurrentLoadout, currentSubmenu);
				}
				else
				{
					addEntry(item, false);
				}
			}
		}
	}

	private void loadSecondaries()
	{
		if (currentCharacter.secondaryWeapons.Count == 0)
		{
			availabilityText.SetActive(true);
			handleWeapon(null);
			return;
		}
		for (int i = 0; i < currentCharacter.secondaryWeapons.Count; i++)
		{
			Item item = currentCharacter.secondaryWeapons[i];
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (ServiceManager.Instance.IsItemBought(item.id) || purchaseableByID.is_for_sale)
			{
				if (item.name == LoadoutManager.Instance.CurrentLoadout.secondary.name)
				{
					GUIButton gUIButton = addEntry(item, true);
					gUIButton.OnButtonPressed();
					handleWeapon(item);
					mainMenu.handleNewWeapon(LoadoutManager.Instance.CurrentLoadout, currentSubmenu);
				}
				else
				{
					addEntry(item, false);
				}
			}
		}
	}

	private void loadMelees()
	{
		if (currentCharacter.meleeWeapons.Count == 0)
		{
			availabilityText.SetActive(true);
			handleWeapon(null);
			return;
		}
		for (int i = 0; i < currentCharacter.meleeWeapons.Count; i++)
		{
			Item item = currentCharacter.meleeWeapons[i];
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (ServiceManager.Instance.IsItemBought(item.id) || purchaseableByID.is_for_sale)
			{
				if (item.name == LoadoutManager.Instance.CurrentLoadout.melee.name)
				{
					GUIButton gUIButton = addEntry(item, true);
					gUIButton.OnButtonPressed();
					handleWeapon(item);
					mainMenu.handleNewWeapon(LoadoutManager.Instance.CurrentLoadout, currentSubmenu);
				}
				else
				{
					addEntry(item, false);
				}
			}
		}
	}

	private void loadSpecials()
	{
		if (Store.Instance.specials.Count == 0)
		{
			availabilityText.SetActive(true);
			handleWeapon(null);
			return;
		}
		for (int i = 0; i < Store.Instance.specials.Count; i++)
		{
			Item item = Store.Instance.specials[i];
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (ServiceManager.Instance.IsItemBought(item.id) || purchaseableByID.is_for_sale)
			{
				if (LoadoutManager.Instance.CurrentLoadout.special != null && Store.Instance.specials[i].name == LoadoutManager.Instance.CurrentLoadout.special.name)
				{
					GUIButton gUIButton = addEntry(Store.Instance.specials[i], true);
					gUIButton.OnButtonPressed();
					handleWeapon(LoadoutManager.Instance.CurrentLoadout.special);
				}
				else
				{
					addEntry(Store.Instance.specials[i], false);
				}
			}
		}
	}

	private void loadEquipment1()
	{
		if (Store.Instance.equipment.Count == 0)
		{
			availabilityText.SetActive(true);
			handleWeapon(null);
			return;
		}
		for (int i = 0; i < Store.Instance.equipment.Count; i++)
		{
			Item item = Store.Instance.equipment[i];
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (ServiceManager.Instance.IsItemBought(item.id) || purchaseableByID.is_for_sale)
			{
				if (LoadoutManager.Instance.CurrentLoadout.equipment1 != null && Store.Instance.equipment[i].name == LoadoutManager.Instance.CurrentLoadout.equipment1.name)
				{
					GUIButton gUIButton = addEntry(Store.Instance.equipment[i], true);
					gUIButton.OnButtonPressed();
					handleWeapon(LoadoutManager.Instance.CurrentLoadout.equipment1);
				}
				else if (LoadoutManager.Instance.CurrentLoadout.equipment2 == null || !(LoadoutManager.Instance.CurrentLoadout.equipment2.name == Store.Instance.equipment[i].name))
				{
					addEntry(Store.Instance.equipment[i], false);
				}
			}
		}
	}

	private void loadEquipment2()
	{
		if (Store.Instance.equipment.Count == 0)
		{
			availabilityText.SetActive(true);
			handleWeapon(null);
			return;
		}
		for (int i = 0; i < Store.Instance.equipment.Count; i++)
		{
			Item item = Store.Instance.equipment[i];
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (ServiceManager.Instance.IsItemBought(item.id) || purchaseableByID.is_for_sale)
			{
				if (LoadoutManager.Instance.CurrentLoadout.equipment2 != null && Store.Instance.equipment[i].name == LoadoutManager.Instance.CurrentLoadout.equipment2.name)
				{
					GUIButton gUIButton = addEntry(Store.Instance.equipment[i], true);
					gUIButton.OnButtonPressed();
					handleWeapon(LoadoutManager.Instance.CurrentLoadout.equipment2);
				}
				else if (LoadoutManager.Instance.CurrentLoadout.equipment1 == null || !(LoadoutManager.Instance.CurrentLoadout.equipment1.name == Store.Instance.equipment[i].name))
				{
					addEntry(Store.Instance.equipment[i], false);
				}
			}
		}
	}

	private void loadSkins()
	{
		if (currentCharacter.skins.Count == 0)
		{
			availabilityText.SetActive(true);
			return;
		}
		for (int i = 0; i < currentCharacter.skins.Count; i++)
		{
			Item item = currentCharacter.skins[i];
			bool flag = item.name.StartsWith("gold");
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (ServiceManager.Instance.IsItemBought(item.id) || purchaseableByID.is_for_sale)
			{
				if (item.name == LoadoutManager.Instance.CurrentLoadout.skin.name)
				{
					GUIButton gUIButton = ((!flag) ? addEntry(item, true) : addGoldSkinEntry(item, true));
					gUIButton.OnButtonPressed();
					handleWeapon(item);
				}
				else if (flag)
				{
					addGoldSkinEntry(item, false);
				}
				else
				{
					addEntry(item, false);
				}
			}
		}
	}

	private void loadProModeOptions()
	{
		if (Store.Instance.proMode.Count == 0)
		{
			availabilityText.SetActive(true);
			handleWeapon(null);
			return;
		}
		for (int i = 0; i < Store.Instance.proMode.Count; i++)
		{
			Item item = Store.Instance.proMode[i];
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(item.id);
			if (ServiceManager.Instance.IsItemBought(item.id) || purchaseableByID.is_for_sale)
			{
				switch (item.name)
				{
				case "jump":
				case "radar":
					addEntry(item, ServiceManager.Instance.IsItemBought(item.id));
					break;
				default:
					addEntry(item, false);
					break;
				}
			}
		}
	}
}
