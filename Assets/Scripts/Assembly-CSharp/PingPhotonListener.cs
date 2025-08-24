using System;
using ExitGames.Client.Photon;

public class PingPhotonListener : IPhotonPeerListener
{
	private DateTime _startTime;

	public DateTime StartTime
	{
		get
		{
			return _startTime;
		}
		set
		{
			_startTime = value;
		}
	}

	public PingPhotonListener(string server)
	{
		_startTime = DateTime.Now;
	}

	public void DebugReturn(DebugLevel level, string message)
	{
	}

	public void EventAction(byte eventCode, Hashtable photonEvent)
	{
	}

	public void OperationResult(byte opCode, int returnCode, Hashtable returnValues, short invocID)
	{
	}

	public void PeerStatusCallback(StatusCode statusCode)
	{
	}

	public void OnEvent(EventData eventData)
	{
		throw new NotImplementedException();
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
		throw new NotImplementedException();
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
		throw new NotImplementedException();
	}
}
