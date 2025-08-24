using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	public class BinaryConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			byte[] byteArray = GetByteArray(value);
			writer.WriteValue(byteArray);
		}

		private byte[] GetByteArray(object value)
		{
			if (value is SqlBinary)
			{
				return ((SqlBinary)value).Value;
			}
			throw new Exception("Unexpected value type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullable(objectType))
				{
					throw new Exception("Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
				}
				return null;
			}
			byte[] value;
			if (reader.TokenType == JsonToken.StartArray)
			{
				value = ReadByteArray(reader);
			}
			else
			{
				if (reader.TokenType != JsonToken.String)
				{
					throw new Exception("Unexpected token parsing binary. Expected String or StartArray, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				string s = reader.Value.ToString();
				value = Convert.FromBase64String(s);
			}
			if (type == typeof(SqlBinary))
			{
				return new SqlBinary(value);
			}
			throw new Exception("Unexpected object type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, objectType));
		}

		private byte[] ReadByteArray(JsonReader reader)
		{
			List<byte> list = new List<byte>();
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case JsonToken.Integer:
					list.Add(Convert.ToByte(reader.Value, CultureInfo.InvariantCulture));
					break;
				case JsonToken.EndArray:
					return list.ToArray();
				default:
					throw new Exception("Unexpected token when reading bytes: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				case JsonToken.Comment:
					break;
				}
			}
			throw new Exception("Unexpected end when reading bytes.");
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(SqlBinary) || objectType == typeof(SqlBinary?))
			{
				return true;
			}
			return false;
		}
	}
}
