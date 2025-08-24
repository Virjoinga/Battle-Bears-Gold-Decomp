namespace SkyVuEngine.SkyVuNetwork.Client
{
	public class ServiceRequest
	{
		public string URI { get; set; }

		public bool IsGetMethod { get; set; }

		public ServiceRequest(int port, string api, string url, bool isGetMethod)
		{
			if (url.EndsWith("/"))
			{
				url = url.Remove(url.LastIndexOf("/"), 1);
			}
			url = url + ":" + port;
			url = ((!api.StartsWith("/")) ? (url + "/" + api) : (url + api));
			if (!url.EndsWith("/"))
			{
				url += "/";
			}
			URI = url;
			IsGetMethod = isGetMethod;
		}
	}
}
