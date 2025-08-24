using System.Collections;
using UnityEngine;

public abstract class BodyAnimatorBase : MonoBehaviour
{
	protected Animation myAnimator;

	public bool isMoving;

	public bool isDisabled;

	protected string currentWeaponName = string.Empty;

	protected bool isFiring;

	public Animation Animator
	{
		get
		{
			return myAnimator;
		}
	}

	public bool IsFiring
	{
		get
		{
			return isFiring;
		}
		set
		{
			isFiring = value;
		}
	}

	public virtual void Awake()
	{
		myAnimator = base.GetComponent<Animation>();
	}

	public virtual void OnReset()
	{
		isFiring = false;
		isMoving = false;
		OnIdle();
	}

	public abstract void OnIdle();

	public virtual void OnAttack()
	{
	}

	public abstract void updateIdleState(bool moving);

	public abstract float OnMeleeAttack();

	public virtual float GetTurretLayingInTime()
	{
		return 0f;
	}

	public virtual float GetTurretLayingOutTime()
	{
		return 0f;
	}

	public virtual float GetDeployableLayingInTime()
	{
		return 0f;
	}

	public virtual float GetDeployableLayingOutTime()
	{
		return 0f;
	}

	protected IEnumerator delayedMelee(float delay)
	{
		yield return new WaitForSeconds(delay);
		isFiring = false;
	}

	public void OnGetBomb()
	{
		StopAllCoroutines();
		isDisabled = true;
		myAnimator["bomb"].layer = 0;
		myAnimator["bomb"].wrapMode = WrapMode.Loop;
		myAnimator.Stop();
		myAnimator.CrossFade("bomb");
	}

	public void OnReleaseBomb()
	{
		isDisabled = false;
		OnIdle();
	}

	public void Pause()
	{
		myAnimator.enabled = false;
	}

	public void UnPause()
	{
		myAnimator.enabled = true;
	}
}
