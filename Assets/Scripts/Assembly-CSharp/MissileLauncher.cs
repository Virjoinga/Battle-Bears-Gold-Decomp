using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class MissileLauncher : WeaponBase
{
	public GameObject projectile;

	public Transform spawnPoint;

	public float missTargetDistance = 1000f;

	public float raycastMaxDistance = 10000f;

	public LayerMask raycastMask;

	public LayerMask targetMask;

	public LayerMask lineOfSightMask;

	public float targettingTime = 3f;

	public float lockRadius = 350f;

	public GameObject targettingSystemPrefab;

	public AudioClip lockingSound;

	public AudioClip lockedSound;

	private Vector3 target;

	private TargettingSystem targettingSystem;

	private bool canLock;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("lockTime", ref targettingTime, base.EquipmentNames);
		item.UpdateProperty("lockRadius", ref lockRadius, base.EquipmentNames);
		item.UpdateProperty("missTargetDistance", ref missTargetDistance, base.EquipmentNames);
		float pVal = 0f;
		item.UpdateProperty("canLockMissile", ref pVal, base.EquipmentNames);
		canLock = pVal > 0f;
		base.ConfigureWeapon(item);
	}

	protected override void Start()
	{
		base.Start();
		if (!isRemote && canLock)
		{
			targettingSystem = base.gameObject.AddComponent(typeof(MissileTargettingSystem)) as TargettingSystem;
			targettingSystem.targetMask = targetMask;
			targettingSystem.lineOfSightMask = lineOfSightMask;
			targettingSystem.targettingTime = targettingTime;
			targettingSystem.lockRadius = lockRadius;
			targettingSystem.targettingSystemPrefab = targettingSystemPrefab;
			StartCoroutine(targettingSounds());
		}
	}

	public void OnOwnerDead()
	{
		StopAllCoroutines();
		Object.Destroy(targettingSystem);
	}

	private IEnumerator targettingSounds()
	{
		if (!canLock || !(targettingSystem != null))
		{
			yield break;
		}
		while (true)
		{
			if (targettingSystem.lockedTarget != null)
			{
				if (lockedSound != null)
				{
					myAudio.PlayOneShot(lockedSound);
					yield return new WaitForSeconds(lockedSound.length);
				}
				else
				{
					yield return new WaitForSeconds(0.1f);
				}
			}
			else if (targettingSystem.isLocking)
			{
				if (lockingSound != null)
				{
					myAudio.PlayOneShot(lockingSound);
					yield return new WaitForSeconds(lockingSound.length);
				}
				else
				{
					yield return new WaitForSeconds(0.1f);
				}
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	public override bool OnAttack()
	{
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponent(typeof(PlayerController)) as PlayerController;
		}
		if (targettingSystem != null && targettingSystem.lockedTarget != null)
		{
			target = targettingSystem.lockedTarget.position;
		}
		else
		{
			RaycastHit hitInfo;
			if (!(base.playerController != null) || !(base.playerController.PlayerCam != null) || !Physics.Raycast(base.playerController.PlayerCam.transform.position, base.playerController.PlayerCam.transform.forward, out hitInfo, raycastMaxDistance, raycastMask))
			{
				return false;
			}
			target = hitInfo.point;
		}
		base.OnAttack();
		GameObject gameObject = Object.Instantiate(projectile, spawnPoint.position, Quaternion.identity) as GameObject;
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.SetItemOverride(base.name);
		component.SetEquipmentNames(base.EquipmentNames);
		component.OwnerID = ownerID;
		component.DamageMultiplier = base.playerController.DamageMultiplier;
		HomingMissile component2 = gameObject.GetComponent<HomingMissile>();
		if (component2 != null)
		{
			component2.StartHomingCountDown(target, 0f, (!(targettingSystem != null)) ? null : targettingSystem.lockedTarget);
		}
		Collider collider = gameObject.collider;
		Component[] componentsInChildren = myTransform.root.GetComponentsInChildren(typeof(Collider));
		Component[] array = componentsInChildren;
		foreach (Component component3 in array)
		{
			if ((component3 as Collider).enabled && collider.enabled)
			{
				Physics.IgnoreCollision(collider, component3 as Collider);
			}
		}
		if (gameObject != null && !dontSendNetworkMessages)
		{
			if (targettingSystem != null && targettingSystem.lockedTarget != null)
			{
				SendFireHomingMissileAction(gameObject.transform.position, target, targettingSystem.lockedTarget.GetComponent<PlayerController>());
			}
			else
			{
				base.NetSyncReporter.SpawnProjectile(gameObject.transform.position, target);
			}
		}
		return true;
	}

	private void SendFireHomingMissileAction(Vector3 pos, Vector3 target, PlayerController lockedPlayer)
	{
		if (lockedPlayer != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add((byte)72, base.OwnerID);
			hashtable.Add((byte)98, pos.x);
			hashtable.Add((byte)99, pos.y);
			hashtable.Add((byte)100, pos.z);
			hashtable.Add((byte)101, target.x);
			hashtable.Add((byte)102, target.y);
			hashtable.Add((byte)103, target.z);
			hashtable.Add((byte)83, PhotonManager.Instance.ServerTimeInMilliseconds);
			hashtable.Add((byte)111, lockedPlayer.OwnerID);
			base.NetSyncReporter.SetAction(60, hashtable);
		}
		else
		{
			Debug.LogError("Tried to fire homing missile on locked player that was null!");
		}
	}

	public override void OnRemoteAttackWithTarget(Vector3 pos, Vector3 target, int delay, PlayerController lockedTarget)
	{
		base.OnRemoteAttackWithTarget(pos, target, delay, lockedTarget);
		FireRemoteMissile(pos, target, delay, lockedTarget);
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 target, int delay)
	{
		base.OnRemoteAttack(pos, target, delay);
		FireRemoteMissile(pos, target, delay);
	}

	private void FireRemoteMissile(Vector3 pos, Vector3 target, int delay, PlayerController lockedTarget = null)
	{
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		GameObject gameObject = Object.Instantiate(projectile, pos, Quaternion.identity) as GameObject;
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.SetItemOverride(base.name);
		component.SetEquipmentNames(base.EquipmentNames);
		component.OwnerID = ownerID;
		component.DamageMultiplier = base.playerController.DamageMultiplier;
		HomingMissile homingMissile = gameObject.GetComponent(typeof(HomingMissile)) as HomingMissile;
		if (homingMissile != null)
		{
			Transform lockedTarget2 = ((!(lockedTarget != null)) ? null : lockedTarget.transform);
			homingMissile.StartHomingCountDown(target, delay, lockedTarget2);
		}
		Collider collider = gameObject.collider;
		collider.enabled = true;
		Component[] componentsInChildren = myTransform.root.GetComponentsInChildren(typeof(Collider));
		Component[] array = componentsInChildren;
		foreach (Component component2 in array)
		{
			if (component2.collider.enabled)
			{
				Physics.IgnoreCollision(collider, component2 as Collider);
			}
		}
		collider.enabled = false;
	}
}
