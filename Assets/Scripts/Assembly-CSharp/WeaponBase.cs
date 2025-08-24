using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
	protected const string FIRE = "fire";

	protected const string IDLE = "idle";

	protected const string RUN = "run";

	protected const string PASSIVE = "passive";

	protected const string RELOAD_IN = "reloadIn";

	protected const string RELOAD_OUT = "reloadOut";

	protected const string RELOAD_LOOP = "reloadLoop";

	protected Transform myTransform;

	protected Animation myAnimation;

	private NetSyncReporter netSyncReporter;

	private static bool fireAchievementUnlocked;

	public float reloadTime = 1f;

	public float firingTime = 0.2f;

	public int clipSize = 1;

	public float fireShakeStrength;

	public float fireShakeLength;

	public GameObject attackEffect;

	public GameObject idleEffect;

	public GameObject reloadEffect;

	public GameObject reloadEffectOnSpawnPoint;

	public bool isConstantFire;

	public bool isSingleFire;

	public bool isChargeable;

	public bool isMelee;

	public bool animateLikeDeployable;

	public bool requireGrounded;

	public bool isMine;

	public bool isTurret;

	public bool isSpecial;

	public bool isPrefabSpawner;

	public bool isFireInLoopOut;

	public bool isRiggedWeapon;

	public bool isAnimatedWeapon;

	public bool ProjectileCreatedFromAnimation;

	public bool AnimationCantCreateProjectileOnRemote;

	public bool dontAnimateAttacks;

	public bool dontAnimateReloads;

	public bool dontSendNetworkMessages;

	public bool shouldAnimateBodyFireIndependentOfFiringTime;

	public bool dontSetFireInLoopOut;

	public float attackAnimationSpeed = 1f;

	public float reloadAnimationSpeed = 1f;

	public float idleAnimationSpeed = 1f;

	public float walkAnimationSpeed = 1f;

	public float passiveWeaponAnimationSpeed = 1f;

	protected AudioSource myAudio;

	protected Transform aimer;

	protected int ownerID = -1;

	public bool isRemote;

	public WeaponMountPoint.Side hand = WeaponMountPoint.Side.RIGHT;

	public ParticleSystem fireEffect;

	public Vector3 mountedPosition = Vector3.zero;

	public Vector3 mountedScale = Vector3.one;

	public AudioClip[] fireSounds;

	public AudioClip[] reloadSounds;

	protected bool isAnimatingReload;

	protected Vector3 _position;

	protected Vector3 _velocity;

	public float LastReloadStart { get; set; }

	public int LastFiredTime { get; set; }

	public PlayerController playerController { get; set; }

	public Item Item { get; private set; }

	public string EquipmentNames
	{
		get
		{
			if (playerController != null && playerController.CharacterManager != null)
			{
				return playerController.CharacterManager.EquipmentNames;
			}
			return string.Empty;
		}
	}

	public int OwnerID
	{
		get
		{
			return ownerID;
		}
		set
		{
			ownerID = value;
		}
	}

	public NetSyncReporter NetSyncReporter
	{
		get
		{
			return netSyncReporter;
		}
		set
		{
			netSyncReporter = value;
		}
	}

	public virtual void ConfigureWeapon(Item item)
	{
		Item = item;
		if (item.type != "melee")
		{
			item.UpdateProperty("reloadTime", ref reloadTime, EquipmentNames);
			item.UpdateProperty("cooldown", ref firingTime, EquipmentNames);
			item.UpdateProperty("clipSize", ref clipSize, EquipmentNames);
		}
		if (!isRemote && item.type != "melee" && HUD.Instance != null && playerController != null && this == playerController.WeaponManager.CurrentWeapon)
		{
			HUD.Instance.OnSetClipSize(clipSize);
		}
	}

	protected void OnDealDirectDamage(DamageReceiver dmgReceiver, float damage, float radiationDmg = 0f)
	{
		if (GameManager.Instance == null)
		{
			dmgReceiver.OnTakeDamage(0f, -1, false, false, false, false, false, radiationDmg, string.Empty);
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(ownerID);
		PlayerCharacterManager playerCharacterManager2 = GameManager.Instance.Players(dmgReceiver.OwnerID);
		if (!(NetSyncReporter != null) || (GameManager.Instance.friendlyFireRatio < 0.01f && playerCharacterManager != null && playerCharacterManager2 != null && dmgReceiver != null && dmgReceiver.OwnerID != ownerID && playerCharacterManager.team == playerCharacterManager2.team))
		{
			return;
		}
		dmgReceiver.OnTakeDamage(damage, ownerID, false, false, false, false, false, radiationDmg, string.Empty);
		if (playerCharacterManager != null && playerCharacterManager2 != null && dmgReceiver != null && playerCharacterManager.team == playerCharacterManager2.team && dmgReceiver.OwnerID != ownerID)
		{
			damage *= GameManager.Instance.friendlyFireRatio;
		}
		GameManager.Instance.playerStats[ownerID].addDamageDealt(dmgReceiver.OwnerID, damage);
		byte action = 0;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = dmgReceiver.OwnerID;
		hashtable[(byte)1] = damage;
		hashtable[(byte)2] = ownerID;
		if (dmgReceiver is PlayerDamageReceiver || dmgReceiver is DamageReceiverProxy)
		{
			action = 29;
			if (radiationDmg > 0f)
			{
				hashtable[(byte)3] = radiationDmg;
			}
		}
		else if (dmgReceiver is TurretDamageReceiver)
		{
			action = 30;
			hashtable[(byte)3] = ((TurretDamageReceiver)dmgReceiver).turretIndex;
			if (radiationDmg > 0f)
			{
				hashtable[(byte)4] = radiationDmg;
			}
		}
		if (!dontSendNetworkMessages)
		{
			NetSyncReporter.SetAction(action, hashtable);
		}
	}

	protected virtual void Awake()
	{
		myTransform = base.transform;
		myAnimation = GetComponentInChildren<Animation>();
		myAudio = base.audio;
		OnIdle();
		if (myAnimation != null && myAnimation["passive"] != null)
		{
			myAnimation["passive"].layer = 1;
			myAnimation["passive"].speed = passiveWeaponAnimationSpeed;
			myAnimation.Play("passive");
		}
	}

	protected virtual void Start()
	{
		playerController = myTransform.root.GetComponentInChildren<PlayerController>();
		if (ServiceManager.Instance != null && HUD.Instance != null)
		{
			ConfigureWeapon(ServiceManager.Instance.GetItemByName(base.name));
			SendMessageUpwards("OnWeaponConfigured", SendMessageOptions.DontRequireReceiver);
		}
		if (myAnimation != null && myAnimation["switch"] != null)
		{
			myAnimation.Play("switch");
		}
	}

	public virtual void OnCurrentAmmo(int currentAmmo)
	{
	}

	public void OnFindAimer()
	{
		aimer = myTransform.root.Find("aimer");
	}

	public virtual void PlayAttackAnimation(float startTime = 0f, float speed = 1f)
	{
		if (myAnimation != null)
		{
			if (myAnimation["fire"] != null)
			{
				myAnimation["fire"].time = startTime;
				myAnimation["fire"].speed = speed;
				myAnimation.CrossFade("fire");
			}
			else if (myAnimation["fireIn"] != null && myAnimation["fireLoop"] != null)
			{
				StartCoroutine("DoFireInFireLoopFireOut", firingTime);
			}
		}
	}

	protected bool PerformLocalAttackAnimationAndEffects()
	{
		if (playerController != null && playerController.GetRadarTracker != null)
		{
			playerController.GetRadarTracker.SetVisible();
		}
		if (!fireAchievementUnlocked)
		{
			Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["DOUBLE_TAP_THAT"]);
			fireAchievementUnlocked = true;
		}
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
		if (fireEffect != null)
		{
			fireEffect.Play(true);
		}
		if (!dontAnimateAttacks)
		{
			PlayAttackAnimation(0f, 1f);
		}
		PlayFireSound();
		CreateCameraShake();
		return true;
	}

	protected virtual void PlayFireSound()
	{
		if (myAudio != null && fireSounds.Length > 0)
		{
			myAudio.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length)], SoundManager.Instance.getEffectsVolume());
		}
	}

	public virtual bool OnAttack()
	{
		return PerformLocalAttackAnimationAndEffects();
	}

	public virtual bool OnAttack(float charge)
	{
		return PerformLocalAttackAnimationAndEffects();
	}

	public virtual bool OnChargedAttack()
	{
		return PerformLocalAttackAnimationAndEffects();
	}

	protected virtual void PerformRemoteAttackAnimationAndEffects(Vector3 pos, Vector3 vel)
	{
		if (playerController != null && playerController.GetRadarTracker != null)
		{
			playerController.GetRadarTracker.SetVisible();
		}
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
		if (fireEffect != null)
		{
			fireEffect.Play(true);
		}
		PlayAttackAnimation(0f, 1f);
		PlayFireSound();
		if (playerController == null)
		{
			playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (isRiggedWeapon)
		{
			_position = pos;
			_velocity = vel;
		}
	}

	public virtual void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		PerformRemoteAttackAnimationAndEffects(pos, vel);
	}

	public virtual void OnRemoteAttackWithTarget(Vector3 pos, Vector3 vel, int delay, PlayerController target)
	{
		PerformRemoteAttackAnimationAndEffects(pos, vel);
	}

	public virtual void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay, float charge)
	{
		PerformRemoteAttackAnimationAndEffects(pos, vel);
	}

	public virtual void OnRemoteChargedAttack(Vector3 pos, Vector3 vel, int delay)
	{
		PerformRemoteAttackAnimationAndEffects(pos, vel);
	}

	public void OnPlayReloadSound()
	{
		if (myAudio != null && reloadSounds.Length > 0)
		{
			myAudio.PlayOneShot(reloadSounds[Random.Range(0, reloadSounds.Length)], SoundManager.Instance.getEffectsVolume());
		}
	}

	protected IEnumerator DoFireInFireLoopFireOut(float firingTime)
	{
		if (!(myAnimation != null))
		{
			yield break;
		}
		float loopTime = 0f;
		if (myAnimation["fireIn"] != null)
		{
			float fireOutTime = 0f;
			if (myAnimation["fireOut"] != null)
			{
				fireOutTime = myAnimation["fireOut"].length;
			}
			loopTime = firingTime - myAnimation["fireIn"].length - fireOutTime;
			myAnimation["fireIn"].wrapMode = WrapMode.Once;
			myAnimation.CrossFade("fireIn", 0.2f);
			yield return new WaitForSeconds(myAnimation["fireIn"].length);
		}
		if (loopTime > 0f && myAnimation["fireLoop"] != null)
		{
			myAnimation["fireLoop"].wrapMode = WrapMode.Loop;
			myAnimation.CrossFade("fireLoop", 0.2f);
			yield return new WaitForSeconds(loopTime);
		}
		if (myAnimation["fireOut"] != null)
		{
			myAnimation["fireOut"].wrapMode = WrapMode.Once;
			myAnimation.CrossFade("fireOut", 0.2f);
			yield return new WaitForSeconds(myAnimation["fireOut"].length);
		}
		OnIdle();
	}

	public virtual void OnReload()
	{
		if (attackEffect != null)
		{
			attackEffect.SetActive(false);
		}
		if (reloadEffect != null)
		{
			reloadEffect.SetActive(true);
		}
		if (idleEffect != null)
		{
			idleEffect.SetActive(false);
		}
		if (fireEffect != null)
		{
			fireEffect.Stop(true);
		}
		if (myAnimation != null && myAnimation["reload"] != null)
		{
			myAnimation.Play("reload");
		}
	}

	private void CreateReloadSpawnPointEffect()
	{
		Transform transform = base.transform.FindChild("spawn");
		if (transform != null && reloadEffectOnSpawnPoint != null)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(reloadEffectOnSpawnPoint, transform.position, transform.rotation);
			gameObject.transform.parent = base.transform;
			DelayedDestroy component = gameObject.GetComponent<DelayedDestroy>();
			if (component != null)
			{
				component.delay = reloadTime;
			}
		}
	}

	public virtual void StartedReloading()
	{
		CreateReloadSpawnPointEffect();
	}

	public virtual void PlayReloadAnimation()
	{
		if (isAnimatedWeapon && !isAnimatingReload && myAnimation["reloadIn"] != null && myAnimation["reloadLoop"] != null)
		{
			isAnimatingReload = true;
			StartCoroutine("ReloadCoroutine");
		}
	}

	private IEnumerator ReloadCoroutine()
	{
		float reloadOutTime = ((!(myAnimation["reloadOut"] == null)) ? myAnimation["reloadOut"].length : 0f);
		float reloadTimeLeft = reloadTime - (Time.fixedTime - LastReloadStart);
		float loopDuration2 = 0f;
		if (reloadTimeLeft == reloadTime)
		{
			loopDuration2 = reloadTime - myAnimation["reloadIn"].length - reloadOutTime;
			myAnimation.CrossFade("reloadIn", 0.2f);
			yield return new WaitForSeconds(myAnimation["reloadIn"].length);
		}
		else
		{
			loopDuration2 = reloadTimeLeft - reloadOutTime;
		}
		myAnimation.Stop();
		yield return null;
		myAnimation.CrossFade("reloadLoop", 0.2f);
		while (loopDuration2 > 0f)
		{
			yield return null;
			loopDuration2 -= Time.deltaTime;
		}
		myAnimation["reloadLoop"].layer = 0;
		if (myAnimation["reloadOut"] != null && reloadTimeLeft >= reloadOutTime)
		{
			myAnimation.CrossFade("reloadOut", 0.2f);
			yield return new WaitForSeconds(myAnimation["reloadOut"].length - 0.1f);
		}
		isAnimatingReload = false;
	}

	public void PlayWeaponAnimation(string name, WrapMode wrapMode, float crossFadeTime, float speed = 1f, float time = 0f)
	{
		if (isAnimatedWeapon && myAnimation != null && myAnimation[name] != null)
		{
			myAnimation[name].wrapMode = wrapMode;
			myAnimation[name].speed = speed;
			myAnimation[name].time = time;
			myAnimation.CrossFade(name, crossFadeTime);
		}
	}

	public void StopWeaponAnimations()
	{
		if (myAnimation != null)
		{
			myAnimation.Stop();
		}
	}

	public virtual void WeaponOnRun(float time = 0f)
	{
		if (isAnimatedWeapon && !isAnimatingReload && myAnimation != null && myAnimation["run"] != null)
		{
			myAnimation["run"].wrapMode = WrapMode.Loop;
			myAnimation["run"].time = time;
			myAnimation.CrossFade("run", 0.2f);
		}
	}

	public virtual void WeaponDeath()
	{
		if (isAnimatedWeapon && myAnimation != null)
		{
			StopCoroutine("ReloadCoroutine");
			StopCoroutine("DoFireInFireLoopFireOut");
			if (myAnimation["death"] != null)
			{
				myAnimation["death"].wrapMode = WrapMode.Once;
				myAnimation.CrossFade("death", 0.2f);
			}
		}
	}

	public virtual void OnIdle()
	{
		if (attackEffect != null)
		{
			attackEffect.SetActive(false);
		}
		if (reloadEffect != null)
		{
			reloadEffect.SetActive(false);
		}
		if (idleEffect != null)
		{
			idleEffect.SetActive(true);
		}
		if (fireEffect != null)
		{
			fireEffect.Stop(true);
		}
		if (myAnimation != null && myAnimation["idle"] != null && !isAnimatingReload)
		{
			myAnimation.CrossFade("idle");
		}
	}

	public virtual void BeginConstantFireEffects()
	{
		if (fireEffect != null)
		{
			fireEffect.Play(true);
		}
	}

	public virtual void EndConstantFireEffects()
	{
		if (fireEffect != null)
		{
			fireEffect.Stop(true);
		}
	}

	public virtual void BeginCharging()
	{
		if (isChargeable)
		{
		}
	}

	public virtual void EndCharging()
	{
		if (isChargeable)
		{
		}
	}

	protected void CreateCameraShake()
	{
		if (fireShakeStrength != 0f && fireShakeLength != 0f && Camera.main != null)
		{
			ShakeCamera shakeCamera = Camera.main.gameObject.AddComponent<ShakeCamera>();
			shakeCamera.shakeDuration = fireShakeLength;
			shakeCamera.shakeStrength = fireShakeStrength;
		}
	}

	protected void SendFireMessage(Vector3 pos, Vector3 vel)
	{
		if (NetSyncReporter != null && !dontSendNetworkMessages)
		{
			NetSyncReporter.SpawnProjectile(pos, vel);
		}
	}
}
