using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json
{
	public abstract class JsonReader : IDisposable
	{
		protected internal enum State
		{
			Start = 0,
			Complete = 1,
			Property = 2,
			ObjectStart = 3,
			Object = 4,
			ArrayStart = 5,
			Array = 6,
			Closed = 7,
			PostValue = 8,
			ConstructorStart = 9,
			Constructor = 10,
			Error = 11,
			Finished = 12
		}

		private JsonToken _tokenType;

		private object _value;

		private char _quoteChar;

		internal State _currentState;

		internal ReadType _readType;

		private JsonPosition _currentPosition;

		private CultureInfo _culture;

		private DateTimeZoneHandling _dateTimeZoneHandling;

		private readonly List<JsonPosition> _stack;

		protected State CurrentState
		{
			get
			{
				return _currentState;
			}
		}

		public bool CloseInput { get; set; }

		public virtual char QuoteChar
		{
			get
			{
				return _quoteChar;
			}
			protected internal set
			{
				_quoteChar = value;
			}
		}

		public DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				return _dateTimeZoneHandling;
			}
			set
			{
				_dateTimeZoneHandling = value;
			}
		}

		public virtual JsonToken TokenType
		{
			get
			{
				return _tokenType;
			}
		}

		public virtual object Value
		{
			get
			{
				return _value;
			}
		}

		public virtual Type ValueType
		{
			get
			{
				if (_value == null)
				{
					return null;
				}
				return _value.GetType();
			}
		}

		public virtual int Depth
		{
			get
			{
				int count = _stack.Count;
				if (IsStartToken(TokenType) || _currentPosition.Type == JsonContainerType.None)
				{
					return count;
				}
				return count + 1;
			}
		}

		public virtual string Path
		{
			get
			{
				if (_currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				return JsonPosition.BuildPath(_stack.Concat(new JsonPosition[1] { _currentPosition }));
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return _culture ?? CultureInfo.InvariantCulture;
			}
			set
			{
				_culture = value;
			}
		}

		protected JsonReader()
		{
			_currentState = State.Start;
			_stack = new List<JsonPosition>(4);
			_dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
			CloseInput = true;
		}

		private void Push(JsonContainerType value)
		{
			UpdateScopeWithFinishedValue();
			if (_currentPosition.Type == JsonContainerType.None)
			{
				_currentPosition.Type = value;
				return;
			}
			_stack.Add(_currentPosition);
			JsonPosition jsonPosition = default(JsonPosition);
			jsonPosition.Type = value;
			JsonPosition currentPosition = jsonPosition;
			_currentPosition = currentPosition;
		}

		private JsonContainerType Pop()
		{
			JsonPosition currentPosition;
			if (_stack.Count > 0)
			{
				currentPosition = _currentPosition;
				_currentPosition = _stack[_stack.Count - 1];
				_stack.RemoveAt(_stack.Count - 1);
			}
			else
			{
				currentPosition = _currentPosition;
				_currentPosition = default(JsonPosition);
			}
			return currentPosition.Type;
		}

		private JsonContainerType Peek()
		{
			return _currentPosition.Type;
		}

		public abstract bool Read();

		public abstract int? ReadAsInt32();

		public abstract string ReadAsString();

		public abstract byte[] ReadAsBytes();

		public abstract decimal? ReadAsDecimal();

		public abstract DateTime? ReadAsDateTime();

		internal virtual bool ReadInternal()
		{
			throw new NotImplementedException();
		}

		internal byte[] ReadAsBytesInternal()
		{
			_readType = ReadType.ReadAsBytes;
			do
			{
				if (!ReadInternal())
				{
					SetToken(JsonToken.None);
					return null;
				}
			}
			while (TokenType == JsonToken.Comment);
			if (IsWrappedInTypeObject())
			{
				byte[] array = ReadAsBytes();
				ReadInternal();
				SetToken(JsonToken.Bytes, array);
				return array;
			}
			if (TokenType == JsonToken.String)
			{
				string text = (string)Value;
				byte[] value = ((text.Length == 0) ? new byte[0] : Convert.FromBase64String(text));
				SetToken(JsonToken.Bytes, value);
			}
			if (TokenType == JsonToken.Null)
			{
				return null;
			}
			if (TokenType == JsonToken.Bytes)
			{
				return (byte[])Value;
			}
			if (TokenType == JsonToken.StartArray)
			{
				List<byte> list = new List<byte>();
				while (ReadInternal())
				{
					switch (TokenType)
					{
					case JsonToken.Integer:
						list.Add(Convert.ToByte(Value, CultureInfo.InvariantCulture));
						break;
					case JsonToken.EndArray:
					{
						byte[] array2 = list.ToArray();
						SetToken(JsonToken.Bytes, array2);
						return array2;
					}
					default:
						throw CreateReaderException(this, "Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
					case JsonToken.Comment:
						break;
					}
				}
				throw CreateReaderException(this, "Unexpected end when reading bytes.");
			}
			if (TokenType == JsonToken.EndArray)
			{
				return null;
			}
			throw CreateReaderException(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}

		internal decimal? ReadAsDecimalInternal()
		{
			_readType = ReadType.ReadAsDecimal;
			do
			{
				if (!ReadInternal())
				{
					SetToken(JsonToken.None);
					return null;
				}
			}
			while (TokenType == JsonToken.Comment);
			if (TokenType == JsonToken.Integer || TokenType == JsonToken.Float)
			{
				if (!(Value is decimal))
				{
					SetToken(JsonToken.Float, Convert.ToDecimal(Value, CultureInfo.InvariantCulture));
				}
				return (decimal)Value;
			}
			if (TokenType == JsonToken.Null)
			{
				return null;
			}
			if (TokenType == JsonToken.String)
			{
				decimal result;
				if (decimal.TryParse((string)Value, NumberStyles.Number, Culture, out result))
				{
					SetToken(JsonToken.Float, result);
					return result;
				}
				throw CreateReaderException(this, "Could not convert string to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, Value));
			}
			if (TokenType == JsonToken.EndArray)
			{
				return null;
			}
			throw CreateReaderException(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}

		internal int? ReadAsInt32Internal()
		{
			_readType = ReadType.ReadAsInt32;
			do
			{
				if (!ReadInternal())
				{
					SetToken(JsonToken.None);
					return null;
				}
			}
			while (TokenType == JsonToken.Comment);
			if (TokenType == JsonToken.Integer || TokenType == JsonToken.Float)
			{
				if (!(Value is int))
				{
					SetToken(JsonToken.Integer, Convert.ToInt32(Value, CultureInfo.InvariantCulture));
				}
				return (int)Value;
			}
			if (TokenType == JsonToken.Null)
			{
				return null;
			}
			if (TokenType == JsonToken.String)
			{
				int result;
				if (int.TryParse((string)Value, NumberStyles.Integer, Culture, out result))
				{
					SetToken(JsonToken.Integer, result);
					return result;
				}
				throw CreateReaderException(this, "Could not convert string to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, Value));
			}
			if (TokenType == JsonToken.EndArray)
			{
				return null;
			}
			throw CreateReaderException(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}

		internal string ReadAsStringInternal()
		{
			_readType = ReadType.ReadAsString;
			do
			{
				if (!ReadInternal())
				{
					SetToken(JsonToken.None);
					return null;
				}
			}
			while (TokenType == JsonToken.Comment);
			if (TokenType == JsonToken.String)
			{
				return (string)Value;
			}
			if (TokenType == JsonToken.Null)
			{
				return null;
			}
			if (IsPrimitiveToken(TokenType) && Value != null)
			{
				string text = (ConvertUtils.IsConvertible(Value) ? ConvertUtils.ToConvertible(Value).ToString(Culture) : ((!(Value is IFormattable)) ? Value.ToString() : ((IFormattable)Value).ToString(null, Culture)));
				SetToken(JsonToken.String, text);
				return text;
			}
			if (TokenType == JsonToken.EndArray)
			{
				return null;
			}
			throw CreateReaderException(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}

		internal DateTime? ReadAsDateTimeInternal()
		{
			_readType = ReadType.ReadAsDateTime;
			do
			{
				if (!ReadInternal())
				{
					SetToken(JsonToken.None);
					return null;
				}
			}
			while (TokenType == JsonToken.Comment);
			if (TokenType == JsonToken.Date)
			{
				return (DateTime)Value;
			}
			if (TokenType == JsonToken.Null)
			{
				return null;
			}
			if (TokenType == JsonToken.String)
			{
				string text = (string)Value;
				if (string.IsNullOrEmpty(text))
				{
					SetToken(JsonToken.Null);
					return null;
				}
				DateTime result;
				if (DateTime.TryParse(text, Culture, DateTimeStyles.RoundtripKind, out result))
				{
					result = JsonConvert.EnsureDateTime(result, DateTimeZoneHandling);
					SetToken(JsonToken.Date, result);
					return result;
				}
				throw CreateReaderException(this, "Could not convert string to DateTime: {0}.".FormatWith(CultureInfo.InvariantCulture, Value));
			}
			if (TokenType == JsonToken.EndArray)
			{
				return null;
			}
			throw CreateReaderException(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}

		private bool IsWrappedInTypeObject()
		{
			_readType = ReadType.Read;
			if (TokenType == JsonToken.StartObject)
			{
				if (!ReadInternal())
				{
					throw CreateReaderException(this, "Unexpected end when reading bytes.");
				}
				if (Value.ToString() == "$type")
				{
					ReadInternal();
					if (Value != null && Value.ToString().StartsWith("System.Byte[]"))
					{
						ReadInternal();
						if (Value.ToString() == "$value")
						{
							return true;
						}
					}
				}
				throw CreateReaderException(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
			}
			return false;
		}

		public void Skip()
		{
			if (TokenType == JsonToken.PropertyName)
			{
				Read();
			}
			if (IsStartToken(TokenType))
			{
				int depth = Depth;
				while (Read() && depth < Depth)
				{
				}
			}
		}

		protected void SetToken(JsonToken newToken)
		{
			SetToken(newToken, null);
		}

		protected void SetToken(JsonToken newToken, object value)
		{
			_tokenType = newToken;
			switch (newToken)
			{
			case JsonToken.StartObject:
				_currentState = State.ObjectStart;
				Push(JsonContainerType.Object);
				break;
			case JsonToken.StartArray:
				_currentState = State.ArrayStart;
				Push(JsonContainerType.Array);
				break;
			case JsonToken.StartConstructor:
				_currentState = State.ConstructorStart;
				Push(JsonContainerType.Constructor);
				break;
			case JsonToken.EndObject:
				ValidateEnd(JsonToken.EndObject);
				break;
			case JsonToken.EndArray:
				ValidateEnd(JsonToken.EndArray);
				break;
			case JsonToken.EndConstructor:
				ValidateEnd(JsonToken.EndConstructor);
				break;
			case JsonToken.PropertyName:
				_currentState = State.Property;
				_currentPosition.PropertyName = (string)value;
				break;
			case JsonToken.Raw:
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				_currentState = ((Peek() != 0) ? State.PostValue : State.Finished);
				UpdateScopeWithFinishedValue();
				break;
			}
			_value = value;
		}

		private void UpdateScopeWithFinishedValue()
		{
			if (_currentPosition.Type == JsonContainerType.Array || _currentPosition.Type == JsonContainerType.Constructor)
			{
				if (!_currentPosition.Position.HasValue)
				{
					_currentPosition.Position = 0;
				}
				else
				{
					_currentPosition.Position++;
				}
			}
		}

		private void ValidateEnd(JsonToken endToken)
		{
			JsonContainerType jsonContainerType = Pop();
			if (GetTypeForCloseToken(endToken) != jsonContainerType)
			{
				throw new JsonReaderException("JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, jsonContainerType));
			}
			_currentState = ((Peek() != 0) ? State.PostValue : State.Finished);
		}

		protected void SetStateBasedOnCurrent()
		{
			JsonContainerType jsonContainerType = Peek();
			switch (jsonContainerType)
			{
			case JsonContainerType.Object:
				_currentState = State.Object;
				break;
			case JsonContainerType.Array:
				_currentState = State.Array;
				break;
			case JsonContainerType.Constructor:
				_currentState = State.Constructor;
				break;
			case JsonContainerType.None:
				_currentState = State.Finished;
				break;
			default:
				throw new JsonReaderException("While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, jsonContainerType));
			}
		}

		internal static bool IsPrimitiveToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsStartToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.StartObject:
			case JsonToken.StartArray:
			case JsonToken.StartConstructor:
				return true;
			case JsonToken.None:
			case JsonToken.PropertyName:
			case JsonToken.Comment:
			case JsonToken.Raw:
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.EndObject:
			case JsonToken.EndArray:
			case JsonToken.EndConstructor:
			case JsonToken.Date:
			case JsonToken.Bytes:
				return false;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("token", token, "Unexpected JsonToken value.");
			}
		}

		private JsonContainerType GetTypeForCloseToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
				return JsonContainerType.Object;
			case JsonToken.EndArray:
				return JsonContainerType.Array;
			case JsonToken.EndConstructor:
				return JsonContainerType.Constructor;
			default:
				throw new JsonReaderException("Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token));
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_currentState != State.Closed && disposing)
			{
				Close();
			}
		}

		public virtual void Close()
		{
			_currentState = State.Closed;
			_tokenType = JsonToken.None;
			_value = null;
		}

		internal JsonReaderException CreateReaderException(JsonReader reader, string message)
		{
			return CreateReaderException(reader, message, null);
		}

		internal JsonReaderException CreateReaderException(JsonReader reader, string message, Exception ex)
		{
			return CreateReaderException(reader as IJsonLineInfo, message, ex);
		}

		internal JsonReaderException CreateReaderException(IJsonLineInfo lineInfo, string message, Exception ex)
		{
			message = FormatExceptionMessage(lineInfo, message);
			int lineNumber;
			int linePosition;
			if (lineInfo != null && lineInfo.HasLineInfo())
			{
				lineNumber = lineInfo.LineNumber;
				linePosition = lineInfo.LinePosition;
			}
			else
			{
				lineNumber = 0;
				linePosition = 0;
			}
			return new JsonReaderException(message, ex, Path, lineNumber, linePosition);
		}

		internal static string FormatExceptionMessage(IJsonLineInfo lineInfo, string message)
		{
			if (!message.EndsWith("."))
			{
				message += ".";
			}
			if (lineInfo != null && lineInfo.HasLineInfo())
			{
				message += " Line {0}, position {1}.".FormatWith(CultureInfo.InvariantCulture, lineInfo.LineNumber, lineInfo.LinePosition);
			}
			return message;
		}
	}
}
