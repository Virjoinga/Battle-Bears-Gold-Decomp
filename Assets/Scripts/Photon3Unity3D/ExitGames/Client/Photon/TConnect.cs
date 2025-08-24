using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	internal class TConnect
	{
		internal const int TCP_HEADER_BYTES = 7;

		private const int MSG_HEADER_BYTES = 2;

		private const int ALL_HEADER_BYTES = 9;

		private EndPoint serverEndPoint;

		internal bool obsolete;

		internal bool isRunning;

		internal TPeer peer;

		private Socket socketConnection;

		internal TConnect(TPeer npeer, string ipPort)
		{
			if ((int)npeer.debugOut >= 5)
			{
				npeer.Listener.DebugReturn(DebugLevel.ALL, "new TConnect()");
			}
			peer = npeer;
		}

		internal bool StartConnection()
		{
			if (isRunning)
			{
				if ((int)peer.debugOut >= 1)
				{
					peer.Listener.DebugReturn(DebugLevel.ERROR, "startConnectionThread() failed: connection thread still running.");
				}
				return false;
			}
			socketConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socketConnection.NoDelay = true;
			new Thread(Run).Start();
			return true;
		}

		internal void StopConnection()
		{
			if ((int)peer.debugOut >= 5)
			{
				peer.Listener.DebugReturn(DebugLevel.ALL, "StopConnection()");
			}
			obsolete = true;
			if (socketConnection != null)
			{
				socketConnection.Close();
			}
		}

		public void sendTcp(byte[] opData)
		{
			if (obsolete)
			{
				if ((int)peer.debugOut >= 3)
				{
					peer.Listener.DebugReturn(DebugLevel.INFO, "Sending was skipped because connection is obsolete. " + Environment.StackTrace);
				}
				return;
			}
			try
			{
				socketConnection.Send(opData);
			}
			catch (NullReferenceException ex)
			{
				if ((int)peer.debugOut >= 1)
				{
					peer.Listener.DebugReturn(DebugLevel.ERROR, ex.Message);
				}
			}
			catch (SocketException ex2)
			{
				if ((int)peer.debugOut >= 1)
				{
					peer.Listener.DebugReturn(DebugLevel.ERROR, ex2.Message);
				}
			}
		}

		public void Run()
		{
			try
			{
				serverEndPoint = PeerBase.GetEndpoint(peer.ServerAddress);
				if (serverEndPoint == null)
				{
					if ((int)peer.debugOut >= 1)
					{
						peer.Listener.DebugReturn(DebugLevel.ERROR, "StartConnection() failed. Address must be 'address:port'. Is: " + peer.ServerAddress);
					}
					return;
				}
				socketConnection.Connect(serverEndPoint);
			}
			catch (SecurityException ex)
			{
				if ((int)peer.debugOut >= 3)
				{
					peer.Listener.DebugReturn(DebugLevel.INFO, "Connect() failed: " + ex.ToString());
				}
				if (socketConnection != null)
				{
					socketConnection.Close();
				}
				isRunning = false;
				obsolete = true;
				peer.EnqueueStatusCallback(StatusCode.ExceptionOnConnect);
				peer.EnqueueActionForDispatch(delegate
				{
					peer.Disconnected();
				});
				return;
			}
			catch (SocketException ex2)
			{
				if ((int)peer.debugOut >= 3)
				{
					peer.Listener.DebugReturn(DebugLevel.INFO, "Connect() failed: " + ex2.ToString());
				}
				if (socketConnection != null)
				{
					socketConnection.Close();
				}
				isRunning = false;
				obsolete = true;
				peer.EnqueueStatusCallback(StatusCode.ExceptionOnConnect);
				peer.EnqueueActionForDispatch(delegate
				{
					peer.Disconnected();
				});
				return;
			}
			obsolete = false;
			isRunning = true;
			if (peer.TcpConnectionPrefix != null)
			{
				sendTcp(peer.TcpConnectionPrefix);
			}
			while (!obsolete)
			{
				MemoryStream opCollectionStream = new MemoryStream(256);
				try
				{
					int num = 0;
					byte[] inBuff = new byte[9];
					while (num < 9)
					{
						num += socketConnection.Receive(inBuff, num, 9 - num, SocketFlags.None);
						if (num == 0)
						{
							peer.SendPing();
							Thread.Sleep(100);
						}
					}
					if (inBuff[0] == 240)
					{
						if (peer.TrafficStatsEnabled)
						{
							peer.TrafficStatsIncoming.CountControlCommand(inBuff.Length);
						}
						if (peer.NetworkSimulationSettings.IsSimulationEnabled)
						{
							peer.ReceiveNetworkSimulated(delegate
							{
								peer.ReceiveIncomingCommands(inBuff, inBuff.Length);
							});
						}
						else
						{
							peer.ReceiveIncomingCommands(inBuff, inBuff.Length);
						}
						continue;
					}
					int num2 = (inBuff[1] << 24) | (inBuff[2] << 16) | (inBuff[3] << 8) | inBuff[4];
					if (peer.TrafficStatsEnabled)
					{
						if (inBuff[5] == 0)
						{
							peer.TrafficStatsIncoming.CountReliableOpCommand(num2);
						}
						else
						{
							peer.TrafficStatsIncoming.CountUnreliableOpCommand(num2);
						}
					}
					if ((int)peer.debugOut >= 5)
					{
						peer.EnqueueDebugReturn(DebugLevel.ALL, "message length: " + num2);
					}
					opCollectionStream.Write(inBuff, 7, num - 7);
					num = 0;
					num2 -= 9;
					for (inBuff = new byte[num2]; num < num2; num += socketConnection.Receive(inBuff, num, num2 - num, SocketFlags.None))
					{
					}
					opCollectionStream.Write(inBuff, 0, num);
					if (opCollectionStream.Length > 0)
					{
						if (peer.NetworkSimulationSettings.IsSimulationEnabled)
						{
							peer.ReceiveNetworkSimulated(delegate
							{
								peer.ReceiveIncomingCommands(opCollectionStream.ToArray(), (int)opCollectionStream.Length);
							});
						}
						else
						{
							peer.ReceiveIncomingCommands(opCollectionStream.ToArray(), (int)opCollectionStream.Length);
						}
					}
					if ((int)peer.debugOut >= 5)
					{
						peer.EnqueueDebugReturn(DebugLevel.ALL, "TCP < " + opCollectionStream.Length);
					}
				}
				catch (SocketException ex3)
				{
					if (!obsolete)
					{
						obsolete = true;
						if ((int)peer.debugOut >= 1)
						{
							peer.EnqueueDebugReturn(DebugLevel.ERROR, "Receiving failed. SocketException: " + ex3.SocketErrorCode);
						}
						switch (ex3.SocketErrorCode)
						{
						case SocketError.ConnectionAborted:
						case SocketError.ConnectionReset:
							peer.EnqueueStatusCallback(StatusCode.DisconnectByServer);
							break;
						default:
							peer.EnqueueStatusCallback(StatusCode.Exception);
							break;
						}
					}
				}
				catch (Exception ex4)
				{
					if (!obsolete && (int)peer.debugOut >= 1)
					{
						peer.EnqueueDebugReturn(DebugLevel.ERROR, "Receiving failed. Exception: " + ex4.ToString());
					}
				}
			}
			if (socketConnection != null)
			{
				socketConnection.Close();
			}
			isRunning = false;
			obsolete = true;
			peer.EnqueueActionForDispatch(delegate
			{
				peer.Disconnected();
			});
		}
	}
}
