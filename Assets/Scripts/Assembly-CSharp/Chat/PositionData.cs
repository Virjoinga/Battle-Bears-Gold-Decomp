using System;

namespace Chat
{
	[Serializable]
	public class PositionData
	{
		public int x;

		public int y;

		public int z;

		public PositionData(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
}
