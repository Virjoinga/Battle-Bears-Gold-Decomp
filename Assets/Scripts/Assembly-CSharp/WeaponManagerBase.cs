using System;
using UnityEngine;

public abstract class WeaponManagerBase : MonoBehaviour
{
	public int lastDeployableIndex;

	protected int currentWeaponIndex;

	protected bool[] canFireWeapon = new bool[2] { true, true };

	public bool isDisabled;

	public bool isRemote;

	protected int ownerID;

	protected WeaponBase currentWeapon;

	protected WeaponBase leftWeapon;

	protected GameObject[] weaponPrefabs = new GameObject[2];

	public Transform rightWeaponMountpoint;

	public Transform leftWeaponMountpoint;

	public Transform backWeaponMountpoint;

	protected GameObject meleeWeaponPrefab;

	protected NetSyncReporter netSyncReporter;

	protected PlayerController playerController;

	public Action OnFirePrimary;

	public bool IsReloading { get; set; }

	public bool isAttackingMelee { get; protected set; }

	public PlayerController PlayerController
	{
		get
		{
			return playerController;
		}
		set
		{
			playerController = value;
		}
	}

	public int CurrentWeaponIndex
	{
		get
		{
			return currentWeaponIndex;
		}
	}

	public WeaponBase CurrentWeapon
	{
		get
		{
			return currentWeapon;
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

	public GameObject PrimaryWeaponPrefab
	{
		get
		{
			return weaponPrefabs[0];
		}
		set
		{
			weaponPrefabs[0] = value;
		}
	}

	public GameObject SecondaryWeaponPrefab
	{
		get
		{
			return weaponPrefabs[1];
		}
		set
		{
			weaponPrefabs[1] = value;
		}
	}

	public GameObject MeleeWeaponPrefab
	{
		get
		{
			return meleeWeaponPrefab;
		}
		set
		{
			meleeWeaponPrefab = value;
		}
	}

	public event Action beginFire;

	public event Action endFire;

	public abstract void OnGetBomb();

	public abstract void OnInstantReload(int weaponIndex, bool isForcedReload = false);

	public abstract void OnTaunt();

	public virtual void OnDeath()
	{
	}

	public virtual void EnableWeapons()
	{
		StopAllCoroutines();
		playerController.canSwitchWeapons = true;
		isDisabled = false;
		for (int i = 0; i < weaponPrefabs.Length; i++)
		{
			canFireWeapon[i] = true;
		}
		OnSetWeapon(currentWeaponIndex);
	}

	public virtual void Awake()
	{
		isAttackingMelee = false;
		Component[] componentsInChildren = GetComponentsInChildren(typeof(WeaponMountPoint));
		Component[] array = componentsInChildren;
		foreach (Component component in array)
		{
			WeaponMountPoint weaponMountPoint = component as WeaponMountPoint;
			if (weaponMountPoint.mountPoint == WeaponMountPoint.Side.LEFT)
			{
				leftWeaponMountpoint = weaponMountPoint.transform;
			}
			else if (weaponMountPoint.mountPoint == WeaponMountPoint.Side.RIGHT)
			{
				rightWeaponMountpoint = weaponMountPoint.transform;
			}
			else if (weaponMountPoint.mountPoint == WeaponMountPoint.Side.BACK)
			{
				backWeaponMountpoint = weaponMountPoint.transform;
			}
			UnityEngine.Object.Destroy(weaponMountPoint);
		}
		playerController = base.transform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
	}

	protected virtual void DisableCloak()
	{
		if (playerController.CamoCloak != null)
		{
			playerController.CamoCloak.enabled = false;
		}
	}

	public virtual void OnSetWeapon(int index)
	{
	}

	public virtual void OnPostCreate()
	{
		netSyncReporter = GetComponent(typeof(NetSyncReporter)) as NetSyncReporter;
	}

	public virtual bool OnNextWeapon()
	{
		return true;
	}

	public virtual void OnMeleeAttack()
	{
		if (PlayerController != null)
		{
			PlayerController.OnPlayMeleeAttackSound();
		}
	}

	public virtual void StopMelee()
	{
		isAttackingMelee = false;
		StopCoroutine("meleeAttack");
		bool flag = isDisabled;
		isDisabled = false;
		isDisabled = flag;
	}

	public abstract void OnPlayReload(float delay);

	public virtual void OnRemoteBeginFiring()
	{
	}

	public virtual void OnRemoteStopFiring()
	{
	}

	public virtual void OnRemoteSetWeapon(int weaponIndex)
	{
	}

	public virtual void OnRemoteReload()
	{
	}

	public virtual void OnRemoteFire(Vector3 pos, Vector3 vel, int delay)
	{
	}

	public virtual void OnRemoteFireWithTarget(Vector3 pos, Vector3 vel, int delay, int targetId)
	{
	}

	public virtual void OnRemoteFire(Vector3 pos, Vector3 vel, float charge, int delay)
	{
	}

	public virtual void OnRemoteFireChargedShot(Vector3 pos, Vector3 vel, int delay)
	{
	}

	public virtual void OnStopFiring()
	{
		if (this.endFire != null)
		{
			this.endFire();
		}
	}

	public virtual void OnResumeFromStun()
	{
	}

	public virtual void OnReset()
	{
	}

	public virtual void OnBeginFiring()
	{
		if (this.beginFire != null)
		{
			this.beginFire();
		}
	}

	public virtual void OnBeginCharging()
	{
	}

	public virtual void OnFire()
	{
	}

	public virtual void OnFire(float charge)
	{
	}

	public virtual void RemoteFireSecondary(Vector3 pos, Vector3 vel, int delay)
	{
	}

	public virtual void OnFireChargedShot()
	{
	}

	public virtual bool isConstantFireMode()
	{
		return false;
	}

	public virtual bool isSingleFireMode()
	{
		return true;
	}

	public virtual bool isChargableFireMode()
	{
		return false;
	}

	public virtual void OnForceReload()
	{
	}

	public virtual void OnCheckForWeaponHiding()
	{
	}

	public virtual void OnDelayedIncreaseAmmo(float delay)
	{
	}

	public virtual void OnDelayedIncreaseAmmo(float delay, int qty)
	{
	}

	public virtual void OnIncreaseAmmo(int weaponIndex)
	{
	}

	public virtual void OnIncreaseAmmo(int weaponIndex, int qty)
	{
	}

	public virtual int GetCurrentClipSize(int weaponIndex)
	{
		return 0;
	}

	public virtual void SetCurrentClipSize(int clipSize)
	{
	}

	public virtual bool CanFireCurrentWeapon()
	{
		return canFireWeapon[currentWeaponIndex];
	}
}
