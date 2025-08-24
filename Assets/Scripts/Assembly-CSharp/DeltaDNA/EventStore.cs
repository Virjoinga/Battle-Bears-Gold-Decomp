using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace DeltaDNA
{
	public class EventStore : IDisposable
	{
		private static readonly string PF_KEY_IN_FILE = "DDSDK_EVENT_IN_FILE";

		private static readonly string PF_KEY_OUT_FILE = "DDSDK_EVENT_OUT_FILE";

		private static readonly string FILE_A = "A";

		private static readonly string FILE_B = "B";

		private static readonly long MAX_FILE_SIZE_BYTES = 1048576L;

		private bool _initialised;

		private bool _disposed;

		private Stream _infs;

		private Stream _outfs;

		private static object _lock = new object();

		public bool IsInitialised
		{
			get
			{
				return _initialised;
			}
		}

		public EventStore(string dir)
		{
			Logger.LogInfo("Creating Event Store");
			if (InitialiseFileStreams(dir))
			{
				_initialised = true;
			}
			else
			{
				Logger.LogError("Failed to initialise event store in " + dir);
			}
		}

		public bool Push(string obj)
		{
			lock (_lock)
			{
				if (!_initialised)
				{
					Logger.LogError("Event Store not initialised");
					return false;
				}
				return PushEvent(obj, _infs);
			}
		}

		public bool Swap()
		{
			lock (_lock)
			{
				if (_initialised && _outfs.Length == 0L)
				{
					SwapStreams(ref _infs, ref _outfs);
					string @string = PlayerPrefs.GetString(PF_KEY_IN_FILE);
					string string2 = PlayerPrefs.GetString(PF_KEY_OUT_FILE);
					if (string.IsNullOrEmpty(@string) || string.IsNullOrEmpty(string2))
					{
						Logger.LogWarning("File path from PlayerPrefs is missing, did you DeleteAll?");
					}
					else
					{
						PlayerPrefs.SetString(PF_KEY_IN_FILE, string2);
						PlayerPrefs.SetString(PF_KEY_OUT_FILE, @string);
					}
					return true;
				}
				return false;
			}
		}

		public List<string> Read()
		{
			lock (_lock)
			{
				List<string> list = new List<string>();
				try
				{
					if (_initialised)
					{
						ReadEvents(_outfs, list);
					}
				}
				catch (Exception ex)
				{
					Logger.LogError("Problem reading events: " + ex.Message);
					ClearStream(_outfs);
					return null;
				}
				return list;
			}
		}

		public void ClearOut()
		{
			lock (_lock)
			{
				if (_initialised)
				{
					ClearStream(_outfs);
				}
			}
		}

		public void ClearAll()
		{
			lock (_lock)
			{
				if (_initialised)
				{
					ClearStream(_infs);
					ClearStream(_outfs);
				}
			}
		}

		public void FlushBuffers()
		{
			lock (_lock)
			{
				if (_initialised)
				{
					_infs.Flush();
					_outfs.Flush();
				}
			}
		}

		~EventStore()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			lock (_lock)
			{
				try
				{
					if (!_disposed && disposing)
					{
						if (_infs != null)
						{
							_infs.Dispose();
						}
						if (_outfs != null)
						{
							_outfs.Dispose();
						}
					}
				}
				catch (Exception ex)
				{
					Logger.LogError("Failed to dispose EventStore cleanly. " + ex.Message);
				}
				finally
				{
					_disposed = true;
				}
			}
		}

		private bool InitialiseFileStreams(string dir)
		{
			try
			{
				string path = null;
				string path2 = null;
				string @string = PlayerPrefs.GetString(PF_KEY_IN_FILE, FILE_A);
				string string2 = PlayerPrefs.GetString(PF_KEY_OUT_FILE, FILE_B);
				if (!string.IsNullOrEmpty(dir))
				{
					if (!Utils.DirectoryExists(dir))
					{
						Logger.LogDebug("Directory not found, creating " + dir);
						Utils.CreateDirectory(dir);
					}
					path = Path.Combine(dir, @string);
					path2 = Path.Combine(dir, string2);
				}
				_infs = Utils.CreateStream(path);
				_infs.Seek(0L, SeekOrigin.End);
				_outfs = Utils.CreateStream(path2);
				_outfs.Seek(0L, SeekOrigin.Begin);
				PlayerPrefs.SetString(PF_KEY_IN_FILE, @string);
				PlayerPrefs.SetString(PF_KEY_OUT_FILE, string2);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError("Failed to initialise file stream: " + ex.Message);
			}
			return false;
		}

		public static bool PushEvent(string obj, Stream stream)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(obj);
			byte[] bytes2 = BitConverter.GetBytes(bytes.Length);
			if (stream.Length + bytes.Length < MAX_FILE_SIZE_BYTES)
			{
				List<byte> list = new List<byte>();
				list.AddRange(bytes2);
				list.AddRange(bytes);
				byte[] array = list.ToArray();
				stream.Write(array, 0, array.Length);
				return true;
			}
			return false;
		}

		public static void ReadEvents(Stream stream, IList<string> events)
		{
			byte[] array = new byte[4];
			while (stream.Read(array, 0, array.Length) > 0)
			{
				int num = BitConverter.ToInt32(array, 0);
				if (num <= 0 || num > MAX_FILE_SIZE_BYTES - 4)
				{
					Logger.LogError("Event Store file corruption while reading event length.");
					ClearStream(stream);
					break;
				}
				byte[] array2 = new byte[num];
				int num2 = stream.Read(array2, 0, array2.Length);
				if (num2 != array2.Length)
				{
					Logger.LogError("Event Store file corruption while reading event.");
					ClearStream(stream);
					break;
				}
				string @string = Encoding.UTF8.GetString(array2, 0, array2.Length);
				events.Add(@string);
			}
			stream.Seek(0L, SeekOrigin.Begin);
		}

		public static void SwapStreams(ref Stream sin, ref Stream sout)
		{
			sin.Flush();
			Stream stream = sin;
			sin = sout;
			sout = stream;
			sin.Seek(0L, SeekOrigin.Begin);
			sin.SetLength(0L);
			sout.Seek(0L, SeekOrigin.Begin);
		}

		public static void ClearStream(Stream stream)
		{
			if (stream != null && stream.CanSeek)
			{
				stream.Seek(0L, SeekOrigin.Begin);
				stream.SetLength(0L);
			}
		}
	}
}
