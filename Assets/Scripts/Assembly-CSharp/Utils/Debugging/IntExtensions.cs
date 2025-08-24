namespace Utils.Debugging
{
	public static class IntExtensions
	{
		public static string ToBinaryString(this int i)
		{
			string text = string.Empty;
			while (i > 0)
			{
				text = (i & 1) + text;
				i >>= 1;
			}
			return text.PadLeft(32, '0');
		}
	}
}
