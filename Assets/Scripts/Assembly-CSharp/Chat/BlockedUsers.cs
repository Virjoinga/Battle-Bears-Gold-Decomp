using System;
using System.Collections.Generic;

namespace Chat
{
	[Serializable]
	public class BlockedUsers
	{
		public List<BlockedUser> Users = new List<BlockedUser>();
	}
}
