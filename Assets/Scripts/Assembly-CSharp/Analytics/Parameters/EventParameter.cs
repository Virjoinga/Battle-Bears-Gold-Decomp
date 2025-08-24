namespace Analytics.Parameters
{
	public abstract class EventParameter : IEventParameter
	{
		protected abstract AnalyticsParameter _parameter { get; }

		public string Name
		{
			get
			{
				return _parameter.ToString();
			}
		}

		public object Value { get; protected set; }
	}
}
