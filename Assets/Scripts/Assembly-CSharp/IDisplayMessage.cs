using System;

public interface IDisplayMessage
{
	void DisplayInfoMessage(string message, string acceptButtonText = "OK", string declineButtonText = "No!", Action accept = null, Action decline = null);

	void DisplayErrorMessage(string message, string confirmButtonText = "OK", Action accept = null);
}
