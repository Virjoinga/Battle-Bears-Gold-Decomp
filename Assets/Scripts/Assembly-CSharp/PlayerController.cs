using System.Collections;
using System.Collections.Generic;
using Analytics;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private JoulesManager _joulesManager;

	private int _numJoulesToSpawnPerDeath = 3;

	public ControllerDirector Director;

	public ControllerPerformer Performer;

	public Camera mainCamera;

	private Transform myTransform;

	private Transform aimer;

	private WeaponManagerBase weaponManager;

	public Transform bodyRotator;

	private Team _team;

	private PlayerDamageReceiver dmgReceiver;

	private System.Collections.ArrayList spawnpoints = new System.Collections.ArrayList();

	private NetSyncReporter netSyncReporter;

	private BodyAnimatorBase bodyAnimator;

	private LegAnimator legAnimator;

	private AudioSource myAudio;

	public AudioClip[] deathSounds;

	private int ownerID;

	public bool isDisabled;

	public GameObject gibs;

	public Transform deathCameraSpot;

	public Transform tauntCameraSpot;

	public GameObject tauntCamera;

	public bool isRemote;

	private bool isDead;

	private float damageMultiplier = 1f;

	private float meleeMultiplier = 1f;

	private float propbeargandaBonus;

	private CharacterMotor motor;

	private CamouflageCloak camoCloak;

	public GameObject damageMultiplierEffectPrefab;

	private GameObject currentDamageMultiplierEffect;

	public bool canControl = true;

	public bool canSwitchWeapons = true;

	public GameObject specialItemPrefab;

	private GameObject currentSpecialItem;

	private float nextSpecialItemChargeTime;

	private float lastSpecialItemUseTime;

	public AudioClip[] teamspeakSayings;

	public AudioClip[] winSayings;

	public AudioClip[] loseSayings;

	public AudioClip[] shieldPickupSayings;

	public AudioClip[] doubleDamagePickupSayings;

	public AudioClip[] meleeAttackSounds;

	public AudioClip[] reloadSounds;

	public AudioClip[] meleeKillSounds;

	public AudioClip[] regularKillSounds;

	protected AudioClip _tauntSound;

	public AudioClip chickenPickupSound;

	public AudioClip healthPickupSound;

	public AudioClip backstabSound;

	public Transform statusEffectMount;

	private float statusRotateTime = 0.75f;

	private GameObject currentDisplayedStatus;

	private int numSniperWarnings;

	private bool isStunned;

	private bool hasPropbearganda;

	public bool hasCoffee;

	public int slowCount;

	public bool isDreamy;

	public bool isImmuneToStun;

	private AudioReverbFilter dreamFilter;

	public Transform hawkMount;

	public GameObject hawkPrefab;

	public GameObject eaglePrefab;

	public GameObject parashootPrefab;

	public GameObject freezeEffect;

	public GameObject radiationExplosionPrefab;

	private GameObject currentBird;

	private string speakIconName = string.Empty;

	private float _timeOfLastTeamspeak;

	private PlayerCamera playerCam;

	private PlayerCharacterManager _playerCharacterManager;

	private Transform bombTimerMount;

	private float specialItemCooldownMultiplier;

	private bool hasBomb;

	private float bombSlowMultiplier = 0.5f;

	private Transform currentBombToGrab;

	private GameObject currentBombTimer;

	private bool spawnGibs = true;

	private double currentBombHoldTime;

	private int bonusPerSecond = 1;

	private int bombDepositBonus = 1;

	private bool isBombPickupAllowed = true;

	private int poisonCount;

	private PoisonColor _poisonColor;

	private RadiationMeter _radiationDisplay;

	private int _previousRadiationLevel = -1;

	private bool _prevFireState;

	private Vector2 lastPosition;

	private Dictionary<string, AudioClip> _customDeathSfxByName = new Dictionary<string, AudioClip>();

	public CharacterHandle _characterHandle;

	private bool _canJump = true;

	private float _mogaSensitivityMod = 1.2f;

	[SerializeField]
	private PlayerGUI _gui;

	private RadarTracker _radarTracker;

	private bool _hasAssignedRadarColor;

	private StatisticManager _statManager;

	public JoulesManager PlayerJoulesManager
	{
		get
		{
			return _joulesManager;
		}
	}

	public bool SatelliteSecondaries
	{
		get
		{
			return weaponManager is SatelliteSecondaryWeaponManager;
		}
	}

	public PoisonColor CurrentPoisonColor
	{
		get
		{
			return _poisonColor;
		}
		set
		{
			_poisonColor = value;
		}
	}

	public CharacterHandle CharacterHandle
	{
		get
		{
			return _characterHandle;
		}
	}

	public bool CanJump
	{
		get
		{
			return _canJump;
		}
		set
		{
			_canJump = value;
			((SimpleControllerPerformer)Performer).DisableJump = !value;
		}
	}

	public bool IsBombPickupAllowed
	{
		get
		{
			return isBombPickupAllowed;
		}
		set
		{
			isBombPickupAllowed = value;
		}
	}

	public bool SpawnGibs
	{
		get
		{
			return spawnGibs;
		}
		set
		{
			spawnGibs = value;
		}
	}

	public float BombSlowMultiplier
	{
		get
		{
			return bombSlowMultiplier;
		}
		set
		{
			bombSlowMultiplier = value;
		}
	}

	public bool HasBomb
	{
		get
		{
			return hasBomb;
		}
	}

	public bool IsDead
	{
		get
		{
			return isDead;
		}
	}

	public PlayerCharacterManager CharacterManager
	{
		get
		{
			return _playerCharacterManager;
		}
		set
		{
			_playerCharacterManager = value;
		}
	}

	public PlayerCamera PlayerCam
	{
		get
		{
			return playerCam;
		}
	}

	public CharacterMotor Motor
	{
		get
		{
			return motor;
		}
	}

	public float NextSpecialItemChargeTime
	{
		get
		{
			return nextSpecialItemChargeTime;
		}
	}

	public float LastSpecialItemUseTime
	{
		get
		{
			return lastSpecialItemUseTime;
		}
	}

	public CamouflageCloak CamoCloak
	{
		get
		{
			return camoCloak;
		}
		set
		{
			camoCloak = value;
		}
	}

	public float DamageMultiplier
	{
		get
		{
			return damageMultiplier + propbeargandaBonus;
		}
		set
		{
			damageMultiplier = value;
		}
	}

	public float MeleeMultiplier
	{
		get
		{
			return meleeMultiplier + propbeargandaBonus;
		}
		set
		{
			meleeMultiplier = value;
		}
	}

	public Team Team
	{
		get
		{
			return _team;
		}
		set
		{
			_team = value;
			TargetableObject componentInChildren = GetComponentInChildren<TargetableObject>();
			if (componentInChildren != null)
			{
				componentInChildren.Team = value;
			}
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
			weaponManager.OwnerID = value;
		}
	}

	public NetSyncReporter NetSync
	{
		get
		{
			return netSyncReporter;
		}
	}

	public WeaponManagerBase WeaponManager
	{
		get
		{
			return weaponManager;
		}
		set
		{
			weaponManager = value;
		}
	}

	public PlayerDamageReceiver DamageReceiver
	{
		get
		{
			return dmgReceiver;
		}
		set
		{
			dmgReceiver = value;
		}
	}

	public BodyAnimatorBase BodyAnimator
	{
		get
		{
			return bodyAnimator;
		}
	}

	public PlayerGUI PlayersGUI
	{
		get
		{
			return _gui;
		}
	}

	public RadarTracker GetRadarTracker
	{
		get
		{
			return _radarTracker;
		}
	}

	public StatisticManager StatManager
	{
		get
		{
			return _statManager;
		}
	}

	private bool _isRoyaleMode
	{
		get
		{
			return Preferences.Instance.CurrentGameMode == GameMode.ROYL;
		}
	}

	public void OnPostCreate()
	{
		netSyncReporter = GetComponent(typeof(NetSyncReporter)) as NetSyncReporter;
		weaponManager.OnPostCreate();
		weaponManager.OnSetWeapon(0);
		if (_isRoyaleMode)
		{
			Camera[] componentsInChildren = GetComponentsInChildren<Camera>();
			foreach (Camera camera in componentsInChildren)
			{
				camera.gameObject.SendMessage("EnableFog", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (!isRemote)
		{
			Performer.PlayerController = this;
		}
	}

	private void OnGUI()
	{
		if (HUD.Instance.currentPauseMenu == null && HUD.Instance.currentStatsOverlay == null && !isRemote && _gui != null)
		{
			if (CustomLayoutController.Instance.IsOpen)
			{
				_gui.UpdateGUI(Time.deltaTime);
				Director.UpdateControls(Time.deltaTime);
				Performer.PerformControls(Director, Time.deltaTime);
				CustomLayoutController.Instance.UpdateHUDElementPositions();
			}
			_gui.RenderGUI();
		}
	}

	private void Awake()
	{
		_joulesManager = base.gameObject.AddComponent<JoulesManager>();
		_radarTracker = ((!_isRoyaleMode) ? base.gameObject.AddComponent<RadarTracker>() : base.gameObject.AddComponent<AlwaysOnRadarTracker>());
		PlayerJoulesManager.PlayerCont = this;
		myTransform = base.transform;
		myAudio = base.audio;
		aimer = myTransform.Find("aimer");
		weaponManager = GetComponentInChildren(typeof(WeaponManagerBase)) as WeaponManagerBase;
		dmgReceiver = GetComponent(typeof(PlayerDamageReceiver)) as PlayerDamageReceiver;
		_radarTracker.CloakHandle = dmgReceiver;
		bodyAnimator = GetComponentInChildren(typeof(BodyAnimatorBase)) as BodyAnimatorBase;
		legAnimator = GetComponentInChildren(typeof(LegAnimator)) as LegAnimator;
		motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;
		if (Camera.mainCamera != null)
		{
			playerCam = Camera.mainCamera.GetComponent(typeof(PlayerCamera)) as PlayerCamera;
		}
		bombTimerMount = myTransform.Find("bombTimerMount");
		_radiationDisplay = myTransform.Find("statusEffect_mount/radiation").GetComponent<RadiationMeter>();
		ServiceManager.Instance.UpdateProperty("bomb_slow_ratio", ref bombSlowMultiplier);
		ServiceManager.Instance.UpdateProperty("bomb_hold_bonus_per_second", ref bonusPerSecond);
		ServiceManager.Instance.UpdateProperty("bomb_deposit_bonus", ref bombDepositBonus);
		lastPosition = new Vector2(-1f, -1f);
	}

	private float BonusProperty(Item item, string property)
	{
		if (item != null)
		{
			return item.GetBonusProperty(property);
		}
		return 0f;
	}

	private void Start()
	{
		if (CharacterManager != null)
		{
			specialItemCooldownMultiplier += BonusProperty(CharacterManager.playerLoadout.skin, "cooldownMultiplier");
			specialItemCooldownMultiplier += BonusProperty(CharacterManager.playerLoadout.primary, "cooldownMultiplier");
			specialItemCooldownMultiplier += BonusProperty(CharacterManager.playerLoadout.secondary, "cooldownMultiplier");
			specialItemCooldownMultiplier += BonusProperty(CharacterManager.playerLoadout.melee, "cooldownMultiplier");
			specialItemCooldownMultiplier += BonusProperty(CharacterManager.playerLoadout.special, "cooldownMultiplier");
			specialItemCooldownMultiplier += BonusProperty(CharacterManager.playerLoadout.equipment1, "cooldownMultiplier");
			specialItemCooldownMultiplier += BonusProperty(CharacterManager.playerLoadout.equipment2, "cooldownMultiplier");
			specialItemCooldownMultiplier += BonusProperty(CharacterManager.playerLoadout.taunt, "cooldownMultiplier");
		}
		if (isRemote)
		{
			_radiationDisplay.radiationLevelSounds = new AudioClip[0];
		}
		else
		{
			_radiationDisplay.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
			_gui = new PlayerGUI();
			HUD.Instance.PlayerController = this;
			HUD.Instance.OnShowControlMode(Preferences.Instance.CurrentControlMode);
		}
		SetRadiationDisplay(0f);
		StartCoroutine(showStatusEffects());
	}

	private void Update()
	{
		if (!_hasAssignedRadarColor)
		{
			if (HUD.Instance.PlayerController != null)
			{
				_radarTracker.BlipColor = ((_team != HUD.Instance.PlayerController._team) ? Color.red : Color.yellow);
			}
			_hasAssignedRadarColor = true;
		}
		if (!isRemote)
		{
			if (_statManager == null)
			{
				_statManager = new StatisticManager(this);
			}
			_statManager.RemoveExpiredMods();
			if (!CustomLayoutController.Instance.IsOpen)
			{
				_gui.UpdateGUI(Time.deltaTime);
				Director.UpdateTextValues();
				if (!isDead)
				{
					Director.UpdateControls(Time.deltaTime);
					if (HUD.Instance.currentPauseMenu != null || HUD.Instance.currentStatsOverlay != null)
					{
						Director.ClearDirectives();
					}
					Performer.PerformControls(Director, Time.deltaTime);
					if (isDreamy)
					{
						motor.inputMoveDirection = new Vector3(0f - motor.inputMoveDirection.x, 0f - motor.inputMoveDirection.y, 0f - motor.inputMoveDirection.z);
					}
				}
			}
			myTransform.localEulerAngles = new Vector3(0f, myTransform.localEulerAngles.y + aimer.localEulerAngles.y, 0f);
			bodyRotator.localEulerAngles = new Vector3(aimer.localEulerAngles.x, 0f, aimer.localEulerAngles.z);
			aimer.localEulerAngles = new Vector3(aimer.localEulerAngles.x, 0f, aimer.localEulerAngles.z);
		}
		if (hasBomb && !isRemote)
		{
			currentBombHoldTime += Time.deltaTime;
		}
	}

	public void OnRemoteStartGrabbingBomb()
	{
		weaponManager.OnGetBomb();
		bodyAnimator.OnGetBomb();
		DamageReceiver.OnGiveBombGrabInvincibility();
	}

	public void OnLocalStartGrabbingBomb(GameObject bomb, int currentRequestTime)
	{
		currentBombToGrab = bomb.transform;
		isDisabled = true;
		Motor.SetVelocity(Vector3.zero);
		Motor.SetControllable(false);
		if (Performer is SimpleControllerPerformer)
		{
			SimpleControllerPerformer simpleControllerPerformer = (SimpleControllerPerformer)Performer;
			simpleControllerPerformer.Disabled = true;
		}
		if (NetSync != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = OwnerID;
			hashtable[(byte)1] = currentRequestTime;
			NetSync.SetAction(1, hashtable);
		}
		BroadcastMessage("OnBombDeactivate", SendMessageOptions.DontRequireReceiver);
		weaponManager.OnGetBomb();
		bodyAnimator.OnGetBomb();
		DamageReceiver.OnGiveBombGrabInvincibility();
	}

	public void OnLocalStopGrabbingBomb()
	{
		currentBombToGrab = null;
		if (Performer is SimpleControllerPerformer)
		{
			SimpleControllerPerformer simpleControllerPerformer = (SimpleControllerPerformer)Performer;
			simpleControllerPerformer.Disabled = false;
			simpleControllerPerformer.LockInputs = false;
		}
		isDisabled = false;
		Motor.SetControllable(true);
		bodyAnimator.OnReleaseBomb();
		weaponManager.EnableWeapons();
		DamageReceiver.OnRemoveBombGrabInvincibility();
	}

	public void OnLocalGetBomb(GameObject bomb)
	{
		currentBombToGrab = null;
		localGetBomb(bomb);
		if (NetSync != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = OwnerID;
			NetSync.SetAction(2, hashtable);
		}
	}

	private void localGetBomb(GameObject bomb)
	{
		bomb.SendMessage("OnGrabbedByPlayer", this);
		if (Performer is SimpleControllerPerformer)
		{
			SimpleControllerPerformer simpleControllerPerformer = (SimpleControllerPerformer)Performer;
			simpleControllerPerformer.Disabled = false;
		}
		hasBomb = true;
		isDisabled = false;
		if (!isRemote)
		{
			motor.SetControllable(true);
			motor.movement.slowdownMultiplier = 1f - bombSlowMultiplier;
			HUD.Instance.OnToggleBomb(true);
		}
	}

	private void localReleaseBomb()
	{
		if (currentBombTimer != null)
		{
			Object.Destroy(currentBombTimer);
		}
		hasBomb = false;
		motor.movement.slowdownMultiplier = 1f;
		HUD.Instance.OnToggleBomb(false);
		bodyAnimator.OnReleaseBomb();
		weaponManager.EnableWeapons();
		bodyAnimator.OnReset();
	}

	private void OnRemoteReleaseBomb()
	{
		if (currentBombTimer != null)
		{
			Object.Destroy(currentBombTimer);
		}
		hasBomb = false;
		if (bodyAnimator != null)
		{
			bodyAnimator.OnReleaseBomb();
		}
		if (weaponManager != null)
		{
			weaponManager.EnableWeapons();
		}
	}

	public void OnRemoteGetBomb(GameObject bomb, GameObject bombTimer)
	{
		bomb.SendMessage("OnGrabbedByPlayer", this);
		hasBomb = true;
		if (currentBombTimer != null)
		{
			Object.Destroy(currentBombTimer);
		}
		currentBombTimer = Object.Instantiate(bombTimer) as GameObject;
		currentBombTimer.transform.parent = bombTimerMount;
		currentBombTimer.transform.localPosition = Vector3.zero;
		currentBombTimer.transform.localEulerAngles = Vector3.zero;
		currentBombTimer.transform.localScale = Vector3.one;
		currentBombTimer.SendMessage("OnStartPulse");
	}

	public void OnRemoteResetBomb()
	{
		OnRemoteReleaseBomb();
		if (CTFManager.Instance != null && myTransform != null)
		{
			CTFManager.Instance.explodeBomb(Team, myTransform.position);
		}
	}

	public void OnRemoteDropBomb(Vector3 pos)
	{
		OnRemoteReleaseBomb();
		if (CTFManager.Instance != null)
		{
			CTFManager.Instance.dropBomb(Team, pos);
		}
	}

	public void OnLocalDepositBomb()
	{
		if (CTFManager.Instance != null && hasBomb && !isRemote)
		{
			if (!isRemote)
			{
				giveDepositBombBonus();
			}
			localReleaseBomb();
			if (NetSync != null)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = OwnerID;
				NetSync.SetAction(4, hashtable);
			}
			CTFManager.Instance.resetBomb(Team);
			GameManager.Instance.OnBombDeposited(Team);
		}
	}

	private void giveDepositBombBonus()
	{
		currentBombHoldTime = 0.0;
		GameManager.Instance.OnAddBombDepositBonus(OwnerID, bombDepositBonus);
		HUD.Instance.OnAddBombDepositNotification(Team, "+ " + bombDepositBonus);
		if (NetSync != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = OwnerID;
			hashtable[(byte)1] = bombDepositBonus;
			NetSync.SetAction(37, hashtable);
		}
	}

	public void OnRemoteDepositBomb()
	{
		if (CTFManager.Instance != null)
		{
			CTFManager.Instance.gameObject.BroadcastMessage("OnRemoteBombDeposit", Team);
			OnRemoteReleaseBomb();
			CTFManager.Instance.resetBomb(Team);
			GameManager.Instance.OnBombDeposited(Team);
		}
	}

	public void OnLocalReleaseBomb()
	{
		if (!(CTFManager.Instance != null) || !hasBomb || isRemote)
		{
			return;
		}
		giveHoldingBombBonus();
		localReleaseBomb();
		if (DamageReceiver.WasDeathFromDeathArea)
		{
			if (NetSync != null)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = OwnerID;
				NetSync.SetAction(3, hashtable);
			}
			CTFManager.Instance.explodeBomb(Team, myTransform.position);
			return;
		}
		if (NetSync != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
			hashtable2[(byte)0] = OwnerID;
			hashtable2[(byte)1] = myTransform.position.x;
			hashtable2[(byte)2] = myTransform.position.y;
			hashtable2[(byte)3] = myTransform.position.z;
			NetSync.SetAction(5, hashtable2);
		}
		CTFManager.Instance.dropBomb(Team, myTransform.position);
	}

	private void OnDestroy()
	{
		if (CTFManager.Instance != null && hasBomb)
		{
			CTFManager.Instance.dropBomb(Team, myTransform.position);
		}
		DeployableObject[] array = Object.FindObjectsOfType(typeof(DeployableObject)) as DeployableObject[];
		DeployableObject[] array2 = array;
		foreach (DeployableObject deployableObject in array2)
		{
			if (deployableObject.OwningPlayer == this)
			{
				Object.Destroy(deployableObject.gameObject);
			}
		}
	}

	public void OnAddSniperWarning()
	{
		numSniperWarnings++;
		StopCoroutine("delayedSniperWarningDisable");
		StartCoroutine("delayedSniperWarningDisable");
	}

	public void OnRemoveSniperWarning()
	{
		numSniperWarnings--;
		if (numSniperWarnings < 0)
		{
			numSniperWarnings = 0;
		}
		StopCoroutine("delayedSniperWarningDisable");
	}

	private IEnumerator delayedSniperWarningDisable()
	{
		yield return new WaitForSeconds(10f);
		numSniperWarnings--;
		if (numSniperWarnings < 0)
		{
			numSniperWarnings = 0;
		}
	}

	private IEnumerator showStatusEffects()
	{
		bool hasWaited2 = false;
		while (true)
		{
			if (currentDisplayedStatus != null)
			{
				Object.Destroy(currentDisplayedStatus);
			}
			hasWaited2 = false;
			if (numSniperWarnings > 0)
			{
				changeStatus(Resources.Load("Icons/StatusEffects/SniperWarning"));
				hasWaited2 = true;
				yield return new WaitForSeconds(statusRotateTime);
			}
			if (isStunned)
			{
				changeStatus(Resources.Load("Icons/StatusEffects/Stun"));
				hasWaited2 = true;
				yield return new WaitForSeconds(statusRotateTime);
			}
			if (poisonCount > 0)
			{
				if (_poisonColor == PoisonColor.GREEN)
				{
					changeStatus(Resources.Load("Icons/StatusEffects/GreenPoison"));
				}
				else
				{
					changeStatus(Resources.Load("Icons/StatusEffects/PurplePoison"));
				}
				hasWaited2 = true;
				yield return new WaitForSeconds(statusRotateTime);
			}
			if (isDreamy)
			{
				changeStatus(Resources.Load("Icons/StatusEffects/QuestionMark"));
				hasWaited2 = true;
				yield return new WaitForSeconds(statusRotateTime);
			}
			if (hasPropbearganda)
			{
				changeStatus(Resources.Load("Icons/StatusEffects/Propbearganda"));
				hasWaited2 = true;
				yield return new WaitForSeconds(statusRotateTime);
			}
			if (hasCoffee)
			{
				changeStatus(Resources.Load("Icons/StatusEffects/HotCoffee"));
				hasWaited2 = true;
				yield return new WaitForSeconds(statusRotateTime);
			}
			if (slowCount > 0)
			{
				changeStatus(Resources.Load("Icons/StatusEffects/SlowMo"));
				hasWaited2 = true;
				yield return new WaitForSeconds(statusRotateTime);
			}
			if (speakIconName != string.Empty)
			{
				changeStatus(Resources.Load("Icons/TeamSpeak/" + speakIconName));
				hasWaited2 = true;
				yield return new WaitForSeconds(statusRotateTime);
			}
			if (!hasWaited2)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	public void SetRadiationDisplay(float percent)
	{
		int num = 0;
		num = ((percent <= 0f) ? (-1) : ((!(percent <= 0.25f)) ? ((percent <= 0.5f) ? 1 : ((!(percent <= 0.75f)) ? 3 : 2)) : 0));
		_radiationDisplay.Visible = num >= 0;
		_radiationDisplay.SetSegmentLevel(num);
		if (!isRemote && netSyncReporter != null && _previousRadiationLevel != num)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = OwnerID;
			hashtable[(byte)1] = num;
			NetSync.SetAction(47, hashtable);
		}
		_previousRadiationLevel = num;
	}

	public void RemoteSetRadiationDisplay(int level)
	{
		_radiationDisplay.Visible = level >= 0;
		_radiationDisplay.SetSegmentLevel(level);
	}

	public void OnPlayVictorySound()
	{
		if (winSayings.Length > 0)
		{
			AudioSource.PlayClipAtPoint(winSayings[Random.Range(0, winSayings.Length)], myAudio.transform.position, SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnPlayDefeatSound()
	{
		if (loseSayings.Length > 0)
		{
			AudioSource.PlayClipAtPoint(loseSayings[Random.Range(0, loseSayings.Length)], myAudio.transform.position, SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnPlayMeleeAttackSound()
	{
		if (meleeAttackSounds.Length > 0 && !isDead && Random.value < 0.25f)
		{
			myAudio.PlayOneShot(meleeAttackSounds[Random.Range(0, meleeAttackSounds.Length)], SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnPlayReloadSound()
	{
		if (reloadSounds.Length > 0 && !isDead && Random.value < 0.2f)
		{
			myAudio.PlayOneShot(reloadSounds[Random.Range(0, reloadSounds.Length)], SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnPlayMeleeKillSound()
	{
		if (meleeKillSounds.Length > 0 && !isDead)
		{
			myAudio.PlayOneShot(meleeKillSounds[Random.Range(0, meleeKillSounds.Length)], SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnPlayRegularKillSound()
	{
		if (regularKillSounds.Length > 0 && !isDead)
		{
			myAudio.PlayOneShot(regularKillSounds[Random.Range(0, regularKillSounds.Length)], SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnPlayTauntSound(AudioClip tauntSound)
	{
		if (tauntSound != null && !isDead)
		{
			myAudio.PlayOneShot(tauntSound, SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnPlayGenericSound(AudioClip genericSound)
	{
		if (genericSound != null)
		{
			myAudio.PlayOneShot(genericSound, SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnPlayBackstabSound()
	{
		if (backstabSound != null)
		{
			myAudio.PlayOneShot(backstabSound, SoundManager.Instance.getEffectsVolume());
		}
	}

	private void changeStatus(Object newStatus)
	{
		if (currentDisplayedStatus != null)
		{
			Object.Destroy(currentDisplayedStatus);
		}
		currentDisplayedStatus = Object.Instantiate(newStatus) as GameObject;
		currentDisplayedStatus.transform.parent = statusEffectMount;
		currentDisplayedStatus.transform.localPosition = Vector3.zero;
		currentDisplayedStatus.transform.localEulerAngles = Vector3.zero;
		currentDisplayedStatus.transform.localScale = new Vector3(1f, 1f, 1f);
		currentDisplayedStatus.layer = LayerMask.NameToLayer("Player");
	}

	public void GetSlowedByRemotePlayer(float slowDuration, float slowAmount, float slowPercentage)
	{
		if (!isRemote && StatManager != null)
		{
			StatisticMod mod = new StatisticMod(Statistic.MaxForwardMovementSpeed, slowDuration, slowAmount, slowPercentage);
			StatisticMod mod2 = new StatisticMod(Statistic.MaxBackwardsMovementSpeed, slowDuration, slowAmount, slowPercentage);
			StatisticMod mod3 = new StatisticMod(Statistic.MaxSidewaysMovementSpeed, slowDuration, slowAmount, slowPercentage);
			StatManager.AddStatMod(mod);
			StatManager.AddStatMod(mod2);
			StatManager.AddStatMod(mod3);
			StartSlowedDurationCoroutine(slowDuration, slowAmount);
		}
	}

	public void OnSlowed(float duration, float slowAmount)
	{
		if (!isRemote)
		{
			StartCoroutine(OnSlowedForDurationCoroutine(duration, slowAmount));
			if (slowAmount > 0f)
			{
				_statManager.AddStatMod(new StatisticMod(Statistic.MaxForwardMovementSpeed, duration, slowAmount));
				_statManager.AddStatMod(new StatisticMod(Statistic.MaxSidewaysMovementSpeed, duration, slowAmount));
				_statManager.AddStatMod(new StatisticMod(Statistic.MaxBackwardsMovementSpeed, duration, slowAmount));
			}
			else
			{
				_statManager.AddStatMod(new StatisticMod(Statistic.MaxForwardMovementSpeed, duration, 0f, 0.01f));
				_statManager.AddStatMod(new StatisticMod(Statistic.MaxSidewaysMovementSpeed, duration, 0f, 0.01f));
				_statManager.AddStatMod(new StatisticMod(Statistic.MaxBackwardsMovementSpeed, duration, 0f, 0.01f));
			}
		}
	}

	public void OnRemoteSlowed(float duration)
	{
		StartCoroutine(OnRemoteSlowedForDurationCoroutine(duration));
	}

	public void StartSlowedDurationCoroutine(float duration, float slowAmount)
	{
		StartCoroutine(OnSlowedForDurationCoroutine(duration, slowAmount));
	}

	private IEnumerator OnSlowedForDurationCoroutine(float duration, float slowAmount)
	{
		if (!isRemote)
		{
			ExitGames.Client.Photon.Hashtable parameters = new ExitGames.Client.Photon.Hashtable();
			parameters[(byte)0] = duration;
			if (DamageReceiver.linkedProxy != null)
			{
				OnStopSpecialItem(0f);
			}
			slowCount++;
			netSyncReporter.SetAction(9, parameters);
			yield return new WaitForSeconds(duration);
			slowCount--;
		}
	}

	private IEnumerator OnRemoteSlowedForDurationCoroutine(float duration)
	{
		slowCount++;
		yield return new WaitForSeconds(duration);
		slowCount--;
	}

	public void OnBirded(float duration, BIRD_MOUNT birdType, int birdOwnerId, float damage)
	{
		if (!isRemote && !isDead)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = duration;
			hashtable[(byte)1] = (int)birdType;
			netSyncReporter.SetAction(8, hashtable);
			addBird(duration, birdType, birdOwnerId, damage);
		}
	}

	public void OnRemoteHawked(float duration, BIRD_MOUNT birdType)
	{
		addBird(duration, birdType, -1, 0f);
	}

	public void OnLocalRadiationExplode(float startHealth, int shooterID)
	{
		OnDeath(shooterID, true, false, false, string.Empty);
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = shooterID;
		hashtable[(byte)1] = true;
		hashtable[(byte)2] = false;
		hashtable[(byte)3] = false;
		netSyncReporter.SetAction(7, hashtable);
		GameObject gameObject = Object.Instantiate(radiationExplosionPrefab, myTransform.position, myTransform.rotation) as GameObject;
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.OwnerID = shooterID;
		RadiationExplosionDamageSource component2 = gameObject.GetComponent<RadiationExplosionDamageSource>();
		component2.maxDamageDistance = 0f;
		component2.minDamageDistance = 0f;
		ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
		hashtable2[(byte)0] = OwnerID;
		hashtable2[(byte)1] = startHealth;
		hashtable2[(byte)2] = shooterID;
		netSyncReporter.SetAction(44, hashtable2);
	}

	public void OnRemoteRadiationExplode(float startHealth, int shooterID)
	{
		GameObject gameObject = Object.Instantiate(radiationExplosionPrefab, myTransform.position, myTransform.rotation) as GameObject;
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.OwnerID = shooterID;
		RadiationExplosionDamageSource component2 = gameObject.GetComponent<RadiationExplosionDamageSource>();
		component2.SetValuesFromHealth(startHealth);
		gameObject.transform.localScale = gameObject.transform.localScale * (startHealth / 300f);
	}

	private void addBird(float duration, BIRD_MOUNT birdType, int birdOwner = -1, float damage = 0f)
	{
		if (currentBird != null)
		{
			Object.Destroy(currentBird);
		}
		Vector3 localScale = new Vector3(1.3f, 1.3f, 1.3f);
		switch (birdType)
		{
		case BIRD_MOUNT.HAWK:
			currentBird = Object.Instantiate(hawkPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case BIRD_MOUNT.EAGLE:
			currentBird = Object.Instantiate(eaglePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case BIRD_MOUNT.PARASHOOT:
		{
			currentBird = Object.Instantiate(parashootPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			Poison poison = base.gameObject.AddComponent<Poison>();
			poison.showPoisonIcon = false;
			poison.poisonAmount = damage;
			poison.poisonDuration = duration;
			poison.poisonerPlayerID = birdOwner;
			localScale = new Vector3(0.5f, 0.5f, 0.5f);
			break;
		}
		}
		if (currentBird != null)
		{
			currentBird.transform.parent = hawkMount;
			currentBird.transform.localPosition = Vector3.zero;
			currentBird.transform.localEulerAngles = Vector3.zero;
			currentBird.transform.localScale = localScale;
			DelayedDestroy component = currentBird.GetComponent<DelayedDestroy>();
			if (component != null)
			{
				component.delay = duration;
			}
		}
	}

	public void OnStun(float stunTime)
	{
		if (!isRemote && !isDisabled && currentBombToGrab == null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = stunTime;
			netSyncReporter.SetAction(10, hashtable);
			speakIconName = string.Empty;
			StopCoroutine("stunCountdown");
			StartCoroutine("stunCountdown", stunTime);
		}
	}

	public void OnRemoteStun(float stunTime)
	{
		speakIconName = string.Empty;
		StopCoroutine("stunCountdown");
		StartCoroutine("stunCountdown", stunTime);
	}

	private IEnumerator stunCountdown(float stunTime)
	{
		isStunned = true;
		legAnimator.isDisabled = true;
		bodyAnimator.isDisabled = true;
		weaponManager.isDisabled = true;
		if (motor != null)
		{
			motor.SetVelocity(Vector3.zero);
			motor.SetControllable(false);
		}
		isDisabled = true;
		weaponManager.OnStopFiring();
		bodyAnimator.StopAllCoroutines();
		bodyAnimator.animation.Stop();
		legAnimator.animation.Stop();
		bodyAnimator.animation.Play("stun");
		if (weaponManager.CurrentWeapon.isRiggedWeapon)
		{
			weaponManager.CurrentWeapon.gameObject.SetActive(false);
		}
		yield return new WaitForSeconds(stunTime);
		if (weaponManager.CurrentWeapon.isRiggedWeapon)
		{
			weaponManager.CurrentWeapon.gameObject.SetActive(true);
		}
		if (!isDead)
		{
			legAnimator.isDisabled = false;
			if (!hasBomb)
			{
				weaponManager.isDisabled = false;
				bodyAnimator.isDisabled = false;
				legAnimator.OnReset();
				bodyAnimator.OnReset();
			}
			else
			{
				bodyAnimator.OnGetBomb();
			}
			weaponManager.OnResumeFromStun();
			isDisabled = false;
			isStunned = false;
			if (motor != null)
			{
				motor.SetControllable(true);
			}
		}
	}

	public void Freeze(float duration)
	{
		StartCoroutine(DisableCharacterCoroutine(duration));
		if (freezeEffect != null)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(freezeEffect, base.transform.position, base.transform.rotation);
			gameObject.transform.parent = base.transform;
			gameObject.AddComponent<DelayedDestroy>().delay = duration;
		}
	}

	private IEnumerator DisableCharacterCoroutine(float disableTime)
	{
		if (BodyAnimator != null)
		{
			BodyAnimator.Pause();
		}
		SimpleControllerPerformer perf = (SimpleControllerPerformer)Performer;
		if (!isRemote)
		{
			IsBombPickupAllowed = false;
			canControl = false;
			weaponManager.isDisabled = true;
			canSwitchWeapons = false;
			_canJump = false;
			weaponManager.OnTaunt();
			bodyAnimator.isDisabled = true;
			legAnimator.isDisabled = true;
			legAnimator.animation.Stop();
			motor.SetVelocity(Vector3.zero);
			motor.SetControllable(false);
			if (perf != null)
			{
				perf.Disabled = true;
			}
		}
		yield return new WaitForSeconds(disableTime);
		if (BodyAnimator != null)
		{
			BodyAnimator.UnPause();
		}
		if (!isRemote)
		{
			IsBombPickupAllowed = true;
			canControl = true;
			weaponManager.isDisabled = false;
			canSwitchWeapons = true;
			_canJump = true;
			weaponManager.EnableWeapons();
			bodyAnimator.isDisabled = false;
			legAnimator.isDisabled = false;
			legAnimator.OnReset();
			motor.SetControllable(true);
			if (perf != null)
			{
				perf.Disabled = false;
			}
		}
	}

	public void OnSetTeam(Team t)
	{
		Team = t;
		if (!isRemote)
		{
			getSpawnpoints(Team);
			HUD.Instance.OnSetTeam(Team);
			givePlayerRandomSpawnposition();
			spawn();
		}
	}

	private IEnumerator delayedInitialSpawn()
	{
		base.enabled = false;
		yield return new WaitForSeconds(2f);
		spawn();
	}

	public void OnTeamSpeak(int speakIndex)
	{
		if (Time.time - _timeOfLastTeamspeak > 0f && !isDead)
		{
			_timeOfLastTeamspeak = Time.time + HUD.Instance.teamspeakDelay;
			actualSpeak(speakIndex, true);
			if (netSyncReporter != null)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = speakIndex;
				netSyncReporter.SetAction(14, hashtable);
			}
		}
	}

	public void OnRemoteTeamSpeak(int speakIndex, bool showNotification)
	{
		actualSpeak(speakIndex, showNotification);
	}

	private void actualSpeak(int speakIndex, bool showNotification)
	{
		bool flag = false;
		string empty = string.Empty;
		switch (speakIndex)
		{
		case 0:
			speakIconName = "follow";
			break;
		case 1:
			speakIconName = "help";
			break;
		case 2:
			speakIconName = "attack";
			break;
		case 3:
			speakIconName = "incoming";
			break;
		case 4:
			if (!isRemote && LoadoutManager.Instance.CurrentLoadout != null && LoadoutManager.Instance.CurrentLoadout.taunt != null)
			{
				empty = LoadoutManager.Instance.CurrentLoadout.taunt.name;
			}
			if (string.IsNullOrEmpty(empty) || !motor.IsGrounded())
			{
				speakIconName = "woohoo";
			}
			else if (!HasBomb)
			{
				StartCoroutine("Taunt");
				flag = true;
				showNotification = false;
			}
			break;
		}
		if (myAudio != null)
		{
			if (flag && !string.IsNullOrEmpty(empty))
			{
				if (_tauntSound != null)
				{
					OnPlayTauntSound(_tauntSound);
				}
			}
			else if (teamspeakSayings[speakIndex] != null)
			{
				myAudio.PlayOneShot(teamspeakSayings[speakIndex], SoundManager.Instance.getEffectsVolume());
			}
		}
		StopCoroutine("delayedTeamIconShow");
		StartCoroutine("delayedTeamIconShow");
		if (showNotification && GameManager.Instance != null && HUD.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(OwnerID);
			if (playerCharacterManager != null)
			{
				HUD.Instance.OnSpeakNotification(playerCharacterManager.playerLoadout.model.name, playerCharacterManager.playerLoadout.skin.name, playerCharacterManager.team, speakIconName);
			}
		}
	}

	private IEnumerator delayedTeamIconShow()
	{
		yield return new WaitForSeconds(5f);
		speakIconName = string.Empty;
	}

	private IEnumerator Taunt()
	{
		if (!isRemote && Application.loadedLevelName != "Tutorial")
		{
			string tauntName = LoadoutManager.Instance.CurrentLoadout.taunt.name;
			ExitGames.Client.Photon.Hashtable parameters = new ExitGames.Client.Photon.Hashtable();
			parameters[(byte)0] = OwnerID;
			parameters[(byte)1] = tauntName;
			netSyncReporter.SetAction(49, parameters);
			tauntCamera = null;
			SimpleControllerPerformer perf2 = null;
			IsBombPickupAllowed = false;
			canControl = false;
			weaponManager.isDisabled = true;
			canSwitchWeapons = false;
			_canJump = false;
			weaponManager.OnTaunt();
			HUD.Instance.OnSetReloadDisplay(0f);
			aimer.localEulerAngles = new Vector3(0f, aimer.localEulerAngles.y, aimer.localEulerAngles.z);
			bodyRotator.rotation = aimer.rotation;
			bodyAnimator.isDisabled = true;
			legAnimator.isDisabled = true;
			legAnimator.animation.Stop();
			motor.SetVelocity(Vector3.zero);
			motor.SetControllable(false);
			perf2 = (SimpleControllerPerformer)Performer;
			if (perf2 != null)
			{
				perf2.Disabled = true;
			}
			playerCam.enabled = false;
			tauntCamera = Object.Instantiate(playerCam.gameObject, tauntCameraSpot.position, playerCam.transform.rotation) as GameObject;
			tauntCamera.name = "tauntCamera" + OwnerID;
			tauntCamera.camera.fieldOfView = 60f;
			Object.Destroy(tauntCamera.GetComponent<FoVAdjuster>());
			tauntCamera.transform.LookAt(myTransform.position);
			playerCam.camera.enabled = false;
			bodyAnimator.animation.CrossFade(tauntName);
			yield return new WaitForSeconds(bodyAnimator.animation[tauntName].length);
			IsBombPickupAllowed = true;
			canControl = true;
			weaponManager.isDisabled = false;
			canSwitchWeapons = true;
			_canJump = true;
			weaponManager.EnableWeapons();
			bodyAnimator.isDisabled = false;
			legAnimator.isDisabled = false;
			legAnimator.OnReset();
			motor.SetControllable(true);
			playerCam.enabled = true;
			playerCam.camera.enabled = true;
			Object.Destroy(tauntCamera);
			playerCam.OnResetNormalPosition();
			if (perf2 != null)
			{
				perf2.Disabled = false;
			}
		}
	}

	public void StartRemoteTaunt(string tauntName)
	{
		StartCoroutine("RemoteTaunt", tauntName);
	}

	private IEnumerator RemoteTaunt(string tauntName)
	{
		IsBombPickupAllowed = false;
		canControl = false;
		weaponManager.isDisabled = true;
		canSwitchWeapons = false;
		_canJump = false;
		weaponManager.OnTaunt();
		aimer.localEulerAngles = new Vector3(0f, aimer.localEulerAngles.y, aimer.localEulerAngles.z);
		bodyRotator.rotation = aimer.rotation;
		bodyAnimator.isDisabled = true;
		legAnimator.isDisabled = true;
		legAnimator.animation.Stop();
		bodyAnimator.animation.CrossFade(tauntName);
		yield return new WaitForSeconds(bodyAnimator.animation[tauntName].length);
		IsBombPickupAllowed = true;
		canControl = true;
		weaponManager.isDisabled = false;
		canSwitchWeapons = true;
		_canJump = true;
		weaponManager.EnableWeapons();
		bodyAnimator.isDisabled = false;
		legAnimator.isDisabled = false;
		legAnimator.OnReset();
	}

	private void getSpawnpoints(Team t)
	{
		Component[] array = Object.FindObjectsOfType(typeof(Spawnpoint)) as Component[];
		Component[] array2 = array;
		foreach (Component component in array2)
		{
			if ((component as Spawnpoint).team == t || Preferences.Instance.CurrentGameMode == GameMode.FFA)
			{
				spawnpoints.Add(component.transform);
			}
		}
	}

	private void givePlayerRandomSpawnposition()
	{
		if (spawnpoints.Count > 0)
		{
			int randomSpawnIndex = GetRandomSpawnIndex();
			Transform transform = (Transform)spawnpoints[Mathf.Min(randomSpawnIndex, spawnpoints.Count)];
			myTransform.position = transform.position;
			myTransform.rotation = transform.rotation;
		}
	}

	private int GetRandomSpawnIndex()
	{
		if (_isRoyaleMode)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			List<string> list = new List<string>();
			MatchEventsHelper.MatchInfo matchInfo = new MatchEventsHelper.MatchInfo();
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			foreach (PhotonPlayer photonPlayer in playerList)
			{
				string text = EventHelper.Hash(matchInfo.MatchId + photonPlayer.ID);
				list.Add(text);
				dictionary.Add(photonPlayer.ID, text);
			}
			list.Sort();
			int num = list.IndexOf(dictionary[PhotonNetwork.player.ID]);
			int num2 = spawnpoints.Count / PhotonNetwork.playerList.Length;
			return Random.Range(num2 * num, num2 * (num + 1));
		}
		return Random.Range(0, spawnpoints.Count);
	}

	public void spawn()
	{
		base.enabled = true;
		SimpleControllerPerformer simpleControllerPerformer = (SimpleControllerPerformer)Performer;
		if (simpleControllerPerformer != null)
		{
			simpleControllerPerformer.LockInputs = false;
		}
		motor.SetVelocity(Vector3.zero);
		motor.SetControllable(true);
		dmgReceiver.OnReset();
		isDisabled = false;
		isDead = false;
		isStunned = false;
		hasPropbearganda = false;
		hasCoffee = false;
		IsBombPickupAllowed = true;
		canControl = true;
		float val = 5f;
		ServiceManager.Instance.UpdateProperty("spawn_shield_duration", ref val);
		if (netSyncReporter != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = val;
			netSyncReporter.SetAction(34, hashtable);
		}
		DamageReceiver.OnInvincibility(val);
	}

	public void OnDeath(int shooterID, bool isExplosion, bool isMelee, bool isHeadshot, string customSfx = "")
	{
		if (!isRemote)
		{
			if (WeaponManager is SatelliteSecondaryWeaponManager)
			{
				SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)WeaponManager;
				satelliteSecondaryWeaponManager.ResetSecondaryCooldown();
			}
			StopCoroutine("Taunt");
			IsBombPickupAllowed = true;
			canControl = true;
			weaponManager.isDisabled = false;
			canSwitchWeapons = true;
			_canJump = true;
			weaponManager.EnableWeapons();
			bodyAnimator.isDisabled = false;
			legAnimator.isDisabled = false;
			legAnimator.OnReset();
			motor.SetControllable(true);
			playerCam.enabled = true;
			playerCam.camera.enabled = true;
			Object.Destroy(tauntCamera);
			playerCam.OnResetNormalPosition();
			SimpleControllerPerformer simpleControllerPerformer = (SimpleControllerPerformer)Performer;
			if (simpleControllerPerformer != null)
			{
				simpleControllerPerformer.Disabled = false;
			}
		}
		HUD.Instance.OnSetHealth(0f);
		SetRadiationDisplay(0f);
		if (deathSounds.Length > 0)
		{
			myAudio.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)], SoundManager.Instance.getEffectsVolume());
		}
		for (int i = 0; i < _numJoulesToSpawnPerDeath; i++)
		{
			PlayerJoulesManager.SpawnJoulesDrop();
		}
		if (hasBomb && !isRemote)
		{
			OnLocalReleaseBomb();
		}
		if (_isRoyaleMode && !isRemote)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add((byte)0, ownerID);
			hashtable.Add((byte)1, PhotonManager.Instance.ServerTimeInMilliseconds);
			NetSync.SetAction(68, hashtable);
		}
		OnReportDeath(shooterID, isExplosion, isMelee, isHeadshot, 0f, customSfx);
	}

	private void giveHoldingBombBonus()
	{
		int num = (int)(currentBombHoldTime * (double)bonusPerSecond);
		currentBombHoldTime = 0.0;
		GameManager.Instance.OnAddBombHoldBonus(OwnerID, num);
		HUD.Instance.OnAddBombHoldBonusNotification(Team, "+ " + num);
		if (NetSync != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = OwnerID;
			hashtable[(byte)1] = num;
			NetSync.SetAction(36, hashtable);
		}
	}

	private IEnumerator delayedRespawn(bool isExplosion, float delay)
	{
		Component[] specialAbilities = GetComponents(typeof(SpecialAbility));
		Component[] array = specialAbilities;
		foreach (Component c in array)
		{
			Object.Destroy(c);
		}
		if (currentSpecialItem != null)
		{
			Object.Destroy(currentSpecialItem);
		}
		if (currentDamageMultiplierEffect != null)
		{
			Object.Destroy(currentDamageMultiplierEffect);
		}
		if (weaponManager.CurrentWeapon != null)
		{
			weaponManager.CurrentWeapon.SendMessage("OnOwnerDead", SendMessageOptions.DontRequireReceiver);
		}
		numSniperWarnings = 0;
		if (currentBird != null)
		{
			Object.Destroy(currentBird);
		}
		if (currentBombToGrab != null && !isRemote)
		{
			OnLocalStopGrabbingBomb();
		}
		damageMultiplier = 1f;
		StopCoroutine("increasedDamageCountdown");
		StopCoroutine("stunCountdown");
		StopCoroutine("dreamCountdown");
		motor.movement.gravity = motor.movement.originalGravity;
		isDreamy = false;
		Component[] poisons = GetComponents<Poison>();
		Component[] array2 = poisons;
		foreach (Component c2 in array2)
		{
			Object.Destroy(c2);
		}
		if (currentBombTimer != null)
		{
			Object.Destroy(currentBombTimer);
		}
		SoundManager.Instance.MusicAudio.pitch = 1f;
		if (dreamFilter != null)
		{
			Object.Destroy(dreamFilter);
		}
		isStunned = false;
		isImmuneToStun = false;
		legAnimator.isDisabled = true;
		bodyAnimator.isDisabled = true;
		weaponManager.isDisabled = true;
		if (!isRemote)
		{
			motor.SetVelocity(Vector3.zero);
			motor.SetControllable(false);
			isDisabled = true;
			aimer.localEulerAngles = Vector3.zero;
			bodyRotator.rotation = aimer.rotation;
		}
		weaponManager.OnStopFiring();
		base.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		bodyAnimator.StopAllCoroutines();
		bodyAnimator.animation.Stop();
		legAnimator.animation.Stop();
		Vector3 originalScale = new Vector3(1f, 1f, 1f);
		GameObject deathCamera2 = null;
		if (!isRemote)
		{
			playerCam.enabled = false;
			playerCam.NormalPosition = deathCameraSpot.localPosition;
			deathCamera2 = Object.Instantiate(playerCam.gameObject, deathCameraSpot.position, playerCam.transform.rotation) as GameObject;
			deathCamera2.name = "deathCamera" + OwnerID;
			Object.Destroy(deathCamera2.GetComponent<FoVAdjuster>());
			deathCamera2.transform.LookAt(myTransform.position);
			playerCam.camera.enabled = false;
		}
		if (isExplosion && gibs != null)
		{
			if (spawnGibs)
			{
				GameObject newGibs = Object.Instantiate(gibs, myTransform.position, myTransform.rotation) as GameObject;
				newGibs.transform.localScale = myTransform.localScale;
				GibsController g = newGibs.GetComponent(typeof(GibsController)) as GibsController;
				g.skinMaterial = dmgReceiver.normalMaterial;
			}
			myTransform.localScale = Vector3.zero;
		}
		else
		{
			if (bodyAnimator.animation["death"] != null)
			{
				if (bodyAnimator.animation[weaponManager.CurrentWeapon.name + "_fireLoop"] != null)
				{
					bodyAnimator.animation[weaponManager.CurrentWeapon.name + "_fireLoop"].layer = 0;
				}
				bodyAnimator.animation["death"].layer = 3;
			}
			yield return null;
			bodyAnimator.animation.Play("death");
			weaponManager.CurrentWeapon.WeaponDeath();
		}
		spawnGibs = true;
		if (!isRemote && _isRoyaleMode)
		{
			_gui.RemoveAll();
			myTransform.localScale = Vector3.zero;
			yield break;
		}
		yield return new WaitForSeconds(4f - delay);
		myTransform.localScale = Vector3.zero;
		yield return new WaitForSeconds(1f);
		if (!isRemote)
		{
			HUD.Instance.OnShowStatsPage(false);
		}
		CharacterController charController = GetComponent<CharacterController>();
		if (motor != null)
		{
			motor.enabled = false;
		}
		if (charController != null)
		{
			charController.enabled = false;
		}
		givePlayerRandomSpawnposition();
		int respawnTime = 5;
		if (Preferences.Instance.CurrentGameMode == GameMode.CTF)
		{
			ServiceManager.Instance.UpdateProperty("ptb_respawn_time", ref respawnTime);
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.TB)
		{
			ServiceManager.Instance.UpdateProperty("respawn_time", ref respawnTime);
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
		{
			ServiceManager.Instance.UpdateProperty("ffa_respawn_time", ref respawnTime);
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.KOTH)
		{
			ServiceManager.Instance.UpdateProperty("koth_respawn_time", ref respawnTime);
		}
		yield return new WaitForSeconds(respawnTime);
	}

	public void RespawnPlayer()
	{
		CharacterController component = GetComponent<CharacterController>();
		GameObject gameObject = GameObject.Find("deathCamera" + OwnerID);
		legAnimator.isDisabled = false;
		bodyAnimator.isDisabled = false;
		weaponManager.isDisabled = false;
		SetRadiationDisplay(0f);
		legAnimator.OnReset();
		bodyAnimator.OnReset();
		weaponManager.OnReset();
		isDead = false;
		base.gameObject.layer = LayerMask.NameToLayer("Player");
		myTransform.localScale = Vector3.one;
		if (!isRemote)
		{
			_playerCharacterManager.SetAction(48, null);
			if (motor != null)
			{
				motor.enabled = true;
			}
			if (component != null)
			{
				component.enabled = true;
			}
			playerCam.OnResetNormalPosition();
			playerCam.camera.enabled = true;
			playerCam.enabled = true;
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
			HUD.Instance.OnReset();
			spawn();
		}
		DamageReceiver.Cloak = null;
	}

	public void OnReportDeath(int shooterID, bool isExplosion, bool isMelee, bool isHeadshot, float delay, string customDeathSfx = "")
	{
		if (!isDead)
		{
			if (isRemote)
			{
				StopCoroutine("RemoteTaunt");
				IsBombPickupAllowed = true;
				canControl = true;
				weaponManager.isDisabled = false;
				canSwitchWeapons = true;
				_canJump = true;
				weaponManager.EnableWeapons();
				bodyAnimator.isDisabled = false;
				legAnimator.isDisabled = false;
				legAnimator.OnReset();
			}
			isDead = true;
			weaponManager.OnDeath();
			if (!string.IsNullOrEmpty(customDeathSfx))
			{
				AudioClip customDeathSfx2 = GetCustomDeathSfx(customDeathSfx);
				myAudio.PlayOneShot(customDeathSfx2, SoundManager.Instance.getEffectsVolume());
			}
			StartCoroutine(delayedRespawn(isExplosion, delay));
			GameManager.Instance.OnPlayerKilled(shooterID, OwnerID, isExplosion, isMelee, isHeadshot);
		}
	}

	private AudioClip GetCustomDeathSfx(string sfxName)
	{
		if (!_customDeathSfxByName.ContainsKey(sfxName))
		{
			_customDeathSfxByName[sfxName] = Resources.Load<AudioClip>(sfxName);
		}
		return _customDeathSfxByName[sfxName];
	}

	public bool hasSpecialItem()
	{
		return specialItemPrefab != null;
	}

	public float OnUseSpecialItem(float delay = 0f)
	{
		if (currentSpecialItem != null || isDisabled || Time.time < nextSpecialItemChargeTime || specialItemPrefab == null)
		{
			return 10f;
		}
		SpecialItem specialItem = specialItemPrefab.GetComponent(typeof(SpecialItem)) as SpecialItem;
		if (specialItem.requiresGrounded && !motor.grounded)
		{
			return specialItem.cooldown;
		}
		InvisibilityCloak invisibilityCloak = GetComponent(typeof(InvisibilityCloak)) as InvisibilityCloak;
		if (invisibilityCloak != null)
		{
			Object.Destroy(invisibilityCloak);
		}
		if (camoCloak != null)
		{
			camoCloak.enabled = false;
		}
		currentSpecialItem = Object.Instantiate(specialItemPrefab) as GameObject;
		Transform transform = myTransform.Find("playerModel");
		currentSpecialItem.transform.position = transform.position;
		currentSpecialItem.transform.rotation = transform.rotation;
		currentSpecialItem.transform.localScale = transform.localScale;
		SpecialItem specialItem2 = currentSpecialItem.GetComponent(typeof(SpecialItem)) as SpecialItem;
		specialItem2.Activate(this, isRemote, delay);
		float num = specialItem2.cooldown * (1f - specialItemCooldownMultiplier / 100f);
		nextSpecialItemChargeTime = Time.time + num;
		lastSpecialItemUseTime = Time.time;
		if (_isRoyaleMode && !isRemote)
		{
			PlayerLoadout currentLoadout = LoadoutManager.Instance.CurrentLoadout;
			currentLoadout.special = null;
			_playerCharacterManager.SetSpecial(null);
		}
		return num;
	}

	public void OnStopSpecialAbilities(float delay)
	{
		BroadcastMessage("OnDeactivateSpecialAbility", SendMessageOptions.DontRequireReceiver);
	}

	public void OnStopSpecialItem(float delay)
	{
		if (currentSpecialItem != null)
		{
			SpecialItem specialItem = currentSpecialItem.GetComponent(typeof(SpecialItem)) as SpecialItem;
			specialItem.OnDeactivate(delay);
		}
	}

	public void OnGetPropbearganda(float bonus)
	{
		hasPropbearganda = true;
		propbeargandaBonus += bonus;
	}

	public void OnLosePropbearganda(float bonus)
	{
		propbeargandaBonus = Mathf.Max(0f, propbeargandaBonus - bonus);
		hasPropbearganda = false;
	}

	public void OnGetDreamy(float duration)
	{
		isDreamy = true;
		StopCoroutine("dreamCountdown");
		StartCoroutine("dreamCountdown", duration);
		if (!isRemote)
		{
			if (dreamFilter == null)
			{
				dreamFilter = BodyAnimator.gameObject.AddComponent<AudioReverbFilter>();
				dreamFilter.reverbPreset = AudioReverbPreset.Drugged;
			}
			SoundManager.Instance.MusicAudio.pitch = -1f;
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = duration;
			netSyncReporter.SetAction(11, hashtable);
		}
	}

	public void OnRemoteGetDreamy(float delay)
	{
		isDreamy = true;
		StopCoroutine("dreamCountdown");
		StartCoroutine("dreamCountdown", delay);
	}

	public void OnGetPoisoned()
	{
		if (!isRemote)
		{
			if (poisonCount == 0)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = (int)_poisonColor;
				netSyncReporter.SetAction(12, hashtable);
			}
			poisonCount++;
		}
	}

	public void OnRemoteGetPoisoned(PoisonColor color)
	{
		poisonCount = 1;
		_poisonColor = color;
	}

	public void OnRemovePoison()
	{
		if (!isRemote)
		{
			poisonCount--;
			if (poisonCount == 0)
			{
				netSyncReporter.SetAction(13, null);
			}
		}
	}

	public void OnRemoteRemovePoisoned()
	{
		poisonCount = 0;
	}

	private IEnumerator dreamCountdown(float duration)
	{
		yield return new WaitForSeconds(duration);
		isDreamy = false;
		if (!isRemote)
		{
			SoundManager.Instance.MusicAudio.pitch = 1f;
			if (dreamFilter != null)
			{
				Object.Destroy(dreamFilter);
			}
		}
	}

	private void usePowerup(GameObject powerup)
	{
		if (!(powerup != null))
		{
			return;
		}
		string[] array = powerup.name.Split(' ');
		JoulesPack component = powerup.GetComponent<JoulesPack>();
		if (component != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = component.OwnerID;
			hashtable[(byte)1] = component.Index;
			netSyncReporter.SetAction(40, hashtable);
			Object.Destroy(powerup);
		}
		else if (array.Length > 1)
		{
			int num = int.Parse(array[1]);
			ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
			hashtable2[(byte)0] = num;
			netSyncReporter.SetAction(31, hashtable2);
			if (PowerupManager.Instance != null)
			{
				PowerupManager.Instance.OnUsePowerup(num, 0);
			}
		}
		else
		{
			Object.Destroy(powerup);
		}
	}

	public void OnGetPowerGlove(GameObject glove, float damageMultiplier, float duration)
	{
		if (netSyncReporter != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = damageMultiplier;
			hashtable[(byte)1] = duration;
			netSyncReporter.SetAction(33, hashtable);
		}
		if (doubleDamagePickupSayings.Length > 0)
		{
			myAudio.PlayOneShot(doubleDamagePickupSayings[Random.Range(0, doubleDamagePickupSayings.Length)], SoundManager.Instance.getEffectsVolume());
		}
		HUD.Instance.OnAddPowerupNotification();
		OnIncreaseDamageMultiplier(damageMultiplier, duration);
		usePowerup(glove);
	}

	public void OnIncreaseDamageMultiplier(float newMultiplier, float duration)
	{
		if (damageMultiplier == 1f)
		{
			StartCoroutine("increasedDamageCountdown", duration);
		}
		else
		{
			StopCoroutine("increasedDamageCountdown");
			StartCoroutine("increasedDamageCountdown", duration);
		}
		damageMultiplier = newMultiplier;
	}

	public void OnGetShield(GameObject shield, float duration)
	{
		if (netSyncReporter != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = duration;
			netSyncReporter.SetAction(34, hashtable);
		}
		if (shieldPickupSayings.Length > 0)
		{
			myAudio.PlayOneShot(shieldPickupSayings[Random.Range(0, shieldPickupSayings.Length)], SoundManager.Instance.getEffectsVolume());
		}
		HUD.Instance.OnAddShieldsNotification();
		DamageReceiver.OnInvincibility(duration);
		usePowerup(shield);
	}

	private IEnumerator increasedDamageCountdown(float duration)
	{
		if (currentDamageMultiplierEffect != null)
		{
			Object.Destroy(currentDamageMultiplierEffect);
		}
		currentDamageMultiplierEffect = Object.Instantiate(damageMultiplierEffectPrefab) as GameObject;
		currentDamageMultiplierEffect.transform.parent = myTransform;
		currentDamageMultiplierEffect.transform.localPosition = Vector3.zero;
		currentDamageMultiplierEffect.transform.localEulerAngles = Vector3.zero;
		currentDamageMultiplierEffect.transform.localScale = new Vector3(1f, 1f, 1f);
		yield return new WaitForSeconds(duration);
		if (currentDamageMultiplierEffect != null)
		{
			Object.Destroy(currentDamageMultiplierEffect);
		}
		damageMultiplier = 1f;
	}

	public bool OnGetBasketGrande(GameObject basket, float healPercentIncrease)
	{
		if (dmgReceiver != null && HUD.Instance != null)
		{
			float num = dmgReceiver.addHealthPercentage(healPercentIncrease);
			HUD.Instance.OnAddChickenNotification("+" + (int)num);
			if (myAudio != null && chickenPickupSound != null)
			{
				myAudio.PlayOneShot(chickenPickupSound, SoundManager.Instance.getEffectsVolume());
			}
			usePowerup(basket);
		}
		return true;
	}

	public void OnGetJoulesPack(GameObject joulesPack, int joulesValue)
	{
		GameManager.Instance.playerStats[OwnerID].joulePacksCollected++;
		if (netSyncReporter != null)
		{
			netSyncReporter.SetAction(32, null);
		}
		HUD.Instance.OnAddJoulesNotification("+" + joulesValue);
		usePowerup(joulesPack);
	}

	public bool OnGetHealthPack(GameObject healthpack, float healValue)
	{
		float num = dmgReceiver.addHealth(healValue);
		HUD.Instance.OnAddHealthNotification("+" + (int)num);
		if (myAudio != null && healthPickupSound != null)
		{
			myAudio.PlayOneShot(healthPickupSound, SoundManager.Instance.getEffectsVolume());
		}
		usePowerup(healthpack);
		return true;
	}

	private void LateUpdate()
	{
		if (!isRemote && currentBombToGrab != null)
		{
			Vector3 center = currentBombToGrab.collider.bounds.center;
			Vector3 vector = center - (center - myTransform.position).normalized * 40f;
			if (motor.IsGrounded())
			{
				Motor.SetVelocity((vector - myTransform.position).normalized * 40f);
			}
			Vector3 eulerAngles = myTransform.eulerAngles;
			myTransform.LookAt(currentBombToGrab);
			float y = Mathf.LerpAngle(eulerAngles.y, myTransform.eulerAngles.y, 1.5f * Time.deltaTime);
			eulerAngles.y = y;
			myTransform.eulerAngles = eulerAngles;
		}
	}

	public void OnJumpPad(Vector3 jumpPower)
	{
		if (motor != null)
		{
			motor.SetVelocity(jumpPower);
		}
	}

	public void OnHealth(float newHealth)
	{
		if (base.enabled)
		{
			HUD.Instance.OnSetHealth(newHealth);
		}
	}
}
