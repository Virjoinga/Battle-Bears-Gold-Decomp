namespace Analytics.Parameters
{
	public class CurrentGasParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.currentGas;
			}
		}

		public CurrentGasParameter(int amount)
			: base(amount)
		{
		}
	}
}
