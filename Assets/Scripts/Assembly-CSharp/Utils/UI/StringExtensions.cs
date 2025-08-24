using UnityEngine;

namespace Utils.UI
{
	public static class StringExtensions
	{
		public static string Colorize(this string toColor, Color color)
		{
			return "<color=#" + color.ToHexString() + ">" + toColor + "</color>";
		}
	}
}
