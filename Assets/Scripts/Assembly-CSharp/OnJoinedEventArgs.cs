using System;

public class OnJoinedEventArgs : EventArgs
{
	public int userID;

	public OnJoinedEventArgs(int userID)
	{
		this.userID = userID;
	}
}
