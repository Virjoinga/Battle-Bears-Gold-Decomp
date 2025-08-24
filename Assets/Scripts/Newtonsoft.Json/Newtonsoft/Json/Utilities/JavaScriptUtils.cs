using System.IO;

namespace Newtonsoft.Json.Utilities
{
	internal static class JavaScriptUtils
	{
		public static void WriteEscapedJavaScriptString(TextWriter writer, string s, char delimiter, bool appendDelimiters)
		{
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
			if (s != null)
			{
				char[] array = null;
				int num = 0;
				for (int i = 0; i < s.Length; i++)
				{
					char c = s[i];
					if (c >= ' ' && c < '\u0080' && c != '\\' && c != delimiter)
					{
						continue;
					}
					string text;
					switch (c)
					{
					case '\t':
						text = "\\t";
						break;
					case '\n':
						text = "\\n";
						break;
					case '\r':
						text = "\\r";
						break;
					case '\f':
						text = "\\f";
						break;
					case '\b':
						text = "\\b";
						break;
					case '\\':
						text = "\\\\";
						break;
					case '\u0085':
						text = "\\u0085";
						break;
					case '\u2028':
						text = "\\u2028";
						break;
					case '\u2029':
						text = "\\u2029";
						break;
					case '\'':
						text = "\\'";
						break;
					case '"':
						text = "\\\"";
						break;
					default:
						text = ((c <= '\u001f') ? StringUtils.ToCharAsUnicode(c) : null);
						break;
					}
					if (text == null)
					{
						continue;
					}
					if (i > num)
					{
						if (array == null)
						{
							array = s.ToCharArray();
						}
						writer.Write(array, num, i - num);
					}
					num = i + 1;
					writer.Write(text);
				}
				if (num == 0)
				{
					writer.Write(s);
				}
				else
				{
					if (array == null)
					{
						array = s.ToCharArray();
					}
					writer.Write(array, num, s.Length - num);
				}
			}
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
		}

		public static string ToEscapedJavaScriptString(string value)
		{
			return ToEscapedJavaScriptString(value, '"', true);
		}

		public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters)
		{
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(StringUtils.GetLength(value) ?? 16))
			{
				WriteEscapedJavaScriptString(stringWriter, value, delimiter, appendDelimiters);
				return stringWriter.ToString();
			}
		}
	}
}
