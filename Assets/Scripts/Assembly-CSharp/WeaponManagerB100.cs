using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class WeaponManagerB100 : WeaponManager
{
	private static readonly float MELEE_CROSS_FADE_TIME = 0.3f;

	private bool _equippedWeapons;

	public WeaponBase SecondaryWeapon { get; private set; }

	public WeaponBase PrimaryWeapon { get; private set; }

	public MeleeWeapon MeleeWeapon { get; private set; }

	public override void OnSetWeapon(int index)
	{
		if (index == 1 || index == 0)
		{
			OnSetWeapon(index, true);
		}
		else
		{
			OnSetWeapon(index, false);
		}
	}

	public void OnSetWeapon(int index, bool ignoreRiggedParent)
	{
		BodyAnimatorB1000 bodyAnimatorB = null;
		if (bodyAnimator is BodyAnimatorB1000)
		{
			bodyAnimatorB = (BodyAnimatorB1000)bodyAnimator;
		}
		if (!_equippedWeapons)
		{
			SecondaryWeapon = EquipWeapon(1, WeaponMountPoint.Side.BACK, ignoreRiggedParent);
			PrimaryWeapon = EquipWeapon(0, WeaponMountPoint.Side.RIGHT, ignoreRiggedParent);
			MeleeWeapon = (MeleeWeapon)EquipWeapon(2, WeaponMountPoint.Side.LEFT, ignoreRiggedParent);
			MeleeWeapon.OwnerID = base.OwnerID;
			MeleeWeapon.isRemote = isRemote;
			if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
			{
				SecondaryWeapon.gameObject.SetActive(false);
			}
			if (bodyAnimatorB != null)
			{
				bodyAnimatorB.SecondaryAnimPrefix = "secondary";
				bodyAnimatorB.MeleeAnimPrefix = MeleeWeapon.name;
			}
		}
		else if (PrimaryWeapon == null)
		{
			PrimaryWeapon = EquipWeapon(0, WeaponMountPoint.Side.RIGHT, ignoreRiggedParent);
		}
		if (bodyAnimatorB != null)
		{
			bodyAnimatorB.UsingSecondaryAnims = index == 1;
		}
		base.OnSetWeapon(index);
		_equippedWeapons = true;
		if (currentWeapon.playerController == null)
		{
			currentWeapon.playerController = playerController;
		}
		currentWeapon.ConfigureWeapon(ServiceManager.Instance.GetItemByName(currentWeapon.name));
	}

	protected override WeaponBase EquipWeapon(int weaponIndex)
	{
		currentWeapon = EquippedWeapon(weaponIndex);
		if (_equippedWeapons && Preferences.Instance.CurrentGameMode == GameMode.ROYL && currentWeapon.name != weaponPrefabs[weaponIndex].name)
		{
			Object.Destroy(currentWeapon.gameObject);
			currentWeapon = EquipWeapon(weaponIndex, (weaponIndex == 0) ? WeaponMountPoint.Side.RIGHT : WeaponMountPoint.Side.BACK, true);
			if (weaponIndex == 0)
			{
				PrimaryWeapon = currentWeapon;
			}
			else
			{
				SecondaryWeapon = currentWeapon;
			}
		}
		return currentWeapon;
	}

	private WeaponBase EquippedWeapon(int weaponIndex)
	{
		currentWeapon = ((weaponIndex != 0) ? SecondaryWeapon : PrimaryWeapon);
		return currentWeapon;
	}

	public void SpawnNotCurrentWeapon(int weaponIndex)
	{
		WeaponBase weaponBase = EquippedWeapon(weaponIndex);
		Object.Destroy(weaponBase.gameObject);
		WeaponBase weaponBase2 = EquipWeapon(weaponIndex, (weaponIndex == 0) ? WeaponMountPoint.Side.RIGHT : WeaponMountPoint.Side.BACK, true);
		if (weaponIndex == 0)
		{
			PrimaryWeapon = weaponBase2;
		}
		else
		{
			SecondaryWeapon = weaponBase2;
		}
	}

	protected virtual WeaponBase EquipWeapon(int weaponIndex, WeaponMountPoint.Side mountPoint, bool ignoreRiggedParent)
	{
		GameObject gameObject = ((weaponIndex >= 2) ? (Object.Instantiate(meleeWeaponPrefab) as GameObject) : (Object.Instantiate(weaponPrefabs[weaponIndex]) as GameObject));
		WeaponBase component = gameObject.GetComponent<WeaponBase>();
		gameObject.name = ((weaponIndex >= 2) ? meleeWeaponPrefab.name : weaponPrefabs[weaponIndex].name);
		if (component.isRiggedWeapon && !ignoreRiggedParent)
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
		else
		{
			switch (mountPoint)
			{
			case WeaponMountPoint.Side.LEFT:
				gameObject.transform.parent = leftWeaponMountpoint;
				break;
			case WeaponMountPoint.Side.RIGHT:
				gameObject.transform.parent = rightWeaponMountpoint;
				break;
			case WeaponMountPoint.Side.BACK:
				gameObject.transform.parent = backWeaponMountpoint;
				break;
			}
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

	public override void OnMeleeAttack()
	{
		if ((!base.isAttackingMelee && !isDisabled && playerController.canSwitchWeapons && !playerController.HasBomb) || isRemote)
		{
			base.isAttackingMelee = true;
			DisableCloak();
			StopCoroutine("MeleeAttackRoutine");
			StartCoroutine("MeleeAttackRoutine");
		}
	}

	private void StopReloading()
	{
		StopCoroutine("OnReload");
		if (!isRemote)
		{
			HUD.Instance.OnSetReloadDisplay(0f);
			canFireWeapon[currentWeaponIndex] = true;
		}
	}

	protected virtual IEnumerator MeleeAttackRoutine()
	{
		if (!isRemote && netSyncReporter != null)
		{
			ExitGames.Client.Photon.Hashtable parameters = new ExitGames.Client.Photon.Hashtable();
			parameters[(byte)0] = 2;
			netSyncReporter.SetAction(22, parameters);
		}
		MeleeWeapon.OnAttack();
		float attackDuration = bodyAnimator.OnMeleeAttack();
		yield return new WaitForSeconds(attackDuration - MELEE_CROSS_FADE_TIME);
		bodyAnimator.OnIdle();
		yield return new WaitForSeconds(MELEE_CROSS_FADE_TIME);
		base.isAttackingMelee = false;
		MeleeWeapon.Reset();
		if (!isRemote && (currentClipSizes[currentWeaponIndex] ^ oldTime[currentWeaponIndex]) <= 0 && !base.IsReloading)
		{
			OnPlayReload(0f);
		}
		if (base.IsReloading)
		{
			bodyAnimator.OnReload(currentWeapon.reloadTime);
			currentWeapon.PlayReloadAnimation();
		}
	}

	public override void OnGetBomb()
	{
		StopAllCoroutines();
		playerController.canSwitchWeapons = false;
		_equippedWeapons = false;
		Object.Destroy(MeleeWeapon.gameObject);
		Object.Destroy(PrimaryWeapon.gameObject);
		Object.Destroy(SecondaryWeapon.gameObject);
	}

	public override void OnTaunt()
	{
		StopAllCoroutines();
		playerController.canSwitchWeapons = false;
		base.IsReloading = false;
	}
}
