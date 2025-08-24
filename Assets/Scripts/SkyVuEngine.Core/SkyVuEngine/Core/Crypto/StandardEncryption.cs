using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SkyVuEngine.Core.Crypto
{
	public class StandardEncryption
	{
		private byte[] _key;

		private byte[] _vector;

		private RijndaelManaged _rijndaelManaged;

		private ICryptoTransform _encryptor;

		private ICryptoTransform _decryptor;

		private UTF8Encoding _encoder;

		public StandardEncryption()
		{
			_key = AnnonymousKey.Key;
			_vector = AnnonymousKey.Vector;
			_rijndaelManaged = new RijndaelManaged();
			_rijndaelManaged.Key = _key;
			_rijndaelManaged.IV = _vector;
			_encryptor = _rijndaelManaged.CreateEncryptor(_key, _vector);
			_decryptor = _rijndaelManaged.CreateDecryptor(_key, _vector);
			_encoder = new UTF8Encoding();
		}

		public StandardEncryption(string key, string vector)
		{
			_key = _encoder.GetBytes(key);
			_vector = _encoder.GetBytes(vector);
			_rijndaelManaged = new RijndaelManaged();
			_rijndaelManaged.Key = _key;
			_rijndaelManaged.IV = _vector;
			_encryptor = _rijndaelManaged.CreateEncryptor(_key, _vector);
			_decryptor = _rijndaelManaged.CreateDecryptor(_key, _vector);
			_encoder = new UTF8Encoding();
		}

		public string Encrypt(string unencrypted)
		{
			if (unencrypted == null)
			{
				return null;
			}
			return Convert.ToBase64String(Encrypt(_encoder.GetBytes(unencrypted)));
		}

		public string Decrypt(string encrypted)
		{
			return _encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
		}

		public byte[] Encrypt(byte[] buffer)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, _encryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(buffer, 0, buffer.Length);
				}
				result = memoryStream.ToArray();
			}
			_encryptor = _rijndaelManaged.CreateEncryptor(_key, _vector);
			return result;
		}

		public byte[] Decrypt(byte[] buffer)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, _decryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(buffer, 0, buffer.Length);
				}
				result = memoryStream.ToArray();
			}
			_decryptor = _rijndaelManaged.CreateDecryptor(_key, _vector);
			return result;
		}
	}
}
