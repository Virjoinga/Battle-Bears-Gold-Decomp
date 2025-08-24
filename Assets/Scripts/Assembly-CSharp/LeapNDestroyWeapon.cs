using System.Collections;
using UnityEngine;

public class LeapNDestroyWeapon : FireInOutLoopWeapon
{
	private static readonly float CROSS_FADE_TIME = 0.2f;

	[SerializeField]
	private GameObject _jumpStartEffect;

	[SerializeField]
	private GameObject _jumpLoopEffect;

	[SerializeField]
	private float _jumpPower = 1000f;

	[SerializeField]
	private GameObject _landEffect;

	[SerializeField]
	private float _jumpDelay = 0.5f;

	[SerializeField]
	private float _explosionDelayAfterLanding = 0.25f;

	[SerializeField]
	private float _landControlDelay = 1f;

	private bool _fireCoroutineRunning;

	private float _groundedCheckInterval = 0.05f;

	protected override void Start()
	{
		base.Start();
		ResetRiggedWeaponAnimation();
	}

	public override bool OnAttack()
	{
		if (!_fireCoroutineRunning && base.playerController.Motor.grounded)
		{
			_fireCoroutineRunning = true;
			if (base.NetSyncReporter != null)
			{
				base.NetSyncReporter.SpawnProjectile(base.gameObject.transform.position, Vector3.zero);
			}
			base.OnAttack();
			return false;
		}
		return false;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		if (!_fireCoroutineRunning)
		{
			_fireCoroutineRunning = true;
			base.OnAttack();
		}
		else
		{
			_fireCoroutineRunning = false;
			base.playerController.Motor.grounded = true;
		}
	}

	protected override IEnumerator FireInOutLoopRoutine()
	{
		yield return StartCoroutine(base.FireInOutLoopRoutine());
		SetMovementAndWeaponSwitching(false);
		base.playerController.BodyAnimator.Animator.Stop();
		base.playerController.BodyAnimator.Animator.CrossFade(_fireIn.name);
		PlayWeaponAnimation("fireIn", WrapMode.Once, CROSS_FADE_TIME);
		GameObject fireInEffectObject = (GameObject)Object.Instantiate(_jumpStartEffect, base.transform.position, base.transform.rotation);
		fireInEffectObject.transform.parent = base.transform;
		yield return new WaitForSeconds(_jumpDelay);
		GameObject fireLoopEffectObject = (GameObject)Object.Instantiate(_jumpLoopEffect, base.transform.position, base.transform.rotation);
		fireLoopEffectObject.transform.parent = base.transform;
		if (!isRemote)
		{
			Vector3 jumpDir = (base.playerController.gameObject.transform.forward + base.playerController.gameObject.transform.up).normalized;
			base.playerController.Motor.SetVelocity(_jumpPower * jumpDir);
		}
		else
		{
			base.playerController.Motor.grounded = false;
		}
		yield return new WaitForSeconds(_fireIn.length - _jumpDelay);
		Object.Destroy(fireInEffectObject);
		base.playerController.BodyAnimator.Animator.CrossFade(_fireLoop.name);
		PlayWeaponAnimation("fireLoop", WrapMode.Loop, CROSS_FADE_TIME);
		while (!base.playerController.Motor.grounded && !base.playerController.IsDead)
		{
			yield return new WaitForSeconds(_groundedCheckInterval);
		}
		Object.Destroy(fireLoopEffectObject);
		if (base.playerController.IsDead)
		{
			if (isRemote)
			{
				base.playerController.Motor.grounded = true;
			}
			_fireCoroutineRunning = false;
			ResetRiggedWeaponAnimation();
			SetMovementAndWeaponSwitching(true);
			ForceDeathAnimation();
			yield break;
		}
		base.playerController.BodyAnimator.Animator.CrossFade(_fireOut.name);
		PlayWeaponAnimation("fireOut", WrapMode.Once, CROSS_FADE_TIME);
		if (!isRemote)
		{
			base.playerController.Motor.SetVelocity(Vector3.zero);
		}
		if (_fireOut.length > _explosionDelayAfterLanding + CROSS_FADE_TIME)
		{
			yield return new WaitForSeconds(_explosionDelayAfterLanding);
			SpawnExplosionEffect();
			yield return new WaitForSeconds(_fireOut.length - CROSS_FADE_TIME - _explosionDelayAfterLanding);
			DoIdleAnimation();
		}
		else
		{
			yield return new WaitForSeconds(_fireOut.length - CROSS_FADE_TIME);
			DoIdleAnimation();
			yield return new WaitForSeconds(_explosionDelayAfterLanding - (_fireOut.length - CROSS_FADE_TIME));
			SpawnExplosionEffect();
		}
		yield return new WaitForSeconds(_landControlDelay - _explosionDelayAfterLanding);
		_fireCoroutineRunning = false;
		SetMovementAndWeaponSwitching(true);
		StartCoroutine(PlayReloadAnimation());
	}

	private new IEnumerator PlayReloadAnimation()
	{
		yield return null;
		base.playerController.WeaponManager.OnPlayReload(0f);
	}

	private void SpawnExplosionEffect()
	{
		if (_landEffect != null)
		{
			Vector3 position = base.gameObject.transform.root.position;
			position.y -= base.playerController.collider.bounds.size.y;
			GameObject gameObject = Object.Instantiate(_landEffect, position, Quaternion.identity) as GameObject;
			InstantaneousDamageSource component = gameObject.GetComponent<InstantaneousDamageSource>();
			if (component != null)
			{
				component.SetItemOverride(base.name);
				component.SetEquipmentNames(base.EquipmentNames);
				component.OwnerID = ownerID;
				component.IgnoreOwner = true;
				component.DamageMultiplier = base.playerController.DamageMultiplier;
			}
			else
			{
				Debug.Log("Instantaneous Damage Source is null!");
			}
		}
		else
		{
			Debug.Log("Land Effect is null!");
		}
		if (!isRemote && base.NetSyncReporter != null)
		{
			base.NetSyncReporter.SpawnProjectile(Vector3.zero, Vector3.zero);
		}
		base.playerController.WeaponManager.SetCurrentClipSize(0);
	}

	private void DoIdleAnimation()
	{
		base.playerController.BodyAnimator.isDisabled = false;
		base.playerController.BodyAnimator.OnIdle();
	}

	private void ForceDeathAnimation()
	{
		base.playerController.BodyAnimator.isDisabled = false;
		if (!base.playerController.BodyAnimator.Animator.clip.name.Equals("death"))
		{
			base.playerController.BodyAnimator.Animator.Stop();
			base.playerController.BodyAnimator.Animator.Play("death");
		}
	}

	private void SetMovementAndWeaponSwitching(bool enabled)
	{
		if (!isRemote)
		{
			base.playerController.canSwitchWeapons = enabled;
			base.playerController.Motor.canControl = enabled;
		}
	}

	private void ResetRiggedWeaponAnimation()
	{
		PlayWeaponAnimation("fireIn", WrapMode.Once, CROSS_FADE_TIME, 0f);
	}
}
