using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class SensitivityChangedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.sensitivityChanged;
			}
		}

		public SensitivityChangedSchema(AimingSensitivityParameter aimingSensitivity)
			: base(aimingSensitivity)
		{
		}
	}
}
