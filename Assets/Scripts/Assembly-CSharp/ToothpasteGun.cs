using System.Collections;
using UnityEngine;

public class ToothpasteGun : RaycastWeapon
{
	private const float PERCENT_OF_ANIMATION_AT_WHICH_LEFT_GUN_FIRES = 0.54f;

	[SerializeField]
	private GameObject _leftGun;

	private bool _isConstantFiring;

	public override void PlayAttackAnimation(float startTime, float speed)
	{
	}

	protected override void Awake()
	{
		base.Awake();
		myAnimation = base.gameObject.GetComponentInChildren<Animation>();
	}

	public override void BeginConstantFireEffects()
	{
		base.BeginConstantFireEffects();
		_isConstantFiring = true;
		if (myAnimation != null && myAnimation["fire"] != null)
		{
			myAnimation["fire"].speed = myAnimation["fire"].length / firingTime;
			myAnimation.CrossFade("fire");
		}
	}

	public override void EndConstantFireEffects()
	{
		base.EndConstantFireEffects();
		_isConstantFiring = false;
		_currentSpawnPointIndex = 0;
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		StartCoroutine(DelayedAttack(false));
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		StartCoroutine(DelayedAttack(true));
	}

	private IEnumerator DelayedAttack(bool isRemote)
	{
		float timeToAttack = Time.time + AnimationDelayForSecondPistol();
		while (Time.time < timeToAttack && _isConstantFiring)
		{
			yield return null;
		}
		if (_isConstantFiring)
		{
			if (isRemote)
			{
				base.OnRemoteAttack(Vector3.zero, Vector3.zero, 0);
			}
			else
			{
				base.OnAttack();
			}
			SendMessageUpwards("HandleContinuousFireWeaponDidAttack");
		}
	}

	private float AnimationDelayForSecondPistol()
	{
		return firingTime * 0.54f;
	}

	public override void WeaponDeath()
	{
		base.WeaponDeath();
		_leftGun.renderer.enabled = false;
	}
}
