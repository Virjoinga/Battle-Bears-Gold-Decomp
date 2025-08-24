using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json
{
	public abstract class JsonWriter : IDisposable
	{
		internal enum State
		{
			Start = 0,
			Property = 1,
			ObjectStart = 2,
			Object = 3,
			ArrayStart = 4,
			Array = 5,
			ConstructorStart = 6,
			Constructor = 7,
			Bytes = 8,
			Closed = 9,
			Error = 10
		}

		private static readonly State[][] StateArray;

		internal static readonly State[][] StateArrayTempate;

		private readonly List<JsonPosition> _stack;

		private JsonPosition _currentPosition;

		private State _currentState;

		private Formatting _formatting;

		private DateFormatHandling _dateFormatHandling;

		private DateTimeZoneHandling _dateTimeZoneHandling;

		public bool CloseOutput { get; set; }

		protected internal int Top
		{
			get
			{
				int num = _stack.Count;
				if (Peek() != 0)
				{
					num++;
				}
				return num;
			}
		}

		internal string ContainerPath
		{
			get
			{
				if (_currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				IEnumerable<JsonPosition> positions = (_currentPosition.InsideContainer() ? _stack : _stack.Concat(new JsonPosition[1] { _currentPosition }));
				return JsonPosition.BuildPath(positions);
			}
		}

		public WriteState WriteState
		{
			get
			{
				switch (_currentState)
				{
				case State.Error:
					return WriteState.Error;
				case State.Closed:
					return WriteState.Closed;
				case State.ObjectStart:
				case State.Object:
					return WriteState.Object;
				case State.ArrayStart:
				case State.Array:
					return WriteState.Array;
				case State.ConstructorStart:
				case State.Constructor:
					return WriteState.Constructor;
				case State.Property:
					return WriteState.Property;
				case State.Start:
					return WriteState.Start;
				default:
					throw new JsonWriterException("Invalid state: " + _currentState);
				}
			}
		}

		public string Path
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

		public Formatting Formatting
		{
			get
			{
				return _formatting;
			}
			set
			{
				_formatting = value;
			}
		}

		public DateFormatHandling DateFormatHandling
		{
			get
			{
				return _dateFormatHandling;
			}
			set
			{
				_dateFormatHandling = value;
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

		internal static State[][] BuildStateArray()
		{
			List<State[]> list = StateArrayTempate.ToList();
			State[] item = StateArrayTempate[0];
			State[] item2 = StateArrayTempate[7];
			foreach (JsonToken value in EnumUtils.GetValues(typeof(JsonToken)))
			{
				if (list.Count <= (int)value)
				{
					switch (value)
					{
					case JsonToken.Integer:
					case JsonToken.Float:
					case JsonToken.String:
					case JsonToken.Boolean:
					case JsonToken.Null:
					case JsonToken.Undefined:
					case JsonToken.Date:
					case JsonToken.Bytes:
						list.Add(item2);
						break;
					default:
						list.Add(item);
						break;
					}
				}
			}
			return list.ToArray();
		}

		static JsonWriter()
		{
			StateArrayTempate = new State[8][]
			{
				new State[10]
				{
					State.Error,
					State.Error,
					State.Error,
					State.Error,
					State.Error,
					State.Error,
					State.Error,
					State.Error,
					State.Error,
					State.Error
				},
				new State[10]
				{
					State.ObjectStart,
					State.ObjectStart,
					State.Error,
					State.Error,
					State.ObjectStart,
					State.ObjectStart,
					State.ObjectStart,
					State.ObjectStart,
					State.Error,
					State.Error
				},
				new State[10]
				{
					State.ArrayStart,
					State.ArrayStart,
					State.Error,
					State.Error,
					State.ArrayStart,
					State.ArrayStart,
					State.ArrayStart,
					State.ArrayStart,
					State.Error,
					State.Error
				},
				new State[10]
				{
					State.ConstructorStart,
					State.ConstructorStart,
					State.Error,
					State.Error,
					State.ConstructorStart,
					State.ConstructorStart,
					State.ConstructorStart,
					State.ConstructorStart,
					State.Error,
					State.Error
				},
				new State[10]
				{
					State.Property,
					State.Error,
					State.Property,
					State.Property,
					State.Error,
					State.Error,
					State.Error,
					State.Error,
					State.Error,
					State.Error
				},
				new State[10]
				{
					State.Start,
					State.Property,
					State.ObjectStart,
					State.Object,
					State.ArrayStart,
					State.Array,
					State.Constructor,
					State.Constructor,
					State.Error,
					State.Error
				},
				new State[10]
				{
					State.Start,
					State.Property,
					State.ObjectStart,
					State.Object,
					State.ArrayStart,
					State.Array,
					State.Constructor,
					State.Constructor,
					State.Error,
					State.Error
				},
				new State[10]
				{
					State.Start,
					State.Object,
					State.Error,
					State.Error,
					State.Array,
					State.Array,
					State.Constructor,
					State.Constructor,
					State.Error,
					State.Error
				}
			};
			StateArray = BuildStateArray();
		}

		protected JsonWriter()
		{
			_stack = new List<JsonPosition>(4);
			_currentState = State.Start;
			_formatting = Formatting.None;
			_dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
			CloseOutput = true;
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

		public abstract void Flush();

		public virtual void Close()
		{
			AutoCompleteAll();
		}

		public virtual void WriteStartObject()
		{
			AutoComplete(JsonToken.StartObject);
			Push(JsonContainerType.Object);
		}

		public virtual void WriteEndObject()
		{
			AutoCompleteClose(JsonToken.EndObject);
		}

		public virtual void WriteStartArray()
		{
			AutoComplete(JsonToken.StartArray);
			Push(JsonContainerType.Array);
		}

		public virtual void WriteEndArray()
		{
			AutoCompleteClose(JsonToken.EndArray);
		}

		public virtual void WriteStartConstructor(string name)
		{
			AutoComplete(JsonToken.StartConstructor);
			Push(JsonContainerType.Constructor);
		}

		public virtual void WriteEndConstructor()
		{
			AutoCompleteClose(JsonToken.EndConstructor);
		}

		public virtual void WritePropertyName(string name)
		{
			_currentPosition.PropertyName = name;
			AutoComplete(JsonToken.PropertyName);
		}

		public virtual void WriteEnd()
		{
			WriteEnd(Peek());
		}

		public void WriteToken(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			int initialDepth = ((reader.TokenType == JsonToken.None) ? (-1) : (IsStartToken(reader.TokenType) ? reader.Depth : (reader.Depth + 1)));
			WriteToken(reader, initialDepth);
		}

		internal void WriteToken(JsonReader reader, int initialDepth)
		{
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.StartObject:
					WriteStartObject();
					break;
				case JsonToken.StartArray:
					WriteStartArray();
					break;
				case JsonToken.StartConstructor:
				{
					string a = reader.Value.ToString();
					if (string.Equals(a, "Date", StringComparison.Ordinal))
					{
						WriteConstructorDate(reader);
					}
					else
					{
						WriteStartConstructor(reader.Value.ToString());
					}
					break;
				}
				case JsonToken.PropertyName:
					WritePropertyName(reader.Value.ToString());
					break;
				case JsonToken.Comment:
					WriteComment(reader.Value.ToString());
					break;
				case JsonToken.Integer:
					WriteValue(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
					break;
				case JsonToken.Float:
					WriteValue(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
					break;
				case JsonToken.String:
					WriteValue(reader.Value.ToString());
					break;
				case JsonToken.Boolean:
					WriteValue(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
					break;
				case JsonToken.Null:
					WriteNull();
					break;
				case JsonToken.Undefined:
					WriteUndefined();
					break;
				case JsonToken.EndObject:
					WriteEndObject();
					break;
				case JsonToken.EndArray:
					WriteEndArray();
					break;
				case JsonToken.EndConstructor:
					WriteEndConstructor();
					break;
				case JsonToken.Date:
					WriteValue((DateTime)reader.Value);
					break;
				case JsonToken.Raw:
					WriteRawValue((string)reader.Value);
					break;
				case JsonToken.Bytes:
					WriteValue((byte[])reader.Value);
					break;
				default:
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", reader.TokenType, "Unexpected token type.");
				case JsonToken.None:
					break;
				}
			}
			while (initialDepth - 1 < reader.Depth - (IsEndToken(reader.TokenType) ? 1 : 0) && reader.Read());
		}

		private void WriteConstructorDate(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw new Exception("Unexpected end when reading date constructor.");
			}
			if (reader.TokenType != JsonToken.Integer)
			{
				throw new Exception("Unexpected token when reading date constructor. Expected Integer, got " + reader.TokenType);
			}
			long javaScriptTicks = (long)reader.Value;
			DateTime value = JsonConvert.ConvertJavaScriptTicksToDateTime(javaScriptTicks);
			if (!reader.Read())
			{
				throw new Exception("Unexpected end when reading date constructor.");
			}
			if (reader.TokenType != JsonToken.EndConstructor)
			{
				throw new Exception("Unexpected token when reading date constructor. Expected EndConstructor, got " + reader.TokenType);
			}
			WriteValue(value);
		}

		private bool IsEndToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
			case JsonToken.EndArray:
			case JsonToken.EndConstructor:
				return true;
			default:
				return false;
			}
		}

		private bool IsStartToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.StartObject:
			case JsonToken.StartArray:
			case JsonToken.StartConstructor:
				return true;
			default:
				return false;
			}
		}

		private void WriteEnd(JsonContainerType type)
		{
			switch (type)
			{
			case JsonContainerType.Object:
				WriteEndObject();
				break;
			case JsonContainerType.Array:
				WriteEndArray();
				break;
			case JsonContainerType.Constructor:
				WriteEndConstructor();
				break;
			default:
				throw new JsonWriterException("Unexpected type when writing end: " + type);
			}
		}

		private void AutoCompleteAll()
		{
			while (Top > 0)
			{
				WriteEnd();
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
				throw new JsonWriterException("No type for token: " + token);
			}
		}

		private JsonToken GetCloseTokenForType(JsonContainerType type)
		{
			switch (type)
			{
			case JsonContainerType.Object:
				return JsonToken.EndObject;
			case JsonContainerType.Array:
				return JsonToken.EndArray;
			case JsonContainerType.Constructor:
				return JsonToken.EndConstructor;
			default:
				throw new JsonWriterException("No close token for type: " + type);
			}
		}

		private void AutoCompleteClose(JsonToken tokenBeingClosed)
		{
			int num = 0;
			JsonContainerType typeForCloseToken = GetTypeForCloseToken(tokenBeingClosed);
			if (_currentPosition.Type == typeForCloseToken)
			{
				num = 1;
			}
			else
			{
				int num2 = Top - 2;
				for (int num3 = num2; num3 >= 0; num3--)
				{
					int index = num2 - num3;
					if (_stack[index].Type == typeForCloseToken)
					{
						num = num3 + 2;
						break;
					}
				}
			}
			if (num == 0)
			{
				throw new JsonWriterException("No token to close.");
			}
			for (int i = 0; i < num; i++)
			{
				JsonToken closeTokenForType = GetCloseTokenForType(Pop());
				if (_formatting == Formatting.Indented && _currentState != State.ObjectStart && _currentState != State.ArrayStart)
				{
					WriteIndent();
				}
				WriteEnd(closeTokenForType);
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
					_currentState = State.Array;
					break;
				case JsonContainerType.None:
					_currentState = State.Start;
					break;
				default:
					throw new JsonWriterException("Unknown JsonType: " + jsonContainerType);
				}
			}
		}

		protected virtual void WriteEnd(JsonToken token)
		{
		}

		protected virtual void WriteIndent()
		{
		}

		protected virtual void WriteValueDelimiter()
		{
		}

		protected virtual void WriteIndentSpace()
		{
		}

		internal void AutoComplete(JsonToken tokenBeingWritten)
		{
			if (tokenBeingWritten != JsonToken.StartObject && tokenBeingWritten != JsonToken.StartArray && tokenBeingWritten != JsonToken.StartConstructor)
			{
				UpdateScopeWithFinishedValue();
			}
			State state = StateArray[(int)tokenBeingWritten][(int)_currentState];
			if (state == State.Error)
			{
				throw new JsonWriterException("Token {0} in state {1} would result in an invalid JSON object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), _currentState.ToString()));
			}
			if ((_currentState == State.Object || _currentState == State.Array || _currentState == State.Constructor) && tokenBeingWritten != JsonToken.Comment)
			{
				WriteValueDelimiter();
			}
			else if (_currentState == State.Property && _formatting == Formatting.Indented)
			{
				WriteIndentSpace();
			}
			if (_formatting == Formatting.Indented)
			{
				WriteState writeState = WriteState;
				if ((tokenBeingWritten == JsonToken.PropertyName && writeState != WriteState.Start) || writeState == WriteState.Array || writeState == WriteState.Constructor)
				{
					WriteIndent();
				}
			}
			_currentState = state;
		}

		public virtual void WriteNull()
		{
			AutoComplete(JsonToken.Null);
		}

		public virtual void WriteUndefined()
		{
			AutoComplete(JsonToken.Undefined);
		}

		public virtual void WriteRaw(string json)
		{
		}

		public virtual void WriteRawValue(string json)
		{
			AutoComplete(JsonToken.Undefined);
			WriteRaw(json);
		}

		public virtual void WriteValue(string value)
		{
			AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(int value)
		{
			AutoComplete(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(uint value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(long value)
		{
			AutoComplete(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ulong value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(float value)
		{
			AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(double value)
		{
			AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(bool value)
		{
			AutoComplete(JsonToken.Boolean);
		}

		public virtual void WriteValue(short value)
		{
			AutoComplete(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ushort value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(char value)
		{
			AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(byte value)
		{
			AutoComplete(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(sbyte value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(decimal value)
		{
			AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(DateTime value)
		{
			AutoComplete(JsonToken.Date);
		}

		public virtual void WriteValue(Guid value)
		{
			AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(TimeSpan value)
		{
			AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(int? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(uint? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(long? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ulong? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(float? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(double? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(bool? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(short? value)
		{
			if (!((int?)value).HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ushort? value)
		{
			if (!((int?)value).HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(char? value)
		{
			if (!((int?)value).HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(byte? value)
		{
			if (!((int?)value).HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(sbyte? value)
		{
			if (!((int?)value).HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(decimal? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(DateTime? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(Guid? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(TimeSpan? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(byte[] value)
		{
			if (value == null)
			{
				WriteNull();
			}
			else
			{
				AutoComplete(JsonToken.Bytes);
			}
		}

		public virtual void WriteValue(Uri value)
		{
			if (value == null)
			{
				WriteNull();
			}
			else
			{
				AutoComplete(JsonToken.String);
			}
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				WriteNull();
				return;
			}
			if (ConvertUtils.IsConvertible(value))
			{
				IConvertible convertible = ConvertUtils.ToConvertible(value);
				switch (convertible.GetTypeCode())
				{
				case TypeCode.String:
					WriteValue(convertible.ToString(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Char:
					WriteValue(convertible.ToChar(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Boolean:
					WriteValue(convertible.ToBoolean(CultureInfo.InvariantCulture));
					return;
				case TypeCode.SByte:
					WriteValue(convertible.ToSByte(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Int16:
					WriteValue(convertible.ToInt16(CultureInfo.InvariantCulture));
					return;
				case TypeCode.UInt16:
					WriteValue(convertible.ToUInt16(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Int32:
					WriteValue(convertible.ToInt32(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Byte:
					WriteValue(convertible.ToByte(CultureInfo.InvariantCulture));
					return;
				case TypeCode.UInt32:
					WriteValue(convertible.ToUInt32(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Int64:
					WriteValue(convertible.ToInt64(CultureInfo.InvariantCulture));
					return;
				case TypeCode.UInt64:
					WriteValue(convertible.ToUInt64(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Single:
					WriteValue(convertible.ToSingle(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Double:
					WriteValue(convertible.ToDouble(CultureInfo.InvariantCulture));
					return;
				case TypeCode.DateTime:
					WriteValue(convertible.ToDateTime(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Decimal:
					WriteValue(convertible.ToDecimal(CultureInfo.InvariantCulture));
					return;
				case TypeCode.DBNull:
					WriteNull();
					return;
				}
			}
			else
			{
				if (value is byte[])
				{
					WriteValue((byte[])value);
					return;
				}
				if (value is Guid)
				{
					WriteValue((Guid)value);
					return;
				}
				if (value is Uri)
				{
					WriteValue((Uri)value);
					return;
				}
				if (value is TimeSpan)
				{
					WriteValue((TimeSpan)value);
					return;
				}
			}
			throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		public virtual void WriteComment(string text)
		{
			AutoComplete(JsonToken.Comment);
		}

		public virtual void WriteWhitespace(string ws)
		{
			if (ws != null && !StringUtils.IsWhiteSpace(ws))
			{
				throw new JsonWriterException("Only white space characters should be used.");
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (_currentState != State.Closed)
			{
				Close();
			}
		}
	}
}
