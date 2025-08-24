using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class OfferwallOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.offerwallOpened;
			}
		}

		public OfferwallOpenedSchema(CurrentGasParameter currentGas)
			: base(currentGas)
		{
		}
	}
}
