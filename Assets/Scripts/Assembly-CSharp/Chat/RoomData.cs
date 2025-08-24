using System;

namespace Chat
{
	[Serializable]
	public class RoomData
	{
		public string RoomId;

		public CrumbData[] PlayerList;
	}
}
