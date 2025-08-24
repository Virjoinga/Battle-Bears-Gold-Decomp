using UnityEngine;

public class DeployableLauncherWeapon : WeaponBase, IDeployableWeapon
{
	public DeployableProjectile projectile;

	public float projectileSpeed = 10f;

	public Transform[] spawnPoints;

	public Transform spawnRoot;

	protected CharacterController charController;

	protected bool _isAttacking;

	public bool IsAttacking
	{
		get
		{
			return _isAttacking;
		}
	}

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("projectileSpeed", ref projectileSpeed, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Start()
	{
		base.Start();
		charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		base.playerController.WeaponManager.OnCheckForWeaponHiding();
		TryRefillMissingAmmo();
	}

	private void TryRefillMissingAmmo()
	{
		int currentWeaponIndex = base.playerController.WeaponManager.CurrentWeaponIndex;
		int currentClipSize = base.playerController.WeaponManager.GetCurrentClipSize(currentWeaponIndex);
		if (!(reloadTime > 0f) || currentClipSize > clipSize)
		{
			return;
		}
		int num = 0;
		DeployableObject[] array = Object.FindObjectsOfType(typeof(DeployableObject)) as DeployableObject[];
		DeployableObject[] array2 = array;
		foreach (DeployableObject deployableObject in array2)
		{
			if (deployableObject == null)
			{
				Debug.LogError("deployable is null, DeplyablePlacementWeapon:36");
			}
			else if (deployableObject.OwningPlayer != null && deployableObject.OwningPlayer.OwnerID == base.OwnerID && deployableObject.weaponIndex == currentWeaponIndex)
			{
				num++;
			}
		}
		if (currentClipSize + num < clipSize)
		{
			int num2 = clipSize - (currentClipSize + num);
			base.playerController.WeaponManager.OnDelayedIncreaseAmmo(reloadTime * (float)(num2 / clipSize), num2);
		}
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (base.playerController.WeaponManager.IsReloading)
		{
			return false;
		}
		_isAttacking = true;
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (spawnPoints.Length == 1 && !isRiggedWeapon)
		{
			GameObject gameObject = SpawnProjectile(spawnPoints[0].position, aimer.forward * projectileSpeed);
			DelayedGravityProjectile component = gameObject.GetComponent<DelayedGravityProjectile>();
			if (component != null)
			{
				component.ownerID = base.OwnerID;
			}
			if (base.NetSyncReporter != null && !dontSendNetworkMessages)
			{
				base.NetSyncReporter.SpawnProjectile(gameObject.transform.position, gameObject.GetComponent<Rigidbody>().velocity);
			}
			gameObject.SendMessage("OnNetworkDelay", 0f, SendMessageOptions.DontRequireReceiver);
		}
		return true;
	}

	protected virtual void AnimationCreateProjectile()
	{
		if (spawnPoints.Length == 1)
		{
			GameObject gameObject = null;
			gameObject = (isRemote ? SpawnProjectile(_position, _velocity) : SpawnProjectile(spawnPoints[0].position, aimer.forward * projectileSpeed));
			if (base.NetSyncReporter != null && !isRemote && !dontSendNetworkMessages)
			{
				base.NetSyncReporter.SpawnProjectile(gameObject.transform.position, gameObject.GetComponent<Rigidbody>().velocity);
			}
			gameObject.SendMessage("OnNetworkDelay", 0f, SendMessageOptions.DontRequireReceiver);
		}
	}

	protected virtual GameObject SpawnProjectile(Vector3 pos, Vector3 velocity)
	{
		if (projectile == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate(projectile.gameObject, pos, Quaternion.identity) as GameObject;
		if ((bool)gameObject)
		{
			gameObject.BroadcastMessage("SetEquipmentNames", base.EquipmentNames, SendMessageOptions.DontRequireReceiver);
			gameObject.BroadcastMessage("SetItemOverride", base.name, SendMessageOptions.DontRequireReceiver);
			gameObject.GetComponent<Rigidbody>().velocity = velocity;
			gameObject.transform.LookAt(gameObject.transform.position + gameObject.GetComponent<Rigidbody>().velocity);
			NetworkObject componentInChildren = gameObject.GetComponentInChildren<NetworkObject>();
			componentInChildren.OwnerID = ownerID;
			componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
			DeployableObject component = gameObject.GetComponent<DeployableObject>();
			component.OwnerID = ownerID;
			component.DamageMultiplier = base.playerController.DamageMultiplier;
			component.weaponIndex = base.playerController.WeaponManager.CurrentWeaponIndex;
			component.OwningPlayer = base.playerController;
			component.deployableIndex = base.playerController.WeaponManager.lastDeployableIndex++;
			Collider componentInChildren2 = gameObject.GetComponent<Collider>();
			if (componentInChildren2 == null)
			{
				componentInChildren2 = gameObject.GetComponentInChildren<Collider>();
			}
			if (charController != null)
			{
				Physics.IgnoreCollision(componentInChildren2, charController);
			}
			else
			{
				Collider collider = myTransform.root.GetComponent(typeof(Collider)) as Collider;
				if (collider != null)
				{
					Physics.IgnoreCollision(componentInChildren2, collider);
				}
			}
		}
		_isAttacking = false;
		return gameObject;
	}

	public override void OnReload()
	{
		if (!_isAttacking)
		{
			base.OnReload();
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (isRiggedWeapon || spawnPoints.Length != 1)
		{
			return;
		}
		GameObject gameObject = SpawnProjectile(pos, vel);
		Collider componentInChildren = gameObject.GetComponent<Collider>();
		if (componentInChildren == null)
		{
			componentInChildren = gameObject.GetComponentInChildren<Collider>();
		}
		float num = Mathf.Max(Mathf.Max(componentInChildren.bounds.size.x, componentInChildren.bounds.size.y), componentInChildren.bounds.size.z);
		int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Shield")) | (1 << LayerMask.NameToLayer("RaycastableWall")) | (1 << LayerMask.NameToLayer("Player"));
		RaycastHit[] array = Physics.SphereCastAll(pos - vel.normalized * num * 2f, num / 2f, vel.normalized, vel.magnitude * ((float)delay / 1000f), layerMask);
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			PlayerController componentInChildren2 = array[i].transform.root.GetComponentInChildren<PlayerController>();
			if (!(componentInChildren2 != null) || componentInChildren2.OwnerID != base.playerController.OwnerID)
			{
				gameObject.transform.position = array[i].point;
				gameObject.SendMessage("DeployOrientedToNormal", array[i], SendMessageOptions.DontRequireReceiver);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			gameObject.transform.position = pos + vel * delay / 1000f;
			gameObject.SendMessage("OnNetworkDelay", (float)delay / 1000f, SendMessageOptions.DontRequireReceiver);
		}
	}
}
