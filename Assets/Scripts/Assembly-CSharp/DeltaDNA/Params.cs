using System;
using System.Collections.Generic;
using System.Globalization;

namespace DeltaDNA
{
	public class Params
	{
		private Dictionary<string, object> _params = new Dictionary<string, object>();

		public Params AddParam(string key, object value)
		{
			try
			{
				if (value is Params)
				{
					_params[key] = ((Params)value).AsDictionary();
				}
				else if (value is DateTime)
				{
					_params[key] = ((DateTime)value).ToString(Settings.EVENT_TIMESTAMP_FORMAT, CultureInfo.InvariantCulture);
				}
				else
				{
					_params[key] = value;
				}
			}
			catch (ArgumentNullException innerException)
			{
				throw new ArgumentNullException("Key can not be null.", innerException);
			}
			return this;
		}

		public object GetParam(string key)
		{
			try
			{
				return (!_params.ContainsKey(key)) ? null : _params[key];
			}
			catch (ArgumentNullException innerException)
			{
				throw new Exception("Key can not be null.", innerException);
			}
			catch (KeyNotFoundException innerException2)
			{
				throw new Exception("Key " + key + " not found.", innerException2);
			}
		}

		public Dictionary<string, object> AsDictionary()
		{
			return _params;
		}
	}
}
