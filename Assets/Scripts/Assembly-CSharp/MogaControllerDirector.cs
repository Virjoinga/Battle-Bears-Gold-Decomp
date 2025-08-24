using UnityEngine;

public class MogaControllerDirector : ShootButtonControllerDirector
{
	private float _mogaAimMultiplier = 12f;

	private bool _jumpEnabled;

	public override void AddTo(PlayerController player)
	{
		base.AddTo(player);
		_jumpEnabled = HUD.Instance.JumpPurchased;
	}

	public override void UpdateControls(float delta)
	{
		base.UpdateControls(delta);
		if (MogaController.Instance.connection != 1)
		{
			return;
		}
		if (MogaController.Instance.axisX != 0f || MogaController.Instance.axisY != 0f)
		{
			Vector2 vector = new Vector2(MogaController.Instance.axisX, 0f - MogaController.Instance.axisY);
			if (vector.magnitude < MoveJoystickControllerDirector.DEADZONE_RADIUS)
			{
				base.Movement = Vector2.zero;
			}
			else
			{
				base.Movement = vector.normalized;
			}
		}
		else
		{
			base.Movement = Vector2.zero;
		}
		if (MogaController.Instance.axisZ != 0f || MogaController.Instance.axisRZ != 0f)
		{
			base.Aiming += new Vector2(MogaController.Instance.axisZ * _mogaAimMultiplier, (0f - MogaController.Instance.axisRZ) * _mogaAimMultiplier);
		}
		base.Focus = false;
		base.Switch = MogaController.Instance.ButtonPressed(100);
		base.Reload = MogaController.Instance.ButtonPressed(99);
		base.Fire = MogaController.Instance.buttonR == 0 || MogaController.Instance.buttonR2 == 0;
		base.Melee = MogaController.Instance.buttonL == 0 || MogaController.Instance.buttonL2 == 0;
		base.Jump = _jumpEnabled && (base.Jump || MogaController.Instance.buttonA == 0);
		base.Special = MogaController.Instance.buttonB == 0 || base.SpecialButton.IsHeld;
	}
}
