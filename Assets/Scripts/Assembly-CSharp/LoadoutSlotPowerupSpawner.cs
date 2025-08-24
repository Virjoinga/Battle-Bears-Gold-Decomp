using System.Collections.Generic;
using UnityEngine;

public class LoadoutSlotPowerupSpawner : PowerupSpawner
{
	private static readonly Dictionary<Item.Types, float> _chancesBySpawnableType = new Dictionary<Item.Types, float>
	{
		{
			Item.Types.primary,
			0.4f
		},
		{
			Item.Types.secondary,
			0.4f
		},
		{
			Item.Types.special,
			0.2f
		}
	};

	public Item.Types ItemType { get; private set; }

	private Item.Types GetItemType()
	{
		float value = Random.value;
		float num = 0f;
		foreach (KeyValuePair<Item.Types, float> item in _chancesBySpawnableType)
		{
			num += item.Value;
			if (value <= num)
			{
				return item.Key;
			}
		}
		return _chancesBySpawnableType.Keys.Random();
	}

	private void Awake()
	{
		ItemType = GetItemType();
	}

	public override void OnSpawn()
	{
		base.OnSpawn();
		currentlySpawnedPowerup.GetComponent<LoadoutSlotPowerup>().Type = ItemType;
	}

	public void RemoteSpawn(Item.Types type)
	{
		ItemType = type;
		OnSpawn();
	}
}
