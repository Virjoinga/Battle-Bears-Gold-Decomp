namespace Analytics.Parameters
{
	public class MusicVolumeParameter : FloatParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.musicVolume;
			}
		}

		public MusicVolumeParameter(float amount)
			: base(amount)
		{
		}
	}
}
