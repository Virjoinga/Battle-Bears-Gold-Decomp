using System.Security.Cryptography;
using System.Text;

namespace Analytics
{
	public abstract class EventHelper
	{
		protected static string SessionId
		{
			get
			{
				return ServiceManager.Instance.SessionId;
			}
		}

		public static string Hash(string toHash)
		{
			SHA256Managed sHA256Managed = new SHA256Managed();
			byte[] bytes = Encoding.UTF8.GetBytes(toHash);
			byte[] array = sHA256Managed.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		protected static Stats GetStats()
		{
			return ServiceManager.Instance.GetStats();
		}
	}
}
