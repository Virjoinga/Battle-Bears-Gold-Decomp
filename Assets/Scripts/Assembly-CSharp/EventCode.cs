using System;

public class EventCode
{
	public const byte GameList = 230;

	public const byte GameListUpdate = 229;

	public const byte QueueState = 228;

	public const byte Match = 227;

	public const byte AppStats = 226;

	public const byte AzureNodeInfo = 210;

	public const byte Join = byte.MaxValue;

	public const byte Leave = 254;

	public const byte PropertiesChanged = 253;

	[Obsolete("Use PropertiesChanged now.")]
	public const byte SetProperties = 253;

	public const byte NoCodeSet = 0;

	public const byte RegisterNetSyncObject = 93;

	public const byte RegisterNetSyncServerObject = 123;

	public const byte RequestNetSyncObjects = 109;

	public const byte UnregisterNetSyncObject = 94;

	public const byte UpdateNetSyncObject = 92;

	public const byte RequestUserData = 95;

	public const byte RegisterUserData = 96;

	public const byte UpdateNetSyncState = 97;

	public const byte UpdateNetSyncAction = 104;

	public const byte SyncFireProjectile = 105;

	public const byte StartPrivateMatch = 106;
}
