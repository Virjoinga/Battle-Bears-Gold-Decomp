using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Utilities
{
	internal static class StringUtils
	{
		public const string CarriageReturnLineFeed = "\r\n";

		public const string Empty = "";

		public const char CarriageReturn = '\r';

		public const char LineFeed = '\n';

		public const char Tab = '\t';

		public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
		{
			ValidationUtils.ArgumentNotNull(format, "format");
			return string.Format(provider, format, args);
		}

		public static bool IsWhiteSpace(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (!char.IsWhiteSpace(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static string NullEmptyString(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}

		public static StringWriter CreateStringWriter(int capacity)
		{
			StringBuilder sb = new StringBuilder(capacity);
			return new StringWriter(sb, CultureInfo.InvariantCulture);
		}

		public static int? GetLength(string value)
		{
			if (value == null)
			{
				return null;
			}
			return value.Length;
		}

		public static string ToCharAsUnicode(char c)
		{
			char c2 = MathUtils.IntToHex(((int)c >> 12) & 0xF);
			char c3 = MathUtils.IntToHex(((int)c >> 8) & 0xF);
			char c4 = MathUtils.IntToHex(((int)c >> 4) & 0xF);
			char c5 = MathUtils.IntToHex(c & 0xF);
			return new string(new char[6] { '\\', 'u', c2, c3, c4, c5 });
		}

		public static TSource ForgivingCaseSensitiveFind<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, string testValue)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (valueSelector == null)
			{
				throw new ArgumentNullException("valueSelector");
			}
			IEnumerable<TSource> source2 = source.Where((TSource s) => string.Equals(valueSelector(s), testValue, StringComparison.OrdinalIgnoreCase));
			if (source2.Count() <= 1)
			{
				return source2.SingleOrDefault();
			}
			IEnumerable<TSource> source3 = source.Where((TSource s) => string.Equals(valueSelector(s), testValue, StringComparison.Ordinal));
			return source3.SingleOrDefault();
		}

		public static string ToCamelCase(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (!char.IsUpper(s[0]))
			{
				return s;
			}
			string text = null;
			text = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
			if (s.Length > 1)
			{
				text += s.Substring(1);
			}
			return text;
		}
	}
}
