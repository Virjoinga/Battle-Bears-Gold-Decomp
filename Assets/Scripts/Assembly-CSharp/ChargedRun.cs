using System.Collections;
using UnityEngine;

public class ChargedRun : ChargingWeapon
{
	private float _minDuration = 5f;

	private float _maxDuration = 10f;

	private float _chargeDamage = 100f;

	private float _chargeStart;

	private float _chargeSpeed = 600f;

	private bool _chargeStartedOnThisWeapon;

	[SerializeField]
	private GameObject _runningEffect;

	[SerializeField]
	private GameObject _chargeReadyEffect;

	private GameObject _currentRunningEffect;

	private GameObject _chargedEffect;

	public override void ConfigureWeapon(Item item)
	{
		base.ConfigureWeapon(item);
		item.UpdateProperty("minDuration", ref _minDuration, base.EquipmentNames);
		item.UpdateProperty("maxDuration", ref _maxDuration, base.EquipmentNames);
		item.UpdateProperty("damage", ref _chargeDamage, base.EquipmentNames);
		item.UpdateProperty("flyingSpeed", ref _chargeSpeed, base.EquipmentNames);
	}

	public override void BeginCharging()
	{
		base.BeginCharging();
		_chargeStart = Time.fixedTime;
		_chargeStartedOnThisWeapon = true;
		if (!isRemote && base.playerController != null && base.playerController.Motor != null)
		{
			base.playerController.Motor.inputMoveDirection = Vector3.zero;
			base.playerController.Motor.SetControllable(false);
		}
	}

	public override void EndCharging()
	{
		if (!(base.playerController != null) || !(base.playerController.WeaponManager != null) || base.playerController.WeaponManager.isDisabled || base.playerController.IsDead)
		{
			return;
		}
		float t = Mathf.Clamp((Time.fixedTime - _chargeStart) / chargeShotTime, 0f, 1f);
		float num = (firingTime = Mathf.Lerp(_minDuration, _maxDuration, t));
		if (!_chargeStartedOnThisWeapon)
		{
			return;
		}
		_chargeStartedOnThisWeapon = false;
		if (!isRemote && base.playerController != null && base.playerController.Motor != null)
		{
			base.playerController.Motor.SetControllable(true);
			StartCoroutine(ChargeForward(num));
		}
		if (_chargedEffect != null)
		{
			Object.Destroy(_chargedEffect);
		}
		if (_runningEffect != null)
		{
			GameObject gameObject = Object.Instantiate(_runningEffect, Vector3.zero, Quaternion.identity) as GameObject;
			if (gameObject != null)
			{
				if (_currentRunningEffect != null)
				{
					Object.Destroy(_currentRunningEffect);
				}
				_currentRunningEffect = gameObject;
				gameObject.transform.parent = base.transform.root;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				DelayedDestroy delayedDestroy = gameObject.AddComponent<DelayedDestroy>();
				delayedDestroy.delay = num;
				ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
				if (component != null)
				{
					component.OwnerID = base.OwnerID;
					component.SetEquipmentNames(base.EquipmentNames);
					component.SetItemOverride(base.name);
					if (base.playerController != null)
					{
						component.DamageMultiplier = base.playerController.DamageMultiplier;
					}
				}
			}
		}
		base.EndCharging();
	}

	protected override void FullyCharged()
	{
		if (_chargeReadyEffect != null)
		{
			if (_chargedEffect != null)
			{
				Object.Destroy(_chargedEffect);
			}
			_chargedEffect = Object.Instantiate(_chargeReadyEffect, Vector3.zero, Quaternion.identity) as GameObject;
			if (_chargedEffect != null)
			{
				_chargedEffect.transform.parent = base.transform.root;
				_chargedEffect.transform.localPosition = Vector3.zero;
				_chargedEffect.transform.localRotation = Quaternion.identity;
			}
		}
	}

	private IEnumerator ChargeForward(float duration)
	{
		float runEnd = Time.fixedTime + duration;
		if (!(base.playerController != null))
		{
			yield break;
		}
		CharacterController controllerToMove = base.playerController.GetComponent<CharacterController>();
		if (controllerToMove != null)
		{
			while (Time.fixedTime < runEnd && !(base.playerController == null) && !base.playerController.IsDead && !(controllerToMove == null) && !(base.playerController.WeaponManager == null) && !base.playerController.WeaponManager.isDisabled)
			{
				Vector3 moveDir = new Vector3(base.playerController.bodyRotator.forward.x, 0f, base.playerController.bodyRotator.forward.z);
				controllerToMove.Move(moveDir * _chargeSpeed * Time.deltaTime);
				yield return null;
			}
		}
	}

	public void OnDisable()
	{
		if (_currentRunningEffect != null)
		{
			Object.Destroy(_currentRunningEffect);
		}
		if (_chargedEffect != null)
		{
			Object.Destroy(_chargedEffect);
		}
		StopAllCoroutines();
	}
}
