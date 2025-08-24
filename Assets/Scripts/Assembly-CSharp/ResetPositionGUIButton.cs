public class ResetPositionGUIButton : GUIRectangleButton
{
	public override void FinalizeInput()
	{
		base.FinalizeInput();
		if (base.IsHeld)
		{
			GUIPositionController.Instance.ResetCustomPositions();
			CustomLayoutController.Instance.ResetMoveableAreas();
		}
	}
}
