using System;

namespace Chat
{
	[Serializable]
	public class PlayerData
	{
		public string PlayerId;

		public string Nickname;

		public string CurrentRoom;

		public bool IsAdmin;

		public PlayerData()
		{
		}

		public PlayerData(CrumbData crumb)
		{
			PlayerId = crumb.i;
			Nickname = crumb.n;
			IsAdmin = crumb.a == 1;
		}
	}
}
