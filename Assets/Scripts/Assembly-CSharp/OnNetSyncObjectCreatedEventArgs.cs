using System;

public class OnNetSyncObjectCreatedEventArgs : EventArgs
{
	public int userID;

	public long netID;

	public OnNetSyncObjectCreatedEventArgs(int userID, long netID)
	{
		this.userID = userID;
		this.netID = netID;
	}
}
