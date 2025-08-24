public class Server
{
	public string server_type = string.Empty;

	public string url = string.Empty;

	public string description = string.Empty;

	public Server()
	{
	}

	public Server(string url, string type, string desc)
	{
		this.url = url;
		server_type = type;
		description = desc;
	}

	public static Server Test()
	{
		return new Server("10.0.1.249:3008", "match", "Test server 1");
	}
}
