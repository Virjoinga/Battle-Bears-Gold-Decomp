using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class StaticShuffle : WeaponBase
{
	public GameObject shotgunDamageBurst;

	public Transform spawnPoint;

	private CharacterController charController;

	public float chargeShotTime;

	public GameObject chargeEffect;

	public GameObject fullChargedEffect;

	public AudioClip chargingSound;

	public float fullyChargedPitch;

	public AudioClip[] chargedShotSounds;

	private float _gameTimeOfChargingStart = -1f;

	protected override void Start()
	{
		base.Start();
		OnFindAimer();
		charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		if (chargeEffect != null)
		{
			chargeEffect.SetActive(false);
		}
		if (fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(false);
		}
	}

	public override bool OnAttack(float charge)
	{
		base.OnAttack(charge);
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (charController != null)
		{
			createBurst(spawnPoint.position, aimer.forward, charge);
			if (base.NetSyncReporter != null)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = ownerID;
				hashtable[(byte)1] = base.playerController.WeaponManager.CurrentWeaponIndex;
				hashtable[(byte)2] = spawnPoint.position.x;
				hashtable[(byte)3] = spawnPoint.position.y;
				hashtable[(byte)4] = spawnPoint.position.z;
				hashtable[(byte)5] = aimer.forward.x;
				hashtable[(byte)6] = aimer.forward.y;
				hashtable[(byte)7] = aimer.forward.z;
				hashtable[(byte)8] = charge;
				base.NetSyncReporter.SetAction(46, hashtable);
			}
			return true;
		}
		return false;
	}

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("chargeTime", ref chargeShotTime, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	private void createBurst(Vector3 pos, Vector3 dir)
	{
		createBurst(pos, dir, 0f);
	}

	private void createBurst(Vector3 pos, Vector3 dir, float chargePercent)
	{
		GameObject gameObject = Object.Instantiate(shotgunDamageBurst, pos, Quaternion.identity) as GameObject;
		gameObject.transform.LookAt(pos + dir);
		ConfigurableNetworkObject componentInChildren = gameObject.GetComponentInChildren<ConfigurableNetworkObject>();
		componentInChildren.SetEquipmentNames(base.EquipmentNames);
		componentInChildren.SetItemOverride(base.name);
		componentInChildren.OwnerID = ownerID;
		componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
		ChargingLineOfSightDamageSource componentInChildren2 = gameObject.GetComponentInChildren<ChargingLineOfSightDamageSource>();
		componentInChildren2.ChargePercent = chargePercent;
		Collider componentInChildren3 = gameObject.GetComponent<Collider>();
		if (componentInChildren3 == null)
		{
			componentInChildren3 = gameObject.GetComponentInChildren<Collider>();
		}
		if (!(componentInChildren3 != null))
		{
			return;
		}
		if (charController != null)
		{
			Physics.IgnoreCollision(componentInChildren3, charController);
			return;
		}
		CapsuleCollider capsuleCollider = myTransform.root.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
		if (capsuleCollider != null)
		{
			Physics.IgnoreCollision(componentInChildren3, capsuleCollider);
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 rot, int delay)
	{
		OnRemoteAttack(pos, rot, delay, 0f);
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 rot, int delay, float charge)
	{
		base.OnRemoteAttack(pos, rot, delay);
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		createBurst(pos, rot, charge);
	}

	public override void BeginCharging()
	{
		if (base.playerController != null && base.playerController.WeaponManager != null && base.playerController.WeaponManager.CanFireCurrentWeapon())
		{
			base.BeginCharging();
			StartCoroutine("ChargingCoRoutine");
			if (chargeEffect != null)
			{
				chargeEffect.SetActive(true);
			}
			if (myAudio != null && chargingSound != null)
			{
				myAudio.loop = true;
				myAudio.clip = chargingSound;
				myAudio.pitch = 1f;
				myAudio.Play();
			}
			if (myAnimation != null && myAnimation["charge"] != null)
			{
				myAnimation.Play("charge");
			}
			_gameTimeOfChargingStart = Time.time;
		}
	}

	protected virtual IEnumerator ChargingCoRoutine()
	{
		float timeOfStart = Time.time;
		while (Time.time - timeOfStart < chargeShotTime)
		{
			myAudio.pitch = Mathf.MoveTowards(myAudio.pitch, fullyChargedPitch, Time.deltaTime / chargeShotTime);
			yield return null;
		}
		if (fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(true);
		}
	}

	public override void EndCharging()
	{
		if (!(base.playerController != null) || !(base.playerController.WeaponManager != null) || base.playerController.WeaponManager.isDisabled)
		{
			return;
		}
		base.EndCharging();
		StopCoroutine("ChargingCoRoutine");
		if (chargeEffect != null)
		{
			chargeEffect.SetActive(false);
		}
		if (fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(false);
		}
		if (myAudio != null)
		{
			myAudio.pitch = 1f;
			myAudio.Stop();
		}
		if (!base.playerController.isRemote)
		{
			if (_gameTimeOfChargingStart > 0f && Time.time - _gameTimeOfChargingStart > chargeShotTime)
			{
				base.playerController.WeaponManager.OnFire(1f);
			}
			else
			{
				base.playerController.WeaponManager.OnFire((Time.time - _gameTimeOfChargingStart) / chargeShotTime);
			}
			_gameTimeOfChargingStart = -1f;
		}
	}
}
