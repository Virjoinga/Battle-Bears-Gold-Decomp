using System;

public class OnLeaveEventArgs : EventArgs
{
	public int userID;

	public OnLeaveEventArgs(int userID)
	{
		this.userID = userID;
	}
}
