namespace Analytics.Parameters
{
	public class DeviceTotalTimePlayedParameter : FloatParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.deviceTotalTimePlayed;
			}
		}

		public DeviceTotalTimePlayedParameter(float amount)
			: base(amount)
		{
		}
	}
}
