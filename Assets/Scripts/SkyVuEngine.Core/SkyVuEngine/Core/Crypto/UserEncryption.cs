using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SkyVuEngine.Core.Crypto
{
	public class UserEncryption
	{
		public int GenerateSaltForPassword()
		{
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			byte[] array = new byte[4];
			rNGCryptoServiceProvider.GetNonZeroBytes(array);
			return (array[0] << 24) + (array[1] << 16) + (array[2] << 8) + array[3];
		}

		public static byte[] ComputePasswordHash(string password, int salt)
		{
			byte[] array = new byte[4]
			{
				(byte)(salt >> 24),
				(byte)(salt >> 16),
				(byte)(salt >> 8),
				(byte)salt
			};
			byte[] bytes = Encoding.UTF8.GetBytes(password);
			byte[] array2 = new byte[array.Length + bytes.Length];
			Buffer.BlockCopy(bytes, 0, array2, 0, bytes.Length);
			Buffer.BlockCopy(array, 0, array2, bytes.Length, array.Length);
			SHA1 sHA = SHA1.Create();
			return sHA.ComputeHash(array2);
		}

		public static bool IsPasswordValid(string passwordToValidate, int salt, byte[] correctPasswordHash)
		{
			byte[] first = ComputePasswordHash(passwordToValidate, salt);
			return first.SequenceEqual(correctPasswordHash);
		}
	}
}
