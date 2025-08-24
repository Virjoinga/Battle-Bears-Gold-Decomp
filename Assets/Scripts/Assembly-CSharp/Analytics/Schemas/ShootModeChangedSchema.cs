using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ShootModeChangedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.shootModeChanged;
			}
		}

		public ShootModeChangedSchema(ShootModeParameter shootMode)
			: base(shootMode)
		{
		}
	}
}
