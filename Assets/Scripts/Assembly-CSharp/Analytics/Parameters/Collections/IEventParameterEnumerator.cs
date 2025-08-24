using System;
using System.Collections;
using System.Collections.Generic;

namespace Analytics.Parameters.Collections
{
	public class IEventParameterEnumerator : IDisposable, IEnumerator, IEnumerator<IEventParameter>
	{
		private int _index;

		private IEventParameter[] _params;

		object IEnumerator.Current
		{
			get
			{
				return _params[_index];
			}
		}

		public IEventParameter Current
		{
			get
			{
				return _params[_index];
			}
		}

		public IEventParameterEnumerator(IEventParameter[] parameters)
		{
			_params = parameters;
			_index = -1;
		}

		public void Dispose()
		{
			_params = new IEventParameter[0];
		}

		public bool MoveNext()
		{
			_index++;
			return _index < _params.Length;
		}

		public void Reset()
		{
			_index = -1;
		}
	}
}
