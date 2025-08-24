public class AcceptChangesGUIButton : GUIRectangleButton
{
	public override void FinalizeInput()
	{
		base.FinalizeInput();
		if (base.IsHeld)
		{
			CustomLayoutController.Instance.CloseOnNextFrame();
		}
	}
}
