using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using JsonFx.Json;
using UnityEngine;

public class LoadoutSlotPowerupManager : PowerupManager
{
	private const string LOADOUT_POWERUP_NAME = "LoadoutSlotPowerup";

	private const string TOTAL_POWERUPS_SETTING = "total_royale_powerups";

	private const string BANNED_ITEMS_SETTINGS_PREFIX = "loadout_powerup_banned_items_";

	[SerializeField]
	private Transform[] _powerupGroupParents;

	private int _totalPowerupsToSpawn;

	private int _powerupsPerParent;

	private int _remainderPowerups;

	private void Start()
	{
		Item itemByName = ServiceManager.Instance.GetItemByName("LoadoutSlotPowerup");
		float bonusProperty = itemByName.GetBonusProperty("reloadTime");
		PowerupSpawner[] array = powerupSpawners;
		foreach (PowerupSpawner powerupSpawner in array)
		{
			powerupSpawner.respawnTime = bonusProperty;
		}
		LoadBannedItemsFromServer();
	}

	private void LoadBannedItemsFromServer()
	{
		List<string> list = new List<string>();
		int i = 0;
		for (string val = string.Empty; ServiceManager.Instance.UpdateProperty("loadout_powerup_banned_items_" + i, ref val); i++)
		{
			list.AddRange(DeserializeBannedItems(val));
		}
		LoadoutSlotPowerup.BannedItemNames = list;
	}

	private void LoadPowerupCountFromServer()
	{
		_totalPowerupsToSpawn = _powerupGroupParents.Length;
		ServiceManager.Instance.UpdateProperty("total_royale_powerups", ref _totalPowerupsToSpawn);
		_powerupsPerParent = Mathf.Max(1, _totalPowerupsToSpawn / _powerupGroupParents.Length);
		_remainderPowerups = _totalPowerupsToSpawn % _powerupGroupParents.Length;
	}

	private List<string> DeserializeBannedItems(string bannedItemsJson)
	{
		LoadoutSlotPowerup.BannedItems bannedItems = JsonReader.Deserialize<LoadoutSlotPowerup.BannedItems>(bannedItemsJson);
		return bannedItems.BannedItemNames;
	}

	public override void OnInitialSpawn()
	{
		if (PhotonNetwork.isMasterClient)
		{
			LoadPowerupCountFromServer();
			DecidePowerupSpawning();
		}
	}

	private void DecidePowerupSpawning()
	{
		Hashtable hashtable = new Hashtable();
		List<byte> list = new List<byte>();
		for (int i = 0; i < _powerupGroupParents.Length; i++)
		{
			if (_totalPowerupsToSpawn <= 0)
			{
				break;
			}
			int numToSpawnForParent = GetNumToSpawnForParent();
			list.AddRange(SpawnOptions(_powerupGroupParents[i], numToSpawnForParent));
			_totalPowerupsToSpawn -= numToSpawnForParent;
		}
		hashtable[(byte)0] = list.ToArray();
		GameManager.Instance.LocalPlayerCharacterManager().PlayerController.NetSync.SetAction(63, hashtable);
	}

	private int GetNumToSpawnForParent()
	{
		int num = _powerupsPerParent;
		if (_remainderPowerups > 0)
		{
			num++;
		}
		_remainderPowerups--;
		return num;
	}

	private byte[] SpawnOptions(Transform parent, int numOptions)
	{
		List<int> list = new List<int>(Enumerable.Range(0, parent.childCount));
		byte[] array = new byte[numOptions * 2];
		for (int i = 0; i < array.Length; i += 2)
		{
			if (list.Count <= 0)
			{
				break;
			}
			int num = list[Random.Range(0, list.Count)];
			list.Remove(num);
			LoadoutSlotPowerupSpawner component = parent.GetChild(num).GetComponent<LoadoutSlotPowerupSpawner>();
			component.OnSpawn();
			array[i] = (byte)num;
			array[i + 1] = (byte)component.ItemType;
		}
		return array;
	}

	public void RemoteInitialSpawn(byte[] spawnInfo)
	{
		LoadPowerupCountFromServer();
		int num = 0;
		for (int i = 0; i < _powerupGroupParents.Length; i++)
		{
			if (num + 1 >= spawnInfo.Length)
			{
				break;
			}
			Transform transform = _powerupGroupParents[i];
			int numToSpawnForParent = GetNumToSpawnForParent();
			for (int j = 0; j < numToSpawnForParent; j++)
			{
				if (num + 1 >= spawnInfo.Length)
				{
					break;
				}
				int index = spawnInfo[num];
				num++;
				int type = spawnInfo[num];
				num++;
				LoadoutSlotPowerupSpawner component = transform.GetChild(index).GetComponent<LoadoutSlotPowerupSpawner>();
				component.RemoteSpawn((Item.Types)type);
			}
		}
	}
}
