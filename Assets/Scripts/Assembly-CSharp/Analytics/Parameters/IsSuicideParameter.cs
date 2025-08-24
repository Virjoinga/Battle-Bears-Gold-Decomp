namespace Analytics.Parameters
{
	public class IsSuicideParameter : BooleanParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.isSuicide;
			}
		}

		public IsSuicideParameter(bool value)
			: base(value)
		{
		}
	}
}
