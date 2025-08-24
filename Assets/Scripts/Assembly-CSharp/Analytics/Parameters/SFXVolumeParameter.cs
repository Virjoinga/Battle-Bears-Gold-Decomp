namespace Analytics.Parameters
{
	public class SFXVolumeParameter : FloatParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.sfxVolume;
			}
		}

		public SFXVolumeParameter(float amount)
			: base(amount)
		{
		}
	}
}
