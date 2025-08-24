namespace Analytics.Schemas
{
	public class SetNicknameClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.setNicknameClosed;
			}
		}
	}
}
