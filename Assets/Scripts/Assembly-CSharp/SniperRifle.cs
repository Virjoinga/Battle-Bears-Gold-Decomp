using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class SniperRifle : WeaponBase
{
	public Transform spawnPoint;

	public float raycastOffset = 40f;

	private Vector3 target;

	protected SniperTargettingSystem targettingSystem;

	public LayerMask targetMask;

	public LayerMask lineOfSightMask;

	public float targettingTime = 3f;

	public LayerMask layersToHit;

	public LayerMask missMask;

	public float lockRadius = 350f;

	public GameObject targettingSystemPrefab;

	public GameObject laserSightPrefab;

	private GameObject currentLaserSight;

	private Transform currentLaserSightTransform;

	public float damage = 50f;

	public GameObject tracerPrefab;

	public GameObject tracerHitSplash;

	private float missDistance = 6000f;

	public AudioClip lockedSound;

	public AudioClip headshotLockSound;

	public float warningTime = 2f;

	private PlayerController warnedPlayer;

	private bool hasPlayedLockSound;

	private bool hasPlayerHeadshotLockSound;

	private bool _configured;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("damage", ref damage, base.EquipmentNames);
		item.UpdateProperty("lockTime", ref targettingTime, base.EquipmentNames);
		item.UpdateProperty("lockRadius", ref lockRadius, base.EquipmentNames);
		item.UpdateProperty("warningTime", ref warningTime, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Start()
	{
		base.Start();
		if (!isRemote && HUD.Instance != null && targettingSystem == null)
		{
			startTargettingSystem();
		}
		base.enabled = false;
	}

	private void startTargettingSystem()
	{
		if (!_configured)
		{
			ConfigureWeapon(ServiceManager.Instance.GetItemByName(base.name));
			_configured = true;
		}
		targettingSystem = base.gameObject.AddComponent(typeof(SniperTargettingSystem)) as SniperTargettingSystem;
		targettingSystem.targetMask = targetMask;
		targettingSystem.lineOfSightMask = lineOfSightMask;
		targettingSystem.targettingTime = targettingTime;
		targettingSystem.lockRadius = lockRadius;
		targettingSystem.targettingSystemPrefab = targettingSystemPrefab;
		targettingSystem.loseLockDelay = 0.35f;
		targettingSystem.lockStartLocation = TargettingSystem.LockStart.NORMAL;
		StartCoroutine(laserSightChecker());
		StartCoroutine(targettingSounds());
	}

	private IEnumerator targettingSounds()
	{
		while (targettingSystem != null)
		{
			if (targettingSystem.lockedTarget != null)
			{
				if (headshotLockSound != null && !hasPlayerHeadshotLockSound)
				{
					myAudio.PlayOneShot(headshotLockSound);
					hasPlayerHeadshotLockSound = true;
				}
			}
			else if (targettingSystem.isLocking && lockedSound != null && !hasPlayedLockSound && targettingSystem.targettingTimeLeft < targettingTime / 2f)
			{
				myAudio.PlayOneShot(lockedSound);
				hasPlayedLockSound = true;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator laserSightChecker()
	{
		while (true)
		{
			float previousTargettingTimeLeft3 = targettingTime;
			if (targettingSystem.currentTarget != null)
			{
				if (currentLaserSight == null)
				{
					currentLaserSight = Object.Instantiate(laserSightPrefab) as GameObject;
					currentLaserSightTransform = currentLaserSight.transform;
					currentLaserSightTransform.position = spawnPoint.position;
					previousTargettingTimeLeft3 = targettingTime;
					base.enabled = true;
					base.NetSyncReporter.SetAction(24, null);
				}
				else if (warnedPlayer == null)
				{
					if (previousTargettingTimeLeft3 > targettingSystem.targettingTime - warningTime && targettingSystem.targettingTimeLeft <= targettingSystem.targettingTime - warningTime)
					{
						warnedPlayer = targettingSystem.currentTarget.root.GetComponent<PlayerController>();
						if (warnedPlayer != null)
						{
							ExitGames.Client.Photon.Hashtable parameters2 = new ExitGames.Client.Photon.Hashtable();
							parameters2[(byte)0] = warnedPlayer.OwnerID;
							base.NetSyncReporter.SetAction(26, parameters2);
						}
						else
						{
							Debug.LogWarning("player should always have a player controller");
						}
					}
					previousTargettingTimeLeft3 = targettingSystem.targettingTimeLeft;
				}
			}
			else
			{
				hasPlayedLockSound = false;
				hasPlayerHeadshotLockSound = false;
				if (warnedPlayer != null)
				{
					ExitGames.Client.Photon.Hashtable parameters = new ExitGames.Client.Photon.Hashtable();
					parameters[(byte)0] = warnedPlayer.OwnerID;
					base.NetSyncReporter.SetAction(27, parameters);
				}
				warnedPlayer = null;
				if (currentLaserSight != null)
				{
					Object.Destroy(currentLaserSight);
					base.enabled = false;
					base.NetSyncReporter.SetAction(25, null);
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	public void OnRemoteStartAiming()
	{
		if (currentLaserSight == null)
		{
			currentLaserSight = Object.Instantiate(laserSightPrefab) as GameObject;
			currentLaserSightTransform = currentLaserSight.transform;
			currentLaserSightTransform.position = spawnPoint.position;
			base.enabled = true;
		}
	}

	public void OnRemoteStopAiming()
	{
		if (currentLaserSight != null)
		{
			Object.Destroy(currentLaserSight);
			base.enabled = false;
		}
	}

	public void OnOwnerDead()
	{
		endWarning();
		StopAllCoroutines();
		Object.Destroy(targettingSystem);
	}

	private void OnDestroy()
	{
		endWarning();
	}

	private void endWarning()
	{
		if (warnedPlayer != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = warnedPlayer.OwnerID;
			base.NetSyncReporter.SetAction(27, hashtable);
		}
		warnedPlayer = null;
		if (currentLaserSight != null)
		{
			Object.Destroy(currentLaserSight);
		}
	}

	private void LateUpdate()
	{
		if (currentLaserSight != null)
		{
			currentLaserSightTransform.position = spawnPoint.position;
			currentLaserSightTransform.rotation = spawnPoint.rotation;
		}
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponent(typeof(PlayerController)) as PlayerController;
		}
		if (targettingSystem == null)
		{
			startTargettingSystem();
		}
		if (targettingSystem.lockedTarget != null)
		{
			DamageReceiver damageReceiver = targettingSystem.lockedTarget.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			Vector3 vector = ((!(damageReceiver.headSpot != null)) ? damageReceiver.transform.position : new Vector3(damageReceiver.headSpot.position.x, damageReceiver.headSpot.position.y + 10f, damageReceiver.headSpot.position.z));
			Vector3 vector2 = vector - spawnPoint.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(spawnPoint.position - vector2 * raycastOffset, vector2, out hitInfo, missDistance, layersToHit))
			{
				Transform transform = hitInfo.transform;
				DamageReceiver damageReceiver2 = transform.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
				if (damageReceiver2 != null && damageReceiver2 == damageReceiver && !(damageReceiver2 is TurretDamageReceiver))
				{
					if (!damageReceiver.isInvincible && base.NetSyncReporter != null)
					{
						ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
						hashtable[(byte)0] = damageReceiver.OwnerID;
						base.NetSyncReporter.SetAction(28, hashtable);
					}
				}
				else if (damageReceiver2 != null && !damageReceiver2.isInvincible)
				{
					OnDealDirectDamage(damageReceiver2, damage * base.playerController.DamageMultiplier);
				}
				createTracer(hitInfo.point, hitInfo.normal);
				if (base.NetSyncReporter != null)
				{
					base.NetSyncReporter.SpawnProjectile(hitInfo.point, hitInfo.normal);
				}
			}
		}
		else if (targettingSystem.isLocking)
		{
			if (targettingSystem.targettingTimeLeft < targettingTime / 2f)
			{
				Vector3 vector3 = targettingSystem.currentTarget.position - spawnPoint.position;
				RaycastHit hitInfo2;
				if (Physics.Raycast(spawnPoint.position - vector3 * raycastOffset, vector3, out hitInfo2, missDistance, layersToHit))
				{
					Transform transform2 = hitInfo2.transform;
					DamageReceiver damageReceiver3 = transform2.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
					if (damageReceiver3 != null && !damageReceiver3.isInvincible)
					{
						OnDealDirectDamage(damageReceiver3, damage * base.playerController.DamageMultiplier);
					}
					createTracer(hitInfo2.point, hitInfo2.normal);
					if (base.NetSyncReporter != null)
					{
						base.NetSyncReporter.SpawnProjectile(hitInfo2.point, hitInfo2.normal);
					}
				}
			}
			else
			{
				float num = 1f - (targettingSystem.targettingTimeLeft - targettingTime / 2f) / (targettingTime / 2f);
				float num2 = 0.1f;
				float num3 = num2 + num * (1f - num2);
				float value = Random.value;
				if (value < num3)
				{
					Vector3 vector4 = targettingSystem.currentTarget.position - spawnPoint.position;
					RaycastHit hitInfo3;
					if (Physics.Raycast(spawnPoint.position - vector4 * raycastOffset, vector4, out hitInfo3, missDistance, layersToHit))
					{
						Transform transform3 = hitInfo3.transform;
						DamageReceiver damageReceiver4 = transform3.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
						if (damageReceiver4 != null && !damageReceiver4.isInvincible)
						{
							OnDealDirectDamage(damageReceiver4, damage * base.playerController.DamageMultiplier);
						}
						createTracer(hitInfo3.point, hitInfo3.normal);
						if (base.NetSyncReporter != null)
						{
							base.NetSyncReporter.SpawnProjectile(hitInfo3.point, hitInfo3.normal);
						}
					}
				}
				else
				{
					RaycastHit raycastHit = calculateRandomDirection();
					if (base.NetSyncReporter != null)
					{
						base.NetSyncReporter.SpawnProjectile(raycastHit.point, raycastHit.normal);
					}
				}
			}
		}
		else
		{
			RaycastHit raycastHit2 = calculateRandomDirection();
			if (base.NetSyncReporter != null)
			{
				base.NetSyncReporter.SpawnProjectile(raycastHit2.point, raycastHit2.normal);
			}
		}
		return true;
	}

	private RaycastHit calculateRandomDirection()
	{
		float num = 0.06f;
		Vector3 vector = aimer.TransformDirection(new Vector3((1f - 2f * Random.value) * num, (1f - 2f * Random.value) * num, 1f));
		RaycastHit hitInfo;
		if (Physics.Raycast(spawnPoint.position - vector * raycastOffset, vector, out hitInfo, missDistance, layersToHit))
		{
			Transform transform = hitInfo.transform;
			DamageReceiver damageReceiver = transform.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			if (damageReceiver != null && !damageReceiver.isInvincible)
			{
				OnDealDirectDamage(damageReceiver, damage * base.playerController.DamageMultiplier);
			}
			createTracer(hitInfo.point, hitInfo.normal);
		}
		else
		{
			Vector3 vector2 = spawnPoint.position + vector * missDistance;
			hitInfo = default(RaycastHit);
			createTracer(vector2, Vector3.zero);
			hitInfo.point = vector2;
			hitInfo.normal = Vector3.zero;
		}
		return hitInfo;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 target, int delay)
	{
		base.OnRemoteAttack(pos, target, delay);
		createTracer(pos, target);
	}

	private void createTracer(Vector3 endPos, Vector3 hitNormal)
	{
		GameObject gameObject = Object.Instantiate(tracerPrefab, spawnPoint.position, Quaternion.identity) as GameObject;
		Transform transform = gameObject.transform;
		transform.localScale = new Vector3(1f, 1f, 1f);
		Transform transform2 = transform.Find("start");
		Transform transform3 = transform.Find("end");
		transform2.position = spawnPoint.position;
		transform3.position = endPos;
		transform2.LookAt(transform3);
		transform3.eulerAngles = transform2.eulerAngles;
		if (Vector3.Distance(spawnPoint.position, endPos) < missDistance - 10f)
		{
			GameObject gameObject2 = Object.Instantiate(tracerHitSplash) as GameObject;
			gameObject2.transform.position = endPos;
			gameObject2.transform.LookAt(spawnPoint);
		}
	}
}
