using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class SatelliteSecondaryWeaponManager : WeaponManager
{
	private static readonly string SECONDARY_ACTIVATE_ANIM_NAME = "activate";

	private static readonly string SECONDARY_DEACTIVATE_ANIM_NAME = "deactivate";

	private static readonly string FIRE_ANIM_NAME = "fire";

	[SerializeField]
	private AudioClip _activateSound;

	[SerializeField]
	private AudioClip _deactivateSound;

	private bool _equippedSecondary;

	private float _nextSecondaryUseTime;

	private bool _remoteConstantFire;

	private bool _isConstantFireSecondary;

	private bool _isSatellite = true;

	public WeaponBase SecondaryWeapon { get; private set; }

	private new void Start()
	{
		_isSatellite = true;
		base.Start();
	}

	public override void OnSetWeapon(int index)
	{
		if (index != 1 || !_isSatellite)
		{
			base.OnSetWeapon(index);
		}
	}

	protected override WeaponBase EquipWeapon(int weaponIndex)
	{
		if (!_isSatellite)
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
		if (!_equippedSecondary)
		{
			SecondaryWeapon = EquipFirstTime(1);
			SecondaryWeapon.gameObject.SetActive(false);
			SecondaryWeapon.NetSyncReporter = netSyncReporter;
			SecondaryWeapon.OwnerID = ownerID;
			if (SecondaryWeapon is SatelliteRaycastWeapon)
			{
				SatelliteRaycastWeapon satelliteRaycastWeapon = (SatelliteRaycastWeapon)SecondaryWeapon;
				if (satelliteRaycastWeapon.ConstantFireSatellite)
				{
					_isConstantFireSecondary = true;
				}
			}
			_equippedSecondary = true;
		}
		WeaponBase weaponBase = null;
		if (weaponIndex != 1)
		{
			return base.EquipWeapon(weaponIndex);
		}
		return SecondaryWeapon;
	}

	protected WeaponBase EquipFirstTime(int weaponIndex)
	{
		GameObject gameObject = ((weaponIndex >= 2) ? (Object.Instantiate(meleeWeaponPrefab) as GameObject) : (Object.Instantiate(weaponPrefabs[weaponIndex]) as GameObject));
		WeaponBase component = gameObject.GetComponent<WeaponBase>();
		gameObject.name = ((weaponIndex >= 2) ? meleeWeaponPrefab.name : weaponPrefabs[weaponIndex].name);
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
		else
		{
			switch (component.hand)
			{
			case WeaponMountPoint.Side.LEFT:
				component.transform.parent = leftWeaponMountpoint;
				break;
			case WeaponMountPoint.Side.RIGHT:
				component.transform.parent = rightWeaponMountpoint;
				break;
			case WeaponMountPoint.Side.BACK:
				component.transform.parent = backWeaponMountpoint;
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

	public override void OnTaunt()
	{
		base.OnTaunt();
		if (SecondaryWeapon != null)
		{
			SecondaryWeapon.gameObject.SetActive(false);
		}
	}

	public override void OnReset()
	{
		base.OnReset();
		if (SecondaryWeapon != null)
		{
			SecondaryWeapon.gameObject.SetActive(false);
			SetColorActive(false);
		}
	}

	public void ResetSecondaryCooldown()
	{
		StopCoroutine("FireSecondaryWithPrimary");
		_nextSecondaryUseTime = Time.fixedTime;
		StartCooldown(0f);
	}

	public override bool OnNextWeapon()
	{
		if (_isSatellite)
		{
			return false;
		}
		return base.OnNextWeapon();
	}

	public override void OnFire()
	{
		base.OnFire();
		if (currentWeaponIndex == 1)
		{
			StartCooldown(GetAttackDuration() + currentWeapon.reloadTime);
		}
	}

	private void StartCooldown(float length)
	{
		if (playerController.Director is SatelliteSecondaryShootButtonDirector)
		{
			SatelliteSecondaryShootButtonDirector satelliteSecondaryShootButtonDirector = (SatelliteSecondaryShootButtonDirector)playerController.Director;
			satelliteSecondaryShootButtonDirector.StartCooldown(Time.fixedTime + length, Time.fixedTime);
		}
		else if (playerController.Director is SatelliteSecondaryDoubleTapDirector)
		{
			SatelliteSecondaryDoubleTapDirector satelliteSecondaryDoubleTapDirector = (SatelliteSecondaryDoubleTapDirector)playerController.Director;
			satelliteSecondaryDoubleTapDirector.StartCooldown(Time.fixedTime + length, Time.fixedTime);
		}
		else if (playerController.Director is SatelliteMogaControllerDirector)
		{
			SatelliteMogaControllerDirector satelliteMogaControllerDirector = (SatelliteMogaControllerDirector)playerController.Director;
			satelliteMogaControllerDirector.StartCooldown(Time.fixedTime + length, Time.fixedTime);
		}
		else if (playerController.Director is SateliteKeyboardAndMouseControllerDirector)
		{
			SateliteKeyboardAndMouseControllerDirector sateliteKeyboardAndMouseControllerDirector = (SateliteKeyboardAndMouseControllerDirector)playerController.Director;
			sateliteKeyboardAndMouseControllerDirector.StartCooldown(Time.fixedTime + length, Time.fixedTime);
		}
	}

	public void RemoteActivateSecondaryWeapon()
	{
		SecondaryWeapon.gameObject.SetActive(true);
		SecondaryWeapon.PlayWeaponAnimation(SECONDARY_ACTIVATE_ANIM_NAME, WrapMode.Once, 0f);
		PlayBackpackAnim(SECONDARY_ACTIVATE_ANIM_NAME);
		DoDelayedIdleAnim(SECONDARY_ACTIVATE_ANIM_NAME);
		PlaySound(_activateSound);
		StartCoroutine("RemoteFireSecondaryWithPrimary", SecondaryWeapon.firingTime);
		if (_isConstantFireSecondary)
		{
			SecondaryWeapon.EndConstantFireEffects();
		}
	}

	public void ActivateSecondaryWeapon()
	{
		if (_isSatellite && Time.fixedTime > _nextSecondaryUseTime)
		{
			SecondaryWeapon.gameObject.SetActive(true);
			_nextSecondaryUseTime = Time.fixedTime + SecondaryWeapon.reloadTime + SecondaryWeapon.firingTime;
			float num = 0f;
			if (SecondaryWeapon.GetComponent<Animation>() != null && SecondaryWeapon.GetComponent<Animation>()[SECONDARY_ACTIVATE_ANIM_NAME] != null)
			{
				num = SecondaryWeapon.GetComponent<Animation>()[SECONDARY_ACTIVATE_ANIM_NAME].length;
			}
			StartCoroutine("FireSecondaryWithPrimary", new float[2] { num, SecondaryWeapon.firingTime });
			SetColorActive(true);
			DoDelayedIdleAnim(SECONDARY_ACTIVATE_ANIM_NAME);
			SecondaryWeapon.PlayWeaponAnimation(SECONDARY_ACTIVATE_ANIM_NAME, WrapMode.Once, 0f);
			PlayBackpackAnim(SECONDARY_ACTIVATE_ANIM_NAME);
			if (netSyncReporter != null && !isRemote)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = ownerID;
				netSyncReporter.SetAction(51, hashtable);
			}
			if (_isConstantFireSecondary)
			{
				SecondaryWeapon.EndConstantFireEffects();
			}
			PlaySound(_activateSound);
		}
	}

	private void PlayBackpackAnim(string animName)
	{
		if (bodyAnimator != null && bodyAnimator.Animator != null && (bool)bodyAnimator.Animator[animName])
		{
			bodyAnimator.Animator[animName].layer = 2;
			bodyAnimator.Animator.Play(animName);
		}
	}

	private IEnumerator FireSecondaryWithPrimary(float[] times)
	{
		if (times.Length < 2)
		{
			yield break;
		}
		float startupTime = times[0];
		yield return new WaitForSeconds(startupTime);
		float startTime = Time.fixedTime;
		float nextSecondaryFireTime = 0f;
		bool prevFire = false;
		SecondaryWeapon.OnFindAimer();
		while (startTime + SecondaryWeapon.firingTime > Time.fixedTime)
		{
			if (playerController.IsDead)
			{
				SecondaryWeapon.gameObject.SetActive(false);
				break;
			}
			if (playerController.Director.Fire && nextSecondaryFireTime <= Time.fixedTime)
			{
				FireSecondary();
				nextSecondaryFireTime = Time.fixedTime + SecondaryWeapon.firingTime / (float)SecondaryWeapon.clipSize;
			}
			if (!prevFire && playerController.Director.Fire && _isConstantFireSecondary)
			{
				SecondaryWeapon.BeginConstantFireEffects();
				SendAction(53);
				StartConstantFireAnimation();
			}
			else if (prevFire && !playerController.Director.Fire && _isConstantFireSecondary)
			{
				SecondaryWeapon.EndConstantFireEffects();
				SendAction(54);
				StopConstantFireAnimationAndIdle();
			}
			prevFire = playerController.Director.Fire;
			yield return null;
		}
		if (_isConstantFireSecondary)
		{
			SendAction(54);
		}
		DeactivateSecondaryWeapon();
	}

	private IEnumerator RemoteFireSecondaryWithPrimary(float firingTime)
	{
		float startTime = Time.fixedTime;
		float nextSecondaryFireTime = 0f;
		Debug.Log("Remote fire secondary with primary");
		SecondaryWeapon.OnFindAimer();
		while (Time.fixedTime - startTime <= firingTime)
		{
			if (playerController.IsDead)
			{
				SecondaryWeapon.gameObject.SetActive(false);
				break;
			}
			if (_remoteConstantFire && nextSecondaryFireTime <= Time.fixedTime)
			{
				RemoteFireSecondary(Vector3.zero, Vector3.zero, 0);
				nextSecondaryFireTime = Time.fixedTime + SecondaryWeapon.firingTime / (float)SecondaryWeapon.clipSize;
			}
			yield return null;
		}
		Debug.Log("Remote stop fire secondary with primary");
	}

	private void FireSecondary()
	{
		SecondaryWeapon.OnAttack();
		if (!_isConstantFireSecondary)
		{
			DoDelayedIdleAnim(FIRE_ANIM_NAME);
		}
		else
		{
			StartConstantFireAnimation();
		}
	}

	private void SendAction(byte action)
	{
		if (netSyncReporter != null)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = ownerID;
			netSyncReporter.SetAction(action, hashtable);
		}
	}

	public override void RemoteFireSecondary(Vector3 pos, Vector3 vel, int delay)
	{
		SecondaryWeapon.OnRemoteAttack(pos, vel, delay);
		DoDelayedIdleAnim(FIRE_ANIM_NAME);
	}

	public void RemoteStartSecondaryConstantFire()
	{
		_remoteConstantFire = true;
		SecondaryWeapon.BeginConstantFireEffects();
		StartConstantFireAnimation();
	}

	public void RemoteStopSecondaryConstantFire()
	{
		_remoteConstantFire = false;
		SecondaryWeapon.EndConstantFireEffects();
		StopConstantFireAnimationAndIdle();
	}

	private IEnumerator DisplaySecondaryButtonState(float delayBeforeCooldown)
	{
		SetColorActive(true);
		yield return new WaitForSeconds(delayBeforeCooldown);
		SetColorActive(false);
		StartCooldown(SecondaryWeapon.reloadTime);
	}

	public void RemoteDeactivateSecondaryWeapon()
	{
		SecondaryWeapon.StopWeaponAnimations();
		SecondaryWeapon.PlayWeaponAnimation(SECONDARY_DEACTIVATE_ANIM_NAME, WrapMode.Once, 0f);
		DoDelayedBackpackAnim(SECONDARY_DEACTIVATE_ANIM_NAME, SECONDARY_DEACTIVATE_ANIM_NAME);
		DoDelayedInactive(SECONDARY_DEACTIVATE_ANIM_NAME);
		PlaySound(_deactivateSound);
		StopCoroutine("RemoteFireSecondaryWithPrimary");
		if (_isConstantFireSecondary)
		{
			SecondaryWeapon.EndConstantFireEffects();
		}
	}

	public void DeactivateSecondaryWeapon()
	{
		StopCoroutine("FireSecondaryWithPrimary");
		SecondaryWeapon.StopWeaponAnimations();
		SecondaryWeapon.PlayWeaponAnimation(SECONDARY_DEACTIVATE_ANIM_NAME, WrapMode.Once, 0f);
		DoDelayedBackpackAnim(SECONDARY_DEACTIVATE_ANIM_NAME, SECONDARY_DEACTIVATE_ANIM_NAME);
		if (netSyncReporter != null && !isRemote)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = ownerID;
			netSyncReporter.SetAction(52, hashtable);
		}
		SetColorActive(false);
		StartCooldown(SecondaryWeapon.reloadTime);
		DoDelayedInactive(SECONDARY_DEACTIVATE_ANIM_NAME);
		PlaySound(_deactivateSound);
		if (_isConstantFireSecondary)
		{
			SecondaryWeapon.EndConstantFireEffects();
		}
	}

	private void DoDelayedBackpackAnim(string followingAnimName, string backpackAnim)
	{
		StartCoroutine(DelayedBackpackAnim(GetAnimLength(followingAnimName), backpackAnim));
	}

	private void DoDelayedIdleAnim(string followingAnimName)
	{
		StartCoroutine(DelayedIdle(GetAnimLength(followingAnimName)));
	}

	private void DoDelayedInactive(string followingAnimName)
	{
		StartCoroutine(DelayedInactive(GetAnimLength(followingAnimName)));
	}

	private IEnumerator DelayedBackpackAnim(float delay, string animName)
	{
		yield return new WaitForSeconds(delay);
		PlayBackpackAnim(animName);
	}

	private IEnumerator DelayedIdle(float delay)
	{
		yield return new WaitForSeconds(delay);
		SecondaryWeapon.PlayWeaponAnimation("idle", WrapMode.Loop, 0f);
	}

	private IEnumerator DelayedInactive(float delay)
	{
		yield return new WaitForSeconds(delay);
		SecondaryWeapon.gameObject.SetActive(false);
	}

	private float GetAnimLength(string animName)
	{
		float result = 0f;
		if (SecondaryWeapon.GetComponent<Animation>() != null && SecondaryWeapon.GetComponent<Animation>()[animName] != null)
		{
			result = SecondaryWeapon.GetComponent<Animation>()[animName].length;
		}
		return result;
	}

	private float GetBodyAnimatorAnimLength(string animName)
	{
		float result = 0f;
		if (bodyAnimator != null && bodyAnimator.Animator != null && (bool)bodyAnimator.Animator[animName])
		{
			result = bodyAnimator.Animator[animName].length;
		}
		return result;
	}

	private void StartConstantFireAnimation()
	{
		if (SecondaryWeapon.GetComponent<Animation>() != null && SecondaryWeapon.GetComponent<Animation>()[FIRE_ANIM_NAME] != null && !SecondaryWeapon.GetComponent<Animation>()[FIRE_ANIM_NAME].enabled)
		{
			SecondaryWeapon.PlayWeaponAnimation(FIRE_ANIM_NAME, WrapMode.Loop, 0f);
		}
	}

	private void StopConstantFireAnimationAndIdle()
	{
		if (_isConstantFireSecondary)
		{
			SecondaryWeapon.StopWeaponAnimations();
			StartCoroutine(DelayedIdle(Time.deltaTime));
		}
	}

	private void SetColorActive(bool enabled)
	{
		if (playerController.Director is SatelliteSecondaryShootButtonDirector)
		{
			SatelliteSecondaryShootButtonDirector satelliteSecondaryShootButtonDirector = (SatelliteSecondaryShootButtonDirector)playerController.Director;
			satelliteSecondaryShootButtonDirector.SecondaryButton.OverrideColorPref = enabled;
			satelliteSecondaryShootButtonDirector.SecondaryButton.ColorOverride = Color.cyan;
		}
		else if (playerController.Director is SatelliteSecondaryDoubleTapDirector)
		{
			SatelliteSecondaryDoubleTapDirector satelliteSecondaryDoubleTapDirector = (SatelliteSecondaryDoubleTapDirector)playerController.Director;
			satelliteSecondaryDoubleTapDirector.SecondaryButton.OverrideColorPref = enabled;
			satelliteSecondaryDoubleTapDirector.SecondaryButton.ColorOverride = Color.cyan;
		}
		else if (playerController.Director is SatelliteMogaControllerDirector)
		{
			SatelliteMogaControllerDirector satelliteMogaControllerDirector = (SatelliteMogaControllerDirector)playerController.Director;
			satelliteMogaControllerDirector.SecondaryButton.OverrideColorPref = enabled;
			satelliteMogaControllerDirector.SecondaryButton.ColorOverride = Color.cyan;
		}
		else if (playerController.Director is SateliteKeyboardAndMouseControllerDirector)
		{
			SateliteKeyboardAndMouseControllerDirector sateliteKeyboardAndMouseControllerDirector = (SateliteKeyboardAndMouseControllerDirector)playerController.Director;
			sateliteKeyboardAndMouseControllerDirector.SecondaryButton.OverrideColorPref = enabled;
			sateliteKeyboardAndMouseControllerDirector.SecondaryButton.ColorOverride = Color.cyan;
		}
	}

	private void PlaySound(AudioClip clip)
	{
		if (base.GetComponent<AudioSource>() != null)
		{
			base.GetComponent<AudioSource>().PlayOneShot(clip, SoundManager.Instance.getEffectsVolume());
		}
	}
}
