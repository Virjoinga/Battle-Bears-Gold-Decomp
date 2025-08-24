using System;

public class OnUserDestroyedEventArgs : EventArgs
{
	public int userID;

	public bool isLocalUser;

	public OnUserDestroyedEventArgs(int userID, bool isLocalUser)
	{
		this.userID = userID;
		this.isLocalUser = isLocalUser;
	}
}
