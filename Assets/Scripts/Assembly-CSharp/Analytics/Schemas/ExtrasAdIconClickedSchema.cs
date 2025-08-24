using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ExtrasAdIconClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.extrasIconAdClicked;
			}
		}

		public ExtrasAdIconClickedSchema(ExtrasAdIconURLParameter extrasAdIconURL)
			: base(extrasAdIconURL)
		{
		}
	}
}
