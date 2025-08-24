using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class WeaponManager : WeaponManagerBase
{
	protected struct IncreaseAmmoOptions
	{
		public float Delay;

		public float StartTime;

		public int Quantity;

		public float TargetTime
		{
			get
			{
				return StartTime + Delay;
			}
		}

		public IncreaseAmmoOptions(float delay, float startTime, int quantity)
		{
			Delay = delay;
			StartTime = startTime;
			Quantity = quantity;
		}

		public float TimeLeft()
		{
			return Delay - (Time.time - StartTime);
		}
	}

	protected int[] currentClipSizes;

	protected int[] oldTime;

	protected int[] newTime;

	private float[] _reloadTimes;

	private float[] _lastReloadStartTimes;

	protected BodyAnimator bodyAnimator;

	protected Coroutine _increaseAmmoRoutine;

	protected IncreaseAmmoOptions _increaseAmmoOptions = default(IncreaseAmmoOptions);

	public float PrimaryReloadTime
	{
		get
		{
			return _reloadTimes[0];
		}
	}

	public float PrimaryReloadStartTime
	{
		get
		{
			return _lastReloadStartTimes[0];
		}
	}

	public float SecondaryReloadTime
	{
		get
		{
			return _reloadTimes[1];
		}
	}

	public float SecondaryReloadStartTime
	{
		get
		{
			return _lastReloadStartTimes[1];
		}
	}

	public override bool isConstantFireMode()
	{
		if (currentWeapon != null)
		{
			return currentWeapon.isConstantFire;
		}
		return true;
	}

	public override bool isSingleFireMode()
	{
		if (currentWeapon != null)
		{
			return currentWeapon.isSingleFire;
		}
		return true;
	}

	public override bool isChargableFireMode()
	{
		if (currentWeapon != null)
		{
			return currentWeapon.isChargeable;
		}
		return true;
	}

	public override void Awake()
	{
		currentClipSizes = new int[weaponPrefabs.Length];
		oldTime = new int[weaponPrefabs.Length];
		newTime = new int[weaponPrefabs.Length];
		_reloadTimes = new float[weaponPrefabs.Length];
		_lastReloadStartTimes = new float[weaponPrefabs.Length];
		base.Awake();
		bodyAnimator = GetComponentInChildren(typeof(BodyAnimator)) as BodyAnimator;
	}

	protected void Start()
	{
		for (int i = 0; i < weaponPrefabs.Length; i++)
		{
			if (weaponPrefabs[i] != null)
			{
				WeaponBase component = weaponPrefabs[i].GetComponent<WeaponBase>();
				if (component != null && (component.isMine || component.isTurret))
				{
					OnInstantReload(i);
				}
			}
		}
	}

	protected void resetClipSizes()
	{
		for (int i = 0; i < weaponPrefabs.Length; i++)
		{
			if (weaponPrefabs[i] == null)
			{
				StartCoroutine(delayedResetClipSizeOnWeapon(i));
				continue;
			}
			WeaponBase component = weaponPrefabs[i].GetComponent<WeaponBase>();
			if (!component.isMine && !component.isTurret)
			{
				OnInstantReload(i);
			}
		}
	}

	private IEnumerator delayedResetClipSizeOnWeapon(int index)
	{
		yield return new WaitForSeconds(0.25f);
		if (weaponPrefabs[index] != null)
		{
			WeaponBase weaponBase = weaponPrefabs[index].GetComponent<WeaponBase>();
			if (!weaponBase.isMine && !weaponBase.isTurret)
			{
				OnInstantReload(index);
			}
		}
	}

	public override void OnGetBomb()
	{
		StopAllCoroutines();
		playerController.canSwitchWeapons = false;
		base.IsReloading = false;
		if (currentWeapon != null)
		{
			Object.Destroy(currentWeapon.gameObject);
		}
		if (leftWeapon != null)
		{
			Object.Destroy(leftWeapon.gameObject);
		}
	}

	public override void OnTaunt()
	{
		OnGetBomb();
	}

	public override void OnReset()
	{
		resetClipSizes();
		StopAllCoroutines();
		_increaseAmmoRoutine = null;
		base.IsReloading = false;
		for (int i = 0; i < weaponPrefabs.Length; i++)
		{
			canFireWeapon[i] = true;
		}
		currentWeaponIndex = 0;
		OnSetWeapon(currentWeaponIndex);
	}

	public override void OnPostCreate()
	{
		base.OnPostCreate();
		resetClipSizes();
	}

	public override void OnMeleeAttack()
	{
		if (!base.isAttackingMelee && !isDisabled && !playerController.HasBomb)
		{
			base.OnMeleeAttack();
			StopCoroutine("continuousFire");
			StopCoroutine("remoteContinuousFire");
			StartCoroutine("meleeAttack");
		}
	}

	private IEnumerator meleeAttack()
	{
		if (!isRemote && netSyncReporter != null)
		{
			ExitGames.Client.Photon.Hashtable parameters = new ExitGames.Client.Photon.Hashtable();
			parameters[(byte)0] = 2;
			netSyncReporter.SetAction(22, parameters);
		}
		base.isAttackingMelee = true;
		if (currentWeapon != null)
		{
			Object.Destroy(currentWeapon.gameObject);
		}
		if (leftWeapon != null)
		{
			Object.Destroy(leftWeapon.gameObject);
		}
		GameObject newWeapon = Object.Instantiate(meleeWeaponPrefab) as GameObject;
		newWeapon.name = meleeWeaponPrefab.name;
		currentWeapon = newWeapon.GetComponent(typeof(WeaponBase)) as WeaponBase;
		if (currentWeapon.hand == WeaponMountPoint.Side.RIGHT)
		{
			newWeapon.transform.parent = rightWeaponMountpoint;
		}
		else
		{
			newWeapon.transform.parent = leftWeaponMountpoint;
		}
		newWeapon.transform.localPosition = Vector3.zero;
		newWeapon.transform.localEulerAngles = Vector3.zero;
		newWeapon.transform.localScale = new Vector3(1f, 1f, 1f);
		currentWeapon.isRemote = isRemote;
		MeleeWeapon i = currentWeapon as MeleeWeapon;
		if (i.useBothHands)
		{
			GameObject newLeftWeapon = Object.Instantiate(meleeWeaponPrefab) as GameObject;
			leftWeapon = newLeftWeapon.GetComponent(typeof(WeaponBase)) as WeaponBase;
			newLeftWeapon.name = meleeWeaponPrefab.name;
			if (currentWeapon.hand == WeaponMountPoint.Side.RIGHT)
			{
				newLeftWeapon.transform.parent = leftWeaponMountpoint;
			}
			else
			{
				newLeftWeapon.transform.parent = rightWeaponMountpoint;
			}
			newLeftWeapon.transform.localPosition = Vector3.zero;
			newLeftWeapon.transform.localEulerAngles = Vector3.zero;
			newLeftWeapon.transform.localScale = new Vector3(1f, 1f, 1f);
			leftWeapon.isRemote = isRemote;
			leftWeapon.OwnerID = ownerID;
			leftWeapon.OnAttack();
		}
		currentWeapon.OwnerID = ownerID;
		DisableCloak();
		if (bodyAnimator != null)
		{
			bodyAnimator.OnSetWeapon(currentWeapon, true);
		}
		currentWeapon.OnAttack();
		float attackDuration = bodyAnimator.OnMeleeAttack();
		yield return new WaitForSeconds(attackDuration);
		base.isAttackingMelee = false;
		if (!base.isAttackingMelee)
		{
			OnSetWeapon(currentWeaponIndex);
			if (base.IsReloading)
			{
				bodyAnimator.OnReload(currentWeapon.reloadTime);
				currentWeapon.PlayReloadAnimation();
			}
		}
	}

	public override bool OnNextWeapon()
	{
		if (base.isAttackingMelee || isDisabled || !playerController.canSwitchWeapons)
		{
			return false;
		}
		currentWeaponIndex++;
		if (currentWeaponIndex >= weaponPrefabs.Length)
		{
			currentWeaponIndex = 0;
		}
		StopCoroutine("OnReload");
		if (_increaseAmmoRoutine != null)
		{
			StopCoroutine(_increaseAmmoRoutine);
		}
		StopCoroutine("remoteContinuousFire");
		StopCoroutine("continuousFire");
		base.IsReloading = false;
		OnSetWeapon(currentWeaponIndex);
		return true;
	}

	public override void OnRemoteSetWeapon(int weaponIndex)
	{
		StopCoroutine("remoteContinuousFire");
		if (weaponIndex == 2)
		{
			OnMeleeAttack();
		}
		else
		{
			OnSetWeapon(weaponIndex);
		}
	}

	public override void OnResumeFromStun()
	{
		OnSetWeapon(currentWeaponIndex);
	}

	public override void OnSetWeapon(int weaponIndex)
	{
		if (isDisabled)
		{
			return;
		}
		if (weaponIndex < 0 || weaponIndex >= weaponPrefabs.Length)
		{
			weaponIndex = 0;
		}
		if (!isRemote && netSyncReporter != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = weaponIndex;
			netSyncReporter.SetAction(22, hashtable);
		}
		base.isAttackingMelee = false;
		currentWeapon = EquipWeapon(weaponIndex);
		currentWeapon.NetSyncReporter = netSyncReporter;
		currentWeapon.OnFindAimer();
		if (playerController != null && playerController.CamoCloak != null)
		{
			playerController.CamoCloak.updateWeapon();
		}
		currentWeaponIndex = weaponIndex;
		currentWeapon.OwnerID = ownerID;
		currentWeapon.isRemote = isRemote;
		currentWeapon.LastReloadStart = _lastReloadStartTimes[currentWeaponIndex];
		currentWeapon.reloadTime = ((!(_reloadTimes[currentWeaponIndex] > 0f)) ? currentWeapon.reloadTime : _reloadTimes[currentWeaponIndex]);
		if (bodyAnimator != null)
		{
			bodyAnimator.OnSetWeapon(currentWeapon, currentWeapon is MeleeWeapon);
		}
		if (!isRemote && HUD.Instance != null)
		{
			HUD.Instance.OnSetAmmo(currentClipSizes[weaponIndex] ^ oldTime[weaponIndex]);
		}
		if (!Bootloader.Instance.InTutorial)
		{
			StopCoroutine("startFiringDelay");
			float num = (float)(PhotonManager.Instance.ServerTimeInMilliseconds - currentWeapon.LastFiredTime) / 1000f;
			if (num > 0f && num < currentWeapon.firingTime)
			{
				StartCoroutine("startFiringDelay", currentWeapon.firingTime - num);
			}
		}
		if ((currentWeapon.isMine || currentWeapon.isTurret) && !isRemote && HUD.Instance != null && !base.IsReloading)
		{
			HUD.Instance.OnSetReloadDisplay(0f);
		}
		if ((currentClipSizes[weaponIndex] ^ oldTime[weaponIndex]) <= 0)
		{
			if (base.gameObject.activeInHierarchy && !currentWeapon.isMine && !currentWeapon.isTurret && bodyAnimator != null)
			{
				if (!base.IsReloading)
				{
					StopCoroutine("OnReload");
					StartCoroutine("OnReload", 0f);
				}
			}
			else
			{
				OnHideCurrentWeapon();
			}
		}
		else if (!isRemote && HUD.Instance != null)
		{
			HUD.Instance.OnSetReloadDisplay(0f);
		}
	}

	protected virtual WeaponBase EquipWeapon(int weaponIndex)
	{
		if (currentWeapon != null)
		{
			Object.Destroy(currentWeapon.gameObject);
		}
		if (leftWeapon != null)
		{
			Object.Destroy(leftWeapon.gameObject);
		}
		GameObject gameObject = Object.Instantiate(weaponPrefabs[weaponIndex]) as GameObject;
		WeaponBase component = gameObject.GetComponent<WeaponBase>();
		gameObject.name = weaponPrefabs[weaponIndex].name;
		if (component.isRiggedWeapon)
		{
			Transform bodyRotator = playerController.bodyRotator;
			if (bodyRotator != null)
			{
				component.transform.parent = bodyRotator;
			}
			else
			{
				Debug.LogError("Could not find aimer transform!");
			}
		}
		else if (component.hand == WeaponMountPoint.Side.LEFT)
		{
			gameObject.transform.parent = leftWeaponMountpoint;
		}
		else if (component.hand == WeaponMountPoint.Side.RIGHT)
		{
			gameObject.transform.parent = rightWeaponMountpoint;
		}
		else if (component.hand == WeaponMountPoint.Side.BACK)
		{
			gameObject.transform.parent = backWeaponMountpoint;
		}
		else
		{
			Debug.Log("no hand set");
			gameObject.transform.parent = rightWeaponMountpoint;
		}
		gameObject.transform.localPosition = component.mountedPosition;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.localScale = component.mountedScale;
		if (!isRemote)
		{
			LOD componentInChildren = gameObject.GetComponentInChildren<LOD>();
			if (componentInChildren != null)
			{
				componentInChildren.setHighMesh();
				Object.Destroy(componentInChildren);
			}
		}
		return component;
	}

	protected float GetAttackDuration()
	{
		if (currentWeapon.isPrefabSpawner)
		{
			return currentWeapon.firingTime;
		}
		return bodyAnimator.AttackDuration;
	}

	public void OnWeaponConfigured()
	{
		if (currentWeapon != null && (currentWeapon.isMine || currentWeapon.isTurret))
		{
			OnShowCurrentWeapon();
		}
	}

	public override void OnForceReload()
	{
		if (isDisabled)
		{
			return;
		}
		IDeployableWeapon deployableWeapon = currentWeapon as IDeployableWeapon;
		if (deployableWeapon != null && deployableWeapon.IsAttacking)
		{
			return;
		}
		if (!currentWeapon.isMine && !currentWeapon.isTurret)
		{
			if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) != 0 && (currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) != currentWeapon.clipSize)
			{
				newTime[currentWeaponIndex] = (int)(Time.time * 1000f);
				currentClipSizes[currentWeaponIndex] = 0 ^ newTime[currentWeaponIndex];
				oldTime[currentWeaponIndex] = newTime[currentWeaponIndex];
				OnSetWeapon(currentWeaponIndex);
			}
		}
		else
		{
			if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) >= currentWeapon.clipSize || HUD.Instance.ReloadInProgress())
			{
				return;
			}
			bool isReloading = base.IsReloading;
			DeployableObject[] array = Object.FindObjectsOfType(typeof(DeployableObject)) as DeployableObject[];
			DeployableObject[] array2 = array;
			foreach (DeployableObject deployableObject in array2)
			{
				if (deployableObject.OwningPlayer.OwnerID == base.OwnerID && deployableObject.weaponIndex == base.CurrentWeaponIndex)
				{
					deployableObject.OnDetonateDeployable(base.PlayerController, false);
				}
			}
			if (!isReloading)
			{
				OnDelayedIncreaseAmmo(currentWeapon.reloadTime, currentWeapon.clipSize);
			}
		}
	}

	public override void OnStopFiring()
	{
		if (base.CurrentWeapon == null)
		{
			return;
		}
		base.OnStopFiring();
		if (currentWeapon.isConstantFire)
		{
			currentWeapon.EndConstantFireEffects();
			if (canFireWeapon[currentWeaponIndex])
			{
				bodyAnimator.OnStopContinuousAttack();
				bodyAnimator.OnIdle();
			}
			if (!isRemote)
			{
				if (netSyncReporter != null)
				{
					netSyncReporter.SetAction(21, null);
				}
				StopCoroutine("continuousFire");
			}
			else
			{
				currentWeapon.EndConstantFireEffects();
				StopCoroutine("remoteContinuousFire");
			}
			if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) <= 0 && canFireWeapon[currentWeaponIndex] && !currentWeapon.isMine && !currentWeapon.isTurret)
			{
				StopCoroutine("OnReload");
				StartCoroutine("OnReload", GetAttackDuration());
			}
		}
		else if (currentWeapon.isChargeable)
		{
			if (canFireWeapon[currentWeaponIndex] || currentWeapon.isFireInLoopOut)
			{
				bodyAnimator.OnIdle();
				currentWeapon.EndCharging();
			}
			if (!isRemote && netSyncReporter != null)
			{
				netSyncReporter.SetAction(21, null);
			}
			if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) <= 0 && canFireWeapon[currentWeaponIndex] && !currentWeapon.isMine && !currentWeapon.isTurret)
			{
				StopCoroutine("OnReload");
				StartCoroutine("OnReload", GetAttackDuration());
			}
		}
	}

	public override void OnRemoteStopFiring()
	{
		OnStopFiring();
	}

	public override void OnBeginFiring()
	{
		if (base.isAttackingMelee || isDisabled || !canFireWeapon[currentWeaponIndex])
		{
			return;
		}
		base.OnBeginFiring();
		if (!currentWeapon.isConstantFire)
		{
			return;
		}
		if (!isRemote)
		{
			if (netSyncReporter != null)
			{
				netSyncReporter.SetAction(20, null);
			}
			StopCoroutine("continuousFire");
			StartCoroutine("continuousFire");
		}
		else
		{
			StopCoroutine("remoteContinuousFire");
			StartCoroutine("remoteContinuousFire");
		}
	}

	private IEnumerator remoteContinuousFire()
	{
		bodyAnimator.OnBeginContinuousAttack();
		currentWeapon.BeginConstantFireEffects();
		while (true)
		{
			SubtractOneAmmoFromCurrentWeapon();
			if (!Bootloader.Instance.InTutorial)
			{
				currentWeapon.LastFiredTime = PhotonManager.Instance.ServerTimeInMilliseconds;
			}
			DisableCloak();
			currentWeapon.OnCurrentAmmo(currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]);
			if (!(currentWeapon is ProjectileWeapon))
			{
				currentWeapon.OnRemoteAttack(Vector3.zero, Vector3.zero, 0);
			}
			yield return new WaitForSeconds(currentWeapon.firingTime);
		}
	}

	public override void OnBeginCharging()
	{
		if (!base.isAttackingMelee && !isDisabled && canFireWeapon[currentWeaponIndex])
		{
			base.OnBeginCharging();
			if (netSyncReporter != null)
			{
				netSyncReporter.SetAction(20, null);
			}
			bodyAnimator.OnWeaponCharge();
			currentWeapon.BeginCharging();
		}
	}

	public override void OnRemoteReload()
	{
		currentWeapon.EndConstantFireEffects();
		StopCoroutine("remoteReload");
		StopCoroutine("remoteContinuousFire");
		StartCoroutine(remoteReload());
	}

	private IEnumerator remoteReload()
	{
		base.IsReloading = true;
		currentWeapon.StartedReloading();
		int reloadingIndex = base.CurrentWeaponIndex;
		canFireWeapon[reloadingIndex] = false;
		yield return null;
		float reloadTime = currentWeapon.reloadTime;
		_reloadTimes[reloadingIndex] = reloadTime;
		currentWeapon.LastReloadStart = Time.fixedTime;
		_lastReloadStartTimes[reloadingIndex] = currentWeapon.LastReloadStart;
		bodyAnimator.OnReload(currentWeapon.reloadTime);
		currentWeapon.OnPlayReloadSound();
		currentWeapon.PlayReloadAnimation();
		yield return new WaitForSeconds(currentWeapon.reloadTime - 0.1f);
		currentWeapon.OnReload();
		yield return new WaitForSeconds(0.1f);
		base.PlayerController.OnPlayReloadSound();
		currentWeapon.OnIdle();
		canFireWeapon[reloadingIndex] = true;
		base.IsReloading = false;
	}

	public override void OnRemoteBeginFiring()
	{
		OnBeginFiring();
	}

	private IEnumerator continuousFire()
	{
		while (!canFireWeapon[currentWeaponIndex])
		{
			yield return new WaitForSeconds(0.1f);
			if (netSyncReporter != null)
			{
				netSyncReporter.SetAction(20, null);
			}
		}
		bodyAnimator.OnBeginContinuousAttack();
		currentWeapon.BeginConstantFireEffects();
		while (true)
		{
			if (!canFireWeapon[currentWeaponIndex])
			{
				yield return new WaitForSeconds(0.1f);
				if (canFireWeapon[currentWeaponIndex])
				{
					bodyAnimator.OnBeginContinuousAttack();
					currentWeapon.BeginConstantFireEffects();
					if (netSyncReporter != null)
					{
						netSyncReporter.SetAction(20, null);
					}
				}
				continue;
			}
			if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) > 0)
			{
				if (currentWeapon.OnAttack())
				{
					HandleContinuousFireWeaponDidAttack();
				}
				float whenDoneAttacking = Time.time + currentWeapon.firingTime;
				while (Time.time < whenDoneAttacking && (currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) > 0)
				{
					yield return null;
				}
			}
			Item item = ServiceManager.Instance.GetItemByName(weaponPrefabs[currentWeaponIndex].name);
			int baseClipSize = currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex];
			if (playerController != null && playerController.CharacterManager != null)
			{
				item.UpdateProperty("clipSize", ref baseClipSize, playerController.CharacterManager.EquipmentNames);
				float stockPrice = 0f;
				ServiceManager.Instance.UpdateProperty("stock_quote", ref stockPrice);
				if (stockPrice < 0f)
				{
					int clipModifier = 0;
					item.UpdateProperty("market_down_clipSize_change", ref clipModifier, playerController.CharacterManager.EquipmentNames);
					baseClipSize += clipModifier;
				}
			}
			if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) <= 0 || ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) > baseClipSize && !currentWeapon.isMine && !currentWeapon.isTurret))
			{
				currentWeapon.EndConstantFireEffects();
				StopCoroutine("OnReload");
				StartCoroutine("OnReload", 0f);
			}
		}
	}

	private void HandleContinuousFireWeaponDidAttack()
	{
		SubtractOneAmmoFromCurrentWeapon();
		if (!Bootloader.Instance.InTutorial)
		{
			currentWeapon.LastFiredTime = PhotonManager.Instance.ServerTimeInMilliseconds;
		}
		DisableCloak();
		currentWeapon.OnCurrentAmmo(currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]);
		if (!isRemote)
		{
			HUD.Instance.OnSetAmmo(currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]);
		}
	}

	protected void SubtractOneAmmoFromCurrentWeapon()
	{
		newTime[currentWeaponIndex] = (int)(Time.time * 1000f);
		currentClipSizes[currentWeaponIndex] = ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) - 1) ^ newTime[currentWeaponIndex];
		oldTime[currentWeaponIndex] = newTime[currentWeaponIndex];
	}

	public override void OnFire()
	{
		if ((!isRemote && (currentWeapon == null || !canFireWeapon[currentWeaponIndex])) || isDisabled || base.IsReloading || (currentWeapon != null && currentWeapon.isMelee) || base.isAttackingMelee || ((currentWeapon.isMine || currentWeapon.isTurret || currentWeapon.requireGrounded) && !playerController.Motor.IsGrounded()))
		{
			return;
		}
		if (!currentWeapon.isConstantFire && !currentWeapon.isChargeable)
		{
			if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) > 0)
			{
				DisableCloak();
				if (!isRemote)
				{
					if (currentWeapon.OnAttack())
					{
						SubtractOneAmmoFromCurrentWeapon();
						currentWeapon.OnCurrentAmmo(currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]);
						if (!Bootloader.Instance.InTutorial)
						{
							currentWeapon.LastFiredTime = PhotonManager.Instance.ServerTimeInMilliseconds;
						}
						if (!currentWeapon.isSpecial && !currentWeapon.dontAnimateAttacks)
						{
							bodyAnimator.OnAttack();
						}
					}
				}
				else if (!currentWeapon.isSpecial && !currentWeapon.dontAnimateAttacks)
				{
					bodyAnimator.OnAttack();
				}
				if (!isRemote)
				{
					HUD.Instance.OnSetAmmo(currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]);
				}
			}
			if (!currentWeapon.isSpecial)
			{
				Item itemByName = ServiceManager.Instance.GetItemByName(weaponPrefabs[currentWeaponIndex].name);
				int pVal = currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex];
				if (playerController != null && playerController.CharacterManager != null)
				{
					itemByName.UpdateProperty("clipSize", ref pVal, playerController.CharacterManager.EquipmentNames);
					float val = 0f;
					ServiceManager.Instance.UpdateProperty("stock_quote", ref val);
					if (val < 0f)
					{
						int pVal2 = 0;
						itemByName.UpdateProperty("market_down_clipSize_change", ref pVal2, playerController.CharacterManager.EquipmentNames);
						pVal += pVal2;
					}
				}
				if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) > 0 && (currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) <= pVal)
				{
					StartCoroutine(startFiringDelay(0f));
				}
				else if (!currentWeapon.isMine && !currentWeapon.isTurret && !currentWeapon.dontAnimateReloads && !isRemote)
				{
					StopCoroutine("OnReload");
					StartCoroutine("OnReload", GetAttackDuration());
				}
			}
		}
		if (currentWeapon.isChargeable && (currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) > 0)
		{
			DisableCloak();
			if (!isRemote)
			{
				bool flag = currentWeapon.OnAttack();
			}
			if (!currentWeapon.isSpecial)
			{
				bodyAnimator.OnAttack();
			}
		}
		if (OnFirePrimary != null)
		{
			OnFirePrimary();
		}
	}

	public override void OnFireChargedShot()
	{
		if ((!isRemote && (currentWeapon == null || !canFireWeapon[currentWeaponIndex])) || isDisabled || (currentWeapon != null && currentWeapon.isMelee) || ((currentWeapon.isMine || currentWeapon.isTurret || currentWeapon.requireGrounded) && !playerController.Motor.IsGrounded()))
		{
			return;
		}
		base.OnFireChargedShot();
		if (currentWeapon.isChargeable && (currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) > 0)
		{
			DisableCloak();
			if (!isRemote)
			{
				currentWeapon.OnChargedAttack();
			}
			if (!currentWeapon.isSpecial)
			{
				bodyAnimator.OnAttack();
			}
		}
	}

	public override void OnCheckForWeaponHiding()
	{
		if ((currentWeapon.isMine || currentWeapon.isTurret) && (currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) == 0)
		{
			OnHideCurrentWeapon();
		}
	}

	protected void OnHideCurrentWeapon()
	{
		if (currentWeapon != null)
		{
			currentWeapon.transform.localScale = Vector3.zero;
		}
	}

	private void OnShowCurrentWeapon()
	{
		if (currentWeapon != null)
		{
			currentWeapon.transform.localScale = Vector3.one;
		}
	}

	public override void OnRemoteFire(Vector3 pos, Vector3 vel, int delay)
	{
		if (!(currentWeapon == null))
		{
			currentWeapon.OnRemoteAttack(pos, vel, delay);
			OnFire();
		}
	}

	public override void OnRemoteFireWithTarget(Vector3 pos, Vector3 vel, int delay, int targetId)
	{
		if (!(currentWeapon == null))
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(targetId);
			if (playerCharacterManager != null)
			{
				currentWeapon.OnRemoteAttackWithTarget(pos, vel, delay, playerCharacterManager.PlayerController);
				OnFire();
			}
		}
	}

	public override void OnRemoteFire(Vector3 pos, Vector3 vel, float charge, int delay)
	{
		if (!(currentWeapon == null))
		{
			currentWeapon.OnRemoteAttack(pos, vel, delay, charge);
			OnFire(charge);
		}
	}

	public override void OnRemoteFireChargedShot(Vector3 pos, Vector3 vel, int delay)
	{
		if (!(currentWeapon == null))
		{
			currentWeapon.OnRemoteChargedAttack(pos, vel, delay);
			OnFireChargedShot();
		}
	}

	protected IEnumerator startFiringDelay(float initialDelay)
	{
		int weaponIndex = currentWeaponIndex;
		canFireWeapon[weaponIndex] = false;
		yield return new WaitForSeconds(initialDelay);
		yield return new WaitForSeconds(Mathf.Max(0f, currentWeapon.firingTime - initialDelay));
		if ((currentClipSizes[weaponIndex] ^ oldTime[weaponIndex]) > 0)
		{
			canFireWeapon[weaponIndex] = true;
		}
	}

	public override void OnPlayReload(float delay)
	{
		StopCoroutine("OnReload");
		StartCoroutine("OnReload", delay);
	}

	private IEnumerator OnReload(float shotDelay)
	{
		if (currentWeapon.reloadTime < 0f)
		{
			yield break;
		}
		if (!currentWeapon.isMine && !currentWeapon.isTurret)
		{
			base.IsReloading = true;
		}
		int reloadingIndex = base.CurrentWeaponIndex;
		canFireWeapon[reloadingIndex] = false;
		yield return null;
		if (currentWeapon.reloadTime < 0f || currentWeapon.isMelee)
		{
			base.IsReloading = false;
			canFireWeapon[reloadingIndex] = true;
			yield break;
		}
		float reloadTime = currentWeapon.reloadTime;
		_reloadTimes[reloadingIndex] = reloadTime;
		yield return new WaitForSeconds(shotDelay);
		if (!isRemote && netSyncReporter != null)
		{
			netSyncReporter.SetAction(23, null);
		}
		currentWeapon.StartedReloading();
		currentWeapon.LastReloadStart = Time.fixedTime;
		_lastReloadStartTimes[reloadingIndex] = currentWeapon.LastReloadStart;
		if (!isRemote)
		{
			HUD.Instance.OnSetReloadDisplay(reloadTime);
		}
		bodyAnimator.OnReload(reloadTime);
		currentWeapon.PlayReloadAnimation();
		currentWeapon.OnPlayReloadSound();
		yield return new WaitForSeconds(reloadTime - 0.1f);
		currentWeapon.OnReload();
		yield return new WaitForSeconds(0.1f);
		bodyAnimator.OnIdle();
		base.PlayerController.OnPlayReloadSound();
		OnInstantReload(reloadingIndex);
		if (!isRemote)
		{
			HUD.Instance.OnSetAmmo(currentClipSizes[reloadingIndex] ^ oldTime[reloadingIndex]);
		}
		canFireWeapon[reloadingIndex] = true;
		base.IsReloading = false;
	}

	public override void OnInstantReload(int weaponIndex, bool isForcedReload = false)
	{
		if (isForcedReload)
		{
			canFireWeapon[weaponIndex] = true;
			if (weaponIndex == currentWeaponIndex)
			{
				StopCoroutine("OnReload");
				base.IsReloading = false;
			}
		}
		newTime[weaponIndex] = (int)(Time.time * 1000f);
		currentClipSizes[weaponIndex] = weaponPrefabs[weaponIndex].GetComponent<WeaponBase>().clipSize ^ newTime[weaponIndex];
		oldTime[weaponIndex] = newTime[weaponIndex];
		if (playerController != null && playerController.CharacterManager != null)
		{
			int serverClipSize = GetServerClipSize(weaponIndex);
			newTime[weaponIndex] = (int)(Time.time * 1000f);
			currentClipSizes[weaponIndex] = serverClipSize ^ newTime[weaponIndex];
			oldTime[weaponIndex] = newTime[weaponIndex];
			float val = 0f;
			ServiceManager.Instance.UpdateProperty("stock_quote", ref val);
			if (val < 0f)
			{
				Item itemByName = ServiceManager.Instance.GetItemByName(weaponPrefabs[weaponIndex].name);
				int pVal = 0;
				itemByName.UpdateProperty("market_down_clipSize_change", ref pVal, playerController.CharacterManager.EquipmentNames);
				newTime[weaponIndex] = (int)(Time.time * 1000f);
				currentClipSizes[weaponIndex] = ((currentClipSizes[weaponIndex] ^ oldTime[weaponIndex]) + pVal) ^ newTime[weaponIndex];
				oldTime[weaponIndex] = newTime[weaponIndex];
			}
		}
		if (isForcedReload && weaponIndex == currentWeaponIndex && HUD.Instance != null)
		{
			HUD.Instance.OnSetAmmo(currentClipSizes[weaponIndex] ^ oldTime[weaponIndex]);
			HUD.Instance.OnSetReloadDisplay(0f);
		}
	}

	private int GetServerClipSize(int weaponIndex)
	{
		int pVal = currentClipSizes[weaponIndex] ^ oldTime[weaponIndex];
		if (playerController != null && playerController.CharacterManager != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(weaponPrefabs[weaponIndex].name);
			itemByName.UpdateProperty("clipSize", ref pVal, playerController.CharacterManager.EquipmentNames);
		}
		return pVal;
	}

	public override void OnDelayedIncreaseAmmo(float delay)
	{
		OnDelayedIncreaseAmmo(delay, 1);
	}

	public override void OnDelayedIncreaseAmmo(float delay, int qty)
	{
		if (_increaseAmmoRoutine == null)
		{
			_increaseAmmoOptions = new IncreaseAmmoOptions(delay, Time.time, qty);
			_increaseAmmoRoutine = StartCoroutine(DelayedIncreaseAmmo());
		}
		else
		{
			_increaseAmmoOptions.Delay = Mathf.Min(_increaseAmmoOptions.Delay + delay, currentWeapon.reloadTime);
			_increaseAmmoOptions.Quantity = Mathf.Min(_increaseAmmoOptions.Quantity + qty, currentWeapon.clipSize);
			if (!playerController.isRemote)
			{
				HUD.Instance.OnSetReloadDisplay(_increaseAmmoOptions.TimeLeft());
			}
		}
		if (qty == currentWeapon.clipSize)
		{
			currentWeapon.LastReloadStart = Time.fixedTime;
			currentWeapon.OnReload();
		}
	}

	protected virtual IEnumerator DelayedIncreaseAmmo()
	{
		base.IsReloading = true;
		if (!playerController.isRemote)
		{
			HUD.Instance.OnSetReloadDisplay(_increaseAmmoOptions.Delay);
		}
		int startingWeaponIndex = currentWeaponIndex;
		while (Time.time < _increaseAmmoOptions.TargetTime)
		{
			yield return null;
		}
		OnIncreaseAmmo(startingWeaponIndex, _increaseAmmoOptions.Quantity);
		canFireWeapon[startingWeaponIndex] = true;
		base.IsReloading = false;
		_increaseAmmoRoutine = null;
	}

	public override void OnIncreaseAmmo(int weaponIndex, int qty)
	{
		WeaponBase component = weaponPrefabs[weaponIndex].GetComponent<WeaponBase>();
		int serverClipSize = GetServerClipSize(weaponIndex);
		if (component != null && component.isTurret)
		{
			if (_increaseAmmoRoutine != null)
			{
				StopCoroutine(_increaseAmmoRoutine);
			}
			IDeployableWeapon deployableWeapon = component as IDeployableWeapon;
			DeployableTurret[] array = Object.FindObjectsOfType(typeof(DeployableTurret)) as DeployableTurret[];
			int num = 0;
			DeployableTurret[] array2 = array;
			foreach (DeployableTurret deployableTurret in array2)
			{
				if (deployableTurret.OwningPlayer.OwnerID == base.OwnerID && deployableTurret.weaponIndex == base.CurrentWeaponIndex && !deployableTurret.IsDead)
				{
					num++;
				}
			}
			if ((currentClipSizes[weaponIndex] ^ oldTime[weaponIndex]) + num >= serverClipSize || (deployableWeapon != null && deployableWeapon.IsAttacking))
			{
				return;
			}
		}
		if ((currentClipSizes[weaponIndex] ^ oldTime[weaponIndex]) + qty <= serverClipSize)
		{
			base.OnIncreaseAmmo(weaponIndex, qty);
			newTime[weaponIndex] = (int)(Time.time * 1000f);
			currentClipSizes[weaponIndex] = ((currentClipSizes[weaponIndex] ^ oldTime[weaponIndex]) + qty) ^ newTime[weaponIndex];
			oldTime[weaponIndex] = newTime[weaponIndex];
			if (!isRemote && currentWeaponIndex == weaponIndex)
			{
				OnShowCurrentWeapon();
				HUD.Instance.OnSetAmmo(currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]);
			}
		}
		else
		{
			OnInstantReload(weaponIndex);
			if (!isRemote && currentWeaponIndex == weaponIndex)
			{
				OnShowCurrentWeapon();
				HUD.Instance.OnSetAmmo(currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]);
			}
		}
	}

	public override void OnIncreaseAmmo(int weaponIndex)
	{
		OnIncreaseAmmo(weaponIndex, 1);
	}

	public override int GetCurrentClipSize(int weaponIndex)
	{
		return currentClipSizes[weaponIndex] ^ oldTime[weaponIndex];
	}

	public override void SetCurrentClipSize(int clipSize)
	{
		newTime[currentWeaponIndex] = (int)(Time.time * 1000f);
		currentClipSizes[currentWeaponIndex] = clipSize ^ newTime[currentWeaponIndex];
		oldTime[currentWeaponIndex] = newTime[currentWeaponIndex];
		if (!isRemote)
		{
			HUD.Instance.OnSetAmmo(currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]);
		}
		if ((currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) <= 0)
		{
			if (base.gameObject.activeInHierarchy && !currentWeapon.isMine && !currentWeapon.isTurret && bodyAnimator != null)
			{
				StopCoroutine("OnReload");
				StartCoroutine("OnReload", GetAttackDuration());
			}
			else
			{
				OnHideCurrentWeapon();
			}
		}
		else if (!isRemote && HUD.Instance != null)
		{
			HUD.Instance.OnSetReloadDisplay(0f);
		}
	}

	public override void EnableWeapons()
	{
		base.EnableWeapons();
		_increaseAmmoRoutine = null;
	}
}
