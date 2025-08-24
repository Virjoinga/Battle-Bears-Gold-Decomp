using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	public class JTokenReader : JsonReader, IJsonLineInfo
	{
		private readonly JToken _root;

		private JToken _parent;

		private JToken _current;

		private bool IsEndElement
		{
			get
			{
				return _current == _parent;
			}
		}

		int IJsonLineInfo.LineNumber
		{
			get
			{
				if (base.CurrentState == State.Start)
				{
					return 0;
				}
				IJsonLineInfo jsonLineInfo = (IsEndElement ? null : _current);
				if (jsonLineInfo != null)
				{
					return jsonLineInfo.LineNumber;
				}
				return 0;
			}
		}

		int IJsonLineInfo.LinePosition
		{
			get
			{
				if (base.CurrentState == State.Start)
				{
					return 0;
				}
				IJsonLineInfo jsonLineInfo = (IsEndElement ? null : _current);
				if (jsonLineInfo != null)
				{
					return jsonLineInfo.LinePosition;
				}
				return 0;
			}
		}

		public JTokenReader(JToken token)
		{
			ValidationUtils.ArgumentNotNull(token, "token");
			_root = token;
			_current = token;
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
			if (base.CurrentState != 0)
			{
				JContainer jContainer = _current as JContainer;
				if (jContainer != null && _parent != jContainer)
				{
					return ReadInto(jContainer);
				}
				return ReadOver(_current);
			}
			SetToken(_current);
			return true;
		}

		public override bool Read()
		{
			_readType = ReadType.Read;
			return ReadInternal();
		}

		private bool ReadOver(JToken t)
		{
			if (t == _root)
			{
				return ReadToEnd();
			}
			JToken next = t.Next;
			if (next == null || next == t || t == t.Parent.Last)
			{
				if (t.Parent == null)
				{
					return ReadToEnd();
				}
				return SetEnd(t.Parent);
			}
			_current = next;
			SetToken(_current);
			return true;
		}

		private bool ReadToEnd()
		{
			SetToken(JsonToken.None);
			return false;
		}

		private JsonToken? GetEndToken(JContainer c)
		{
			switch (c.Type)
			{
			case JTokenType.Object:
				return JsonToken.EndObject;
			case JTokenType.Array:
				return JsonToken.EndArray;
			case JTokenType.Constructor:
				return JsonToken.EndConstructor;
			case JTokenType.Property:
				return null;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", c.Type, "Unexpected JContainer type.");
			}
		}

		private bool ReadInto(JContainer c)
		{
			JToken first = c.First;
			if (first == null)
			{
				return SetEnd(c);
			}
			SetToken(first);
			_current = first;
			_parent = c;
			return true;
		}

		private bool SetEnd(JContainer c)
		{
			JsonToken? endToken = GetEndToken(c);
			if (endToken.HasValue)
			{
				SetToken(endToken.Value);
				_current = c;
				_parent = c;
				return true;
			}
			return ReadOver(c);
		}

		private void SetToken(JToken token)
		{
			switch (token.Type)
			{
			case JTokenType.Object:
				SetToken(JsonToken.StartObject);
				break;
			case JTokenType.Array:
				SetToken(JsonToken.StartArray);
				break;
			case JTokenType.Constructor:
				SetToken(JsonToken.StartConstructor);
				break;
			case JTokenType.Property:
				SetToken(JsonToken.PropertyName, ((JProperty)token).Name);
				break;
			case JTokenType.Comment:
				SetToken(JsonToken.Comment, ((JValue)token).Value);
				break;
			case JTokenType.Integer:
				SetToken(JsonToken.Integer, ((JValue)token).Value);
				break;
			case JTokenType.Float:
				SetToken(JsonToken.Float, ((JValue)token).Value);
				break;
			case JTokenType.String:
				SetToken(JsonToken.String, ((JValue)token).Value);
				break;
			case JTokenType.Boolean:
				SetToken(JsonToken.Boolean, ((JValue)token).Value);
				break;
			case JTokenType.Null:
				SetToken(JsonToken.Null, ((JValue)token).Value);
				break;
			case JTokenType.Undefined:
				SetToken(JsonToken.Undefined, ((JValue)token).Value);
				break;
			case JTokenType.Date:
				SetToken(JsonToken.Date, ((JValue)token).Value);
				break;
			case JTokenType.Raw:
				SetToken(JsonToken.Raw, ((JValue)token).Value);
				break;
			case JTokenType.Bytes:
				SetToken(JsonToken.Bytes, ((JValue)token).Value);
				break;
			case JTokenType.Guid:
				SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
				break;
			case JTokenType.Uri:
				SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
				break;
			case JTokenType.TimeSpan:
				SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
				break;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", token.Type, "Unexpected JTokenType.");
			}
		}

		private string SafeToString(object value)
		{
			if (value == null)
			{
				return null;
			}
			return value.ToString();
		}

		bool IJsonLineInfo.HasLineInfo()
		{
			if (base.CurrentState == State.Start)
			{
				return false;
			}
			IJsonLineInfo jsonLineInfo = (IsEndElement ? null : _current);
			if (jsonLineInfo != null)
			{
				return jsonLineInfo.HasLineInfo();
			}
			return false;
		}
	}
}
