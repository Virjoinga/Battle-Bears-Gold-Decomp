public interface InputGUIComponent
{
	void WipeInput();

	bool ClaimsInput(SVTouch touch);

	bool ConsumeInput(SVTouch touch);

	void FinalizeInput();
}
