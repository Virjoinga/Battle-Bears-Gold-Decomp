using System.Collections.Generic;

namespace DeltaDNA
{
	internal class HttpRequest
	{
		internal enum HTTPMethodType
		{
			GET = 0,
			POST = 1
		}

		private Dictionary<string, string> headers = new Dictionary<string, string>();

		internal string URL { get; private set; }

		internal HTTPMethodType HTTPMethod { get; set; }

		internal string HTTPBody { get; set; }

		internal int TimeoutSeconds { get; set; }

		internal HttpRequest(string url)
		{
			URL = url;
			TimeoutSeconds = Singleton<DDNA>.Instance.Settings.HttpRequestCollectTimeoutSeconds;
		}

		internal Dictionary<string, string> getHeaders()
		{
			return headers;
		}

		internal void setHeader(string field, string value)
		{
			headers[field] = value;
		}

		public override string ToString()
		{
			return string.Concat("HttpRequest: ", URL, "\n", HTTPMethod, "\n", HTTPBody, "\n");
		}
	}
}
