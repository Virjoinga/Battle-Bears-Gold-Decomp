namespace Analytics.Parameters
{
	public class ButtonSizeParameter : FloatParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.buttonSize;
			}
		}

		public ButtonSizeParameter(float amount)
			: base(amount)
		{
		}
	}
}
