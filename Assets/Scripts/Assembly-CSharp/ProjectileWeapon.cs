using UnityEngine;

public class ProjectileWeapon : WeaponBase
{
	private const int WALL_LAYER = 9;

	private const int PLAYER_LAYER = 10;

	private const int RAYCASTABLE_WALL_LAYER = 26;

	private const int DESTRUCTABLE_LAYER = 28;

	private const int CEILING_LAYER = 24;

	private const float DISTANCE_TO_RAYCAST = 4000f;

	public GameObject projectile;

	public float projectileSpeed = 10f;

	public Transform[] spawnPoints;

	public Transform spawnRoot;

	[SerializeField]
	private bool _accountForDelay = true;

	[SerializeField]
	protected LayerMask _layersToHit = LayersToHitFromConstants();

	protected CharacterController charController;

	protected int spawnIndex;

	private static LayerMask LayersToHitFromConstants()
	{
		return 352323072;
	}

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("projectileSpeed", ref projectileSpeed, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (spawnPoints.Length == 1 && !isRiggedWeapon && !ProjectileCreatedFromAnimation)
		{
			GameObject gameObject = SpawnProjectile(spawnPoints[spawnIndex].position, DirectionFromReticleRaycast(spawnIndex) * projectileSpeed);
			DelayedGravityProjectile component = gameObject.GetComponent<DelayedGravityProjectile>();
			if (component != null)
			{
				component.ownerID = base.OwnerID;
			}
			if (base.NetSyncReporter != null && !dontSendNetworkMessages)
			{
				base.NetSyncReporter.SpawnProjectile(gameObject.transform.position, gameObject.rigidbody.velocity);
			}
			gameObject.SendMessage("OnNetworkDelay", 0f, SendMessageOptions.DontRequireReceiver);
		}
		return true;
	}

	protected Vector3 DirectionFromReticleRaycast(int spawnIndex)
	{
		Vector3 position = spawnPoints[spawnIndex].position;
		Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		RaycastHit hitInfo;
		Vector3 vector = ((!Physics.Raycast(ray.origin, ray.direction, out hitInfo, 4000f, _layersToHit)) ? (ray.origin + ray.direction * 4000f) : hitInfo.point);
		return (vector - position).normalized;
	}

	public virtual void AnimationCreateProjectile()
	{
		if (spawnPoints.Length != 1 || (AnimationCantCreateProjectileOnRemote && isRemote))
		{
			return;
		}
		GameObject gameObject = null;
		gameObject = (isRemote ? SpawnProjectile(_position, _velocity) : SpawnProjectile(spawnPoints[0].position, DirectionFromReticleRaycast(spawnIndex) * projectileSpeed));
		if (gameObject == null)
		{
			Debug.LogError("AnimationCreateProjectile could not create projectile!");
			return;
		}
		DelayedGravityProjectile component = gameObject.GetComponent<DelayedGravityProjectile>();
		if (component != null)
		{
			component.ownerID = base.OwnerID;
		}
		if (base.NetSyncReporter != null && !isRemote && !dontSendNetworkMessages)
		{
			base.NetSyncReporter.SpawnProjectile(gameObject.transform.position, gameObject.rigidbody.velocity);
		}
		gameObject.SendMessage("OnNetworkDelay", 0f, SendMessageOptions.DontRequireReceiver);
	}

	protected virtual GameObject SpawnProjectile(Vector3 pos, Vector3 velocity)
	{
		if (projectile == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate(projectile, pos, Quaternion.identity) as GameObject;
		if ((bool)gameObject)
		{
			gameObject.BroadcastMessage("SetEquipmentNames", base.EquipmentNames, SendMessageOptions.DontRequireReceiver);
			gameObject.BroadcastMessage("SetItemOverride", base.name, SendMessageOptions.DontRequireReceiver);
			gameObject.rigidbody.velocity = velocity;
			gameObject.transform.LookAt(gameObject.transform.position + gameObject.rigidbody.velocity);
			NetworkObject componentInChildren = gameObject.GetComponentInChildren<NetworkObject>();
			componentInChildren.OwnerID = ownerID;
			if (base.playerController != null)
			{
				componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
			}
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
		if (!isRiggedWeapon && spawnPoints.Length == 1)
		{
			SpawnRemoteProjectile(pos, vel, delay);
		}
	}

	protected void SpawnRemoteProjectile(Vector3 pos, Vector3 vel, int delay)
	{
		GameObject gameObject = SpawnProjectile(pos, vel);
		Collider componentInChildren = gameObject.collider;
		if (componentInChildren == null)
		{
			componentInChildren = gameObject.GetComponentInChildren<Collider>();
		}
		Bounds bounds = componentInChildren.bounds;
		float num = Mathf.Max(Mathf.Max(componentInChildren.bounds.size.x, componentInChildren.bounds.size.y), componentInChildren.bounds.size.z);
		int layerMask = PhysicsCollisionMatrixLayerMasks.MaskForLayer(gameObject.layer);
		float distance = vel.magnitude * ((float)delay / 1000f);
		Vector3 normalized = vel.normalized;
		Vector3 origin = PointAlongProjectilePathAtPlayerPosition(pos, vel);
		float radius = num / 2f;
		RaycastHit[] array = Physics.SphereCastAll(origin, radius, normalized, distance, layerMask);
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			PlayerController componentInChildren2 = array[i].transform.root.GetComponentInChildren<PlayerController>();
			if (!(componentInChildren2 != null) || !(base.playerController != null) || componentInChildren2.OwnerID != base.playerController.OwnerID)
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

	private Vector3 PointAlongProjectilePathAtPlayerPosition(Vector3 projectileSpawnPosition, Vector3 projectileVelocity)
	{
		Plane plane = new Plane(base.playerController.transform.forward, base.playerController.transform.position);
		int num = ((!plane.GetSide(projectileSpawnPosition)) ? 1 : (-1));
		Ray ray = new Ray(projectileSpawnPosition, projectileVelocity * float.PositiveInfinity * num);
		float enter = 0f;
		if (plane.Raycast(ray, out enter))
		{
			return ray.GetPoint(enter);
		}
		return projectileSpawnPosition;
	}
}
