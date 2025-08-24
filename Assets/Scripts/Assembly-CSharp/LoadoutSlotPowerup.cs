using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;

public class LoadoutSlotPowerup : Powerup
{
	[Serializable]
	public class BannedItems
	{
		public List<string> BannedItemNames;
	}

	public static List<string> BannedItemNames = new List<string>
	{
		"snowball_kingSlayer", "LastResort", "Satellite", "whipped_lactose", "lactose", "TurretHeld", "CloakField", "FairysDust", "SmokeScreen", "peekaBoo",
		"Rake", "dreamCatcher", "IronCurtain", "TeslaShield", "SixthSenseGoggles", "Propbearganda", "Camouflage", "MGSBox"
	};

	[SerializeField]
	private Renderer _renderer;

	private bool _pickingUp;

	private Item.Types _type;

	public Item.Types Type
	{
		set
		{
			_type = value;
			UpdateDisplay();
		}
	}

	private void UpdateDisplay()
	{
		switch (_type)
		{
		case Item.Types.special:
			_renderer.material.color = Color.yellow;
			break;
		case Item.Types.primary:
			_renderer.material.color = Color.blue;
			break;
		case Item.Types.secondary:
			_renderer.material.color = Color.green;
			break;
		default:
			throw new Exception("No display defined for loadout slot " + _type);
		}
	}

	protected override void OnPickup(GameObject obj, PlayerController p)
	{
		if (!_pickingUp)
		{
			_pickingUp = true;
			PlayerLoadout currentLoadout = LoadoutManager.Instance.CurrentLoadout;
			IList<Item> l = AllowedItemsForType(currentLoadout);
			switch (_type)
			{
			case Item.Types.primary:
				currentLoadout.primary = l.Random();
				break;
			case Item.Types.secondary:
				currentLoadout.secondary = l.Random();
				break;
			case Item.Types.special:
				currentLoadout.special = l.Random();
				break;
			default:
				throw new Exception("No pickup behaviour defined for " + _type);
			}
			StartCoroutine(ChangeLoadoutRoutine(currentLoadout, p));
		}
	}

	private IList<Item> AllowedItemsForType(PlayerLoadout loadout)
	{
		IList<Item> list = ((_type != Item.Types.primary && _type != Item.Types.secondary) ? ServiceManager.Instance.GetItemsForType(_type.ToString()) : ServiceManager.Instance.GetItemsForCharacterAndType(loadout.model.name, _type.ToString()));
		Item equippedItem;
		switch (_type)
		{
		case Item.Types.special:
			equippedItem = loadout.special;
			break;
		case Item.Types.primary:
			equippedItem = loadout.primary;
			break;
		case Item.Types.secondary:
			equippedItem = loadout.secondary;
			break;
		default:
			throw new Exception("No pickup behaviour defined for " + _type);
		}
		if (equippedItem != null)
		{
			list = list.Where((Item i) => !BannedItemNames.Contains(i.name) && i.name != equippedItem.name).ToList();
		}
		else
		{
			(list as List<Item>).RemoveAll((Item i) => BannedItemNames.Contains(i.name));
		}
		return list;
	}

	private IEnumerator ChangeLoadoutRoutine(PlayerLoadout loadout, PlayerController pc)
	{
		yield return new WaitForEndOfFrame();
		PlayerCharacterManager pcm = GameManager.Instance.LocalPlayerCharacterManager();
		switch (_type)
		{
		case Item.Types.primary:
			pcm.SetPrimary(loadout.primary);
			break;
		case Item.Types.secondary:
			pcm.SetSecondary(loadout.secondary);
			break;
		case Item.Types.special:
			pcm.SetSpecial(loadout.special);
			break;
		default:
			throw new Exception("No pickup behaviour defined for " + _type);
		}
		if (PowerupManager.Instance != null)
		{
			string[] splitString = base.name.Split(' ');
			int powerupIndex = int.Parse(splitString[1]);
			ReportPickupToRemotePlayers(pc, powerupIndex);
			PowerupManager.Instance.OnUsePowerup(powerupIndex, 0);
		}
	}

	private void ReportPickupToRemotePlayers(PlayerController pc, int powerupIndex)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = powerupIndex;
		pc.NetSync.SetAction(31, hashtable);
	}

	protected override void Configure(Item item)
	{
	}
}
