namespace Photon.NetSyncObject.Shared
{
	public enum OperationCode : byte
	{
		Join = 90,
		Leave = 91,
		RaiseEvent = 92,
		SetProperties = 93,
		GetProperties = 94,
		Ping = 104,
		RegisterNetSyncObject = 106,
		UnregisterNetSyncObject = 107,
		UpdateNetSyncObject = 108,
		RequestNetSyncObjects = 109,
		RequestUserData = 110,
		RegisterUserData = 111,
		UpdateNetSyncState = 112,
		UpdateNetSyncAction = 122,
		RegisterNetSyncServerObject = 123,
		SyncFireProjectile = 124
	}
}
