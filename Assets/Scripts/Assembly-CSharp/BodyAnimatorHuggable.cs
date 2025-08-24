using System.Collections;
using UnityEngine;

public class BodyAnimatorHuggable : BodyAnimatorBase
{
	private static readonly float CROSS_FADE_TIME = 0.2f;

	private WeaponBase currentWeapon;

	public override void Awake()
	{
		base.Awake();
		myAnimator["tired"].wrapMode = WrapMode.Loop;
		myAnimator["tired"].layer = 0;
	}

	public void OnSetAttack(GameObject attack, bool isMelee)
	{
		OnSetAttack(attack, isMelee, false);
	}

	public void OnSetAttack(GameObject attack, bool isMelee, bool isCharge)
	{
		isFiring = false;
		StopAllCoroutines();
		if (attack.name.Contains("_"))
		{
			currentWeaponName = attack.name.Split('_')[1];
		}
		else
		{
			currentWeaponName = attack.name;
		}
		currentWeapon = attack.GetComponent(typeof(WeaponBase)) as WeaponBase;
		if (isMelee)
		{
			myAnimator[currentWeaponName].layer = 2;
			return;
		}
		if (isCharge)
		{
			if (currentWeapon.isFireInLoopOut)
			{
				myAnimator[currentWeaponName + "_fireIn"].layer = 2;
				myAnimator[currentWeaponName + "_fireLoop"].layer = 2;
				myAnimator[currentWeaponName + "_fireLoop"].wrapMode = WrapMode.Loop;
				myAnimator[currentWeaponName + "_fireOut"].layer = 2;
			}
			else
			{
				myAnimator[currentWeaponName + "_fire"].layer = 2;
				if (currentWeapon != null)
				{
					myAnimator[currentWeaponName + "_fire"].speed = currentWeapon.attackAnimationSpeed;
					myAnimator[currentWeaponName + "_charge"].speed = currentWeapon.attackAnimationSpeed;
					myAnimator[currentWeaponName + "_run"].speed = currentWeapon.walkAnimationSpeed;
					myAnimator[currentWeaponName + "_idle"].speed = currentWeapon.idleAnimationSpeed;
				}
			}
			myAnimator[currentWeaponName + "_charge"].layer = 2;
			myAnimator[currentWeaponName + "_run"].layer = 0;
			myAnimator[currentWeaponName + "_idle"].layer = 0;
		}
		else
		{
			if (currentWeapon.isFireInLoopOut)
			{
				myAnimator[currentWeaponName + "_fireIn"].layer = 2;
				myAnimator[currentWeaponName + "_fireLoop"].layer = 2;
				myAnimator[currentWeaponName + "_fireLoop"].wrapMode = WrapMode.Loop;
				myAnimator[currentWeaponName + "_fireOut"].layer = 2;
			}
			else
			{
				myAnimator[currentWeaponName + "_fire"].layer = 2;
			}
			myAnimator[currentWeaponName + "_run"].layer = 0;
			myAnimator[currentWeaponName + "_idle"].layer = 0;
			if (currentWeapon != null)
			{
				if (currentWeapon.isFireInLoopOut)
				{
					myAnimator[currentWeaponName + "_fireIn"].speed = currentWeapon.attackAnimationSpeed;
					myAnimator[currentWeaponName + "_fireLoop"].speed = currentWeapon.attackAnimationSpeed;
					myAnimator[currentWeaponName + "_fireOut"].speed = currentWeapon.attackAnimationSpeed;
				}
				else
				{
					myAnimator[currentWeaponName + "_fire"].speed = currentWeapon.attackAnimationSpeed;
				}
				myAnimator[currentWeaponName + "_run"].speed = currentWeapon.walkAnimationSpeed;
				myAnimator[currentWeaponName + "_idle"].speed = currentWeapon.idleAnimationSpeed;
			}
		}
		OnIdle();
	}

	public void OnBeginContinuousAttack()
	{
		if (!isDisabled)
		{
			isFiring = true;
			myAnimator[currentWeaponName + "_fire"].wrapMode = WrapMode.Loop;
			myAnimator.CrossFade(currentWeaponName + "_fire");
		}
	}

	public void OnStopContinuousAttack()
	{
		isFiring = false;
	}

	public override float OnMeleeAttack()
	{
		if (isDisabled || isFiring)
		{
			return 0f;
		}
		StopAllCoroutines();
		isFiring = true;
		myAnimator[currentWeaponName].wrapMode = WrapMode.Once;
		myAnimator.Play(currentWeaponName);
		StartCoroutine("delayedMelee", myAnimator[currentWeaponName].length);
		OnIdle();
		return myAnimator[currentWeaponName].length;
	}

	public void OnAttack(float duration)
	{
		if (isDisabled || isFiring)
		{
			return;
		}
		MeleeAttack meleeAttack = currentWeapon as MeleeAttack;
		if (meleeAttack != null || currentWeapon.isFireInLoopOut)
		{
			StartCoroutine(executeAttack(duration));
		}
		else
		{
			if (isDisabled)
			{
				return;
			}
			float num = 0f;
			if (currentWeapon.isMine || currentWeapon.isTurret)
			{
				StartCoroutine(placeDeployable());
				duration = myAnimator[currentWeaponName + "_fireIn"].length + myAnimator[currentWeaponName + "_fireOut"].length;
			}
			else if (currentWeapon != null)
			{
				if (currentWeapon.isFireInLoopOut)
				{
					StartCoroutine("DoFireInFireLoopFireOut", duration);
					num = duration;
				}
				else
				{
					myAnimator.Play(currentWeaponName + "_fire");
					num = myAnimator[currentWeaponName + "_fire"].length;
				}
			}
			StopCoroutine("attackCountdown");
			StartCoroutine("attackCountdown", num);
		}
	}

	private IEnumerator DoFireInFireLoopFireOut(float firingTime)
	{
		if (isDisabled || !(myAnimator != null))
		{
			yield break;
		}
		float loopTime = 0f;
		if (myAnimator[currentWeaponName + "_fireIn"] != null)
		{
			float fireOutTime = 0f;
			if (myAnimator[currentWeaponName + "_fireOut"] != null)
			{
				fireOutTime = myAnimator[currentWeaponName + "_fireOut"].length;
			}
			loopTime = firingTime - myAnimator[currentWeaponName + "_fireIn"].length - fireOutTime;
			isFiring = true;
			myAnimator[currentWeaponName + "_fireIn"].wrapMode = WrapMode.Once;
			if (!isDisabled)
			{
				myAnimator.CrossFade(myAnimator[currentWeaponName + "_fireIn"].name, CROSS_FADE_TIME);
			}
			yield return new WaitForSeconds(myAnimator[currentWeaponName + "_fireIn"].length);
		}
		if (loopTime > 0f && myAnimator[currentWeaponName + "_fireLoop"] != null)
		{
			myAnimator[currentWeaponName + "_fireLoop"].wrapMode = WrapMode.Loop;
			myAnimator[currentWeaponName + "_fireLoop"].layer = 2;
			if (!isDisabled)
			{
				myAnimator.CrossFade(myAnimator[currentWeaponName + "_fireLoop"].name, 0.1f);
			}
			yield return new WaitForSeconds(loopTime);
			myAnimator[currentWeaponName + "_fireLoop"].layer = 0;
		}
		if (myAnimator[currentWeaponName + "_fireOut"] != null)
		{
			myAnimator[currentWeaponName + "_fireOut"].wrapMode = WrapMode.Once;
			if (!isDisabled)
			{
				myAnimator.CrossFade(myAnimator[currentWeaponName + "_fireOut"].name, CROSS_FADE_TIME);
			}
			yield return new WaitForSeconds(myAnimator[currentWeaponName + "_fireOut"].length - CROSS_FADE_TIME);
		}
		isFiring = false;
		OnIdle();
	}

	private IEnumerator placeDeployable()
	{
		myAnimator.Play(currentWeaponName + "_fireIn");
		yield return new WaitForSeconds(myAnimator[currentWeaponName + "_fireIn"].length);
		myAnimator.Play(currentWeaponName + "_fireOut");
	}

	private IEnumerator attackCountdown(float time)
	{
		isFiring = true;
		yield return new WaitForSeconds(time);
		isFiring = false;
		OnIdle();
	}

	private IEnumerator executeAttack(float duration)
	{
		if (isDisabled)
		{
			yield break;
		}
		isFiring = true;
		float speedMultiplier = 1f;
		if (currentWeapon != null)
		{
			speedMultiplier = currentWeapon.attackAnimationSpeed;
		}
		if (currentWeapon.isFireInLoopOut)
		{
			float loopTime = duration - myAnimator[currentWeaponName + "_fireIn"].length / speedMultiplier - myAnimator[currentWeaponName + "_fireOut"].length / speedMultiplier;
			if (isDisabled)
			{
				yield break;
			}
			myAnimator.Play(currentWeaponName + "_fireIn");
			yield return new WaitForSeconds(myAnimator[currentWeaponName + "_fireIn"].length / speedMultiplier);
			if (isDisabled)
			{
				yield break;
			}
			myAnimator.Play(currentWeaponName + "_fireLoop");
			if (loopTime > 0f)
			{
				yield return new WaitForSeconds(loopTime);
			}
			if (isDisabled)
			{
				yield break;
			}
			myAnimator.CrossFade(currentWeaponName + "_fireOut");
			yield return new WaitForSeconds(myAnimator[currentWeaponName + "_fireOut"].length / speedMultiplier);
		}
		else
		{
			if (isDisabled)
			{
				yield break;
			}
			myAnimator.Play(currentWeaponName + "_fire");
			yield return new WaitForSeconds(myAnimator[currentWeaponName + "_fire"].length / speedMultiplier);
		}
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

	public void OnWeaponCharge()
	{
		if (!isDisabled)
		{
			myAnimator[currentWeaponName + "_charge"].wrapMode = WrapMode.Loop;
			myAnimator.Play(currentWeaponName + "_charge");
		}
	}

	public override void OnIdle()
	{
		if (isDisabled)
		{
			return;
		}
		if (isMoving)
		{
			if (myAnimator[currentWeaponName + "_run"] != null)
			{
				myAnimator[currentWeaponName + "_run"].wrapMode = WrapMode.Loop;
				myAnimator.CrossFade(currentWeaponName + "_run", 0.5f);
			}
		}
		else if (myAnimator[currentWeaponName + "_idle"] != null)
		{
			myAnimator[currentWeaponName + "_idle"].wrapMode = WrapMode.Loop;
			myAnimator.CrossFade(currentWeaponName + "_idle", 0.5f);
		}
	}

	public float getAnimationLength(string animationName)
	{
		if (myAnimator[animationName] != null)
		{
			return myAnimator[animationName].length;
		}
		return 0f;
	}

	public void ForceIdle()
	{
		myAnimator.Stop();
		if (isMoving)
		{
			if (myAnimator[currentWeaponName + "_run"] != null)
			{
				myAnimator[currentWeaponName + "_run"].wrapMode = WrapMode.Loop;
				myAnimator.Play(currentWeaponName + "_run");
			}
		}
		else if (myAnimator[currentWeaponName + "_idle"] != null)
		{
			myAnimator[currentWeaponName + "_idle"].wrapMode = WrapMode.Loop;
			myAnimator.Play(currentWeaponName + "_idle");
		}
	}
}
