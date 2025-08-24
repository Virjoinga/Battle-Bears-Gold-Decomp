using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class WeaponManagerHuggable : WeaponManagerBase
{
	private GameObject huggableHead;

	protected BodyAnimatorHuggable bodyAnimator;

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

	protected override void DisableCloak()
	{
		base.DisableCloak();
		InvisibilityCloak invisibilityCloak = GetComponent(typeof(InvisibilityCloak)) as InvisibilityCloak;
		if (invisibilityCloak != null)
		{
			invisibilityCloak.enabled = false;
		}
	}

	public override void EnableWeapons()
	{
		OnSemiReset();
		playerController.canSwitchWeapons = true;
	}

	private void OnSemiReset()
	{
		StopAllCoroutines();
		for (int i = 0; i < weaponPrefabs.Length; i++)
		{
			canFireWeapon[i] = true;
		}
		base.isAttackingMelee = false;
		isDisabled = false;
		OnSetWeapon(currentWeaponIndex);
	}

	public override void OnReset()
	{
		StopAllCoroutines();
		currentWeaponIndex = 0;
		for (int i = 0; i < weaponPrefabs.Length; i++)
		{
			canFireWeapon[i] = true;
		}
		base.isAttackingMelee = false;
		isDisabled = false;
		OnSetWeapon(currentWeaponIndex);
	}

	public override void OnDeath()
	{
		if (currentWeapon is ChargedRun)
		{
			ChargedRun chargedRun = (ChargedRun)currentWeapon;
			chargedRun.OnDisable();
		}
	}

	public override void OnStopFiring()
	{
		if (currentWeapon.isConstantFire)
		{
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
				StopCoroutine("remoteContinuousFire");
			}
		}
		else if (currentWeapon.isChargeable)
		{
			if (canFireWeapon[currentWeaponIndex] && playerController != null && !playerController.IsDead)
			{
				bodyAnimator.OnIdle();
				currentWeapon.EndCharging();
			}
			if (!isRemote && playerController != null && !playerController.IsDead && netSyncReporter != null)
			{
				netSyncReporter.SetAction(21, null);
			}
		}
	}

	public override void OnRemoteStopFiring()
	{
		OnStopFiring();
	}

	public override void OnFireChargedShot()
	{
		if ((!isRemote && (currentWeapon == null || !canFireWeapon[currentWeaponIndex])) || isDisabled || (currentWeapon != null && currentWeapon.isMelee) || ((currentWeapon.isMine || currentWeapon.isTurret) && !playerController.Motor.IsGrounded()))
		{
			return;
		}
		base.OnFireChargedShot();
		if (currentWeapon.isChargeable)
		{
			DisableCloak();
			if (!currentWeapon.isSpecial)
			{
				bodyAnimator.OnAttack(currentWeapon.firingTime);
			}
			if (!isRemote)
			{
				HUD.Instance.OnSetReloadDisplay(currentWeapon.firingTime + currentWeapon.reloadTime);
			}
			StartCoroutine(restTime(currentWeapon.firingTime, currentWeapon.reloadTime));
		}
	}

	public override void OnBeginFiring()
	{
		if (base.isAttackingMelee || isDisabled || !canFireWeapon[currentWeaponIndex] || !currentWeapon.isConstantFire)
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
		while (true)
		{
			DisableCloak();
			currentWeapon.OnRemoteAttack(Vector3.zero, Vector3.zero, 0);
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

	public override void OnSetWeapon(int weaponIndex)
	{
		if (isDisabled)
		{
			return;
		}
		if (!isRemote && netSyncReporter != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = weaponIndex;
			netSyncReporter.SetAction(22, hashtable);
		}
		if (currentWeapon != null)
		{
			Object.Destroy(currentWeapon.gameObject);
		}
		if (leftWeapon != null)
		{
			Object.Destroy(leftWeapon.gameObject);
		}
		GameObject gameObject = Object.Instantiate(weaponPrefabs[weaponIndex]) as GameObject;
		currentWeapon = gameObject.GetComponent(typeof(WeaponBase)) as WeaponBase;
		currentWeapon.name = weaponPrefabs[weaponIndex].name;
		MeleeAttack meleeAttack = currentWeapon as MeleeAttack;
		if (meleeAttack != null)
		{
			gameObject.transform.parent = base.transform;
		}
		else if (currentWeapon.isRiggedWeapon)
		{
			Transform bodyRotator = playerController.bodyRotator;
			if (bodyRotator != null)
			{
				currentWeapon.transform.parent = bodyRotator;
			}
			else
			{
				Debug.LogError("Could not find aimer transform!");
			}
		}
		else if (currentWeapon.hand == WeaponMountPoint.Side.LEFT)
		{
			gameObject.transform.parent = leftWeaponMountpoint;
		}
		else
		{
			gameObject.transform.parent = rightWeaponMountpoint;
		}
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		currentWeapon.NetSyncReporter = netSyncReporter;
		currentWeapon.OwnerID = ownerID;
		currentWeapon.isRemote = isRemote;
		currentWeaponIndex = weaponIndex;
		bodyAnimator.OnSetAttack(currentWeapon.gameObject, currentWeapon is MeleeWeapon, currentWeapon.isChargeable);
	}

	public override void OnGetBomb()
	{
		StopAllCoroutines();
		playerController.canSwitchWeapons = false;
		if (currentWeapon != null)
		{
			Object.Destroy(currentWeapon.gameObject);
		}
		if (leftWeapon != null)
		{
			Object.Destroy(leftWeapon.gameObject);
		}
		if (huggableHead != null)
		{
			huggableHead.transform.localScale = Vector3.one;
		}
	}

	public override void OnTaunt()
	{
		OnGetBomb();
		if (currentWeapon is ChargedRun)
		{
			ChargedRun chargedRun = (ChargedRun)currentWeapon;
			chargedRun.OnDisable();
		}
	}

	private GameObject GetHuggableHead(Transform startingTransform)
	{
		GameObject result = null;
		for (int i = 0; i < startingTransform.GetChildCount(); i++)
		{
			Transform child = startingTransform.GetChild(i);
			if (child.gameObject.name.Equals("head_geo"))
			{
				result = child.gameObject;
			}
			else if (child.GetChildCount() > 0)
			{
				GameObject gameObject = GetHuggableHead(child);
				if (gameObject != null)
				{
					result = gameObject;
				}
			}
		}
		return result;
	}

	public override void OnMeleeAttack()
	{
		if (!base.isAttackingMelee && !isDisabled && base.gameObject.activeInHierarchy && !playerController.HasBomb)
		{
			base.OnMeleeAttack();
			StopCoroutine("meleeAttack");
			if (meleeWeaponPrefab != null)
			{
				StartCoroutine("meleeAttack");
			}
		}
	}

	public override void OnPlayReload(float duration)
	{
	}

	public override void StopMelee()
	{
		base.StopMelee();
		OnSetWeapon(currentWeaponIndex);
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
		DisableCloak();
		GameObject newWeapon = Object.Instantiate(meleeWeaponPrefab) as GameObject;
		newWeapon.name = meleeWeaponPrefab.name;
		currentWeapon = newWeapon.GetComponent(typeof(MeleeWeapon)) as WeaponBase;
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
		base.isAttackingMelee = true;
		bodyAnimator.OnSetAttack(currentWeapon.gameObject, true);
		currentWeapon.OnAttack();
		float attackDuration = bodyAnimator.OnMeleeAttack();
		yield return new WaitForSeconds(attackDuration);
		base.isAttackingMelee = false;
		yield return new WaitForSeconds(0.03f);
		if (!base.isAttackingMelee)
		{
			OnSetWeapon(currentWeaponIndex);
		}
	}

	public override void OnRemoteSetWeapon(int weaponIndex)
	{
		if (weaponIndex == 2)
		{
			OnMeleeAttack();
		}
		else
		{
			OnSetWeapon(weaponIndex);
		}
	}

	public override bool OnNextWeapon()
	{
		if (base.isAttackingMelee || isDisabled || !canFireWeapon[currentWeaponIndex] || !playerController.canSwitchWeapons)
		{
			return false;
		}
		currentWeaponIndex++;
		if (currentWeaponIndex >= weaponPrefabs.Length)
		{
			currentWeaponIndex = 0;
		}
		OnSetWeapon(currentWeaponIndex);
		return true;
	}

	public override void Awake()
	{
		base.Awake();
		bodyAnimator = GetComponentInChildren(typeof(BodyAnimatorHuggable)) as BodyAnimatorHuggable;
		huggableHead = GetHuggableHead(base.transform);
	}

	public override void OnFire()
	{
		if ((!isRemote && (currentWeapon == null || !canFireWeapon[currentWeaponIndex])) || isDisabled || (currentWeapon != null && currentWeapon.isMelee))
		{
			return;
		}
		DisableCloak();
		MeleeAttack meleeAttack = currentWeapon as MeleeAttack;
		if (meleeAttack != null)
		{
			bodyAnimator.OnAttack(meleeAttack.duration);
			if (!isRemote)
			{
				currentWeapon.OnAttack();
			}
			if (!isRemote)
			{
				HUD.Instance.OnSetReloadDisplay(meleeAttack.tiredTime + meleeAttack.duration);
			}
			StartCoroutine(restTime(meleeAttack.duration, meleeAttack.tiredTime));
		}
		else if (!currentWeapon.isConstantFire)
		{
			bodyAnimator.OnAttack(currentWeapon.firingTime);
			if (!isRemote)
			{
				currentWeapon.OnAttack();
			}
			if (!isRemote)
			{
				HUD.Instance.OnSetReloadDisplay(currentWeapon.firingTime + currentWeapon.reloadTime);
			}
			StartCoroutine(restTime(currentWeapon.firingTime, currentWeapon.reloadTime));
		}
	}

	public override void OnFire(float charge)
	{
		if ((!isRemote && (currentWeapon == null || !canFireWeapon[currentWeaponIndex])) || isDisabled || (currentWeapon != null && currentWeapon.isMelee))
		{
			return;
		}
		DisableCloak();
		MeleeAttack meleeAttack = currentWeapon as MeleeAttack;
		if (meleeAttack != null)
		{
			bodyAnimator.OnAttack(meleeAttack.duration);
			if (!isRemote)
			{
				currentWeapon.OnAttack(charge);
			}
			if (!isRemote)
			{
				HUD.Instance.OnSetReloadDisplay(meleeAttack.tiredTime + meleeAttack.duration);
			}
			StartCoroutine(restTime(meleeAttack.duration, meleeAttack.tiredTime));
		}
		else
		{
			bodyAnimator.OnAttack(currentWeapon.firingTime);
			if (!isRemote)
			{
				currentWeapon.OnAttack(charge);
			}
			if (!isRemote)
			{
				HUD.Instance.OnSetReloadDisplay(currentWeapon.firingTime + currentWeapon.reloadTime);
			}
			StartCoroutine(restTime(currentWeapon.firingTime, currentWeapon.reloadTime));
		}
	}

	private IEnumerator restTime(float attackDuration, float timeToRest)
	{
		canFireWeapon[currentWeaponIndex] = false;
		yield return new WaitForSeconds(attackDuration);
		yield return new WaitForSeconds(timeToRest);
		canFireWeapon[currentWeaponIndex] = true;
		base.PlayerController.OnPlayReloadSound();
	}

	public override void OnRemoteFire(Vector3 pos, Vector3 vel, int delay)
	{
		if (!(currentWeapon == null))
		{
			OnFire();
			DisableCloak();
			currentWeapon.OnRemoteAttack(pos, vel, delay);
			isDisabled = false;
			base.isAttackingMelee = false;
			canFireWeapon[currentWeaponIndex] = true;
		}
	}

	public override void OnRemoteFire(Vector3 pos, Vector3 vel, float charge, int delay)
	{
		if (!(currentWeapon == null))
		{
			OnFire(charge);
			DisableCloak();
			currentWeapon.OnRemoteAttack(pos, vel, delay, charge);
			isDisabled = false;
			base.isAttackingMelee = false;
			canFireWeapon[currentWeaponIndex] = true;
		}
	}

	public override void OnInstantReload(int weaponIndex, bool isForcedReload = false)
	{
	}

	public void OnExitMGSBox()
	{
		isDisabled = false;
		base.isAttackingMelee = false;
		bodyAnimator.isDisabled = false;
		bodyAnimator.IsFiring = false;
		bodyAnimator.OnReset();
		bodyAnimator.ForceIdle();
	}
}
