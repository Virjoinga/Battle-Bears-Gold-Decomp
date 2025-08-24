using System;

namespace Photon.NetSyncObject.Shared
{
	[Flags]
	public enum PropertyType : byte
	{
		None = 0,
		Game = 1,
		Actor = 2,
		GameAndActor = 3
	}
}
