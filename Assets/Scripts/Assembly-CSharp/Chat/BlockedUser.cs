using System;

namespace Chat
{
	[Serializable]
	public class BlockedUser
	{
		public string Id;

		public string Name;

		public BlockedUser(string id, string name)
		{
			Id = id;
			Name = name;
		}

		public BlockedUser()
		{
		}
	}
}
