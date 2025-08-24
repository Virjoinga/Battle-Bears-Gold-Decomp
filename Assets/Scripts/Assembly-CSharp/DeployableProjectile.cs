using ExitGames.Client.Photon;
using UnityEngine;

public class DeployableProjectile : DeployableObject
{
	public DeployableObject deployableToSpawn;

	public bool destroyedByTeslaShield = true;

	public bool deflectedByIronCurtain = true;

	public bool explodeOnPlayer = true;

	public bool explodeOnMGSBox = true;

	protected bool _isBeingDeployed;

	protected bool _isBeingDestroyed;

	private Collision _collisionInfoForDeployment;

	private new void Start()
	{
		if (explodeOnPlayer)
		{
			return;
		}
		foreach (PlayerCharacterManager playerCharacterManager in GameManager.Instance.GetPlayerCharacterManagers())
		{
			if (base.GetComponent<Collider>() != null && playerCharacterManager != null && playerCharacterManager.PlayerController != null && playerCharacterManager.PlayerController.GetComponent<Collider>() != null && base.GetComponent<Collider>().enabled && playerCharacterManager.PlayerController.GetComponent<Collider>().enabled)
			{
				Physics.IgnoreCollision(base.GetComponent<Collider>(), playerCharacterManager.PlayerController.GetComponent<Collider>());
			}
		}
	}

	protected void OnCollisionEnter(Collision c)
	{
		if (!_isBeingDeployed && !_isBeingDestroyed && !HitIgnoredPlayerOrMGSBox(c.transform, c.collider))
		{
			_isBeingDeployed = true;
			_collisionInfoForDeployment = c;
			Physics.IgnoreCollision(base.GetComponent<Collider>(), c.collider);
		}
	}

	protected bool HitIgnoredPlayerOrMGSBox(Transform transform, Collider otherCollider)
	{
		if (!explodeOnPlayer)
		{
			PlayerController componentInChildren = transform.root.GetComponentInChildren<PlayerController>();
			if (componentInChildren != null)
			{
				Physics.IgnoreCollision(base.GetComponent<Collider>(), otherCollider);
				return true;
			}
		}
		if (!explodeOnMGSBox)
		{
			DamageReceiverProxy componentInChildren2 = transform.root.GetComponentInChildren<DamageReceiverProxy>();
			if (componentInChildren2 != null)
			{
				Physics.IgnoreCollision(base.GetComponent<Collider>(), otherCollider);
				return true;
			}
		}
		return false;
	}

	protected void OnTriggerEnter(Collider other)
	{
		TryDestroyByShieldOrCurtain(other);
	}

	protected bool TryDestroyByShieldOrCurtain(Collider other)
	{
		TeslaShield componentInChildren = other.GetComponentInChildren<TeslaShield>();
		if (componentInChildren != null && !componentInChildren.PlayerOnOwnersTeam(base.OwnerID) && destroyedByTeslaShield)
		{
			_isBeingDestroyed = true;
			OnDestroyDeployable();
			return true;
		}
		IronCurtain componentInChildren2 = other.GetComponentInChildren<IronCurtain>();
		if (componentInChildren2 != null && deflectedByIronCurtain)
		{
			_isBeingDestroyed = true;
			OnDestroyDeployable();
			return true;
		}
		return false;
	}

	protected virtual void LateUpdate()
	{
		if (!_isBeingDestroyed && _isBeingDeployed)
		{
			if (_collisionInfoForDeployment != null)
			{
				DeployOrientedToNormal(_collisionInfoForDeployment);
			}
			else
			{
				Debug.LogError("Deployable queued for deployment with no saved collision info");
			}
		}
	}

	public void DeployOrientedToNormal(Collision collisionInfo)
	{
		if (_isBeingDestroyed)
		{
			return;
		}
		float raycastDistance = GetRaycastDistance();
		Ray ray = GetRay(collisionInfo, raycastDistance);
		RaycastHit[] array = Physics.RaycastAll(ray, raycastDistance, 1 << collisionInfo.gameObject.layer);
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (raycastHit.collider == collisionInfo.collider)
			{
				DeployOrientedToNormal(collisionInfo.transform, raycastHit.normal, raycastHit.point);
				return;
			}
		}
		DeployOrientedToNormal(collisionInfo.transform, collisionInfo.contacts[0].normal, collisionInfo.contacts[0].point);
	}

	protected virtual Ray GetRay(Collision collisionInfo, float distance)
	{
		Vector3 point = collisionInfo.contacts[0].point;
		Vector3 vector = point + distance * collisionInfo.contacts[0].normal;
		return new Ray(vector, point - vector);
	}

	protected virtual float GetRaycastDistance()
	{
		SphereCollider component = GetComponent<SphereCollider>();
		return (!(component != null)) ? 40f : (component.radius * 4f);
	}

	public void DeployOrientedToNormal(RaycastHit raycastInfo)
	{
		if (!_isBeingDestroyed)
		{
			DeployOrientedToNormal(raycastInfo.transform, raycastInfo.normal, raycastInfo.point);
		}
	}

	private void DeployOrientedToNormal(Transform objectHit, Vector3 surfaceNormal, Vector3 collisionPoint)
	{
		if (_isBeingDestroyed)
		{
			return;
		}
		if (deployableToSpawn != null && !hasSpawned)
		{
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
			GameObject gameObject = Object.Instantiate(deployableToSpawn.gameObject, collisionPoint, rotation) as GameObject;
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.OwnerID = base.OwnerID;
			component.DamageMultiplier = base.DamageMultiplier;
			component.SetItemOverride(spawnItemOverride);
			component.SetEquipmentNames(equipmentNames);
			hasSpawned = true;
			DeployableObject component2 = gameObject.GetComponent<DeployableObject>();
			component2.OwnerID = base.OwnerID;
			component2.DamageMultiplier = base.DamageMultiplier;
			component2.weaponIndex = weaponIndex;
			component2.OwningPlayer = OwningPlayer;
			component2.deployableIndex = deployableIndex;
			ConstantDamageSource component3 = gameObject.GetComponent<ConstantDamageSource>();
			if (component3 != null)
			{
				component3.DamageMultiplier = base.DamageMultiplier;
				component3.OwnerID = base.OwnerID;
				component3.SetEquipmentNames(equipmentNames);
				component3.SetItemOverride(spawnItemOverride);
			}
			PlayerController componentInChildren = objectHit.root.GetComponentInChildren<PlayerController>();
			if (componentInChildren != null && explodeOnPlayer)
			{
				component2.OnDetonateDeployable(componentInChildren, false);
			}
			else
			{
				DamageReceiverProxy componentInChildren2 = objectHit.root.GetComponentInChildren<DamageReceiverProxy>();
				if (componentInChildren2 != null && explodeOnMGSBox)
				{
					component2.OnDetonateDeployable(GameManager.Instance.Players(componentInChildren2.OwnerID).PlayerController, false);
				}
				else
				{
					FollowTransform followTransform = gameObject.AddComponent<FollowTransform>();
					followTransform.FollowTransformAtStartPoint(objectHit, collisionPoint);
				}
			}
		}
		Object.Destroy(base.gameObject);
	}

	public override void OnDetonateDeployable(PlayerController triggeringPlayer, bool fromExplosion)
	{
		if (OwningPlayer != null && triggeringPlayer.NetSync != null && weaponIndex != -1)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)0] = OwningPlayer.OwnerID;
			hashtable[(byte)1] = weaponIndex;
			hashtable[(byte)2] = deployableIndex;
			triggeringPlayer.NetSync.SetAction(38, hashtable);
		}
		OnDestroyDeployable();
	}

	public override void OnDestroyDeployable()
	{
		if (objectToSpawn != null)
		{
			Object.Instantiate(objectToSpawn, base.transform.position, base.transform.rotation);
		}
		if (OwningPlayer.WeaponManager.CurrentWeaponIndex == weaponIndex && !OwningPlayer.WeaponManager.IsReloading)
		{
			OwningPlayer.WeaponManager.OnDelayedIncreaseAmmo(OwningPlayer.WeaponManager.CurrentWeapon.reloadTime);
		}
		Object.Destroy(base.gameObject);
	}
}
