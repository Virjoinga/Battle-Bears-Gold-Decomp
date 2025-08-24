using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class SFXVolumeChangedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.sfxVolumeChanged;
			}
		}

		public SFXVolumeChangedSchema(SFXVolumeParameter sfxVolume)
			: base(sfxVolume)
		{
		}
	}
}
