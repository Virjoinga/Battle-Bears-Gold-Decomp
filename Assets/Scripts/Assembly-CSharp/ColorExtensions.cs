using System;
using UnityEngine;

public static class ColorExtensions
{
	public static string ToHexString(this Color color)
	{
		return BitConverter.ToString(new byte[4]
		{
			Convert.ToByte(Mathf.RoundToInt(color.r * 255f)),
			Convert.ToByte(Mathf.RoundToInt(color.g * 255f)),
			Convert.ToByte(Mathf.RoundToInt(color.b * 255f)),
			Convert.ToByte(Mathf.RoundToInt(color.a * 255f))
		}).Replace("-", string.Empty);
	}

	public static Color Random(this Color color, float alpha = 1f)
	{
		return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), alpha);
	}
}
