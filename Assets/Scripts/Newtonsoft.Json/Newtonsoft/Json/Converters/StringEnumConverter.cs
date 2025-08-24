using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Converters
{
	public class StringEnumConverter : JsonConverter
	{
		private readonly Dictionary<Type, BidirectionalDictionary<string, string>> _enumMemberNamesPerType = new Dictionary<Type, BidirectionalDictionary<string, string>>();

		public bool CamelCaseText { get; set; }

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Enum @enum = (Enum)value;
			string text = @enum.ToString("G");
			if (char.IsNumber(text[0]) || text[0] == '-')
			{
				writer.WriteValue(value);
				return;
			}
			BidirectionalDictionary<string, string> enumNameMap = GetEnumNameMap(@enum.GetType());
			string second;
			enumNameMap.TryGetByFirst(text, out second);
			second = second ?? text;
			if (CamelCaseText)
			{
				string[] value2 = (from item in second.Split(',')
					select StringUtils.ToCamelCase(item.Trim())).ToArray();
				second = string.Join(", ", value2);
			}
			writer.WriteValue(second);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullableType(objectType))
				{
					throw new Exception("Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
				}
				return null;
			}
			if (reader.TokenType == JsonToken.String)
			{
				BidirectionalDictionary<string, string> enumNameMap = GetEnumNameMap(type);
				string first;
				enumNameMap.TryGetBySecond(reader.Value.ToString(), out first);
				first = first ?? reader.Value.ToString();
				return Enum.Parse(type, first, true);
			}
			if (reader.TokenType == JsonToken.Integer)
			{
				return ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, type);
			}
			throw new Exception("Unexpected token when parsing enum. Expected String or Integer, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}

		private BidirectionalDictionary<string, string> GetEnumNameMap(Type t)
		{
			BidirectionalDictionary<string, string> value;
			if (!_enumMemberNamesPerType.TryGetValue(t, out value))
			{
				lock (_enumMemberNamesPerType)
				{
					if (_enumMemberNamesPerType.TryGetValue(t, out value))
					{
						return value;
					}
					value = new BidirectionalDictionary<string, string>(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
					FieldInfo[] fields = t.GetFields();
					foreach (FieldInfo fieldInfo in fields)
					{
						string name = fieldInfo.Name;
						string name2 = fieldInfo.Name;
						string first;
						if (value.TryGetBySecond(name2, out first))
						{
							throw new Exception("Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture, name2, t.Name));
						}
						value.Add(name, name2);
					}
					_enumMemberNamesPerType[t] = value;
				}
			}
			return value;
		}

		public override bool CanConvert(Type objectType)
		{
			Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
			return type.IsEnum();
		}
	}
}
