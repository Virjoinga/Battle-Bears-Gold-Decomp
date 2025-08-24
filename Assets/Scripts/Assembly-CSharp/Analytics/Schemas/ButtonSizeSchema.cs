using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ButtonSizeSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.buttonSizeChanged;
			}
		}

		public ButtonSizeSchema(ButtonSizeParameter buttonSize)
			: base(buttonSize)
		{
		}
	}
}
