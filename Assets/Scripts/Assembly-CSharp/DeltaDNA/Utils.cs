using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace DeltaDNA
{
	internal static class Utils
	{
		public static Dictionary<K, V> HashtableToDictionary<K, V>(Hashtable table)
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			foreach (DictionaryEntry item in table)
			{
				dictionary.Add((K)item.Key, (V)item.Value);
			}
			return dictionary;
		}

		public static Dictionary<K, V> HashtableToDictionary<K, V>(Dictionary<K, V> dictionary)
		{
			return dictionary;
		}

		public static byte[] ComputeMD5Hash(byte[] buffer)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			return mD5CryptoServiceProvider.ComputeHash(buffer);
		}

		public static bool IsDirectoryWritable(string path)
		{
			try
			{
				if (!DirectoryExists(path))
				{
					CreateDirectory(path);
				}
				string path2 = Path.Combine(path, Path.GetRandomFileName());
				using (File.Create(path2, 1))
				{
				}
				File.Delete(path2);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool FileExists(string path)
		{
			if (Singleton<DDNA>.Instance.Settings.UseEventStore)
			{
				return File.Exists(path);
			}
			return false;
		}

		public static bool DirectoryExists(string path)
		{
			if (Singleton<DDNA>.Instance.Settings.UseEventStore)
			{
				return Directory.Exists(path);
			}
			return false;
		}

		public static void CreateDirectory(string path)
		{
			if (Singleton<DDNA>.Instance.Settings.UseEventStore)
			{
				Directory.CreateDirectory(path);
			}
		}

		public static Stream CreateStream(string path)
		{
			if (Singleton<DDNA>.Instance.Settings.UseEventStore)
			{
				Logger.LogDebug("Creating file based stream");
				return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			}
			Logger.LogDebug("Creating memory based stream");
			return new MemoryStream();
		}

		public static Stream OpenStream(string path)
		{
			if (Singleton<DDNA>.Instance.Settings.UseEventStore)
			{
				Logger.LogDebug("Opening file based stream");
				return new FileStream(path, FileMode.Open, FileAccess.Read);
			}
			Logger.LogDebug("Opening memory based stream");
			return new MemoryStream();
		}
	}
}
