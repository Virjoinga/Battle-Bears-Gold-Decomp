using UnityEngine;

public class WASDControllerDirector : ShootButtonControllerDirector
{
	private bool _jumpEnabled;

	public override void AddTo(PlayerController player)
	{
		base.AddTo(player);
		_jumpEnabled = HUD.Instance.JumpPurchased;
	}

	public override void UpdateControls(float delta)
	{
		base.UpdateControls(delta);
		base.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		base.Jump = (base.Jump || Input.GetButton("Jump") || Input.GetKeyDown(KeyCode.Space)) && _jumpEnabled;
		base.Aiming = new Vector2(Input.GetAxis("Mouse X") * 15f, Input.GetAxis("Mouse Y") * 10f);
		base.Focus = Input.GetMouseButton(1);
		base.Fire = Input.GetMouseButton(0);
		base.Reload = Input.GetKeyDown(KeyCode.R) || (base.ReloadButton != null && base.ReloadButton.IsHeld);
		base.Switch = Input.GetKeyDown(KeyCode.Tab) || (base.SwitchButton != null && base.SwitchButton.WasPressed);
		base.Melee = Input.GetKeyDown(KeyCode.E) || Input.GetMouseButton(1) || (base.MeleeButton != null && base.MeleeButton.IsHeld);
		base.Special = Input.GetKeyDown(KeyCode.LeftShift) || (base.SpecialButton != null && base.SpecialButton.IsHeld);
	}
}
