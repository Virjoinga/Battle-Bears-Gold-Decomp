namespace Photon.NetSyncObject.Shared
{
	public enum EventCode : byte
	{
		NoCodeSet = 0,
		Join = 90,
		Leave = 91,
		PropertiesChanged = 92,
		RegisterNetSyncObject = 93,
		UnregisterNetSyncObject = 94,
		UpdateNetSyncObject = 95,
		RegisterUserData = 96,
		UpdateNetSyncState = 97,
		UpdateNetSyncAction = 104,
		SyncFireProjectile = 105
	}
}
