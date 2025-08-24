using UnityEngine;

public class BodyAnimatorB1000 : BodyAnimator
{
	private AnimationState _meleeAnimation;

	private string _secondaryWeaponPrefix;

	private string _meleeAnimPrefix;

	public string SecondaryAnimPrefix
	{
		get
		{
			return _secondaryWeaponPrefix;
		}
		set
		{
			_secondaryWeaponPrefix = value;
		}
	}

	public string MeleeAnimPrefix
	{
		get
		{
			return _meleeAnimPrefix;
		}
		set
		{
			if (value.Contains("_"))
			{
				_meleeAnimPrefix = value.Split('_')[1];
			}
			else
			{
				_meleeAnimPrefix = value;
			}
		}
	}

	public bool UsingSecondaryAnims { get; set; }

	public override void OnSetWeapon(WeaponBase w, bool isMelee)
	{
		base.OnSetWeapon(w, isMelee);
		if (UsingSecondaryAnims)
		{
			_runAnimation = myAnimator[_secondaryWeaponPrefix + "_run"];
			_runAnimation.layer = 0;
			_runAnimation.speed = currentWeapon.walkAnimationSpeed;
			_idleAnimation = myAnimator[_secondaryWeaponPrefix + "_idle"];
			_idleAnimation.layer = 0;
			_idleAnimation.speed = currentWeapon.idleAnimationSpeed;
			_reloadInAnimation = myAnimator[_secondaryWeaponPrefix + "_reloadIn"];
			_reloadInAnimation.layer = 0;
			_reloadLoopAnimation = myAnimator[_secondaryWeaponPrefix + "_reloadLoop"];
			_reloadLoopAnimation.layer = 0;
			_reloadLoopAnimation.wrapMode = WrapMode.Loop;
			_reloadOutAnimation = myAnimator[_secondaryWeaponPrefix + "_reloadOut"];
			_reloadOutAnimation.layer = 0;
		}
		_meleeAnimation = myAnimator[_meleeAnimPrefix + "_fire"];
		OnIdle();
	}

	public override float OnMeleeAttack()
	{
		if (isDisabled)
		{
			return 0f;
		}
		myAnimator.Stop(_meleeAnimation.name);
		StopAllCoroutines();
		isFiring = true;
		_meleeAnimation.wrapMode = WrapMode.Once;
		myAnimator.Play(_meleeAnimation.name);
		StartCoroutine("delayedMelee", _meleeAnimation.length);
		return _meleeAnimation.length;
	}
}
