using System;
using ExitGames.Client.Photon;

public class OnUserCreatedEventArgs : EventArgs
{
	public int userID;

	public Hashtable userParameters;

	public bool isLocalUser;

	public OnUserCreatedEventArgs(int userID, Hashtable userParameters, bool isLocalUser)
	{
		this.userID = userID;
		this.userParameters = userParameters;
		this.isLocalUser = isLocalUser;
	}
}
