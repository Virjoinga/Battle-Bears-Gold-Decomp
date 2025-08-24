using ExitGames.Client.Photon;

public interface IPhotonClient
{
	void IngameUpdate();

	void NetworkTick(int serverTime);

	void HandleCustomEvent(byte evCode, ref Hashtable data, int senderID);

	void HandleCustomPeerReturn(byte opCode, int returnCode, ref Hashtable returnValues, short invocID);

	void HandleGetProperties(Hashtable properties, short invocID);
}
