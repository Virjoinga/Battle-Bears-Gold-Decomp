namespace Analytics.Parameters
{
	public class ExtrasAdIconURLParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.extrasAdIconURL;
			}
		}

		public ExtrasAdIconURLParameter(string value)
			: base(value)
		{
		}
	}
}
