using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerCharacterManager : NetSyncObject
{
	public Team team;

	public PlayerLoadout playerLoadout;

	private PlayerController playerController;

	private PlayerDamageReceiver dmgReceiver;

	private static readonly string _deathCameraPrefix = "deathCamera";

	public string socialPlatformName = string.Empty;

	public int level = 1;

	public int skill;

	private bool _startedMusic;

	private Component[] synchronizedAnimations;

	public string EquipmentNames
	{
		get
		{
			string text = string.Empty;
			if (playerLoadout != null)
			{
				if (playerLoadout.equipment1 != null)
				{
					text += playerLoadout.equipment1.name;
				}
				text += "|";
				if (playerLoadout.equipment2 != null)
				{
					text += playerLoadout.equipment2.name;
				}
				return text;
			}
			return text;
		}
	}

	public PlayerController PlayerController
	{
		get
		{
			return playerController;
		}
	}

	public void SetPrimary(Item primary)
	{
		bool flag = playerController.WeaponManager.CurrentWeaponIndex == 0;
		bool shouldSwitchWeapons = !flag && playerController.WeaponManager.CurrentWeapon.Item.type == Item.Types.melee.ToString();
		playerController.WeaponManager.PrimaryWeaponPrefab = Resources.Load(PathToWeapon(primary, playerLoadout)) as GameObject;
		TrySendActionAndReloadHUD(primary, 64);
		TryEquipWeapon(flag, shouldSwitchWeapons, 0, BelongsToB1000(primary));
	}

	public void SetSecondary(Item secondary)
	{
		bool flag = playerController.WeaponManager.CurrentWeaponIndex == 1;
		bool shouldSwitchWeapons = !flag && playerController.WeaponManager.CurrentWeapon.Item.type == Item.Types.melee.ToString();
		playerController.WeaponManager.SecondaryWeaponPrefab = Resources.Load(PathToWeapon(secondary, playerLoadout)) as GameObject;
		TrySendActionAndReloadHUD(secondary, 65);
		TryEquipWeapon(flag, shouldSwitchWeapons, 1, BelongsToB1000(secondary));
	}

	private bool BelongsToB1000(Item item)
	{
		return item.parent_id == ServiceManager.Instance.GetItemByName("B1000").id;
	}

	public void SetSpecial(Item special)
	{
		playerController.specialItemPrefab = ((special == null) ? null : (Resources.Load("Specials/" + special.name) as GameObject));
		TrySendActionAndReloadHUD(special, 66);
		if (!playerController.isRemote)
		{
			HUD.Instance.ReloadSpecialIcon();
		}
	}

	private void TryEquipWeapon(bool shouldEquipWeapon, bool shouldSwitchWeapons, int weaponIndex, bool belongsToB1000)
	{
		if (playerController.isRemote)
		{
			return;
		}
		if (playerController.WeaponManager.CurrentWeapon is IDeployableWeapon)
		{
			playerController.WeaponManager.OnForceReload();
		}
		if (shouldEquipWeapon)
		{
			playerController.WeaponManager.OnSetWeapon(weaponIndex);
			playerController.WeaponManager.OnInstantReload(weaponIndex, true);
		}
		else if (shouldSwitchWeapons)
		{
			if (!playerController.HasBomb && playerController.WeaponManager.OnNextWeapon())
			{
				playerController.WeaponManager.OnInstantReload(weaponIndex, true);
				HUD.Instance.SwitchedWeapons();
			}
		}
		else
		{
			if (belongsToB1000)
			{
				(playerController.WeaponManager as WeaponManagerB100).SpawnNotCurrentWeapon(weaponIndex);
			}
			playerController.WeaponManager.OnInstantReload(weaponIndex, true);
		}
	}

	private void TrySendActionAndReloadHUD(Item item, byte code)
	{
		if (!playerController.isRemote)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = ((item == null) ? (-1) : item.id);
			playerController.NetSync.SetAction(code, hashtable);
			HUD.Instance.OnSetTeam(playerController.Team);
		}
	}

	public void ChangePlayerLoadout(PlayerLoadout loadout, string model = "")
	{
		playerLoadout = loadout;
		if (!PlayerController.isRemote)
		{
			LoadoutManager.Instance.CurrentLoadout = playerLoadout;
			UnityEngine.Object.DestroyImmediate(GameObject.Find(_deathCameraPrefix + base.OwnerID));
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = LoadoutManager.Instance.CreateJSONForLoadout(playerLoadout);
			hashtable[(byte)1] = model;
			PlayerController.NetSync.SetAction(0, hashtable);
		}
		UnityEngine.Object.DestroyImmediate(PlayerController.gameObject);
		CreateObject(Vector3.zero, 0f, 0f, playerLoadout);
		PlayerController.OnPostCreate();
		PlayerController.DamageReceiver.OnPostCreate();
		if (!PlayerController.isRemote)
		{
			HUD.Instance.PlayerController = PlayerController;
			HUD.Instance.OnReset();
			HUD.Instance.PlayerController.spawn();
		}
	}

	public override GameObject CreateObject(Vector3 startPos, float startBaseY, float startAngleX)
	{
		return CreateObject(startPos, startBaseY, startAngleX, null);
	}

	public GameObject CreateObject(Vector3 startPos, float startBaseY, float startAngleX, PlayerLoadout loadout)
	{
		synchronizedAnimations = UnityEngine.Object.FindObjectsOfType(typeof(SynchronizedAnimation)) as Component[];
		PlayerParameterModel playerParameterModel = new PlayerParameterModel(PhotonManager.Instance.UserList[base.OwnerID].UserParameters);
		if (loadout == null)
		{
			playerLoadout = new PlayerLoadout();
			playerLoadout.model = ServiceManager.Instance.GetItemByName(playerParameterModel.Character);
			playerLoadout.skin = ServiceManager.Instance.GetItemByName(playerParameterModel.Skin);
			playerLoadout.primary = ServiceManager.Instance.GetItemByName(playerParameterModel.Primary);
			playerLoadout.secondary = ServiceManager.Instance.GetItemByName(playerParameterModel.Secondary);
			playerLoadout.melee = ServiceManager.Instance.GetItemByName(playerParameterModel.Melee);
			string special = playerParameterModel.Special;
			if (special != LoadoutManager.EMPTY_SLOT_NAME)
			{
				playerLoadout.special = ServiceManager.Instance.GetItemByName(special);
			}
			string equipmentOne = playerParameterModel.EquipmentOne;
			if (equipmentOne != LoadoutManager.EMPTY_SLOT_NAME)
			{
				playerLoadout.equipment1 = ServiceManager.Instance.GetItemByName(equipmentOne);
			}
			string equipmentTwo = playerParameterModel.EquipmentTwo;
			if (equipmentTwo != LoadoutManager.EMPTY_SLOT_NAME)
			{
				playerLoadout.equipment2 = ServiceManager.Instance.GetItemByName(equipmentTwo);
			}
			string taunt = playerParameterModel.Taunt;
			if (taunt != LoadoutManager.EMPTY_SLOT_NAME)
			{
				playerLoadout.taunt = ServiceManager.Instance.GetItemByName(taunt);
			}
		}
		else
		{
			playerLoadout = loadout;
		}
		socialPlatformName = playerParameterModel.SocialName;
		playerLoadout.pid = playerParameterModel.PlayerId;
		level = playerParameterModel.Level;
		skill = playerParameterModel.Skill;
		GameMode currentGameMode = Preferences.Instance.CurrentGameMode;
		if (currentGameMode == GameMode.FFA || currentGameMode == GameMode.ROYL)
		{
			team = ((base.OwnerID != PhotonManager.Instance.LocalUserID) ? Team.BLUE : Team.RED);
		}
		else
		{
			team = (Team)playerParameterModel.PlayerTeam;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Characters/" + playerLoadout.model.name + "/" + playerLoadout.model.name), startPos, Quaternion.Euler(new Vector3(0f, startBaseY, 0f)));
		CharacterHandle component = gameObject.GetComponent<CharacterHandle>();
		Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(LOD));
		socialPlatformName = socialPlatformName.Replace('_', ' ');
		gameObject.transform.Find("playerName/name").GetComponent<TextMesh>().text = socialPlatformName;
		string text = ((team != Team.BLUE) ? "_red" : "_blue");
		LoadSkin(text, component);
		Material cloakMaterial = Resources.Load("Materials/Characters/Cloaking/cloaking" + text) as Material;
		PlayerDamageReceiver playerDamageReceiver = gameObject.GetComponentInChildren(typeof(PlayerDamageReceiver)) as PlayerDamageReceiver;
		if (playerDamageReceiver != null)
		{
			playerDamageReceiver.normalMaterial = component.Skin;
			playerDamageReceiver.cloakMaterial = cloakMaterial;
			string path = "Skins/Default/" + BBRQuality.SkinQuality + "/damaged";
			Material material = Resources.Load(path) as Material;
			if (material == null)
			{
				if (component.Skin != null)
				{
					playerDamageReceiver.hitMaterial = new Material(component.Skin);
					playerDamageReceiver.hitMaterial.color = Color.white;
				}
			}
			else
			{
				playerDamageReceiver.hitMaterial = UnityEngine.Object.Instantiate(material) as Material;
				playerDamageReceiver.hitMaterial.mainTexture = component.Skin.mainTexture;
			}
		}
		Material teamMaterial = Resources.Load((team != 0) ? "goggleMaterialBlue" : "goggleMaterialRed") as Material;
		component.TeamMaterial = teamMaterial;
		Transform transform = (gameObject.GetComponentInChildren(typeof(AngleControllerPlaceholder)) as AngleControllerPlaceholder).transform;
		transform.localEulerAngles = new Vector3(startAngleX, 0f, 0f);
		playerController = gameObject.GetComponent<PlayerController>();
		playerController.CharacterManager = this;
		dmgReceiver = gameObject.GetComponent(typeof(PlayerDamageReceiver)) as PlayerDamageReceiver;
		playerController.OwnerID = base.OwnerID;
		dmgReceiver.OwnerID = base.OwnerID;
		playerController.WeaponManager.PrimaryWeaponPrefab = Resources.Load(PathToWeapon(playerLoadout.primary, playerLoadout)) as GameObject;
		playerController.WeaponManager.SecondaryWeaponPrefab = Resources.Load(PathToWeapon(playerLoadout.secondary, playerLoadout)) as GameObject;
		playerController.WeaponManager.MeleeWeaponPrefab = Resources.Load(PathToWeapon(playerLoadout.melee, playerLoadout)) as GameObject;
		if (playerLoadout.special != null)
		{
			playerController.specialItemPrefab = Resources.Load("Specials/" + playerLoadout.special.name) as GameObject;
		}
		if (base.OwnerID != PhotonManager.Instance.LocalUserID)
		{
			gameObject.name = "Remote Player: " + base.OwnerID;
			CharacterController characterController = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
			UnityEngine.Object.Destroy(gameObject.GetComponent(typeof(FPSInputController)));
			UnityEngine.Object.Destroy(gameObject.GetComponent(typeof(CharacterMotor)));
			UnityEngine.Object.Destroy(characterController);
			dmgReceiver.isRemote = true;
			Component[] componentsInChildren2 = gameObject.GetComponentsInChildren(typeof(Camera));
			Component[] array = componentsInChildren2;
			foreach (Component component2 in array)
			{
				if (component2 != null)
				{
					UnityEngine.Object.Destroy(component2.gameObject);
				}
			}
			playerController.isRemote = true;
			playerController.OnSetTeam(team);
			playerController.WeaponManager.isRemote = true;
		}
		else
		{
			CapsuleCollider capsuleCollider = gameObject.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			UnityEngine.Object.Destroy(capsuleCollider);
			Component[] array2 = componentsInChildren;
			foreach (Component component3 in array2)
			{
				UnityEngine.Object.Destroy(component3);
			}
			CameraFacingBillboard component4 = playerController.statusEffectMount.GetComponent<CameraFacingBillboard>();
			if (component4 != null)
			{
				UnityEngine.Object.Destroy(component4);
			}
			playerController.statusEffectMount.parent = transform;
			UnityEngine.Object.Destroy(gameObject.transform.Find("gogglesHighlight").gameObject);
			UnityEngine.Object.Destroy(gameObject.transform.Find("playerHighlight").gameObject);
			Transform transform2 = gameObject.transform.Find("playerName");
			if (transform2 != null)
			{
				UnityEngine.Object.Destroy(transform2.gameObject);
			}
			dmgReceiver.isRemote = false;
			gameObject.name = "Local Player: " + base.OwnerID;
			Component componentInChildren = gameObject.GetComponentInChildren(typeof(Camera));
			HUD.Instance.PlayerCamera = componentInChildren.transform;
			gameObject.transform.Find("playerModel").gameObject.AddComponent(typeof(AudioListener));
			playerController.isRemote = false;
			playerController.OnSetTeam(team);
			playerController.WeaponManager.isRemote = false;
			if (!_startedMusic)
			{
				_startedMusic = true;
				SoundManager.Instance.playMusic(GameManager.Instance.music, true);
			}
			GameManager.Instance.localPlayerID = base.OwnerID;
		}
		if (loadout == null)
		{
			GameManager.Instance.playerStats.Add(base.OwnerID, new PlayerStats(team, playerLoadout, socialPlatformName, level, skill, base.OwnerID));
		}
		else
		{
			if (GameManager.Instance.playerStats.ContainsKey(base.OwnerID))
			{
				GameManager.Instance.playerStats[base.OwnerID].playerLoadout = loadout;
			}
			obj = gameObject;
			NetSyncManager.Instance.RegisterNetSyncObject(netID, base.OwnerID, this, obj);
		}
		return gameObject;
	}

	private string PathToWeapon(Item weapon, PlayerLoadout loadout)
	{
		string text = "Characters/" + loadout.model.name;
		Item.Types types = (Item.Types)(int)Enum.Parse(typeof(Item.Types), weapon.type);
		switch (types)
		{
		case Item.Types.primary:
			text += "/PrimaryWeapons/";
			break;
		case Item.Types.secondary:
			text += "/SecondaryWeapons/";
			break;
		case Item.Types.melee:
			text += "/MeleeWeapons/";
			break;
		default:
			throw new Exception("No path defined for Item type " + types);
		}
		return text + weapon.name;
	}

	private bool TryLoadSkin(string teamName, CharacterHandle handle)
	{
		Material material = Resources.Load("Skins/" + playerLoadout.model.name + "/" + BBRQuality.SkinQuality + "/" + playerLoadout.skin.name + teamName) as Material;
		if (material == null)
		{
			material = Resources.Load("Skins/" + playerLoadout.model.name + "/" + BBRQuality.SkinQuality + "/" + playerLoadout.skin.name) as Material;
		}
		if (material != null)
		{
			handle.Skin = UnityEngine.Object.Instantiate(material) as Material;
			handle.Skin.mainTexture = Resources.Load("Characters/" + playerLoadout.model.name + "/Skins/" + BBRQuality.TextureQuality + "/" + playerLoadout.skin.name + teamName) as Texture2D;
			return true;
		}
		return false;
	}

	private void LoadSkin(string teamName, CharacterHandle handle)
	{
		if (!TryLoadSkin(teamName, handle))
		{
			string text = playerLoadout.skin.name;
			if (text.Contains("|"))
			{
				int length = text.IndexOf("|");
				text = text.Substring(0, length);
			}
			Material material = Resources.Load("Skins/Default/" + BBRQuality.SkinQuality + "/normal") as Material;
			if (material != null)
			{
				handle.Skin = UnityEngine.Object.Instantiate(material) as Material;
			}
			Texture2D texture2D = Resources.Load("Characters/" + playerLoadout.model.name + "/Skins/" + BBRQuality.TextureQuality + "/" + text + teamName) as Texture2D;
			if (texture2D != null)
			{
				handle.Skin.mainTexture = texture2D;
			}
		}
	}

	private void OnActivate()
	{
		PowerupManager.Instance.OnInitialSpawn();
		HUD.Instance.OnPlayTutorialAnimation(false);
		if (!GameManager.Instance.IsSynchronized && base.OwnerID == PhotonManager.Instance.LocalUserID)
		{
			StartCoroutine("delayedSynchronization");
		}
	}

	private IEnumerator delayedSynchronization()
	{
		yield return new WaitForSeconds(3f);
		for (int i = 0; i < PhotonManager.Instance.playerSkillUsers.Count; i++)
		{
			if (GameManager.Instance.IsSynchronized)
			{
				break;
			}
			if (playerLoadout.pid == PhotonManager.Instance.playerSkillUsers[i].id)
			{
				ExitGames.Client.Photon.Hashtable parameters = new ExitGames.Client.Photon.Hashtable();
				parameters[(byte)0] = PhotonManager.Instance.ServerTimeInMilliseconds;
				playerController.NetSync.SetAction(6, parameters);
				GameManager.Instance.StartTime = PhotonManager.Instance.ServerTimeInMilliseconds;
				GameManager.Instance.IsSynchronized = true;
				Component[] array = synchronizedAnimations;
				foreach (Component s in array)
				{
					(s as SynchronizedAnimation).OnStartup(0f);
				}
				break;
			}
			yield return new WaitForSeconds(5f);
		}
		if (!GameManager.Instance.IsSynchronized)
		{
			Debug.LogWarning("synchronization went wrong... setting time normally");
			GameManager.Instance.StartTime = PhotonManager.Instance.ServerTimeInMilliseconds;
			Component[] array2 = synchronizedAnimations;
			foreach (Component s2 in array2)
			{
				(s2 as SynchronizedAnimation).OnStartup(0f);
			}
		}
	}

	private void LogGame()
	{
		int @int = PlayerPrefs.GetInt(ServiceManager.Instance.GetStats().pid + "_battle", 1);
		if (@int <= 20)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("bear_name", playerLoadout.model.name);
			dictionary.Add("skill_color", Enum.GetName(typeof(Rank), (int)ServiceManager.GetRank(ServiceManager.Instance.GetStats().skill)));
			PlayerPrefs.SetInt(ServiceManager.Instance.GetStats().pid + "_battle", @int + 1);
		}
	}

	private void LogLoadout()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("primary_name", playerLoadout.primary.name);
		dictionary.Add("secondary_name", playerLoadout.secondary.name);
		dictionary.Add("melee_name", playerLoadout.melee.name);
		if (playerLoadout.special != null)
		{
			dictionary.Add("special_item_name", playerLoadout.special.name);
		}
		if (playerLoadout.equipment1 != null)
		{
			dictionary.Add("equipment1_name", playerLoadout.equipment1.name);
		}
		if (playerLoadout.equipment2 != null)
		{
			dictionary.Add("equipment2_name", playerLoadout.equipment2.name);
		}
	}

	private void addPlayer(int OwnerID)
	{
		GameManager.Instance.AddPlayer(OwnerID, this);
		if (OwnerID == PhotonManager.Instance.LocalUserID)
		{
			LogGame();
			LogLoadout();
			OnActivate();
		}
	}

	public override void PostCreate()
	{
		playerController.OnPostCreate();
		dmgReceiver.OnPostCreate();
		addPlayer(base.OwnerID);
	}

	public override void DestroyObject()
	{
		GameManager.Instance.OnPlayerHasLeft(base.OwnerID);
		UnityEngine.Object.Destroy(base.GameObject);
	}

	public override bool HandleStateChange(int state)
	{
		return false;
	}

	public override bool HandleActionChange(byte action, ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		if (delay > 3000)
		{
			delay = 3000;
		}
		else if (delay < 0)
		{
			delay = 0;
		}
		switch (action)
		{
		case 0:
			ChangeLoadoutAction(parameters);
			break;
		case 1:
			OnRequestBombAction(parameters);
			break;
		case 2:
			OnPickupBombAction(parameters, delay);
			break;
		case 3:
			OnResetBombAction(parameters);
			break;
		case 4:
			OnDepositBombAction(parameters);
			break;
		case 5:
			OnDropBombAction(parameters);
			break;
		case 6:
			OnSynchronizationMessageAction(parameters, delay);
			break;
		case 7:
			OnDeathAction(parameters, delay);
			break;
		case 8:
			OnBirdedAction(parameters, delay);
			break;
		case 9:
			OnSlowedAction(parameters);
			break;
		case 10:
			OnStunAction(parameters, delay);
			break;
		case 11:
			OnGetDreamyAction(parameters, delay);
			break;
		case 12:
			OnGetPoisonedAction(parameters);
			break;
		case 13:
			OnRemovePoisonedAction(parameters);
			break;
		case 14:
			OnTeamSpeakAction(parameters);
			break;
		case 15:
			OnStartSpecialAction(parameters, delay);
			break;
		case 16:
			OnStopSpecialAction(parameters, delay);
			break;
		case 17:
			OnStopSpecialAbilitiesAction(parameters, delay);
			break;
		case 18:
			OnRequestScoreAction(parameters);
			break;
		case 19:
			OnSetScoresAction(parameters);
			break;
		case 20:
			BeginFiringAction(parameters);
			break;
		case 21:
			StopFiringAction(parameters);
			break;
		case 22:
			SwitchWeaponAction(parameters);
			break;
		case 23:
			ReloadAction(parameters);
			break;
		case 24:
			OnStartAimingAction(parameters);
			break;
		case 25:
			OnStopAimingAction(parameters);
			break;
		case 26:
			OnSniperWarningAction(parameters);
			break;
		case 27:
			OnSniperWarningEndAction(parameters);
			break;
		case 28:
			OnHeadshotAction(parameters);
			break;
		case 29:
			OnHitPlayerAction(parameters);
			break;
		case 30:
			OnHitTurretAction(parameters);
			break;
		case 31:
			OnGetPowerupAction(parameters, delay);
			break;
		case 32:
			OnGetJoulePackAction(parameters);
			break;
		case 33:
			OnDamageMultiplierAction(parameters, delay);
			break;
		case 34:
			OnShieldAction(parameters, delay);
			break;
		case 35:
			OnTakeDamageAction(parameters);
			break;
		case 36:
			OnAddBombHoldBonusAction(parameters);
			break;
		case 37:
			OnAddBombDepositBonusAction(parameters);
			break;
		case 38:
			OnDeployableDetonatedAction(parameters);
			break;
		case 39:
			OnDiscDestroyedAction(parameters);
			break;
		case 40:
			OnGetSpawnedJoulesAction(parameters);
			break;
		case 41:
			SpawnJoulesAction(parameters);
			break;
		case 42:
			OnTurretFiredAction(parameters);
			break;
		case 43:
			OnInteractionPointAction(parameters);
			break;
		case 44:
			OnRadiationExplodeAction(parameters);
			break;
		case 45:
			OnChargedShotAction(parameters, delay);
			break;
		case 46:
			OnPartialChargedShotAction(parameters, delay);
			break;
		case 47:
			RadiationLevelAction(parameters);
			break;
		case 48:
			RespawnAction(parameters);
			break;
		case 49:
			TauntAction(parameters);
			break;
		case 50:
			FireSecondaryAction(parameters, delay);
			break;
		case 51:
			ActivateSecondaryWeaponAction(parameters);
			break;
		case 52:
			DeactivateSecondaryWeaponAction(parameters);
			break;
		case 53:
			SecondaryBeginFiringAction(parameters);
			break;
		case 54:
			SecondaryStopFiringAction(parameters);
			break;
		case 55:
			GetSlowedAction(parameters);
			break;
		case 56:
			DelayedRaycastFireAction(parameters);
			break;
		case 57:
			OnDiscProjectileFiredAction(parameters);
			break;
		case 58:
			UpdateKOTHScore(parameters);
			break;
		case 59:
			ChangeKOTHPoint(parameters);
			break;
		case 60:
			FireHomingMissile(parameters, delay);
			break;
		case 61:
			ApplyPoison(parameters);
			break;
		case 62:
			ApplyFreeze(parameters, delay);
			break;
		case 63:
			SpawnLoadoutSlotPowerups(parameters);
			break;
		case 64:
		{
			Item itemByID3 = ServiceManager.Instance.GetItemByID((int)parameters[(byte)0]);
			SetPrimary(itemByID3);
			break;
		}
		case 65:
		{
			Item itemByID2 = ServiceManager.Instance.GetItemByID((int)parameters[(byte)0]);
			SetSecondary(itemByID2);
			break;
		}
		case 66:
		{
			Item itemByID = ServiceManager.Instance.GetItemByID((int)parameters[(byte)0]);
			SetSpecial(itemByID);
			break;
		}
		case 68:
		{
			int id = (int)parameters[(byte)0];
			int deathTimeInMS = (int)parameters[(byte)1];
			GameManager.Instance.ReportDeathTime(id, deathTimeInMS);
			break;
		}
		default:
			Debug.LogError("No case for action: " + action);
			break;
		}
		return false;
	}

	protected void ChangeLoadoutAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		string text = parameters[(byte)0] as string;
		string text2 = parameters[(byte)1] as string;
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			PlayerLoadout loadout = LoadoutManager.Instance.CreateLoadout(playerLoadout.pid, text2, text, 0, true);
			ChangePlayerLoadout(loadout, string.Empty);
			if (GameManager.Instance.playerStats.ContainsKey(base.OwnerID))
			{
				GameManager.Instance.playerStats[base.OwnerID].playerLoadout = loadout;
			}
		}
	}

	protected void OnRequestBombAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		int pickupTime = (int)parameters[(byte)1];
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (playerCharacterManager != null && playerCharacterManager.PlayerController != null && CTFManager.Instance != null)
		{
			CTFManager.Instance.OnRemoteRequestBombPickup(playerCharacterManager.PlayerController, pickupTime);
		}
	}

	protected void OnPickupBombAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		int id = (int)parameters[(byte)0];
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (playerCharacterManager != null && playerCharacterManager.PlayerController != null && CTFManager.Instance != null)
		{
			CTFManager.Instance.OnRemoteBombPickup(playerCharacterManager.PlayerController, delay);
		}
	}

	protected void OnResetBombAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (playerCharacterManager != null && playerCharacterManager.PlayerController != null)
		{
			playerCharacterManager.PlayerController.OnRemoteResetBomb();
		}
	}

	protected void OnDepositBombAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (playerCharacterManager != null && playerCharacterManager.PlayerController != null)
		{
			playerCharacterManager.PlayerController.OnRemoteDepositBomb();
		}
	}

	protected void OnDropBombAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		float x = (float)parameters[(byte)1];
		float y = (float)parameters[(byte)2];
		float z = (float)parameters[(byte)3];
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (playerCharacterManager != null && playerCharacterManager.PlayerController != null)
		{
			playerCharacterManager.PlayerController.OnRemoteDropBomb(new Vector3(x, y, z));
		}
	}

	protected void OnSynchronizationMessageAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		if (GameManager.Instance != null && !GameManager.Instance.IsSynchronized)
		{
			StopCoroutine("delayedSynchronization");
			GameManager.Instance.IsSynchronized = true;
			int startTime = (int)parameters[(byte)0];
			GameManager.Instance.StartTime = startTime;
			Component[] array = synchronizedAnimations;
			foreach (Component component in array)
			{
				(component as SynchronizedAnimation).OnStartup((float)delay / 1000f);
			}
		}
	}

	protected void OnDeathAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		int shooterID = (int)parameters[(byte)0];
		bool isExplosion = (bool)parameters[(byte)1];
		bool isMelee = (bool)parameters[(byte)2];
		bool isHeadshot = (bool)parameters[(byte)3];
		string customDeathSfx = parameters[(byte)4] as string;
		if (playerController != null)
		{
			playerController.OnReportDeath(shooterID, isExplosion, isMelee, isHeadshot, (float)delay / 1000f, customDeathSfx);
		}
	}

	protected void OnBirdedAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		float num = (float)parameters[(byte)0];
		if (playerController != null)
		{
			int birdType = (int)parameters[(byte)1];
			playerController.OnRemoteHawked(num - (float)delay / 1000f, (BIRD_MOUNT)birdType);
		}
	}

	protected void OnSlowedAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		float duration = (float)parameters[(byte)0];
		if (playerController != null)
		{
			playerController.OnRemoteSlowed(duration);
		}
	}

	protected void OnStunAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		float num = (float)parameters[(byte)0];
		if (playerController != null)
		{
			playerController.OnRemoteStun(num - (float)delay / 1000f);
		}
	}

	protected void OnGetDreamyAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		float num = (float)parameters[(byte)0];
		if (playerController != null)
		{
			playerController.OnRemoteGetDreamy(num - (float)delay / 1000f);
		}
	}

	protected void OnGetPoisonedAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int color = (int)parameters[(byte)0];
		if (playerController != null)
		{
			playerController.OnRemoteGetPoisoned((PoisonColor)color);
		}
	}

	protected void OnRemovePoisonedAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (playerController != null)
		{
			playerController.OnRemoteRemovePoisoned();
		}
	}

	protected void OnTeamSpeakAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int speakIndex = (int)parameters[(byte)0];
		if (HUD.Instance != null && HUD.Instance.PlayerController != null && playerController != null)
		{
			bool showNotification = team == HUD.Instance.PlayerController.Team;
			playerController.OnRemoteTeamSpeak(speakIndex, showNotification);
		}
	}

	protected void OnStartSpecialAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		if (playerController != null)
		{
			playerController.OnUseSpecialItem((float)delay / 1000f);
		}
	}

	protected void OnStopSpecialAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		if (playerController != null)
		{
			playerController.OnStopSpecialItem((float)delay / 1000f);
		}
	}

	protected void OnStopSpecialAbilitiesAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		if (playerController != null)
		{
			playerController.OnStopSpecialAbilities((float)delay / 1000f);
		}
	}

	protected void OnRequestScoreAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (!(GameManager.Instance != null))
		{
			return;
		}
		foreach (PlayerCharacterManager playerCharacterManager in GameManager.Instance.GetPlayerCharacterManagers())
		{
			if (playerCharacterManager.playerController != null && playerCharacterManager.playerController.NetSync != null)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = GameManager.Instance.RedKills;
				hashtable[(byte)1] = GameManager.Instance.BlueKills;
				playerCharacterManager.playerController.NetSync.SetAction(19, hashtable);
			}
		}
	}

	protected void OnSetScoresAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (GameManager.Instance != null)
		{
			int redKills = (int)parameters[(byte)0];
			int blueKills = (int)parameters[(byte)0];
			GameManager.Instance.RedKills = redKills;
			GameManager.Instance.BlueKills = blueKills;
		}
	}

	protected void BeginFiringAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (playerController != null && playerController.WeaponManager != null)
		{
			if (playerController.WeaponManager.CurrentWeapon.isChargeable)
			{
				playerController.WeaponManager.OnBeginCharging();
			}
			else
			{
				playerController.WeaponManager.OnRemoteBeginFiring();
			}
		}
	}

	protected void StopFiringAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (playerController != null && playerController.WeaponManager != null)
		{
			if (playerController.WeaponManager.CurrentWeapon.isChargeable)
			{
				playerController.WeaponManager.CurrentWeapon.EndCharging();
			}
			else
			{
				playerController.WeaponManager.OnRemoteStopFiring();
			}
		}
	}

	protected void SwitchWeaponAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int weaponIndex = (int)parameters[(byte)0];
		if (playerController != null && playerController.WeaponManager != null)
		{
			playerController.WeaponManager.OnRemoteSetWeapon(weaponIndex);
		}
	}

	protected void ReloadAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (playerController != null && playerController.WeaponManager != null)
		{
			playerController.WeaponManager.OnRemoteReload();
		}
	}

	protected void OnStartAimingAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (playerController != null)
		{
			playerController.gameObject.BroadcastMessage("OnRemoteStartAiming", SendMessageOptions.DontRequireReceiver);
		}
	}

	protected void OnStopAimingAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (playerController != null)
		{
			playerController.gameObject.BroadcastMessage("OnRemoteStopAiming", SendMessageOptions.DontRequireReceiver);
		}
	}

	protected void OnSniperWarningAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int num = (int)parameters[(byte)0];
		if (!(HUD.Instance != null) || !(HUD.Instance.PlayerController != null) || !(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(num);
		if (playerCharacterManager != null && playerCharacterManager.playerController != null)
		{
			playerCharacterManager.playerController.OnAddSniperWarning();
			if (HUD.Instance.PlayerController == playerCharacterManager.playerController && HUD.Instance.sniperWarningSound != null)
			{
				AudioSource.PlayClipAtPoint(HUD.Instance.sniperWarningSound, Vector3.zero);
			}
		}
		else
		{
			Debug.LogWarning("this should never be null in OnSniperWarning, but it is for owner id: " + num);
		}
	}

	protected void OnSniperWarningEndAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int num = (int)parameters[(byte)0];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(num);
			if (playerCharacterManager != null && playerCharacterManager.playerController != null)
			{
				playerCharacterManager.playerController.OnRemoveSniperWarning();
			}
			else
			{
				Debug.LogWarning("this should never be null in OnSniperWarningEnd, but it is for owner id: " + num);
			}
		}
	}

	protected void OnHeadshotAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		if (!(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (playerCharacterManager != null)
		{
			PlayerDamageReceiver playerDamageReceiver = playerCharacterManager.dmgReceiver;
			if (playerDamageReceiver != null && (!playerDamageReceiver.isInvincible || GameManager.Instance.IsGameSubmitted))
			{
				playerDamageReceiver.OnTakeDamage(playerDamageReceiver.CurrentHP, base.OwnerID, false, false, true, true, GameManager.Instance.IsGameSubmitted, 0f, string.Empty);
				playerDamageReceiver.OnUpdateProxy();
			}
		}
	}

	protected void OnHitPlayerAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int num = (int)parameters[(byte)0];
		float num2 = (float)parameters[(byte)1];
		int num3 = (int)parameters[(byte)2];
		float radiationDmg = 0f;
		if (parameters.Count > 3)
		{
			radiationDmg = (float)parameters[(byte)3];
		}
		if (!(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(num);
		if (playerCharacterManager != null && playerCharacterManager.dmgReceiver != null)
		{
			GameManager.Instance.playerStats[num3].addDamageDealt(num, num2);
			if (!playerCharacterManager.dmgReceiver.isInvincible || GameManager.Instance.IsGameSubmitted)
			{
				playerCharacterManager.dmgReceiver.OnTakeDamage(num2, num3, false, false, false, false, GameManager.Instance.IsGameSubmitted, radiationDmg, string.Empty);
				playerCharacterManager.dmgReceiver.OnUpdateProxy();
			}
		}
	}

	protected void OnHitTurretAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		float dmg = (float)parameters[(byte)1];
		int shooterID = (int)parameters[(byte)2];
		int num = (int)parameters[(byte)3];
		float radiationDmg = 0f;
		if (parameters.Count > 4)
		{
			radiationDmg = (float)parameters[(byte)4];
		}
		if (!(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (!(playerCharacterManager != null) || !(playerCharacterManager.dmgReceiver != null))
		{
			return;
		}
		DeployableTurret[] array = UnityEngine.Object.FindObjectsOfType(typeof(DeployableTurret)) as DeployableTurret[];
		DeployableTurret[] array2 = array;
		foreach (DeployableTurret deployableTurret in array2)
		{
			if (deployableTurret.deployableIndex == num && deployableTurret.OwningPlayer != null && deployableTurret.OwningPlayer.OwnerID == playerCharacterManager.OwnerID)
			{
				DamageReceiver componentInChildren = deployableTurret.GetComponentInChildren<DamageReceiver>();
				if (componentInChildren != null)
				{
					componentInChildren.OnTakeDamage(dmg, shooterID, false, false, false, false, GameManager.Instance.IsGameSubmitted, radiationDmg, string.Empty);
				}
			}
		}
	}

	protected void OnGetPowerupAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		int index = (int)parameters[(byte)0];
		if (PowerupManager.Instance != null)
		{
			PowerupManager.Instance.OnUsePowerup(index, delay);
		}
	}

	protected void OnGetJoulePackAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerStats[base.OwnerID].joulePacksCollected++;
		}
	}

	protected void OnDamageMultiplierAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		float newMultiplier = (float)parameters[(byte)0];
		float num = (float)parameters[(byte)1];
		if (playerController != null)
		{
			playerController.OnIncreaseDamageMultiplier(newMultiplier, num - (float)delay / 1000f);
		}
	}

	protected void OnShieldAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		float num = (float)parameters[(byte)0];
		if (playerController != null && playerController.DamageReceiver != null)
		{
			playerController.DamageReceiver.OnInvincibility(num - (float)delay / 1000f);
		}
	}

	protected void OnTakeDamageAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int playerID = (int)parameters[(byte)0];
		float damage = (float)parameters[(byte)1];
		int num = (int)parameters[(byte)2];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(num);
			if (playerCharacterManager != null)
			{
				GameManager.Instance.playerStats[num].addDamageDealt(playerID, damage);
			}
		}
	}

	protected void OnAddBombHoldBonusAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int holderID = (int)parameters[(byte)0];
		int amount = (int)parameters[(byte)1];
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnAddBombHoldBonus(holderID, amount);
		}
	}

	protected void OnAddBombDepositBonusAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int holderID = (int)parameters[(byte)0];
		int amount = (int)parameters[(byte)1];
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnAddBombDepositBonus(holderID, amount);
		}
	}

	protected void OnDeployableDetonatedAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		int num = (int)parameters[(byte)1];
		int num2 = (int)parameters[(byte)2];
		if (!(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (!(playerCharacterManager != null) || !(playerCharacterManager.PlayerController != null))
		{
			return;
		}
		DeployableObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(DeployableObject)) as DeployableObject[];
		DeployableObject[] array2 = array;
		foreach (DeployableObject deployableObject in array2)
		{
			if (deployableObject.deployableIndex == num2 && deployableObject.OwningPlayer != null && deployableObject.OwningPlayer.OwnerID == playerCharacterManager.OwnerID)
			{
				deployableObject.OnDetonateDeployable(playerCharacterManager.PlayerController, false);
			}
		}
	}

	protected void OnDiscDestroyedAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		int num = (int)parameters[(byte)1];
		if (!(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (!(playerCharacterManager != null) || !(playerCharacterManager.PlayerController != null))
		{
			return;
		}
		DiscController[] array = UnityEngine.Object.FindObjectsOfType(typeof(DiscController)) as DiscController[];
		DiscController[] array2 = array;
		foreach (DiscController discController in array2)
		{
			if (discController.DiscIndex == num && discController.Owner != null && discController.Owner.OwnerID == playerCharacterManager.OwnerID)
			{
				discController.OnDestroyDisc();
			}
		}
	}

	protected void OnGetSpawnedJoulesAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		int index = (int)parameters[(byte)1];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager != null && playerCharacterManager.PlayerController != null)
			{
				playerCharacterManager.PlayerController.PlayerJoulesManager.DespawnJoulesDrop(index);
			}
		}
	}

	protected void SpawnJoulesAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		int index = (int)parameters[(byte)1];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager != null && playerCharacterManager.PlayerController != null)
			{
				playerCharacterManager.PlayerController.PlayerJoulesManager.SpawnJoulesDrop(index);
			}
		}
	}

	protected void OnTurretFiredAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		int num = (int)parameters[(byte)1];
		int num2 = (int)parameters[(byte)2];
		if (!(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (!(playerCharacterManager != null) || !(playerCharacterManager.PlayerController != null))
		{
			return;
		}
		DeployableTurret[] array = UnityEngine.Object.FindObjectsOfType(typeof(DeployableTurret)) as DeployableTurret[];
		DeployableTurret[] array2 = array;
		foreach (DeployableTurret deployableTurret in array2)
		{
			if (deployableTurret.deployableIndex == num2 && deployableTurret.OwningPlayer != null && deployableTurret.OwningPlayer.OwnerID == playerCharacterManager.OwnerID)
			{
				float x = (float)parameters[(byte)3];
				float y = (float)parameters[(byte)4];
				float z = (float)parameters[(byte)5];
				deployableTurret.OnRemoteAttack(new Vector3(x, y, z));
			}
		}
	}

	protected void OnInteractionPointAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int index = (int)parameters[(byte)0];
		int num = (int)parameters[(byte)1];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(num);
			if (playerCharacterManager != null && playerCharacterManager.PlayerController != null && playerCharacterManager.PlayerController.isRemote)
			{
				InteractionPointManager.Instance.OnRemoteTriggerInteractionPoint(index, num);
			}
		}
	}

	protected void OnRadiationExplodeAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		float startHealth = (float)parameters[(byte)1];
		int num = (int)parameters[(byte)2];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			PlayerCharacterManager playerCharacterManager2 = GameManager.Instance.Players(num);
			if (playerCharacterManager != null && playerCharacterManager.PlayerController != null && playerCharacterManager.PlayerController.isRemote)
			{
				playerCharacterManager.PlayerController.OnRemoteRadiationExplode(startHealth, num);
			}
		}
	}

	protected void OnChargedShotAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		int id = (int)parameters[(byte)0];
		int num = (int)parameters[(byte)1];
		float x = (float)parameters[(byte)2];
		float y = (float)parameters[(byte)3];
		float z = (float)parameters[(byte)4];
		float x2 = (float)parameters[(byte)5];
		float y2 = (float)parameters[(byte)6];
		float z2 = (float)parameters[(byte)7];
		Vector3 pos = new Vector3(x, y, z);
		Vector3 vel = new Vector3(x2, y2, z2);
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			playerCharacterManager.PlayerController.WeaponManager.OnRemoteFireChargedShot(pos, vel, delay);
		}
	}

	protected void OnPartialChargedShotAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		int id = (int)parameters[(byte)0];
		int num = (int)parameters[(byte)1];
		float x = (float)parameters[(byte)2];
		float y = (float)parameters[(byte)3];
		float z = (float)parameters[(byte)4];
		float x2 = (float)parameters[(byte)5];
		float y2 = (float)parameters[(byte)6];
		float z2 = (float)parameters[(byte)7];
		float charge = (float)parameters[(byte)8];
		Vector3 pos = new Vector3(x, y, z);
		Vector3 vel = new Vector3(x2, y2, z2);
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			playerCharacterManager.PlayerController.WeaponManager.OnRemoteFire(pos, vel, charge, delay);
		}
	}

	protected void RadiationLevelAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		int num = (int)parameters[(byte)1];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			playerCharacterManager.PlayerController.RemoteSetRadiationDisplay(num);
		}
	}

	protected void RespawnAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		PlayerController.RespawnPlayer();
	}

	protected void TauntAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		string tauntName = (string)parameters[(byte)1];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			playerCharacterManager.PlayerController.StartRemoteTaunt(tauntName);
		}
	}

	protected void ActivateSecondaryWeaponAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager.PlayerController.WeaponManager is SatelliteSecondaryWeaponManager)
			{
				SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)playerCharacterManager.playerController.WeaponManager;
				satelliteSecondaryWeaponManager.RemoteActivateSecondaryWeapon();
			}
		}
	}

	protected void DeactivateSecondaryWeaponAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager.PlayerController.WeaponManager is SatelliteSecondaryWeaponManager)
			{
				SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)playerCharacterManager.playerController.WeaponManager;
				satelliteSecondaryWeaponManager.RemoteDeactivateSecondaryWeapon();
			}
		}
	}

	protected void SecondaryBeginFiringAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager.PlayerController.WeaponManager is SatelliteSecondaryWeaponManager)
			{
				SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)playerCharacterManager.playerController.WeaponManager;
				satelliteSecondaryWeaponManager.RemoteStartSecondaryConstantFire();
			}
		}
	}

	protected void SecondaryStopFiringAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager.PlayerController.WeaponManager is SatelliteSecondaryWeaponManager)
			{
				SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)playerCharacterManager.playerController.WeaponManager;
				satelliteSecondaryWeaponManager.RemoteStopSecondaryConstantFire();
			}
		}
	}

	protected void UpdateKOTHScore(ExitGames.Client.Photon.Hashtable parameters)
	{
		int num = (int)parameters[(byte)0];
		Team team = (Team)(int)parameters[(byte)1];
		int score = (int)parameters[(byte)2];
		if (KOTHManager.Instance != null)
		{
			KOTHManager.Instance.SetScore(team, score);
		}
	}

	protected void ChangeKOTHPoint(ExitGames.Client.Photon.Hashtable parameters)
	{
		int num = (int)parameters[(byte)0];
		int pointIndex = (int)parameters[(byte)1];
		if (KOTHManager.Instance != null)
		{
			KOTHManager.Instance.ChangePoint(pointIndex);
		}
	}

	protected void FireHomingMissile(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		int id = (int)parameters[(byte)72];
		float x = (float)parameters[(byte)98];
		float y = (float)parameters[(byte)99];
		float z = (float)parameters[(byte)100];
		float x2 = (float)parameters[(byte)101];
		float y2 = (float)parameters[(byte)102];
		float z2 = (float)parameters[(byte)103];
		float num = (int)parameters[(byte)83];
		int targetId = (int)parameters[(byte)111];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager != null && playerCharacterManager.playerController != null && playerCharacterManager.playerController.WeaponManager != null)
			{
				playerCharacterManager.PlayerController.WeaponManager.OnRemoteFireWithTarget(new Vector3(x, y, z), new Vector3(x2, y2, z2), delay, targetId);
			}
		}
	}

	protected void ApplyPoison(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		float poisonAmount = (float)parameters[(byte)1];
		int poisonerPlayerID = (int)parameters[(byte)2];
		float poisonDuration = (float)parameters[(byte)3];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager != null && playerCharacterManager.dmgReceiver != null && (!playerCharacterManager.dmgReceiver.isInvincible || GameManager.Instance.IsGameSubmitted) && playerCharacterManager.playerController != null && playerCharacterManager.playerController.gameObject != null)
			{
				Poison poison = playerCharacterManager.playerController.gameObject.AddComponent<Poison>();
				poison.poisonAmount = poisonAmount;
				poison.poisonDuration = poisonDuration;
				poison.poisonerPlayerID = poisonerPlayerID;
			}
		}
	}

	protected void ApplyFreeze(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		int id = (int)parameters[(byte)0];
		float duration = (float)parameters[(byte)1];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager != null && (!playerCharacterManager.dmgReceiver.isInvincible || GameManager.Instance.IsGameSubmitted) && playerCharacterManager.playerController != null && playerCharacterManager.playerController.gameObject != null)
			{
				playerCharacterManager.playerController.Freeze(duration);
			}
		}
	}

	private void SpawnLoadoutSlotPowerups(ExitGames.Client.Photon.Hashtable parameters)
	{
		LoadoutSlotPowerupManager loadoutSlotPowerupManager = PowerupManager.Instance as LoadoutSlotPowerupManager;
		loadoutSlotPowerupManager.RemoteInitialSpawn((byte[])parameters[(byte)0]);
	}

	protected void FireSecondaryAction(ExitGames.Client.Photon.Hashtable parameters, int delay)
	{
		int id = (int)parameters[(byte)0];
		float x = (float)parameters[(byte)1];
		float y = (float)parameters[(byte)2];
		float z = (float)parameters[(byte)3];
		float x2 = (float)parameters[(byte)4];
		float y2 = (float)parameters[(byte)5];
		float z2 = (float)parameters[(byte)6];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			if (playerCharacterManager != null && playerCharacterManager.playerController != null && playerCharacterManager.playerController.WeaponManager != null)
			{
				playerCharacterManager.PlayerController.WeaponManager.RemoteFireSecondary(new Vector3(x, y, z), new Vector3(x2, y2, z2), delay);
			}
		}
	}

	protected void GetSlowedAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		float slowDuration = (float)parameters[(byte)1];
		float slowAmount = (float)parameters[(byte)2];
		float slowPercentage = (float)parameters[(byte)3];
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
			playerCharacterManager.PlayerController.GetSlowedByRemotePlayer(slowDuration, slowAmount, slowPercentage);
		}
	}

	protected void DelayedRaycastFireAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		float x = (float)parameters[(byte)1];
		float y = (float)parameters[(byte)2];
		float z = (float)parameters[(byte)3];
		if (!(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (playerCharacterManager != null && playerCharacterManager.PlayerController != null && playerCharacterManager.PlayerController.WeaponManager != null)
		{
			DelayableRaycastWeapon delayableRaycastWeapon = (DelayableRaycastWeapon)playerCharacterManager.PlayerController.WeaponManager.CurrentWeapon;
			if (delayableRaycastWeapon != null)
			{
				delayableRaycastWeapon.DoRemoteFire(new Vector3(x, y, z));
			}
		}
	}

	protected void OnDiscProjectileFiredAction(ExitGames.Client.Photon.Hashtable parameters)
	{
		int id = (int)parameters[(byte)0];
		int num = (int)parameters[(byte)1];
		if (!(GameManager.Instance != null))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(id);
		if (!(playerCharacterManager != null) || !(playerCharacterManager.PlayerController != null))
		{
			return;
		}
		ProjectileDiscController[] array = UnityEngine.Object.FindObjectsOfType(typeof(ProjectileDiscController)) as ProjectileDiscController[];
		ProjectileDiscController[] array2 = array;
		foreach (ProjectileDiscController projectileDiscController in array2)
		{
			if (projectileDiscController.DiscIndex == num && projectileDiscController.Owner != null && projectileDiscController.Owner.OwnerID == playerCharacterManager.OwnerID)
			{
				projectileDiscController.FireProjectile();
			}
		}
	}

	public override bool HandleFireProjectile(Vector3 pos, Vector3 vel, int delay)
	{
		playerController.WeaponManager.OnRemoteFire(pos, vel, delay);
		return false;
	}
}
