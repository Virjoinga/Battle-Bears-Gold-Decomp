using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	public class KeyValuePairConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Type type = value.GetType();
			PropertyInfo property = type.GetProperty("Key");
			PropertyInfo property2 = type.GetProperty("Value");
			writer.WriteStartObject();
			writer.WritePropertyName("Key");
			serializer.Serialize(writer, ReflectionUtils.GetMemberValue(property, value));
			writer.WritePropertyName("Value");
			serializer.Serialize(writer, ReflectionUtils.GetMemberValue(property2, value));
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool flag = ReflectionUtils.IsNullableType(objectType);
			if (reader.TokenType == JsonToken.Null)
			{
				if (!flag)
				{
					throw new Exception("Could not deserialize Null to KeyValuePair.");
				}
				return null;
			}
			Type type = (flag ? Nullable.GetUnderlyingType(objectType) : objectType);
			IList<Type> genericArguments = type.GetGenericArguments();
			Type objectType2 = genericArguments[0];
			Type objectType3 = genericArguments[1];
			object obj = null;
			object obj2 = null;
			reader.Read();
			while (reader.TokenType == JsonToken.PropertyName)
			{
				switch (reader.Value.ToString())
				{
				case "Key":
					reader.Read();
					obj = serializer.Deserialize(reader, objectType2);
					break;
				case "Value":
					reader.Read();
					obj2 = serializer.Deserialize(reader, objectType3);
					break;
				default:
					reader.Skip();
					break;
				}
				reader.Read();
			}
			return ReflectionUtils.CreateInstance(type, obj, obj2);
		}

		public override bool CanConvert(Type objectType)
		{
			Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
			if (type.IsValueType() && type.IsGenericType())
			{
				return type.GetGenericTypeDefinition() == typeof(KeyValuePair<, >);
			}
			return false;
		}
	}
}
