namespace Analytics.Schemas
{
	public class SetNicknameOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.setNicknameOpened;
			}
		}
	}
}
