using System;
using System.Collections.Generic;
using DeltaDNA.MiniJSON;

namespace DeltaDNA
{
	public class Engagement<T> where T : Engagement<T>
	{
		private readonly Params parameters;

		private string response;

		public string DecisionPoint { get; private set; }

		public string Flavour { get; private set; }

		public string Raw
		{
			get
			{
				return response;
			}
			set
			{
				Dictionary<string, object> dictionary = null;
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						dictionary = Json.Deserialize(value) as Dictionary<string, object>;
					}
					catch (Exception)
					{
					}
				}
				response = value;
				JSON = dictionary ?? new Dictionary<string, object>();
			}
		}

		public int StatusCode { get; set; }

		public string Error { get; set; }

		public Dictionary<string, object> JSON { get; private set; }

		public Engagement(string decisionPoint)
		{
			if (string.IsNullOrEmpty(decisionPoint))
			{
				throw new ArgumentException("decisionPoint cannot be null or empty");
			}
			DecisionPoint = decisionPoint;
			Flavour = "engagement";
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
			dictionary.Add("decisionPoint", DecisionPoint);
			dictionary.Add("flavour", Flavour);
			dictionary.Add("parameters", new Dictionary<string, object>(parameters.AsDictionary()));
			return dictionary;
		}

		public override string ToString()
		{
			return string.Format("[Engagement: DecisionPoint={0}, Flavour={1}, Raw={2}, StatusCode={3}, Error={4}, JSON={5}]", DecisionPoint, Flavour, Raw, StatusCode, Error, JSON);
		}
	}
	public class Engagement : Engagement<Engagement>
	{
		public Engagement(string decisionPoint)
			: base(decisionPoint)
		{
		}
	}
}
