using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class Mine : DeployableObject
{
	public float primingTime = 1f;

	protected virtual void Awake()
	{
		if (primingTime > 0f)
		{
			base.GetComponent<Collider>().enabled = false;
			StartCoroutine(PrimingCoRoutine(primingTime));
		}
	}

	protected new virtual void Start()
	{
		ConfigureObject();
	}

	protected IEnumerator PrimingCoRoutine(float time)
	{
		if (time > 0f)
		{
			yield return new WaitForSeconds(primingTime);
		}
		base.GetComponent<Collider>().enabled = true;
	}

	public override void ConfigureObject()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("primingTime", ref primingTime, equipmentNames);
			itemByName.UpdateProperty("duration", ref effectDuration, equipmentNames);
		}
	}

	protected virtual void OnTriggerEnter(Collider c)
	{
		checkMineCollision(c.gameObject);
	}

	protected virtual void OnCollisionEnter(Collision c)
	{
		checkMineCollision(c.gameObject);
	}

	private void checkMineCollision(GameObject target)
	{
		PlayerController component = target.GetComponent<PlayerController>();
		TeslaShield component2 = target.GetComponent<TeslaShield>();
		if (component != null && !component.isRemote && OwningPlayer != null && component.Team != OwningPlayer.Team)
		{
			triggerMine(component);
		}
		else if (component2 != null && !component2.PlayerOnOwnersTeam(base.OwnerID))
		{
			triggerMine(component2.PlayerController);
		}
		else
		{
			if (target.layer != LayerMask.NameToLayer("Explosion"))
			{
				return;
			}
			NetworkObject componentInChildren = target.GetComponentInChildren<NetworkObject>();
			if (componentInChildren != null && GameManager.Instance != null)
			{
				PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(componentInChildren.OwnerID);
				if (playerCharacterManager != null && OwningPlayer != null && playerCharacterManager.team != OwningPlayer.Team)
				{
					OnDetonateDeployable(playerCharacterManager.PlayerController, true);
				}
			}
		}
	}

	public override void OnDetonateDeployable(PlayerController triggeringPlayer, bool fromExplosion)
	{
		if (OwningPlayer != null && triggeringPlayer.NetSync != null && weaponIndex != -1)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = OwningPlayer.OwnerID;
			hashtable[(byte)1] = weaponIndex;
			hashtable[(byte)2] = deployableIndex;
			triggeringPlayer.NetSync.SetAction(38, hashtable);
		}
		OnDestroyDeployable();
	}

	public virtual void triggerMine(PlayerController triggeringPlayer)
	{
		OnDetonateDeployable(triggeringPlayer, false);
	}

	public override void OnDestroyDeployable()
	{
		if (objectToSpawn != null && !hasSpawned)
		{
			GameObject gameObject = Object.Instantiate(objectToSpawn, base.transform.position, base.transform.rotation) as GameObject;
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			if (component != null)
			{
				component.OwnerID = base.OwnerID;
				component.DamageMultiplier = base.DamageMultiplier;
				component.SetItemOverride(spawnItemOverride);
				component.SetEquipmentNames(equipmentNames);
			}
			if (OwningPlayer.WeaponManager.CurrentWeaponIndex == weaponIndex)
			{
				int num = 1;
				WeaponBase currentWeapon = OwningPlayer.WeaponManager.CurrentWeapon;
				OwningPlayer.WeaponManager.OnDelayedIncreaseAmmo(currentWeapon.reloadTime * (float)num / (float)currentWeapon.clipSize, num);
			}
			hasSpawned = true;
		}
		Object.Destroy(base.gameObject);
	}
}
