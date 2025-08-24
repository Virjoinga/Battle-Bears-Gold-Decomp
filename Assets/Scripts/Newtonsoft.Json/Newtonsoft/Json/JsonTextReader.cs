using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	public class JsonTextReader : JsonReader, IJsonLineInfo
	{
		private readonly TextReader _reader;

		private char[] _chars;

		private int _charsUsed;

		private int _charPos;

		private int _lineStartPos;

		private int _lineNumber;

		private bool _isEndOfFile;

		private StringBuffer _buffer;

		private StringReference _stringReference;

		public int LineNumber
		{
			get
			{
				if (base.CurrentState == State.Start && LinePosition == 0)
				{
					return 0;
				}
				return _lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return _charPos - _lineStartPos;
			}
		}

		public JsonTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			_reader = reader;
			_lineNumber = 1;
			_chars = new char[4097];
		}

		internal void SetCharBuffer(char[] chars)
		{
			_chars = chars;
		}

		private StringBuffer GetBuffer()
		{
			if (_buffer == null)
			{
				_buffer = new StringBuffer(4096);
			}
			else
			{
				_buffer.Position = 0;
			}
			return _buffer;
		}

		private void OnNewLine(int pos)
		{
			_lineNumber++;
			_lineStartPos = pos - 1;
		}

		private void ParseString(char quote)
		{
			_charPos++;
			ShiftBufferIfNeeded();
			ReadStringIntoBuffer(quote);
			if (_readType == ReadType.ReadAsBytes)
			{
				byte[] value = ((_stringReference.Length != 0) ? Convert.FromBase64CharArray(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length) : new byte[0]);
				SetToken(JsonToken.Bytes, value);
				return;
			}
			if (_readType == ReadType.ReadAsString)
			{
				string value2 = _stringReference.ToString();
				SetToken(JsonToken.String, value2);
				QuoteChar = quote;
				return;
			}
			string text = _stringReference.ToString();
			if (text.Length > 0)
			{
				if (text[0] == '/')
				{
					if (text.StartsWith("/Date(", StringComparison.Ordinal) && text.EndsWith(")/", StringComparison.Ordinal))
					{
						ParseDateMicrosoft(text);
						return;
					}
				}
				else if (char.IsDigit(text[0]) && text.Length >= 19 && text.Length <= 40 && ParseDateIso(text))
				{
					return;
				}
			}
			SetToken(JsonToken.String, text);
			QuoteChar = quote;
		}

		private bool ParseDateIso(string text)
		{
			DateTime result;
			if (DateTime.TryParseExact(text, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
			{
				result = JsonConvert.EnsureDateTime(result, base.DateTimeZoneHandling);
				SetToken(JsonToken.Date, result);
				return true;
			}
			return false;
		}

		private void ParseDateMicrosoft(string text)
		{
			string text2 = text.Substring(6, text.Length - 8);
			DateTimeKind dateTimeKind = DateTimeKind.Utc;
			int num = text2.IndexOf('+', 1);
			if (num == -1)
			{
				num = text2.IndexOf('-', 1);
			}
			TimeSpan zero = TimeSpan.Zero;
			if (num != -1)
			{
				dateTimeKind = DateTimeKind.Local;
				ReadOffset(text2.Substring(num));
				text2 = text2.Substring(0, num);
			}
			long javaScriptTicks = long.Parse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture);
			DateTime dateTime = JsonConvert.ConvertJavaScriptTicksToDateTime(javaScriptTicks);
			DateTime value;
			switch (dateTimeKind)
			{
			case DateTimeKind.Unspecified:
				value = DateTime.SpecifyKind(dateTime.ToLocalTime(), DateTimeKind.Unspecified);
				break;
			case DateTimeKind.Local:
				value = dateTime.ToLocalTime();
				break;
			default:
				value = dateTime;
				break;
			}
			value = JsonConvert.EnsureDateTime(value, base.DateTimeZoneHandling);
			SetToken(JsonToken.Date, value);
		}

		private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
		}

		private void ShiftBufferIfNeeded()
		{
			int num = _chars.Length;
			if ((double)(num - _charPos) <= (double)num * 0.1)
			{
				int num2 = _charsUsed - _charPos;
				if (num2 > 0)
				{
					BlockCopyChars(_chars, _charPos, _chars, 0, num2);
				}
				_lineStartPos -= _charPos;
				_charPos = 0;
				_charsUsed = num2;
				_chars[_charsUsed] = '\0';
			}
		}

		private int ReadData(bool append)
		{
			return ReadData(append, 0);
		}

		private int ReadData(bool append, int charsRequired)
		{
			if (_isEndOfFile)
			{
				return 0;
			}
			if (_charsUsed + charsRequired >= _chars.Length - 1)
			{
				if (append)
				{
					int num = Math.Max(_chars.Length * 2, _charsUsed + charsRequired + 1);
					char[] array = new char[num];
					BlockCopyChars(_chars, 0, array, 0, _chars.Length);
					_chars = array;
				}
				else
				{
					int num2 = _charsUsed - _charPos;
					if (num2 + charsRequired + 1 >= _chars.Length)
					{
						char[] array2 = new char[num2 + charsRequired + 1];
						if (num2 > 0)
						{
							BlockCopyChars(_chars, _charPos, array2, 0, num2);
						}
						_chars = array2;
					}
					else if (num2 > 0)
					{
						BlockCopyChars(_chars, _charPos, _chars, 0, num2);
					}
					_lineStartPos -= _charPos;
					_charPos = 0;
					_charsUsed = num2;
				}
			}
			int count = _chars.Length - _charsUsed - 1;
			int num3 = _reader.Read(_chars, _charsUsed, count);
			_charsUsed += num3;
			if (num3 == 0)
			{
				_isEndOfFile = true;
			}
			_chars[_charsUsed] = '\0';
			return num3;
		}

		private bool EnsureChars(int relativePosition, bool append)
		{
			if (_charPos + relativePosition >= _charsUsed)
			{
				return ReadChars(relativePosition, append);
			}
			return true;
		}

		private bool ReadChars(int relativePosition, bool append)
		{
			if (_isEndOfFile)
			{
				return false;
			}
			int num = _charPos + relativePosition - _charsUsed + 1;
			int num2 = ReadData(append, num);
			if (num2 < num)
			{
				return false;
			}
			return true;
		}

		private static TimeSpan ReadOffset(string offsetText)
		{
			bool flag = offsetText[0] == '-';
			int num = int.Parse(offsetText.Substring(1, 2), NumberStyles.Integer, CultureInfo.InvariantCulture);
			int num2 = 0;
			if (offsetText.Length >= 5)
			{
				num2 = int.Parse(offsetText.Substring(3, 2), NumberStyles.Integer, CultureInfo.InvariantCulture);
			}
			TimeSpan result = TimeSpan.FromHours(num) + TimeSpan.FromMinutes(num2);
			if (flag)
			{
				result = result.Negate();
			}
			return result;
		}

		[DebuggerStepThrough]
		public override bool Read()
		{
			_readType = ReadType.Read;
			if (!ReadInternal())
			{
				SetToken(JsonToken.None);
				return false;
			}
			return true;
		}

		public override byte[] ReadAsBytes()
		{
			return ReadAsBytesInternal();
		}

		public override decimal? ReadAsDecimal()
		{
			return ReadAsDecimalInternal();
		}

		public override int? ReadAsInt32()
		{
			return ReadAsInt32Internal();
		}

		public override string ReadAsString()
		{
			return ReadAsStringInternal();
		}

		public override DateTime? ReadAsDateTime()
		{
			return ReadAsDateTimeInternal();
		}

		internal override bool ReadInternal()
		{
			do
			{
				IL_0000:
				switch (_currentState)
				{
				case State.Complete:
				case State.Closed:
				case State.Error:
					break;
				case State.Start:
				case State.Property:
				case State.ArrayStart:
				case State.Array:
				case State.ConstructorStart:
				case State.Constructor:
					return ParseValue();
				case State.ObjectStart:
				case State.Object:
					return ParseObject();
				case State.PostValue:
					continue;
				case State.Finished:
					if (EnsureChars(0, false))
					{
						EatWhitespace(false);
						if (_isEndOfFile)
						{
							return false;
						}
						if (_chars[_charPos] == '/')
						{
							ParseComment();
							return true;
						}
						throw CreateReaderException(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
					}
					return false;
				default:
					throw CreateReaderException(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
				}
				goto IL_0000;
			}
			while (!ParsePostValue());
			return true;
		}

		private void ReadStringIntoBuffer(char quote)
		{
			int num = _charPos;
			int charPos = _charPos;
			int num2 = _charPos;
			StringBuffer stringBuffer = null;
			while (true)
			{
				switch (_chars[num++])
				{
				case '\0':
					if (_charsUsed == num - 1)
					{
						num--;
						if (ReadData(true) == 0)
						{
							_charPos = num;
							throw CreateReaderException(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
						}
					}
					break;
				case '\\':
				{
					_charPos = num;
					if (!EnsureChars(0, true))
					{
						_charPos = num;
						throw CreateReaderException(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
					}
					int num3 = num - 1;
					char c = _chars[num];
					char value;
					switch (c)
					{
					case 'b':
						num++;
						value = '\b';
						break;
					case 't':
						num++;
						value = '\t';
						break;
					case 'n':
						num++;
						value = '\n';
						break;
					case 'f':
						num++;
						value = '\f';
						break;
					case 'r':
						num++;
						value = '\r';
						break;
					case '\\':
						num++;
						value = '\\';
						break;
					case '"':
					case '\'':
					case '/':
						value = c;
						num++;
						break;
					case 'u':
						num = (_charPos = num + 1);
						if (EnsureChars(4, true))
						{
							string s = new string(_chars, num, 4);
							char c2 = Convert.ToChar(int.Parse(s, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
							value = c2;
							num += 4;
							break;
						}
						_charPos = num;
						throw CreateReaderException(this, "Unexpected end while parsing unicode character.");
					default:
						num++;
						_charPos = num;
						throw CreateReaderException(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, "\\" + c));
					}
					if (stringBuffer == null)
					{
						stringBuffer = GetBuffer();
					}
					if (num3 > num2)
					{
						stringBuffer.Append(_chars, num2, num3 - num2);
					}
					stringBuffer.Append(value);
					num2 = num;
					break;
				}
				case '\r':
					_charPos = num - 1;
					ProcessCarriageReturn(true);
					num = _charPos;
					break;
				case '\n':
					_charPos = num - 1;
					ProcessLineFeed();
					num = _charPos;
					break;
				case '"':
				case '\'':
					if (_chars[num - 1] != quote)
					{
						break;
					}
					num--;
					if (charPos == num2)
					{
						_stringReference = new StringReference(_chars, charPos, num - charPos);
					}
					else
					{
						if (stringBuffer == null)
						{
							stringBuffer = GetBuffer();
						}
						if (num > num2)
						{
							stringBuffer.Append(_chars, num2, num - num2);
						}
						_stringReference = new StringReference(stringBuffer.GetInternalBuffer(), 0, stringBuffer.Position);
					}
					num++;
					_charPos = num;
					return;
				}
			}
		}

		private void ReadNumberIntoBuffer()
		{
			int num = _charPos;
			while (true)
			{
				switch (_chars[num++])
				{
				case '+':
				case '-':
				case '.':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
				case 'X':
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
				case 'x':
					break;
				case '\0':
					if (_charsUsed == num - 1)
					{
						num = (_charPos = num - 1);
						if (ReadData(true) == 0)
						{
							return;
						}
					}
					break;
				default:
					_charPos = num - 1;
					return;
				}
			}
		}

		private void ClearRecentString()
		{
			if (_buffer != null)
			{
				_buffer.Position = 0;
			}
			_stringReference = default(StringReference);
		}

		private bool ParsePostValue()
		{
			while (true)
			{
				char c = _chars[_charPos];
				switch (c)
				{
				case '\0':
					if (_charsUsed == _charPos)
					{
						if (ReadData(false) == 0)
						{
							_currentState = State.Finished;
							return false;
						}
					}
					else
					{
						_charPos++;
					}
					break;
				case '}':
					_charPos++;
					SetToken(JsonToken.EndObject);
					return true;
				case ']':
					_charPos++;
					SetToken(JsonToken.EndArray);
					return true;
				case ')':
					_charPos++;
					SetToken(JsonToken.EndConstructor);
					return true;
				case '/':
					ParseComment();
					return true;
				case ',':
					_charPos++;
					SetStateBasedOnCurrent();
					return false;
				case '\t':
				case ' ':
					_charPos++;
					break;
				case '\r':
					ProcessCarriageReturn(false);
					break;
				case '\n':
					ProcessLineFeed();
					break;
				default:
					if (char.IsWhiteSpace(c))
					{
						_charPos++;
						break;
					}
					throw CreateReaderException(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
				}
			}
		}

		private bool ParseObject()
		{
			while (true)
			{
				char c = _chars[_charPos];
				switch (c)
				{
				case '\0':
					if (_charsUsed == _charPos)
					{
						if (ReadData(false) == 0)
						{
							return false;
						}
					}
					else
					{
						_charPos++;
					}
					break;
				case '}':
					SetToken(JsonToken.EndObject);
					_charPos++;
					return true;
				case '/':
					ParseComment();
					return true;
				case '\r':
					ProcessCarriageReturn(false);
					break;
				case '\n':
					ProcessLineFeed();
					break;
				case '\t':
				case ' ':
					_charPos++;
					break;
				default:
					if (char.IsWhiteSpace(c))
					{
						_charPos++;
						break;
					}
					return ParseProperty();
				}
			}
		}

		private bool ParseProperty()
		{
			char c = _chars[_charPos];
			char c2;
			if (c == '"' || c == '\'')
			{
				_charPos++;
				c2 = c;
				ShiftBufferIfNeeded();
				ReadStringIntoBuffer(c2);
			}
			else
			{
				if (!ValidIdentifierChar(c))
				{
					throw CreateReaderException(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
				}
				c2 = '\0';
				ShiftBufferIfNeeded();
				ParseUnquotedProperty();
			}
			string value = _stringReference.ToString();
			EatWhitespace(false);
			if (_chars[_charPos] != ':')
			{
				throw CreateReaderException(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
			}
			_charPos++;
			SetToken(JsonToken.PropertyName, value);
			QuoteChar = c2;
			ClearRecentString();
			return true;
		}

		private bool ValidIdentifierChar(char value)
		{
			if (!char.IsLetterOrDigit(value) && value != '_')
			{
				return value == '$';
			}
			return true;
		}

		private void ParseUnquotedProperty()
		{
			int charPos = _charPos;
			char c;
			while (true)
			{
				if (_chars[_charPos] == '\0')
				{
					if (_charsUsed != _charPos)
					{
						_stringReference = new StringReference(_chars, charPos, _charPos - charPos);
						return;
					}
					if (ReadData(true) == 0)
					{
						throw CreateReaderException(this, "Unexpected end while parsing unquoted property name.");
					}
				}
				else
				{
					c = _chars[_charPos];
					if (!ValidIdentifierChar(c))
					{
						break;
					}
					_charPos++;
				}
			}
			if (char.IsWhiteSpace(c) || c == ':')
			{
				_stringReference = new StringReference(_chars, charPos, _charPos - charPos);
				return;
			}
			throw CreateReaderException(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private bool ParseValue()
		{
			while (true)
			{
				char c = _chars[_charPos];
				switch (c)
				{
				case '\0':
					if (_charsUsed == _charPos)
					{
						if (ReadData(false) == 0)
						{
							return false;
						}
					}
					else
					{
						_charPos++;
					}
					break;
				case '"':
				case '\'':
					ParseString(c);
					return true;
				case 't':
					ParseTrue();
					return true;
				case 'f':
					ParseFalse();
					return true;
				case 'n':
					if (EnsureChars(1, true))
					{
						switch (_chars[_charPos + 1])
						{
						case 'u':
							ParseNull();
							break;
						case 'e':
							ParseConstructor();
							break;
						default:
							throw CreateReaderException(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
						}
						return true;
					}
					throw CreateReaderException(this, "Unexpected end.");
				case 'N':
					ParseNumberNaN();
					return true;
				case 'I':
					ParseNumberPositiveInfinity();
					return true;
				case '-':
					if (EnsureChars(1, true) && _chars[_charPos + 1] == 'I')
					{
						ParseNumberNegativeInfinity();
					}
					else
					{
						ParseNumber();
					}
					return true;
				case '/':
					ParseComment();
					return true;
				case 'u':
					ParseUndefined();
					return true;
				case '{':
					_charPos++;
					SetToken(JsonToken.StartObject);
					return true;
				case '[':
					_charPos++;
					SetToken(JsonToken.StartArray);
					return true;
				case ']':
					_charPos++;
					SetToken(JsonToken.EndArray);
					return true;
				case ',':
					SetToken(JsonToken.Undefined);
					return true;
				case ')':
					_charPos++;
					SetToken(JsonToken.EndConstructor);
					return true;
				case '\r':
					ProcessCarriageReturn(false);
					break;
				case '\n':
					ProcessLineFeed();
					break;
				case '\t':
				case ' ':
					_charPos++;
					break;
				default:
					if (char.IsWhiteSpace(c))
					{
						_charPos++;
						break;
					}
					if (char.IsNumber(c) || c == '-' || c == '.')
					{
						ParseNumber();
						return true;
					}
					throw CreateReaderException(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
				}
			}
		}

		private void ProcessLineFeed()
		{
			_charPos++;
			OnNewLine(_charPos);
		}

		private void ProcessCarriageReturn(bool append)
		{
			_charPos++;
			if (EnsureChars(1, append) && _chars[_charPos] == '\n')
			{
				_charPos++;
			}
			OnNewLine(_charPos);
		}

		private bool EatWhitespace(bool oneOrMore)
		{
			bool flag = false;
			bool result = false;
			while (!flag)
			{
				char c = _chars[_charPos];
				switch (c)
				{
				case '\0':
					if (_charsUsed == _charPos)
					{
						if (ReadData(false) == 0)
						{
							flag = true;
						}
					}
					else
					{
						_charPos++;
					}
					break;
				case '\r':
					ProcessCarriageReturn(false);
					break;
				case '\n':
					ProcessLineFeed();
					break;
				default:
					if (c == ' ' || char.IsWhiteSpace(c))
					{
						result = true;
						_charPos++;
					}
					else
					{
						flag = true;
					}
					break;
				}
			}
			if (oneOrMore)
			{
				return result;
			}
			return true;
		}

		private void ParseConstructor()
		{
			if (!MatchValueWithTrailingSeperator("new"))
			{
				return;
			}
			EatWhitespace(false);
			int charPos = _charPos;
			int charPos2;
			while (true)
			{
				char c = _chars[_charPos];
				if (c == '\0')
				{
					if (_charsUsed == _charPos)
					{
						if (ReadData(true) == 0)
						{
							throw CreateReaderException(this, "Unexpected end while parsing constructor.");
						}
						continue;
					}
					charPos2 = _charPos;
					_charPos++;
					break;
				}
				if (char.IsLetterOrDigit(c))
				{
					_charPos++;
					continue;
				}
				switch (c)
				{
				case '\r':
					charPos2 = _charPos;
					ProcessCarriageReturn(true);
					break;
				case '\n':
					charPos2 = _charPos;
					ProcessLineFeed();
					break;
				default:
					if (char.IsWhiteSpace(c))
					{
						charPos2 = _charPos;
						_charPos++;
						break;
					}
					if (c == '(')
					{
						charPos2 = _charPos;
						break;
					}
					throw CreateReaderException(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
				}
				break;
			}
			_stringReference = new StringReference(_chars, charPos, charPos2 - charPos);
			string value = _stringReference.ToString();
			EatWhitespace(false);
			if (_chars[_charPos] != '(')
			{
				throw CreateReaderException(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
			}
			_charPos++;
			ClearRecentString();
			SetToken(JsonToken.StartConstructor, value);
		}

		private void ParseNumber()
		{
			ShiftBufferIfNeeded();
			char c = _chars[_charPos];
			int charPos = _charPos;
			ReadNumberIntoBuffer();
			_stringReference = new StringReference(_chars, charPos, _charPos - charPos);
			bool flag = char.IsDigit(c) && _stringReference.Length == 1;
			bool flag2 = c == '0' && _stringReference.Length > 1 && _stringReference.Chars[_stringReference.StartIndex + 1] != '.' && _stringReference.Chars[_stringReference.StartIndex + 1] != 'e' && _stringReference.Chars[_stringReference.StartIndex + 1] != 'E';
			object value;
			JsonToken newToken;
			if (_readType == ReadType.ReadAsInt32)
			{
				if (flag)
				{
					value = c - 48;
				}
				else if (flag2)
				{
					string text = _stringReference.ToString();
					int num = (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(text, 16) : Convert.ToInt32(text, 8));
					value = num;
				}
				else
				{
					string value2 = _stringReference.ToString();
					value = Convert.ToInt32(value2, CultureInfo.InvariantCulture);
				}
				newToken = JsonToken.Integer;
			}
			else if (_readType == ReadType.ReadAsDecimal)
			{
				if (flag)
				{
					value = (decimal)c - 48m;
				}
				else if (flag2)
				{
					string text2 = _stringReference.ToString();
					long value3 = (text2.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text2, 16) : Convert.ToInt64(text2, 8));
					value = Convert.ToDecimal(value3);
				}
				else
				{
					string s = _stringReference.ToString();
					value = decimal.Parse(s, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture);
				}
				newToken = JsonToken.Float;
			}
			else if (flag)
			{
				value = (long)c - 48L;
				newToken = JsonToken.Integer;
			}
			else if (flag2)
			{
				string text3 = _stringReference.ToString();
				value = (text3.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text3, 16) : Convert.ToInt64(text3, 8));
				newToken = JsonToken.Integer;
			}
			else
			{
				string text4 = _stringReference.ToString();
				if (text4.IndexOf('.') != -1 || text4.IndexOf('E') != -1 || text4.IndexOf('e') != -1)
				{
					value = Convert.ToDouble(text4, CultureInfo.InvariantCulture);
					newToken = JsonToken.Float;
				}
				else
				{
					try
					{
						value = Convert.ToInt64(text4, CultureInfo.InvariantCulture);
					}
					catch (OverflowException innerException)
					{
						throw new JsonReaderException("JSON integer {0} is too large or small for an Int64.".FormatWith(CultureInfo.InvariantCulture, text4), innerException);
					}
					newToken = JsonToken.Integer;
				}
			}
			ClearRecentString();
			SetToken(newToken, value);
		}

		private void ParseComment()
		{
			_charPos++;
			if (!EnsureChars(1, false) || _chars[_charPos] != '*')
			{
				throw CreateReaderException(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
			}
			_charPos++;
			int charPos = _charPos;
			bool flag = false;
			while (!flag)
			{
				switch (_chars[_charPos])
				{
				case '\0':
					if (_charsUsed == _charPos)
					{
						if (ReadData(true) == 0)
						{
							throw CreateReaderException(this, "Unexpected end while parsing comment.");
						}
					}
					else
					{
						_charPos++;
					}
					break;
				case '*':
					_charPos++;
					if (EnsureChars(0, true) && _chars[_charPos] == '/')
					{
						_stringReference = new StringReference(_chars, charPos, _charPos - charPos - 1);
						_charPos++;
						flag = true;
					}
					break;
				case '\r':
					ProcessCarriageReturn(true);
					break;
				case '\n':
					ProcessLineFeed();
					break;
				default:
					_charPos++;
					break;
				}
			}
			SetToken(JsonToken.Comment, _stringReference.ToString());
			ClearRecentString();
		}

		private bool MatchValue(string value)
		{
			if (!EnsureChars(value.Length - 1, true))
			{
				return false;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (_chars[_charPos + i] != value[i])
				{
					return false;
				}
			}
			_charPos += value.Length;
			return true;
		}

		private bool MatchValueWithTrailingSeperator(string value)
		{
			if (!MatchValue(value))
			{
				return false;
			}
			if (!EnsureChars(0, false))
			{
				return true;
			}
			if (!IsSeperator(_chars[_charPos]))
			{
				return _chars[_charPos] == '\0';
			}
			return true;
		}

		private bool IsSeperator(char c)
		{
			switch (c)
			{
			case ',':
			case ']':
			case '}':
				return true;
			case '/':
				if (!EnsureChars(1, false))
				{
					return false;
				}
				return _chars[_charPos + 1] == '*';
			case ')':
				if (base.CurrentState == State.Constructor || base.CurrentState == State.ConstructorStart)
				{
					return true;
				}
				break;
			case '\t':
			case '\n':
			case '\r':
			case ' ':
				return true;
			default:
				if (char.IsWhiteSpace(c))
				{
					return true;
				}
				break;
			}
			return false;
		}

		private void ParseTrue()
		{
			if (MatchValueWithTrailingSeperator(JsonConvert.True))
			{
				SetToken(JsonToken.Boolean, true);
				return;
			}
			throw CreateReaderException(this, "Error parsing boolean value.");
		}

		private void ParseNull()
		{
			if (MatchValueWithTrailingSeperator(JsonConvert.Null))
			{
				SetToken(JsonToken.Null);
				return;
			}
			throw CreateReaderException(this, "Error parsing null value.");
		}

		private void ParseUndefined()
		{
			if (MatchValueWithTrailingSeperator(JsonConvert.Undefined))
			{
				SetToken(JsonToken.Undefined);
				return;
			}
			throw CreateReaderException(this, "Error parsing undefined value.");
		}

		private void ParseFalse()
		{
			if (MatchValueWithTrailingSeperator(JsonConvert.False))
			{
				SetToken(JsonToken.Boolean, false);
				return;
			}
			throw CreateReaderException(this, "Error parsing boolean value.");
		}

		private void ParseNumberNegativeInfinity()
		{
			if (MatchValueWithTrailingSeperator(JsonConvert.NegativeInfinity))
			{
				SetToken(JsonToken.Float, double.NegativeInfinity);
				return;
			}
			throw CreateReaderException(this, "Error parsing negative infinity value.");
		}

		private void ParseNumberPositiveInfinity()
		{
			if (MatchValueWithTrailingSeperator(JsonConvert.PositiveInfinity))
			{
				SetToken(JsonToken.Float, double.PositiveInfinity);
				return;
			}
			throw CreateReaderException(this, "Error parsing positive infinity value.");
		}

		private void ParseNumberNaN()
		{
			if (MatchValueWithTrailingSeperator(JsonConvert.NaN))
			{
				SetToken(JsonToken.Float, double.NaN);
				return;
			}
			throw CreateReaderException(this, "Error parsing NaN value.");
		}

		public override void Close()
		{
			base.Close();
			if (base.CloseInput && _reader != null)
			{
				_reader.Close();
			}
			if (_buffer != null)
			{
				_buffer.Clear();
			}
		}

		public bool HasLineInfo()
		{
			return true;
		}
	}
}
