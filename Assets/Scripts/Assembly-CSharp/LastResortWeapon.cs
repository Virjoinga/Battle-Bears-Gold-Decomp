using System.Collections;
using UnityEngine;

public class LastResortWeapon : ChargingWeapon
{
	private static readonly float CROSS_FADE_TIME = 0.2f;

	private static readonly float LONG_CROSS_FADE_TIME = 0.4f;

	private static readonly string PLAY_OUT_METHOD_NAME = "PlayOutAnimationAndRestoreControl";

	private static readonly string PLAY_IN_METHOD_NAME = "InAndLoopAnimationRoutine";

	private static readonly string FIRE_IN = "_fireIn";

	private static readonly string FIRE_LOOP = "_fireLoop";

	private static readonly string FIRE_OUT = "_fireOut";

	[SerializeField]
	private float _explosionDelayTime;

	[SerializeField]
	private float _playerDeathDelayTime;

	[SerializeField]
	private GameObject _explosionEffect;

	private GameObject _chargeEffectObject;

	protected AnimationState _fireIn;

	protected AnimationState _fireLoop;

	protected AnimationState _fireOut;

	protected string _fireInAnimName;

	protected string _fireLoopAnimName;

	protected string _fireOutAnimName;

	protected override void Start()
	{
		base.Start();
		PlayWeaponAnimation("fire", WrapMode.Once, 0f, 0f);
		_fireInAnimName = base.name + FIRE_IN;
		_fireLoopAnimName = base.name + FIRE_LOOP;
		_fireOutAnimName = base.name + FIRE_OUT;
	}

	public override void OnIdle()
	{
		base.OnIdle();
		if (base.playerController != null && base.playerController.IsDead)
		{
			isAnimatingReload = false;
			PlayWeaponAnimation("fire", WrapMode.Once, 0f, 0f);
		}
	}

	public override void BeginCharging()
	{
		PlayWeaponAnimation("fire", WrapMode.Once, 0f);
		StartCoroutine(PLAY_IN_METHOD_NAME);
		SpawnChargeEffect();
		SetMovementAndWeaponSwitching(false);
		base.BeginCharging();
	}

	public override void EndCharging()
	{
		if (!CheckForDeath() && !base.playerController.WeaponManager.isAttackingMelee)
		{
			if (!isRemote && base.playerController.WeaponManager.GetCurrentClipSize(1) > 0)
			{
				StopWeaponAnimations();
				base.playerController.WeaponManager.SetCurrentClipSize(0);
				StartCoroutine(PLAY_OUT_METHOD_NAME);
				DestroyChargeEffect();
				base.NetSyncReporter.SetAction(21, null);
			}
			if (isRemote)
			{
				StopWeaponAnimations();
				StartCoroutine(PLAY_OUT_METHOD_NAME);
				DestroyChargeEffect();
			}
			base.EndCharging();
		}
	}

	protected override IEnumerator ChargingCoRoutine()
	{
		float timeOfStart = Time.time;
		while (Time.time - timeOfStart < chargeShotTime)
		{
			myAudio.pitch = Mathf.MoveTowards(myAudio.pitch, fullyChargedPitch, Time.deltaTime / chargeShotTime);
			yield return null;
		}
		if (fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(true);
		}
		base.playerController.WeaponManager.OnFireChargedShot();
	}

	public override bool OnChargedAttack()
	{
		if (!base.playerController.IsDead)
		{
			if (base.NetSyncReporter != null)
			{
				base.NetSyncReporter.SpawnProjectile(base.gameObject.transform.position, Vector3.zero);
			}
			CreateExplosion(base.gameObject.transform.position);
		}
		SetMovementAndWeaponSwitching(true);
		base.playerController.BodyAnimator.OnIdle();
		return base.OnChargedAttack();
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		CreateExplosion(pos);
	}

	private void CreateExplosion(Vector3 position)
	{
		GameObject gameObject = Object.Instantiate(_explosionEffect, position, base.playerController.transform.rotation) as GameObject;
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.SetItemOverride(base.name);
		component.SetEquipmentNames(base.EquipmentNames);
		component.OwnerID = ownerID;
		component.DamageMultiplier = base.playerController.DamageMultiplier;
		if (!isRemote)
		{
			base.playerController.DamageReceiver.OnKilledByDeathArea();
		}
	}

	private IEnumerator InAndLoopAnimationRoutine()
	{
		_fireIn = base.playerController.BodyAnimator.Animator[_fireInAnimName];
		_fireLoop = base.playerController.BodyAnimator.Animator[_fireLoopAnimName];
		StopCoroutine(PLAY_OUT_METHOD_NAME);
		base.playerController.BodyAnimator.Animator.CrossFade(_fireIn.name);
		yield return new WaitForSeconds(_fireIn.length - LONG_CROSS_FADE_TIME);
		base.playerController.BodyAnimator.Animator.CrossFade(_fireLoop.name, LONG_CROSS_FADE_TIME);
	}

	private IEnumerator PlayOutAnimationAndRestoreControl()
	{
		_fireOut = base.playerController.BodyAnimator.Animator[_fireOutAnimName];
		StopCoroutine(PLAY_IN_METHOD_NAME);
		base.playerController.BodyAnimator.Animator.Play(_fireOut.name);
		yield return new WaitForSeconds(_fireOut.length - LONG_CROSS_FADE_TIME);
		DoIdleAnimation();
		yield return new WaitForSeconds(LONG_CROSS_FADE_TIME);
		SetMovementAndWeaponSwitching(true);
		StartCoroutine(PlayReloadAnimation());
	}

	private new IEnumerator PlayReloadAnimation()
	{
		yield return null;
		base.playerController.WeaponManager.OnPlayReload(0f);
	}

	private bool CheckForDeath()
	{
		if (base.playerController.IsDead)
		{
			ForceDeathAnimation();
			return true;
		}
		return false;
	}

	private void ForceDeathAnimation()
	{
		base.playerController.BodyAnimator.isDisabled = false;
		base.playerController.BodyAnimator.StopAllCoroutines();
		StopAllCoroutines();
		base.playerController.WeaponManager.StopAllCoroutines();
		SetMovementAndWeaponSwitching(true);
		if (!base.playerController.BodyAnimator.Animator.clip.name.Equals("death"))
		{
			base.playerController.BodyAnimator.Animator.Stop();
			base.playerController.BodyAnimator.Animator.Play("death");
		}
	}

	private void DoIdleAnimation()
	{
		base.playerController.BodyAnimator.isDisabled = false;
		base.playerController.BodyAnimator.OnIdle();
	}

	private void SetMovementAndWeaponSwitching(bool enabled)
	{
		base.playerController.canSwitchWeapons = enabled;
		base.playerController.Motor.canControl = enabled;
	}

	private void SpawnChargeEffect()
	{
		_chargeEffectObject = (GameObject)Object.Instantiate(chargeEffect, base.transform.position, base.transform.rotation);
		_chargeEffectObject.SetActive(true);
		_chargeEffectObject.transform.parent = base.transform;
	}

	private void DestroyChargeEffect()
	{
		Object.Destroy(_chargeEffectObject);
	}
}
