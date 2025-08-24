using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class ChargingWeapon : WeaponBase
{
	public float chargeShotTime;

	public int overheatPerShot;

	public int overheatRecoveryPerSecond;

	public float delayBeforeOverheatRecovery;

	public AudioClip chargingSound;

	public float fullyChargedPitch;

	public AudioClip[] chargedShotSounds;

	public bool spawnsMovingProjectile = true;

	public GameObject chargeEffect;

	public GameObject fullChargedEffect;

	public GameObject normalProjectile;

	public float normalProjectileSpeed = 10f;

	public GameObject chargedProjectile;

	public float chargedProjectileSpeed = 10f;

	public Transform[] spawnPoints;

	public Transform spawnRoot;

	protected CharacterController charController;

	private float _gameTimeOfChargingStart = -1f;

	private float _gameTimeToStartRecovery;

	private bool _isCharging;

	private float _currentOverheat;

	protected static readonly byte _chargedShotAction = 45;

	public bool IsCharging { get; set; }

	protected void Update()
	{
		if (!base.playerController.isRemote && base.playerController.WeaponManager.CurrentWeapon == this && !_isCharging && _currentOverheat < (float)clipSize && base.playerController.WeaponManager.CanFireCurrentWeapon() && Time.time > _gameTimeToStartRecovery)
		{
			_currentOverheat = Mathf.MoveTowards(_currentOverheat, clipSize, (float)overheatRecoveryPerSecond * Time.deltaTime);
			base.playerController.WeaponManager.SetCurrentClipSize((int)_currentOverheat);
		}
		if (base.playerController != null && base.playerController.IsDead)
		{
			if (myAudio != null && myAudio.isPlaying)
			{
				myAudio.pitch = 1f;
				myAudio.Stop();
			}
			SetEffects(false);
		}
	}

	protected override void Awake()
	{
		_gameTimeOfChargingStart = -1f;
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		if (chargeEffect != null)
		{
			chargeEffect.SetActive(false);
		}
		if (fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(false);
		}
		_currentOverheat = base.playerController.WeaponManager.GetCurrentClipSize(base.playerController.WeaponManager.CurrentWeaponIndex);
		if (_currentOverheat <= 0f)
		{
			_currentOverheat = clipSize;
		}
	}

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("chargeTime", ref chargeShotTime, base.EquipmentNames);
		item.UpdateProperty("overheatPerShot", ref overheatPerShot, base.EquipmentNames);
		item.UpdateProperty("chargeCooldownRate", ref overheatRecoveryPerSecond, base.EquipmentNames);
		item.UpdateProperty("cooldownDelay", ref delayBeforeOverheatRecovery, base.EquipmentNames);
		item.UpdateProperty("chargedShotSpeed", ref chargedProjectileSpeed, base.EquipmentNames);
		item.UpdateProperty("normalShotSpeed", ref normalProjectileSpeed, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	public override void OnIdle()
	{
		base.OnIdle();
		if (chargeEffect != null)
		{
			chargeEffect.SetActive(false);
		}
		if (fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(false);
		}
	}

	public override void OnReload()
	{
		StopCoroutine("ChargingCoRoutine");
		SetEffects(false);
		base.OnReload();
	}

	public override void BeginCharging()
	{
		if (_currentOverheat > 0f && base.playerController.WeaponManager.CanFireCurrentWeapon())
		{
			base.BeginCharging();
			IsCharging = true;
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
				myAnimation.CrossFade("charge");
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
		FullyCharged();
	}

	protected virtual void FullyCharged()
	{
	}

	public override void EndCharging()
	{
		if (!(base.playerController != null) || !(base.playerController.WeaponManager != null) || base.playerController.WeaponManager.isDisabled)
		{
			return;
		}
		base.EndCharging();
		IsCharging = false;
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
		if (myAnimation != null && !isAnimatingReload)
		{
			myAnimation.Stop();
		}
		if (base.playerController.isRemote)
		{
			return;
		}
		if (_gameTimeOfChargingStart > 0f)
		{
			if (Time.time - _gameTimeOfChargingStart > chargeShotTime)
			{
				base.playerController.WeaponManager.OnFireChargedShot();
			}
			else
			{
				base.playerController.WeaponManager.OnFire();
			}
		}
		if (base.NetSyncReporter != null && !dontSendNetworkMessages)
		{
			base.NetSyncReporter.SetAction(21, null);
		}
		_gameTimeOfChargingStart = -1f;
	}

	public override bool OnAttack()
	{
		if (_currentOverheat > 0f && base.playerController.WeaponManager.CanFireCurrentWeapon())
		{
			_currentOverheat -= overheatPerShot;
			_gameTimeToStartRecovery = Time.time + delayBeforeOverheatRecovery;
			base.OnAttack();
			CreateNormalShot();
			if (_currentOverheat <= 0f)
			{
				_currentOverheat = clipSize;
				base.playerController.WeaponManager.SetCurrentClipSize(0);
				if (base.NetSyncReporter != null && !base.playerController.isRemote && !dontSendNetworkMessages)
				{
					base.NetSyncReporter.SetAction(23, null);
				}
			}
			else
			{
				base.playerController.WeaponManager.SetCurrentClipSize((int)_currentOverheat);
			}
			return true;
		}
		return false;
	}

	public override bool OnChargedAttack()
	{
		if (_currentOverheat > 0f && base.playerController.WeaponManager.CanFireCurrentWeapon())
		{
			_currentOverheat = clipSize;
			base.playerController.WeaponManager.SetCurrentClipSize(0);
			if (attackEffect != null)
			{
				attackEffect.SetActive(true);
			}
			if (reloadEffect != null)
			{
				reloadEffect.SetActive(false);
			}
			if (idleEffect != null)
			{
				idleEffect.SetActive(false);
			}
			if (myAnimation != null && myAnimation["fire"] != null)
			{
				myAnimation.Play("fire");
			}
			if (myAudio != null && chargedShotSounds.Length > 0)
			{
				myAudio.PlayOneShot(chargedShotSounds[Random.Range(0, chargedShotSounds.Length)], 1f);
			}
			CreateChargedShot();
			if (base.NetSyncReporter != null && !base.playerController.isRemote && !dontSendNetworkMessages)
			{
				base.NetSyncReporter.SetAction(23, null);
			}
			return true;
		}
		return false;
	}

	protected virtual void CreateChargedShot()
	{
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (spawnPoints.Length == 1)
		{
			GameObject gameObject = SpawnProjectile(spawnPoints[0].position, aimer.forward * chargedProjectileSpeed, chargedProjectile);
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			if (component != null)
			{
				component.OwnerID = base.OwnerID;
				component.SetItemOverride(base.name);
				component.SetEquipmentNames(base.EquipmentNames);
				component.DamageMultiplier = base.playerController.DamageMultiplier;
			}
			if (base.NetSyncReporter != null && !dontSendNetworkMessages)
			{
				Vector3 position = gameObject.transform.position;
				Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity;
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = ownerID;
				hashtable[(byte)1] = base.playerController.WeaponManager.CurrentWeaponIndex;
				hashtable[(byte)2] = position.x;
				hashtable[(byte)3] = position.y;
				hashtable[(byte)4] = position.z;
				hashtable[(byte)5] = velocity.x;
				hashtable[(byte)6] = velocity.y;
				hashtable[(byte)7] = velocity.z;
				base.NetSyncReporter.SetAction(_chargedShotAction, hashtable);
			}
			gameObject.SendMessage("OnNetworkDelay", 0f, SendMessageOptions.DontRequireReceiver);
		}
	}

	protected virtual void CreateNormalShot()
	{
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (spawnPoints.Length == 1)
		{
			GameObject gameObject = SpawnProjectile(spawnPoints[0].position, aimer.forward * normalProjectileSpeed, normalProjectile);
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
	}

	protected virtual GameObject SpawnProjectile(Vector3 pos, Vector3 velocity, GameObject projectile)
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
			gameObject.GetComponent<Rigidbody>().velocity = velocity;
			gameObject.transform.LookAt(gameObject.transform.position + gameObject.GetComponent<Rigidbody>().velocity);
			NetworkObject componentInChildren = gameObject.GetComponentInChildren<NetworkObject>();
			if (componentInChildren != null)
			{
				componentInChildren.OwnerID = ownerID;
				componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
			}
			Collider componentInChildren2 = gameObject.GetComponent<Collider>();
			if (componentInChildren2 == null)
			{
				componentInChildren2 = gameObject.GetComponentInChildren<Collider>();
			}
			if (charController != null && componentInChildren2 != null)
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

	public override void OnRemoteChargedAttack(Vector3 pos, Vector3 vel, int delay)
	{
		if (attackEffect != null)
		{
			attackEffect.SetActive(true);
		}
		if (reloadEffect != null)
		{
			reloadEffect.SetActive(false);
		}
		if (idleEffect != null)
		{
			idleEffect.SetActive(false);
		}
		if (myAnimation != null && myAnimation["fire"] != null)
		{
			myAnimation.Play("fire");
		}
		if (myAudio != null && chargedShotSounds.Length > 0)
		{
			myAudio.PlayOneShot(chargedShotSounds[Random.Range(0, chargedShotSounds.Length)], 1f);
		}
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (spawnPoints.Length != 1)
		{
			return;
		}
		GameObject gameObject = SpawnProjectile(pos, vel, chargedProjectile);
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		if (component != null)
		{
			component.OwnerID = base.OwnerID;
			component.SetItemOverride(base.name);
			component.SetEquipmentNames(base.EquipmentNames);
			component.DamageMultiplier = base.playerController.DamageMultiplier;
		}
		if (!spawnsMovingProjectile)
		{
			return;
		}
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
				gameObject.SendMessage("Explode", array[i].transform.gameObject, SendMessageOptions.DontRequireReceiver);
				gameObject.SendMessage("handleCollision", array[i].transform.gameObject, SendMessageOptions.DontRequireReceiver);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			SetEffects(false);
			gameObject.transform.position = pos + vel * delay / 1000f;
			gameObject.SendMessage("OnNetworkDelay", (float)delay / 1000f, SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		if (attackEffect != null)
		{
			attackEffect.SetActive(true);
		}
		if (reloadEffect != null)
		{
			reloadEffect.SetActive(false);
		}
		if (idleEffect != null)
		{
			idleEffect.SetActive(false);
		}
		if (myAnimation != null && myAnimation["fire"] != null)
		{
			myAnimation.Play("fire");
		}
		if (myAudio != null && fireSounds.Length > 0)
		{
			myAudio.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length)], 1f);
		}
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (spawnPoints.Length != 1)
		{
			return;
		}
		GameObject gameObject = SpawnProjectile(pos, vel, normalProjectile);
		DelayedGravityProjectile component = gameObject.GetComponent<DelayedGravityProjectile>();
		if (component != null)
		{
			component.ownerID = base.OwnerID;
		}
		if (!spawnsMovingProjectile)
		{
			return;
		}
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
				gameObject.SendMessage("Explode", array[i].transform.gameObject, SendMessageOptions.DontRequireReceiver);
				gameObject.SendMessage("handleCollision", array[i].transform.gameObject, SendMessageOptions.DontRequireReceiver);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			SetEffects(false);
			gameObject.transform.position = pos + vel * delay / 1000f;
			gameObject.SendMessage("OnNetworkDelay", (float)delay / 1000f, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnEnterMGSBox()
	{
		_gameTimeOfChargingStart = -1f;
		if (chargeEffect != null)
		{
			chargeEffect.SetActive(false);
		}
		if (fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(false);
		}
	}

	private void SetEffects(bool active)
	{
		if (chargeEffect != null)
		{
			chargeEffect.SetActive(active);
		}
		if (fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(active);
		}
	}
}
