using UnityEngine;

public class KeyboardAndMouseControllerDirector : SimpleControllerDirector
{
	private bool _jumpEnabled;

	public override void AddTo(PlayerController player)
	{
		base.AddTo(player);
		_jumpEnabled = HUD.Instance.JumpPurchased;
		player.PlayersGUI.RemoveComponent(base.JumpButton);
		player.PlayersGUI.RemoveComponent(base.TeamspeakButton);
		player.PlayersGUI.RemoveComponent(base.MeleeButton);
	}

	public override void RemoveFrom(PlayerController player)
	{
		base.RemoveFrom(player);
	}

	public override void UpdateControls(float delta)
	{
		base.UpdateControls(delta);
		base.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		base.Jump = (base.Jump || Input.GetButton("Jump") || Input.GetKeyDown(KeyCode.Space)) && _jumpEnabled;
		base.Aiming = new Vector2(Input.GetAxis("Mouse X") * 15f * (Preferences.Instance.Sensitivity + 0.1f), Input.GetAxis("Mouse Y") * 10f * (Preferences.Instance.Sensitivity + 0.1f));
		base.Fire = Input.GetMouseButton(0);
		base.Reload = Input.GetKeyDown(KeyCode.R);
		base.Switch = Input.GetKeyDown(KeyCode.Tab);
		base.Melee = Input.GetMouseButton(1);
		base.Special = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.LeftShift);
	}
}
