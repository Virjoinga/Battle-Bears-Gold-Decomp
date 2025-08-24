namespace SkyVuEngine.Core.Console
{
	public class Command
	{
		public delegate string Execute(string[] options);

		public string Name { get; set; }

		public string Info { get; set; }

		public Execute ExecuteCommand { get; set; }
	}
}
