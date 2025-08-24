using UnityEngine;

public abstract class DeployableObject : ConfigurableNetworkObject
{
	public GameObject objectToSpawn;

	protected bool hasSpawned;

	public string spawnItemOverride = string.Empty;

	public int weaponIndex = -1;

	public int deployableIndex = -1;

	public float effectDuration = 1f;

	public virtual PlayerController OwningPlayer { get; set; }

	protected string EquipmentNames
	{
		get
		{
			return equipmentNames;
		}
	}

	public virtual void ConfigureObject()
	{
	}

	public abstract void OnDetonateDeployable(PlayerController triggeringPlayer, bool fromExplosion);

	public abstract void OnDestroyDeployable();
}
