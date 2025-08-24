using System.Collections;
using UnityEngine;

public class BodyAnimator : BodyAnimatorBase
{
	private static readonly float CROSS_FADE_TIME = 0.2f;

	protected WeaponBase currentWeapon;

	protected AnimationState _fireInAnimation;

	protected AnimationState _fireOutAnimation;

	protected AnimationState _fireLoopAnimation;

	protected AnimationState _chargeAnimation;

	protected AnimationState _reloadInAnimation;

	protected AnimationState _reloadOutAnimation;

	protected AnimationState _reloadLoopAnimation;

	protected AnimationState _fireAnimation;

	protected AnimationState _runAnimation;

	protected AnimationState _idleAnimation;

	protected AnimationState _switchAnimation;

	public float AttackDuration
	{
		get
		{
			float num = 0f;
			if (isDisabled)
			{
				return 0f;
			}
			if (currentWeapon.isFireInLoopOut)
			{
				num = _fireInAnimation.length;
				if (_fireLoopAnimation != null)
				{
					num += _fireLoopAnimation.length;
				}
				if (_fireOutAnimation != null)
				{
					num += _fireOutAnimation.length;
				}
			}
			else if (_fireAnimation != null)
			{
				num = _fireAnimation.length / currentWeapon.attackAnimationSpeed;
			}
			return num;
		}
	}

	public virtual void OnSetWeapon(WeaponBase w, bool isMelee)
	{
		if (this == null || base.gameObject == null)
		{
			return;
		}
		StopAllCoroutines();
		currentWeapon = w;
		if (currentWeapon.name.Contains("_"))
		{
			currentWeaponName = currentWeapon.name.Split('_')[1];
		}
		else
		{
			currentWeaponName = currentWeapon.name;
		}
		if ((w.isMine || w.isTurret || w.isPrefabSpawner || w.isFireInLoopOut || w.animateLikeDeployable) && !w.dontSetFireInLoopOut)
		{
			_fireInAnimation = myAnimator[currentWeaponName + "_fireIn"];
			if (_fireAnimation != null)
			{
				_fireInAnimation.layer = 2;
				_fireInAnimation.speed = currentWeapon.attackAnimationSpeed;
			}
			_fireLoopAnimation = myAnimator[currentWeaponName + "_fireLoop"];
			if (_fireLoopAnimation != null)
			{
				_fireLoopAnimation.layer = 2;
				_fireLoopAnimation.speed = currentWeapon.attackAnimationSpeed;
			}
			if (w.isMine || w.isTurret || w.isFireInLoopOut || w.animateLikeDeployable)
			{
				_fireOutAnimation = myAnimator[currentWeaponName + "_fireOut"];
				if (_fireOutAnimation != null)
				{
					_fireOutAnimation.layer = 2;
					_fireOutAnimation.speed = currentWeapon.attackAnimationSpeed;
				}
			}
		}
		else
		{
			_fireAnimation = myAnimator[currentWeaponName + "_fire"];
			if (_fireAnimation != null)
			{
				_fireAnimation.layer = 0;
				_fireAnimation.speed = currentWeapon.attackAnimationSpeed;
			}
			else if (currentWeapon.isMelee)
			{
				_fireAnimation = myAnimator["melee_fire"];
			}
		}
		_chargeAnimation = myAnimator[currentWeaponName + "_charge"];
		if (!isMelee)
		{
			if (!w.isMine && !w.isTurret)
			{
				_reloadInAnimation = myAnimator[currentWeaponName + "_reloadIn"];
				if (_reloadInAnimation != null)
				{
					_reloadInAnimation.layer = 0;
					_reloadInAnimation.speed = currentWeapon.reloadAnimationSpeed;
				}
				_reloadOutAnimation = myAnimator[currentWeaponName + "_reloadOut"];
				if (_reloadOutAnimation != null)
				{
					_reloadOutAnimation.layer = 0;
					_reloadOutAnimation.speed = currentWeapon.reloadAnimationSpeed;
				}
				_reloadLoopAnimation = myAnimator[currentWeaponName + "_reloadLoop"];
				if (_reloadLoopAnimation != null)
				{
					_reloadLoopAnimation.layer = 0;
					_reloadLoopAnimation.speed = currentWeapon.reloadAnimationSpeed;
				}
			}
			_runAnimation = myAnimator[currentWeaponName + "_run"];
			if (_runAnimation != null)
			{
				_runAnimation.layer = 0;
				_runAnimation.speed = currentWeapon.walkAnimationSpeed;
			}
			_idleAnimation = myAnimator[currentWeaponName + "_idle"];
			if (_idleAnimation != null)
			{
				_idleAnimation.layer = 0;
				_idleAnimation.speed = currentWeapon.idleAnimationSpeed;
			}
			if (w.GetComponent<Animation>() != null && w.GetComponent<Animation>()["switch"] != null)
			{
				_switchAnimation = myAnimator[w.name + "_switch"];
				if (_switchAnimation != null)
				{
					StartCoroutine(PlayWeaponSwitchAnimation(w.name));
				}
			}
			else
			{
				OnIdle();
			}
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
		{
			_idleAnimation = myAnimator["melee_idle"];
			_idleAnimation.layer = 0;
			_idleAnimation.speed = currentWeapon.idleAnimationSpeed;
			OnIdle();
		}
	}

	protected IEnumerator PlayWeaponSwitchAnimation(string weaponName)
	{
		myAnimator.CrossFade(_switchAnimation.name, CROSS_FADE_TIME);
		yield return new WaitForSeconds(_switchAnimation.length);
		OnIdle();
	}

	public void OnBeginContinuousAttack()
	{
		if (isDisabled)
		{
			return;
		}
		isFiring = true;
		if ((bool)_fireAnimation)
		{
			_fireAnimation.wrapMode = WrapMode.Loop;
			if (!currentWeapon.shouldAnimateBodyFireIndependentOfFiringTime)
			{
				_fireAnimation.speed = _fireAnimation.length / currentWeapon.firingTime;
			}
			myAnimator.CrossFade(_fireAnimation.name);
		}
	}

	public void OnStopContinuousAttack()
	{
		isFiring = false;
	}

	public override void OnAttack()
	{
		if (isDisabled)
		{
			return;
		}
		float num = 0f;
		StopFiringAnimations();
		if (currentWeapon.isMine || currentWeapon.isTurret || currentWeapon.animateLikeDeployable)
		{
			StartCoroutine(placeDeployable());
			num = _fireInAnimation.length + _fireOutAnimation.length;
		}
		else if (currentWeapon.isPrefabSpawner || currentWeapon.isFireInLoopOut)
		{
			StopAllCoroutines();
			if (currentWeapon.isPrefabSpawner)
			{
				num = currentWeapon.firingTime;
			}
			else
			{
				num = _fireInAnimation.length;
				if (_fireLoopAnimation != null)
				{
					num += _fireLoopAnimation.length;
				}
				if (_fireOutAnimation != null)
				{
					num += _fireOutAnimation.length;
				}
			}
			StartCoroutine("DoFireInFireLoopFireOut", num);
		}
		else if (_fireAnimation != null)
		{
			if (!isDisabled)
			{
				myAnimator.CrossFade(_fireAnimation.name, 0.1f);
			}
			num = _fireAnimation.length;
		}
		StopCoroutine("attackCountdown");
		StartCoroutine("attackCountdown", num - CROSS_FADE_TIME);
	}

	private void StopFiringAnimations()
	{
		if (_fireAnimation != null)
		{
			myAnimator.Stop(_fireAnimation.name);
		}
		if (_fireInAnimation != null)
		{
			myAnimator.Stop(_fireInAnimation.name);
		}
		if (_fireLoopAnimation != null)
		{
			myAnimator.Stop(_fireLoopAnimation.name);
		}
		if (_fireOutAnimation != null)
		{
			myAnimator.Stop(_fireOutAnimation.name);
		}
	}

	private IEnumerator placeTurret()
	{
		myAnimator.Play(_fireInAnimation.name);
		yield return new WaitForSeconds(_fireInAnimation.length);
		myAnimator.Play(_fireOutAnimation.name);
	}

	public override float GetTurretLayingInTime()
	{
		if (_fireInAnimation != null)
		{
			return _fireInAnimation.length;
		}
		return 0f;
	}

	public override float GetTurretLayingOutTime()
	{
		if (_fireOutAnimation != null)
		{
			return _fireOutAnimation.length;
		}
		return 0f;
	}

	private IEnumerator DoFireInFireLoopFireOut(float firingTime)
	{
		if (isDisabled || !(myAnimator != null))
		{
			yield break;
		}
		float loopTime = 0f;
		if (_fireInAnimation != null)
		{
			float fireOutTime = 0f;
			if (_fireOutAnimation != null)
			{
				fireOutTime = _fireOutAnimation.length;
			}
			loopTime = firingTime - _fireInAnimation.length - fireOutTime;
			isFiring = true;
			_fireInAnimation.wrapMode = WrapMode.Once;
			if (!isDisabled)
			{
				myAnimator.CrossFade(_fireInAnimation.name, CROSS_FADE_TIME);
			}
			yield return new WaitForSeconds(_fireInAnimation.length);
		}
		if (loopTime > 0f && _fireLoopAnimation != null)
		{
			_fireLoopAnimation.wrapMode = WrapMode.Loop;
			_fireLoopAnimation.layer = 2;
			if (!isDisabled)
			{
				myAnimator.CrossFade(_fireLoopAnimation.name, 0.1f);
			}
			yield return new WaitForSeconds(loopTime);
			_fireLoopAnimation.layer = 0;
		}
		if (_fireOutAnimation != null)
		{
			_fireOutAnimation.wrapMode = WrapMode.Once;
			if (!isDisabled)
			{
				myAnimator.CrossFade(_fireOutAnimation.name, CROSS_FADE_TIME);
			}
			yield return new WaitForSeconds(_fireOutAnimation.length - CROSS_FADE_TIME);
		}
		isFiring = false;
		OnIdle();
	}

	private IEnumerator placeDeployable()
	{
		if (_fireInAnimation != null)
		{
			myAnimator.Play(_fireInAnimation.name);
			yield return new WaitForSeconds(_fireInAnimation.length);
			myAnimator.Play(_fireOutAnimation.name);
		}
		else
		{
			yield return null;
		}
		OnIdle();
	}

	public override float GetDeployableLayingInTime()
	{
		if (_fireInAnimation != null)
		{
			return _fireInAnimation.length;
		}
		return 0f;
	}

	public override float GetDeployableLayingOutTime()
	{
		if (_fireOutAnimation != null)
		{
			return _fireOutAnimation.length;
		}
		return 0f;
	}

	public void OnWeaponCharge()
	{
		if (!currentWeapon.isFireInLoopOut && _chargeAnimation != null)
		{
			_chargeAnimation.wrapMode = WrapMode.Loop;
			myAnimator.CrossFade(_chargeAnimation.name);
		}
		else if (!currentWeapon.dontAnimateAttacks)
		{
			StartCoroutine("DoFireInFireLoopFireOut", AttackDuration);
		}
	}

	public void OnReload(float reloadTime)
	{
		if (_fireLoopAnimation != null && _fireLoopAnimation.layer != 0)
		{
			_fireLoopAnimation.layer = 0;
		}
		StopAllCoroutines();
		StartCoroutine("reload", reloadTime);
	}

	public override float OnMeleeAttack()
	{
		if (isDisabled)
		{
			return 0f;
		}
		StopAllCoroutines();
		isFiring = true;
		_fireAnimation.wrapMode = WrapMode.Once;
		myAnimator.Play(_fireAnimation.name);
		StartCoroutine("delayedMelee", _fireAnimation.length);
		return _fireAnimation.length;
	}

	public IEnumerator reload(float reloadTime)
	{
		if (isDisabled || !(myAnimator != null))
		{
			yield break;
		}
		float loopTime = 0f;
		float reloadTimeLeft = reloadTime - (Time.fixedTime - currentWeapon.LastReloadStart);
		if (reloadTimeLeft <= 0f)
		{
			yield break;
		}
		float reloadOutTime = ((!(_reloadOutAnimation == null)) ? _reloadOutAnimation.length : 0f);
		if (reloadTimeLeft == reloadTime)
		{
			if (_reloadInAnimation != null)
			{
				loopTime = reloadTime - _reloadInAnimation.length - reloadOutTime;
				isFiring = true;
				_reloadInAnimation.wrapMode = WrapMode.Once;
				if (!isDisabled)
				{
					myAnimator.CrossFade(_reloadInAnimation.name, CROSS_FADE_TIME);
				}
				yield return new WaitForSeconds(_reloadInAnimation.length);
			}
		}
		else
		{
			loopTime = reloadTimeLeft - reloadOutTime;
		}
		if (loopTime > 0f && _reloadLoopAnimation != null)
		{
			_reloadLoopAnimation.wrapMode = WrapMode.Loop;
			if (!isDisabled)
			{
				myAnimator.CrossFade(_reloadLoopAnimation.name, CROSS_FADE_TIME);
			}
			yield return new WaitForSeconds(loopTime);
		}
		if (_reloadOutAnimation != null && reloadTimeLeft > _reloadOutAnimation.length)
		{
			_reloadOutAnimation.wrapMode = WrapMode.Once;
			if (!isDisabled)
			{
				myAnimator.CrossFade(_reloadOutAnimation.name, CROSS_FADE_TIME);
			}
			yield return new WaitForSeconds(_reloadOutAnimation.length - CROSS_FADE_TIME);
		}
		isFiring = false;
		OnIdle();
	}

	private IEnumerator attackCountdown(float time)
	{
		isFiring = true;
		yield return new WaitForSeconds(time);
		isFiring = false;
		OnIdle();
	}

	public override void updateIdleState(bool moving)
	{
		isMoving = moving;
		if (!isFiring)
		{
			OnIdle();
		}
	}

	public override void OnIdle()
	{
		isFiring = false;
		if (isDisabled || currentWeapon == null || (Preferences.Instance.CurrentGameMode != GameMode.ROYL && currentWeapon.isMelee))
		{
			return;
		}
		StopCoroutine("reload");
		if (isMoving && _runAnimation != null)
		{
			_runAnimation.wrapMode = WrapMode.Loop;
			ChargingWeapon chargingWeapon = null;
			if (currentWeapon is ChargingWeapon)
			{
				chargingWeapon = (ChargingWeapon)currentWeapon;
			}
			if (chargingWeapon == null || !chargingWeapon.IsCharging)
			{
				myAnimator.CrossFade(_runAnimation.name);
				currentWeapon.WeaponOnRun(_runAnimation.time);
			}
		}
		else if (_idleAnimation != null)
		{
			_idleAnimation.wrapMode = WrapMode.Loop;
			ChargingWeapon chargingWeapon2 = currentWeapon as ChargingWeapon;
			if (chargingWeapon2 == null || !chargingWeapon2.IsCharging)
			{
				myAnimator.CrossFade(_idleAnimation.name);
				currentWeapon.OnIdle();
			}
		}
	}

	public float getAnimationLength(string animationName)
	{
		return myAnimator[animationName].length;
	}
}
