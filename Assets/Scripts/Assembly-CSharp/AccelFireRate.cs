using System.Collections;
using UnityEngine;

public class AccelFireRate : WeaponBase
{
	[SerializeField]
	private Transform[] _spawnPoints;

	private int _currentSpawnPointIndex;

	private Transform _defaultSpawnPoint;

	private float originalFiringTime = 1f;

	public float maxSpeedUpPercentage = 50f;

	public float timeToReachMaxSpeedUpPercentage = 4f;

	private float _maxAngleDifference = 20f;

	private float _minDistanceToUseCameraDir = 50f;

	public float raycastOffset = 30f;

	public LayerMask layersToHit;

	public float damage = 1f;

	public float dispersionAngleAmount;

	public float tracerToggleTime = 0.1f;

	public GameObject tracerPrefab;

	private RaycastTracerPool _pool;

	private bool showingTracer;

	private Transform bodyRotator;

	[SerializeField]
	private GameObject _hitEffect;

	protected Transform SpawnPoint
	{
		get
		{
			Transform result;
			if (_spawnPoints.Length > 0)
			{
				result = _spawnPoints[_currentSpawnPointIndex];
				_currentSpawnPointIndex = ((_currentSpawnPointIndex + 1 < _spawnPoints.Length) ? (_currentSpawnPointIndex + 1) : 0);
			}
			else
			{
				result = _defaultSpawnPoint;
			}
			return result;
		}
	}

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("damage", ref damage, base.EquipmentNames);
		item.UpdateProperty("dispersionAngle", ref dispersionAngleAmount, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Awake()
	{
		base.Awake();
		_defaultSpawnPoint = myTransform.Find("spawn");
		originalFiringTime = firingTime;
	}

	protected override void Start()
	{
		base.Start();
		PlayerController playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		base.playerController.WeaponManager.beginFire += BeginFire;
		base.playerController.WeaponManager.endFire += EndFire;
		if (playerController != null)
		{
			bodyRotator = playerController.bodyRotator;
		}
		_pool = base.gameObject.AddComponent<RaycastTracerPool>();
		_pool.Init(tracerPrefab, tracerToggleTime);
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (!isConstantFire && !isRemote && base.NetSyncReporter != null)
		{
			base.NetSyncReporter.SpawnProjectile(Vector3.zero, Vector3.zero);
		}
		DoRaycastAttack(Vector3.zero);
		return true;
	}

	protected virtual Vector3 DoRaycastAttack(Vector3 hitPos)
	{
		bool flag = false;
		Vector3 position = SpawnPoint.position;
		Vector3 vector;
		if (isRemote)
		{
			vector = aimer.TransformDirection(new Vector3((1f - 2f * Random.value) * dispersionAngleAmount, (1f - 2f * Random.value) * dispersionAngleAmount, 1f));
		}
		else
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
			RaycastHit hitInfo;
			Vector3 vector2 = ((!Physics.Raycast(ray.origin, ray.direction, out hitInfo, 4000f, layersToHit)) ? (ray.origin + ray.direction * 4000f) : hitInfo.point);
			vector = vector2 - position;
			if (Vector3.Angle(aimer.forward, vector) > _maxAngleDifference || Vector3.Distance(vector2, ray.origin) < _minDistanceToUseCameraDir)
			{
				vector = aimer.forward;
			}
			vector.Normalize();
			vector.x += (1f - 2f * Random.value) * dispersionAngleAmount;
			vector.y += (1f - 2f * Random.value) * dispersionAngleAmount;
		}
		vector.Normalize();
		RaycastHit hitInfo2;
		Vector3 vector3;
		if (Physics.Raycast(position, vector, out hitInfo2, 4000f, layersToHit))
		{
			vector3 = ((!(hitPos != Vector3.zero)) ? hitInfo2.point : hitPos);
			flag = true;
			Transform transform = hitInfo2.transform;
			DamageReceiver damageReceiver = transform.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			if (damageReceiver != null && !damageReceiver.isInvincible)
			{
				OnDealDirectDamage(damageReceiver, damage * base.playerController.DamageMultiplier);
			}
		}
		else
		{
			vector3 = ((!isRemote) ? (position + vector * 4000f) : (position + vector * 4000f));
			if (hitPos != Vector3.zero)
			{
				vector3 = hitPos;
			}
			flag = false;
		}
		SpawnHitEffect(vector3);
		_pool.CreateTracer(position, vector3, flag);
		if (base.playerController.WeaponManager.OnFirePrimary != null)
		{
			base.playerController.WeaponManager.OnFirePrimary();
		}
		return vector3;
	}

	private void SpawnHitEffect(Vector3 position)
	{
		if (_hitEffect != null)
		{
			GameObject gameObject = Object.Instantiate(_hitEffect, position, Quaternion.identity) as GameObject;
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			if (component != null)
			{
				component.SetItemOverride(base.name);
				component.SetEquipmentNames(base.EquipmentNames);
				component.OwnerID = ownerID;
				component.DamageMultiplier = base.playerController.DamageMultiplier;
			}
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		bool flag = false;
		if (bodyRotator == null && myTransform != null)
		{
			bodyRotator = (myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController).bodyRotator;
		}
		if (bodyRotator != null)
		{
			Vector3 vector = ((!(dispersionAngleAmount > 0f)) ? bodyRotator.transform.forward : bodyRotator.TransformDirection(new Vector3((1f - 2f * Random.value) * dispersionAngleAmount, (1f - 2f * Random.value) * dispersionAngleAmount, 1f)));
			Vector3 position = SpawnPoint.position;
			RaycastHit hitInfo;
			Vector3 vector2;
			if (Physics.Raycast(position - vector * raycastOffset, vector, out hitInfo, 4000f, layersToHit))
			{
				vector2 = hitInfo.point;
				flag = true;
			}
			else
			{
				vector2 = position + vector * 4000f;
				flag = false;
			}
			if (_pool != null)
			{
				_pool.CreateTracer(position, vector2, flag);
			}
			if (flag)
			{
				SpawnHitEffect(vector2);
			}
		}
	}

	private void OnDestroy()
	{
		if (base.playerController != null)
		{
			base.playerController.WeaponManager.beginFire -= BeginFire;
			base.playerController.WeaponManager.endFire -= EndFire;
		}
	}

	private void BeginFire()
	{
		StopCoroutine("Accelerate");
		StartCoroutine("Accelerate");
	}

	private void EndFire()
	{
		StopCoroutine("Accelerate");
		firingTime = originalFiringTime;
	}

	private IEnumerator Accelerate()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			if (firingTime > originalFiringTime * ((100f - maxSpeedUpPercentage) * 0.01f))
			{
				firingTime -= originalFiringTime * (maxSpeedUpPercentage / timeToReachMaxSpeedUpPercentage * 0.001f);
			}
		}
	}
}
