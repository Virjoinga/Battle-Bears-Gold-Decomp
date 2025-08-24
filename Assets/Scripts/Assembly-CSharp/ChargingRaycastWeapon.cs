using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class ChargingRaycastWeapon : ChargingWeapon
{
	[SerializeField]
	public LayerMask _layersToHit;

	[SerializeField]
	public int _raycastDamage = 15;

	[SerializeField]
	public GameObject _fullyChargedRaycastEffect;

	[SerializeField]
	public GameObject _normalRaycastEffect;

	[SerializeField]
	public float _fullyZoomedFOV = 40f;

	private float _defaultFOV;

	private float _currentFOV;

	protected override void Start()
	{
		base.Start();
		if (Camera.main != null)
		{
			_defaultFOV = Camera.main.fov;
		}
		ServiceManager.Instance.UpdateProperty(base.name, ref _raycastDamage);
	}

	protected override void CreateNormalShot()
	{
		base.CreateNormalShot();
		DoRaycast(false, Vector3.zero);
	}

	protected override void CreateChargedShot()
	{
		DoRaycast(true, Vector3.zero);
	}

	public override void OnRemoteChargedAttack(Vector3 pos, Vector3 vel, int delay)
	{
		DoRaycast(true, pos);
		base.OnRemoteChargedAttack(pos, vel, delay);
	}

	public override void BeginCharging()
	{
		base.BeginCharging();
		StartCoroutine(ZoomingCoRoutine());
	}

	public override void EndCharging()
	{
		base.EndCharging();
		if (!isRemote)
		{
			ResetFOV();
		}
	}

	private void DoRaycast(bool fullyCharged, Vector3 hitLocation)
	{
		GameObject gameObject = null;
		if (_fullyChargedRaycastEffect != null && _normalRaycastEffect != null)
		{
			gameObject = Object.Instantiate((!fullyCharged) ? _normalRaycastEffect : _fullyChargedRaycastEffect) as GameObject;
		}
		else if (_fullyChargedRaycastEffect != null && fullyCharged)
		{
			gameObject = Object.Instantiate(_fullyChargedRaycastEffect) as GameObject;
		}
		else if (_normalRaycastEffect != null)
		{
			gameObject = Object.Instantiate(_normalRaycastEffect) as GameObject;
		}
		if (isRemote && gameObject != null && hitLocation != Vector3.zero)
		{
			gameObject.transform.position = hitLocation;
			ConfigureNetworkStats(gameObject);
		}
		else
		{
			if (isRemote)
			{
				return;
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(spawnPoints[0].position, aimer.forward, out hitInfo, 4000f, _layersToHit))
			{
				if (gameObject != null)
				{
					gameObject.transform.position = ((!(hitInfo.point == Vector3.zero)) ? hitInfo.point : (aimer.forward * 4000f));
					gameObject.transform.rotation = Quaternion.identity;
					ConfigureNetworkStats(gameObject);
				}
				if (!fullyCharged)
				{
					Transform transform = hitInfo.transform;
					DamageReceiver damageReceiver = transform.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
					if (damageReceiver != null && !damageReceiver.isInvincible)
					{
						OnDealDirectDamage(damageReceiver, (float)_raycastDamage * base.playerController.DamageMultiplier);
					}
				}
			}
			if (fullyCharged && !dontSendNetworkMessages)
			{
				GameObject gameObject2 = SpawnProjectile(spawnPoints[0].position, aimer.forward * chargedProjectileSpeed, chargedProjectile);
				ConfigurableNetworkObject component = gameObject2.GetComponent<ConfigurableNetworkObject>();
				if (component != null)
				{
					component.OwnerID = base.OwnerID;
					component.SetItemOverride(base.name);
					component.SetEquipmentNames(base.EquipmentNames);
					component.DamageMultiplier = base.playerController.DamageMultiplier;
				}
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = ownerID;
				hashtable[(byte)1] = base.playerController.WeaponManager.CurrentWeaponIndex;
				hashtable[(byte)2] = hitInfo.point.x;
				hashtable[(byte)3] = hitInfo.point.y;
				hashtable[(byte)4] = hitInfo.point.z;
				hashtable[(byte)5] = 0f;
				hashtable[(byte)6] = 0f;
				hashtable[(byte)7] = 0f;
				base.NetSyncReporter.SetAction(ChargingWeapon._chargedShotAction, hashtable);
			}
		}
	}

	protected IEnumerator ZoomingCoRoutine()
	{
		if (Camera.main != null)
		{
			_defaultFOV = Camera.main.fov;
		}
		float halfChargeShotTime = chargeShotTime / 2f;
		float timeOfStart = Time.fixedTime + halfChargeShotTime;
		while (base.IsCharging && base.playerController != null && !base.playerController.IsDead)
		{
			if (!isRemote && Time.fixedTime - timeOfStart > halfChargeShotTime)
			{
				LerpZoomForward(timeOfStart, halfChargeShotTime);
			}
			yield return null;
		}
		ResetFOV();
	}

	private void ConfigureNetworkStats(GameObject ob)
	{
		ConfigurableNetworkObject component = ob.GetComponent<ConfigurableNetworkObject>();
		if (component != null)
		{
			component.OwnerID = base.OwnerID;
			component.SetItemOverride(base.name);
			component.SetEquipmentNames(base.EquipmentNames);
			component.DamageMultiplier = base.playerController.DamageMultiplier;
		}
	}

	private void LerpZoomForward(float chargingStartTime, float halfChargeShotTime)
	{
		float num = chargingStartTime + halfChargeShotTime;
		float t = 1f - (halfChargeShotTime - (Time.fixedTime - num));
		_currentFOV = Mathf.Lerp(_defaultFOV, _fullyZoomedFOV, t);
		if (Camera.main != null)
		{
			Camera.main.fov = _currentFOV;
		}
	}

	private void ResetFOV()
	{
		if (base.playerController != null && !base.playerController.IsDead)
		{
			_currentFOV = _defaultFOV;
			if (Camera.main != null)
			{
				Camera.main.fov = _currentFOV;
			}
		}
	}
}
