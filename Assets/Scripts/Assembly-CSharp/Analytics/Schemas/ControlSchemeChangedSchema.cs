using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ControlSchemeChangedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.controlSchemeChanged;
			}
		}

		public ControlSchemeChangedSchema(ControlSchemeParameter controlScheme)
			: base(controlScheme)
		{
		}
	}
}
