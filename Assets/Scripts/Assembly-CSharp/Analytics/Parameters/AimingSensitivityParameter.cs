namespace Analytics.Parameters
{
	public class AimingSensitivityParameter : FloatParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.aimingSensitivity;
			}
		}

		public AimingSensitivityParameter(float amount)
			: base(amount)
		{
		}
	}
}
