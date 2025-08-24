using System.Collections.Generic;

namespace Analytics
{
	public abstract class EventSchema
	{
		protected List<IEventParameter> _parameters = new List<IEventParameter>();

		public abstract AnalyticsEvent Name { get; }

		public EventSchema()
		{
		}

		public EventSchema(params IEventParameter[] eventParameters)
		{
			foreach (IEventParameter item in eventParameters)
			{
				_parameters.Add(item);
			}
		}

		public virtual IDictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (IEventParameter parameter in _parameters)
			{
				dictionary.Add(parameter.Name, parameter.Value);
			}
			return dictionary;
		}
	}
}
