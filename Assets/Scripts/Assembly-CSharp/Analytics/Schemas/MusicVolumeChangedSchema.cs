using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class MusicVolumeChangedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.musicVolumeChanged;
			}
		}

		public MusicVolumeChangedSchema(MusicVolumeParameter musicVolume)
			: base(musicVolume)
		{
		}
	}
}
