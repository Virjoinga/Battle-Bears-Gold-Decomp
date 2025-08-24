namespace Analytics.Schemas
{
	public class TutorialStartedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.tutorialStarted;
			}
		}
	}
}
