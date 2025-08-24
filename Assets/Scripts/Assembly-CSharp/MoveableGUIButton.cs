using UnityEngine;

public class MoveableGUIButton : GUIRectangleButton
{
	public override bool ClaimsInput(SVTouch touch)
	{
		if (base.ClaimsInput(touch))
		{
			base.ButtonX = touch.position.x;
			base.ButtonY = (float)Screen.height - touch.position.y;
			return true;
		}
		return false;
	}
}
