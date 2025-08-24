namespace Analytics.Parameters
{
	public class VirtualCurrencyNameParameter : StringParameter
	{
		public enum CurrencyName
		{
			GAS = 0,
			JOULES = 1
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.virtualCurrencyName;
			}
		}

		public VirtualCurrencyNameParameter(CurrencyName name)
			: base(name.ToString())
		{
		}
	}
}
