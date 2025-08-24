using System.Collections.Generic;
using UnityEngine;

public class AnimatedLoadoutScroller : AnimatedScroller
{
	[SerializeField]
	private GameObject _loadoutContainerPrefab;

	[SerializeField]
	private GameObject _lockedContainerPrefab;

	private static readonly float _containerBaseScale = 1f;

	private static readonly float _containerSelectedScale = 1.2f;

	private static readonly int _slotDistanceDiff = -100;

	private static readonly float _zOffset = -5f;

	private static readonly float _iconScale = 0.65f;

	private static readonly string _slotPrefix = "slot";

	private static readonly string _emptyButtonName = "emptyButton";

	private static readonly string _blueTeamString = "_blue";

	private static readonly string _redTeamString = "_red";

	private static readonly string _characterMountName = "CharacterMount";

	private static readonly string _primaryMountName = "PrimaryMount";

	private static readonly string _secondaryMountName = "SecondaryMount";

	private static readonly string _equipmentOneMountName = "EquipmentOneMount";

	private static readonly string _equipmentTwoMountName = "EquipmentTwoMount";

	private static readonly string _characterPath = "Icons/Characters/";

	private static readonly string _weaponsPath = "Icons/Weapons/";

	private static readonly string _equipmentPath = "Icons/Equipment/";

	private static readonly string _iconLayerName = "HUD";

	private static readonly string _upArrowName = "upArrow";

	private static readonly string _downArrowName = "downArrow";

	public int SelectedLoadout { get; private set; }

	public override void Awake()
	{
		base.Awake();
		SelectedLoadout = LoadoutManager.Instance.CurrentLoadout.loadoutNumber;
		if (_loadoutContainerPrefab == null)
		{
			Debug.LogWarning("Could not create loadout scroller, container prefab was null!");
			return;
		}
		Dictionary<string, Item> allItemsByName = ServiceManager.Instance.GetAllItemsByName();
		foreach (Item value in allItemsByName.Values)
		{
			if (value.type.Equals(LoadoutManager.Instance.LoadoutPrefix.ToLower()))
			{
				if (ServiceManager.Instance.IsItemBought(value.id))
				{
					AddLoadoutButton(value);
				}
				else
				{
					AddLockedButton();
				}
			}
		}
		currentIndex = SelectedLoadout - 1;
		MoveEverySlotByValueAndScale(0f);
	}

	private void AddLoadoutButton(Item item)
	{
		AddEmptySlot();
		GameObject gameObject = Object.Instantiate(_loadoutContainerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		int loadoutNumber = int.Parse(item.name.Replace(LoadoutManager.Instance.LoadoutPrefix, string.Empty));
		AddLoadoutIconsToContainer(gameObject, loadoutNumber);
		GUIButton component = gameObject.GetComponent<GUIButton>();
		if (component != null)
		{
			addButton(component);
		}
	}

	private void AddLockedButton()
	{
		AddEmptySlot();
		GameObject gameObject = Object.Instantiate(_lockedContainerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		GUIButton component = gameObject.GetComponent<GUIButton>();
		if (component != null)
		{
			addButton(component);
		}
	}

	private void AddEmptySlot()
	{
		GameObject gameObject = new GameObject(_slotPrefix + slots.Count);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = new Vector3(_slotDistanceDiff * slots.Count, 0f, _zOffset);
		gameObject.transform.localRotation = Quaternion.identity;
		slots.Add(gameObject.transform);
	}

	private void AddEmptyButton()
	{
		AddEmptySlot();
	}

	private void AddLoadoutIconsToContainer(GameObject container, int loadoutNumber)
	{
		Transform parent = container.transform.FindChild(_characterMountName);
		Transform parent2 = container.transform.FindChild(_primaryMountName);
		Transform parent3 = container.transform.FindChild(_secondaryMountName);
		Transform parent4 = container.transform.FindChild(_equipmentOneMountName);
		Transform parent5 = container.transform.FindChild(_equipmentTwoMountName);
		PlayerLoadout playerLoadout = LoadoutManager.Instance.GetLoadoutByNumber(loadoutNumber, string.Empty);
		if (playerLoadout == null)
		{
			playerLoadout = LoadoutManager.Instance.CreateDefaultLoadout(loadoutNumber, string.Empty);
			LoadoutManager.Instance.SaveLoadout(playerLoadout.pid);
		}
		string text = _blueTeamString;
		if (HUD.Instance != null && HUD.Instance.PlayerController != null && HUD.Instance.PlayerController.Team != Team.BLUE)
		{
			text = _redTeamString;
		}
		InitIcon(_characterPath + playerLoadout.model.name + "/" + playerLoadout.skin.name + text, parent);
		InitIcon(_weaponsPath + playerLoadout.model.name + "/" + playerLoadout.primary.name, parent2);
		InitIcon(_weaponsPath + playerLoadout.model.name + "/" + playerLoadout.secondary.name, parent3);
		if (playerLoadout.equipment1 != null)
		{
			InitIcon(_equipmentPath + playerLoadout.equipment1.name, parent4);
		}
		if (playerLoadout.equipment2 != null)
		{
			InitIcon(_equipmentPath + playerLoadout.equipment2.name, parent5);
		}
		TextMesh componentInChildren = container.GetComponentInChildren<TextMesh>();
		if (componentInChildren != null)
		{
			componentInChildren.text = LoadoutManager.Instance.LoadoutPrefix + " " + loadoutNumber;
		}
	}

	private void InitIcon(string resourcePath, Transform parent)
	{
		Object @object = Resources.Load(resourcePath);
		if (@object != null)
		{
			GameObject gameObject = Object.Instantiate(@object) as GameObject;
			gameObject.layer = LayerMask.NameToLayer(_iconLayerName);
			ParentToTransformAndScale(gameObject, parent, _iconScale);
		}
	}

	private void ParentToTransformAndScale(GameObject icon, Transform parent, float scale)
	{
		if (icon != null && parent != null)
		{
			icon.transform.parent = parent;
			icon.transform.localPosition = Vector3.zero;
			icon.transform.localRotation = Quaternion.identity;
			icon.transform.localScale = new Vector3(scale, scale, scale);
		}
	}

	public override void Update()
	{
		if (slots[currentIndex].transform.position.x != base.transform.position.x)
		{
			float num = Mathf.Lerp(slots[currentIndex].transform.position.x, base.transform.position.x, Time.deltaTime * scrollSpeed);
			float deltaX = num - slots[currentIndex].transform.position.x;
			MoveEverySlotByValueAndScale(deltaX);
		}
	}

	public override void OnGUIButtonClicked(GUIButton b)
	{
		if (b.name == _upArrowName)
		{
			if (currentIndex > 0)
			{
				currentIndex--;
			}
		}
		else if (b.name == _downArrowName)
		{
			if (currentIndex < buttonList.Count - 1)
			{
				currentIndex++;
			}
		}
		else
		{
			int num = -1;
			for (int i = 0; i < buttonList.Count; i++)
			{
				if (b == buttonList[i])
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				currentIndex = num;
			}
		}
		SelectedLoadout = currentIndex + 1;
	}

	private void MoveEverySlotByValueAndScale(float deltaX)
	{
		foreach (Transform slot in slots)
		{
			Vector3 position = slot.position;
			position.x += deltaX;
			slot.position = position;
			float num = _containerBaseScale + Mathf.Clamp(1f - Mathf.Abs((position.x - base.transform.position.x) / (float)_slotDistanceDiff), 0f, 1f) * (_containerSelectedScale - _containerBaseScale);
			slot.localScale = new Vector3(num, num, num);
		}
	}
}
