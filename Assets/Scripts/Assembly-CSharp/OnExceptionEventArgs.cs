using System;

public class OnExceptionEventArgs : EventArgs
{
	public string error;

	public OnExceptionEventArgs(string error)
	{
		this.error = error;
	}
}
