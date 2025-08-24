using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class OpenUDIDBinding
{
	public static string GetOpenUDID()
	{
		string empty = string.Empty;
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.provider.Settings$Secure");
		string static2 = androidJavaClass2.GetStatic<string>("ANDROID_ID");
		AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
		empty = androidJavaClass2.CallStatic<string>("getString", new object[2] { androidJavaObject, static2 });
		if (empty == null)
		{
			empty = string.Empty;
		}
		return getMd5Hash(empty);
	}

	private static string getMd5Hash(string input)
	{
		if (input == string.Empty)
		{
			return string.Empty;
		}
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(Encoding.Default.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
