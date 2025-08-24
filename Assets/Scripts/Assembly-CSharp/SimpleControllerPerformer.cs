using UnityEngine;

public class SimpleControllerPerformer : ControllerPerformer
{
	[SerializeField]
	private Transform _view;

	[SerializeField]
	private CharacterMotor _motor;

	private bool _previousFire;

	private bool _previousSecondaryFire;

	private bool _wantsToFire;

	private bool _previousTeamspeak;

	[SerializeField]
	private float _minimumAimY = -75f;

	[SerializeField]
	private float _maximumAimY = 75f;

	private Vector3 _desiredEulerAngles;

	private IAimAccelerator _aimAccelerator;

	private float _lerpMultiplier = 0.8f;

	private string _backgroundCameraName = "backgroundCamera";

	private Camera _backgroundCamera;

	private float _zoomFov;

	private float _normalFov;

	private float _smooth = 10f;

	public bool LockInputs { get; set; }

	public bool Disabled { get; set; }

	public bool DisableJump { get; set; }

	private void Start()
	{
		LockInputs = false;
		_desiredEulerAngles = _view.localEulerAngles;
		_aimAccelerator = new CODAimAccelerator();
		GameObject gameObject = GameObject.Find(_backgroundCameraName);
		if (gameObject != null)
		{
			_backgroundCamera = gameObject.gameObject.camera;
		}
		if (Camera.mainCamera != null)
		{
			_normalFov = Camera.mainCamera.fieldOfView;
		}
		else
		{
			_normalFov = 75f;
		}
		_zoomFov = _normalFov / 1.4f;
	}

	public override void PerformControls(ControllerDirector director, float delta)
	{
		if (!Disabled)
		{
			PerformAiming(director, delta);
			if (Preferences.Instance.ZoomModeToggledOn && !base.PlayerController.HasBomb)
			{
				PerformCameraZoom(director, delta);
			}
			PerformPause(director, delta);
			PerformOpenTeamspeak(director, delta);
			if (!LockInputs)
			{
				PerformMovement(director, delta);
				PerformMelee(director, delta);
				PerformSpecial(director, delta);
				UpdateWeaponControls(director, delta);
			}
			else
			{
				_motor.inputMoveDirection = Vector3.zero;
				_motor.inputJump = false;
			}
		}
	}

	private void PerformMovement(ControllerDirector director, float delta)
	{
		Vector2 movement = director.Movement;
		Vector3 vector = new Vector3(movement.x, movement.y, 0f);
		if (vector != Vector3.zero)
		{
			float magnitude = vector.magnitude;
			vector /= magnitude;
			magnitude = Mathf.Min(1f, magnitude);
			magnitude *= magnitude;
			vector *= magnitude;
		}
		vector = _view.rotation * vector;
		Quaternion quaternion = Quaternion.FromToRotation(-_view.forward, base.transform.up);
		vector = quaternion * vector;
		_motor.inputMoveDirection = vector;
		if (!DisableJump)
		{
			_motor.inputJump = director.Jump;
		}
	}

	private void PerformMelee(ControllerDirector director, float delta)
	{
		if (director.Melee)
		{
			base.PlayerController.WeaponManager.OnMeleeAttack();
		}
	}

	private void PerformSpecial(ControllerDirector director, float delta)
	{
		if (director.Special && HUD.Instance.isSpecialAllowed && !base.PlayerController.HasBomb && base.PlayerController.NextSpecialItemChargeTime - Time.time <= 0f)
		{
			base.PlayerController.OnUseSpecialItem();
			SimpleControllerDirector simpleControllerDirector = (SimpleControllerDirector)director;
			if (simpleControllerDirector != null)
			{
				simpleControllerDirector.SpecialButton.StartCooldown(base.PlayerController.NextSpecialItemChargeTime);
			}
		}
	}

	private void PerformAiming(ControllerDirector director, float delta)
	{
		Vector2 aiming = director.Aiming;
		if (Preferences.Instance.Sensitivity > 0f)
		{
			aiming *= Preferences.Instance.Sensitivity * _aimAccelerator.CalculateSensitivityCoefficient(aiming, delta);
		}
		else
		{
			aiming *= 0.05f * _aimAccelerator.CalculateSensitivityCoefficient(aiming, delta);
		}
		_desiredEulerAngles.y += aiming.x;
		_desiredEulerAngles.x -= aiming.y;
		_desiredEulerAngles.x = Mathf.Clamp(_desiredEulerAngles.x, _minimumAimY, _maximumAimY);
		_desiredEulerAngles.z = 0f;
		float num = (_desiredEulerAngles.y - _view.localEulerAngles.y) * _lerpMultiplier;
		Quaternion localRotation = Quaternion.Lerp(_view.localRotation, Quaternion.Euler(_desiredEulerAngles), _lerpMultiplier);
		_view.localRotation = localRotation;
		_desiredEulerAngles.y -= num;
	}

	private void PerformCameraZoom(ControllerDirector director, float delta)
	{
		if (base.PlayerController == null || base.PlayerController.WeaponManager == null)
		{
			return;
		}
		if (Camera.mainCamera != null && !base.PlayerController.IsDead && base.PlayerController.WeaponManager.CurrentWeapon.isConstantFire)
		{
			if (director.Fire)
			{
				Camera.mainCamera.fieldOfView = Mathf.Lerp(Camera.mainCamera.fieldOfView, _zoomFov, delta * _smooth);
				if (_backgroundCamera != null)
				{
					_backgroundCamera.fieldOfView = Mathf.Lerp(_backgroundCamera.fieldOfView, _zoomFov, delta * _smooth);
				}
			}
			else
			{
				Camera.mainCamera.fieldOfView = Mathf.Lerp(Camera.mainCamera.fieldOfView, _normalFov, delta * _smooth);
				if (_backgroundCamera != null)
				{
					_backgroundCamera.fieldOfView = Mathf.Lerp(_backgroundCamera.fieldOfView, _normalFov, delta * _smooth);
				}
			}
		}
		else if (Camera.mainCamera != null && (!base.PlayerController.WeaponManager.CurrentWeapon.isConstantFire || base.PlayerController.IsDead))
		{
			Camera.mainCamera.fieldOfView = _normalFov;
			if (_backgroundCamera != null)
			{
				_backgroundCamera.fieldOfView = _normalFov;
			}
		}
	}

	private void PerformPause(ControllerDirector director, float delta)
	{
		if (director.Pause)
		{
			HUD.Instance.createPauseMenu();
		}
	}

	private void PerformOpenTeamspeak(ControllerDirector director, float delta)
	{
		if (director.Teamspeak && !_previousTeamspeak)
		{
			HUD.Instance.createTeamSpeakPopup();
		}
		_previousTeamspeak = director.Teamspeak;
	}

	private void UpdateWeaponControls(ControllerDirector director, float delta)
	{
		if (!(base.PlayerController != null) || !(base.PlayerController.WeaponManager != null))
		{
			return;
		}
		if (director.Reload && HUD.Instance.isReloadAllowed && !base.PlayerController.HasBomb)
		{
			base.PlayerController.WeaponManager.OnForceReload();
		}
		if (director.Switch && base.PlayerController.WeaponManager.OnNextWeapon() && !base.PlayerController.HasBomb && base.PlayerController.canSwitchWeapons)
		{
			HUD.Instance.SwitchedWeapons();
		}
		if (!_previousFire && director.Fire && !director.Switch)
		{
			if (base.PlayerController.WeaponManager.CanFireCurrentWeapon() && !base.PlayerController.WeaponManager.isAttackingMelee)
			{
				BeginFiringCurrentWeapon();
			}
			else
			{
				_wantsToFire = true;
			}
		}
		else if (_previousFire && !director.Fire)
		{
			_wantsToFire = false;
			StopFiringCurrentWeapon();
		}
		else if (_previousFire && director.Fire)
		{
			if (_wantsToFire && base.PlayerController.WeaponManager.CanFireCurrentWeapon() && !base.PlayerController.WeaponManager.isAttackingMelee)
			{
				_wantsToFire = false;
				BeginFiringCurrentWeapon();
			}
			if (!base.PlayerController.WeaponManager.isConstantFireMode() && !base.PlayerController.WeaponManager.isChargableFireMode())
			{
				base.PlayerController.WeaponManager.OnFire();
			}
		}
		if (!_previousSecondaryFire && director.FireSecondary && base.PlayerController.WeaponManager is SatelliteSecondaryWeaponManager)
		{
			SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)base.PlayerController.WeaponManager;
			satelliteSecondaryWeaponManager.ActivateSecondaryWeapon();
		}
		_previousFire = director.Fire;
		_previousSecondaryFire = director.FireSecondary;
	}

	private void BeginFiringCurrentWeapon()
	{
		if (!base.PlayerController.HasBomb)
		{
			if (base.PlayerController.WeaponManager.isConstantFireMode())
			{
				base.PlayerController.WeaponManager.OnBeginFiring();
			}
			else if (base.PlayerController.WeaponManager.isChargableFireMode())
			{
				base.PlayerController.WeaponManager.OnBeginCharging();
			}
			else
			{
				base.PlayerController.WeaponManager.OnFire();
			}
		}
	}

	private void StopFiringCurrentWeapon()
	{
		if (!base.PlayerController.HasBomb && (base.PlayerController.WeaponManager.isConstantFireMode() || base.PlayerController.WeaponManager.isChargableFireMode()))
		{
			base.PlayerController.WeaponManager.OnStopFiring();
		}
	}
}
