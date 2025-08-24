using System.Collections;
using System.Collections.Generic;

namespace Analytics.Parameters.Collections
{
	public abstract class IEventParameterEnumerable : IEnumerable, IEnumerable<IEventParameter>
	{
		protected IEventParameter[] _eventParameters;

		public IEventParameterEnumerable(params IEventParameter[] eventParameters)
		{
			_eventParameters = eventParameters;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<IEventParameter> GetEnumerator()
		{
			return new IEventParameterEnumerator(_eventParameters);
		}
	}
}
