using UnityEngine;

public class HuggableProjectileWeapon : MeleeAttack
{
	public Transform spawnPoint;

	public GameObject projectilePrefab;

	public float projectileSpeed = 10f;

	[SerializeField]
	private bool _accountForDelay = true;

	protected CharacterController charController;

	protected override void Start()
	{
		base.Start();
		charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		OnFindAimer();
	}

	public new void OnFindAimer()
	{
		aimer = myTransform.root.Find("aimer");
	}

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("projectileSpeed", ref projectileSpeed, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	public override bool OnAttack()
	{
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (aimer == null)
		{
			OnFindAimer();
		}
		if (!isRiggedWeapon)
		{
			GameObject gameObject = SpawnProjectile(spawnPoint.position, aimer.forward * projectileSpeed);
			DelayedGravityProjectile component = gameObject.GetComponent<DelayedGravityProjectile>();
			if (component != null)
			{
				component.ownerID = base.OwnerID;
			}
			if (base.NetSyncReporter != null)
			{
				base.NetSyncReporter.SpawnProjectile(gameObject.transform.position, gameObject.rigidbody.velocity);
			}
			gameObject.SendMessage("OnNetworkDelay", 0f, SendMessageOptions.DontRequireReceiver);
			ConfigurableNetworkObject component2 = gameObject.GetComponent<ConfigurableNetworkObject>();
			component2.SetItemOverride(base.name);
			component2.SetEquipmentNames(base.EquipmentNames);
			component2.OwnerID = ownerID;
			component2.DamageMultiplier = base.playerController.DamageMultiplier;
			component2.MeleeMultiplier = base.playerController.MeleeMultiplier;
			Physics.IgnoreCollision(gameObject.collider, myTransform.root.collider);
		}
		return PerformLocalAttackAnimationAndEffects();
	}

	protected virtual GameObject SpawnProjectile(Vector3 pos, Vector3 velocity)
	{
		if (projectilePrefab == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate(projectilePrefab, pos, Quaternion.identity) as GameObject;
		if ((bool)gameObject)
		{
			gameObject.BroadcastMessage("SetEquipmentNames", base.EquipmentNames, SendMessageOptions.DontRequireReceiver);
			gameObject.BroadcastMessage("SetItemOverride", base.name, SendMessageOptions.DontRequireReceiver);
			gameObject.rigidbody.velocity = velocity;
			gameObject.transform.LookAt(gameObject.transform.position + gameObject.rigidbody.velocity);
			NetworkObject componentInChildren = gameObject.GetComponentInChildren<NetworkObject>();
			componentInChildren.OwnerID = ownerID;
			componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
			Collider componentInChildren2 = gameObject.collider;
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
				CapsuleCollider capsuleCollider = myTransform.root.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
				if (capsuleCollider != null)
				{
					Physics.IgnoreCollision(componentInChildren2, capsuleCollider);
				}
			}
		}
		return gameObject;
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
		if (isRiggedWeapon || !(spawnPoint != null))
		{
			return;
		}
		GameObject gameObject = SpawnProjectile(pos, vel);
		Collider componentInChildren = gameObject.collider;
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
				gameObject.SendMessage("Explode", array[i].transform.gameObject, SendMessageOptions.DontRequireReceiver);
				gameObject.SendMessage("handleCollision", array[i].transform.gameObject, SendMessageOptions.DontRequireReceiver);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			if (_accountForDelay)
			{
				gameObject.transform.position = pos + vel * delay / 1000f;
			}
			gameObject.SendMessage("OnNetworkDelay", (float)delay / 1000f, SendMessageOptions.DontRequireReceiver);
		}
	}
}
