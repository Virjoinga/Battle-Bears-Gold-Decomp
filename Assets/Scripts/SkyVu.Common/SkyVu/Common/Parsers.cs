using System.Collections.Generic;

namespace SkyVu.Common
{
	public static class Parsers
	{
		public static bool? ParseBoolean(object toParse)
		{
			if (toParse == null)
			{
				return null;
			}
			bool? result = null;
			if (toParse is bool)
			{
				result = toParse as bool?;
			}
			else
			{
				string text = toParse.ToString();
				bool result2;
				if (bool.TryParse(text, out result2))
				{
					result = result2;
				}
				else if (text == "1")
				{
					result = true;
				}
				else if (text == "0")
				{
					result = false;
				}
			}
			return result;
		}

		public static short? ParseShort(object toParse)
		{
			if (toParse == null)
			{
				return null;
			}
			short? result = null;
			short result2;
			if (toParse is short)
			{
				result = toParse as short?;
			}
			else if (short.TryParse(toParse.ToString(), out result2))
			{
				result = result2;
			}
			return result;
		}

		public static int? ParseInt(object toParse)
		{
			if (toParse == null)
			{
				return null;
			}
			int? result = null;
			int result2;
			if (toParse is int)
			{
				result = toParse as int?;
			}
			else if (int.TryParse(toParse.ToString(), out result2))
			{
				result = result2;
			}
			return result;
		}

		public static long? ParseLong(object toParse)
		{
			if (toParse == null)
			{
				return null;
			}
			long? result = null;
			long result2;
			if (toParse is long)
			{
				result = toParse as long?;
			}
			else if (long.TryParse(toParse.ToString(), out result2))
			{
				result = result2;
			}
			return result;
		}

		public static float? ParseFloat(object toParse)
		{
			if (toParse == null)
			{
				return null;
			}
			float? result = null;
			float result2;
			if (toParse is float)
			{
				result = toParse as float?;
			}
			else if (float.TryParse(toParse.ToString(), out result2))
			{
				result = result2;
			}
			return result;
		}

		public static double? ParseDouble(object toParse)
		{
			if (toParse == null)
			{
				return null;
			}
			double? result = null;
			double result2;
			if (toParse is double)
			{
				result = toParse as double?;
			}
			else if (double.TryParse(toParse.ToString(), out result2))
			{
				result = result2;
			}
			return result;
		}

		public static Dictionary<string, string> ParseKeyValuePair(string toParse, string objectDelimiter, string keyValueDelimiter)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			toParse = toParse.Replace(objectDelimiter, "`");
			string[] array = toParse.Split('`');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Replace(keyValueDelimiter, "`").Split('`');
				if (array3.Length == 3)
				{
					dictionary.Add(array3[0], array3[2]);
				}
				else if (array3.Length == 2)
				{
					dictionary.Add(array3[0], array3[1]);
				}
				else if (array3.Length >= 4)
				{
					string text2 = string.Empty;
					for (int j = 2; j < array3.Length; j++)
					{
						text2 += array3[j];
					}
					dictionary.Add(array3[0], text2);
				}
			}
			return dictionary;
		}

		public static void ParseKeyValuePair(string toParse, string objectDelimiter, string keyValueDelimiter, ref Dictionary<string, string> dictionary)
		{
			Dictionary<string, string> dictionary2 = ParseKeyValuePair(toParse, objectDelimiter, keyValueDelimiter);
			foreach (KeyValuePair<string, string> item in dictionary2)
			{
				if (!dictionary.ContainsKey(item.Key))
				{
					dictionary.Add(item.Key, item.Value);
				}
				else
				{
					dictionary[item.Key] = item.Value;
				}
			}
		}
	}
}
