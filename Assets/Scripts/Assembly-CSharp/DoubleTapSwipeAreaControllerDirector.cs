using UnityEngine;

public class DoubleTapSwipeAreaControllerDirector : MoveJoystickControllerDirector
{
	private GUIDoubleTapJoystickArea _swipeArea;

	public override void AddTo(PlayerController player)
	{
		base.AddTo(player);
		player.PlayersGUI.RemoveComponent(_swipe);
		_swipe = null;
		_swipeArea = new GUIDoubleTapJoystickArea();
		_swipeArea.InputDepth = 0f;
		_swipeArea.RenderDepth = 10f;
		player.PlayersGUI.AddComponent(_swipeArea);
		PositionGUI();
	}

	public override void RemoveFrom(PlayerController player)
	{
		base.RemoveFrom(player);
		player.PlayersGUI.RemoveComponent(_swipeArea);
	}

	public override void UpdateControls(float delta)
	{
		base.Aiming = new Vector2(0f, 0f);
		base.UpdateControls(delta);
		base.Aiming += _swipeArea.JoystickAreaDirection;
		base.Fire = _swipeArea.DoubleTapped;
	}
}
