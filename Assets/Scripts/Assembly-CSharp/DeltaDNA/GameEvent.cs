using System;
using System.Collections.Generic;

namespace DeltaDNA
{
	public class GameEvent<T> where T : GameEvent<T>
	{
		private readonly Params parameters;

		public string Name { get; private set; }

		public GameEvent(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be null or empty");
			}
			Name = name;
			parameters = new Params();
		}

		public T AddParam(string key, object value)
		{
			parameters.AddParam(key, value);
			return (T)this;
		}

		public Dictionary<string, object> AsDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("eventName", Name);
			dictionary.Add("eventParams", new Dictionary<string, object>(parameters.AsDictionary()));
			return dictionary;
		}
	}
	public class GameEvent : GameEvent<GameEvent>
	{
		public GameEvent(string name)
			: base(name)
		{
		}
	}
}
