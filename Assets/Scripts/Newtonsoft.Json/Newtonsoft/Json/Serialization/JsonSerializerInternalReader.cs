using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonSerializerInternalReader : JsonSerializerInternalBase
	{
		internal enum PropertyPresence
		{
			None = 0,
			Null = 1,
			Value = 2
		}

		private JsonSerializerProxy _internalSerializer;

		private JsonFormatterConverter _formatterConverter;

		public JsonSerializerInternalReader(JsonSerializer serializer)
			: base(serializer)
		{
		}

		public void Populate(JsonReader reader, object target)
		{
			ValidationUtils.ArgumentNotNull(target, "target");
			Type type = target.GetType();
			JsonContract jsonContract = Serializer.ContractResolver.ResolveContract(type);
			if (reader.TokenType == JsonToken.None)
			{
				reader.Read();
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				if (jsonContract.ContractType == JsonContractType.Array)
				{
					PopulateList(CollectionUtils.CreateCollectionWrapper(target), reader, null, (JsonArrayContract)jsonContract);
					return;
				}
				throw CreateSerializationException(reader, "Cannot populate JSON array onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
			}
			if (reader.TokenType == JsonToken.StartObject)
			{
				CheckedRead(reader);
				string id = null;
				if (reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$id", StringComparison.Ordinal))
				{
					CheckedRead(reader);
					id = ((reader.Value != null) ? reader.Value.ToString() : null);
					CheckedRead(reader);
				}
				if (jsonContract.ContractType == JsonContractType.Dictionary)
				{
					PopulateDictionary(CollectionUtils.CreateDictionaryWrapper(target), reader, (JsonDictionaryContract)jsonContract, id);
					return;
				}
				if (jsonContract.ContractType == JsonContractType.Object)
				{
					PopulateObject(target, reader, (JsonObjectContract)jsonContract, id);
					return;
				}
				throw CreateSerializationException(reader, "Cannot populate JSON object onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
			}
			throw CreateSerializationException(reader, "Unexpected initial token '{0}' when populating object. Expected JSON object or array.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}

		private JsonContract GetContractSafe(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return Serializer.ContractResolver.ResolveContract(type);
		}

		public object Deserialize(JsonReader reader, Type objectType)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			JsonContract contractSafe = GetContractSafe(objectType);
			JsonConverter converter = GetConverter(contractSafe, null);
			if (reader.TokenType == JsonToken.None && !ReadForType(reader, contractSafe, converter != null, false))
			{
				if (!contractSafe.IsNullable)
				{
					throw new JsonSerializationException("No JSON content found and type '{0}' is not nullable.".FormatWith(CultureInfo.InvariantCulture, contractSafe.UnderlyingType));
				}
				return null;
			}
			return CreateValueNonProperty(reader, objectType, contractSafe, converter);
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (_internalSerializer == null)
			{
				_internalSerializer = new JsonSerializerProxy(this);
			}
			return _internalSerializer;
		}

		private JsonFormatterConverter GetFormatterConverter()
		{
			if (_formatterConverter == null)
			{
				_formatterConverter = new JsonFormatterConverter(GetInternalSerializer());
			}
			return _formatterConverter;
		}

		private JToken CreateJToken(JsonReader reader, JsonContract contract)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (contract != null && contract.UnderlyingType == typeof(JRaw))
			{
				return JRaw.Create(reader);
			}
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jTokenWriter.WriteToken(reader);
				return jTokenWriter.Token;
			}
		}

		private JToken CreateJObject(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jTokenWriter.WriteStartObject();
				if (reader.TokenType == JsonToken.PropertyName)
				{
					jTokenWriter.WriteToken(reader, reader.Depth - 1);
				}
				else
				{
					jTokenWriter.WriteEndObject();
				}
				return jTokenWriter.Token;
			}
		}

		private object CreateValueProperty(JsonReader reader, JsonProperty property, JsonConverter propertyConverter, object target, bool gottenCurrentValue, object currentValue)
		{
			if (property.PropertyContract == null)
			{
				property.PropertyContract = GetContractSafe(property.PropertyType);
			}
			JsonContract jsonContract;
			JsonConverter jsonConverter;
			if (currentValue == null)
			{
				jsonContract = property.PropertyContract;
				jsonConverter = propertyConverter;
			}
			else
			{
				jsonContract = GetContractSafe(currentValue.GetType());
				jsonConverter = ((jsonContract == property.PropertyContract) ? propertyConverter : GetConverter(jsonContract, property.MemberConverter));
			}
			Type propertyType = property.PropertyType;
			if (jsonConverter != null && jsonConverter.CanRead)
			{
				if (!gottenCurrentValue && target != null && property.Readable)
				{
					currentValue = property.ValueProvider.GetValue(target);
				}
				return jsonConverter.ReadJson(reader, propertyType, currentValue, GetInternalSerializer());
			}
			return CreateValueInternal(reader, propertyType, jsonContract, property, currentValue);
		}

		private object CreateValueNonProperty(JsonReader reader, Type objectType, JsonContract contract, JsonConverter converter)
		{
			if (converter != null && converter.CanRead)
			{
				return converter.ReadJson(reader, objectType, null, GetInternalSerializer());
			}
			return CreateValueInternal(reader, objectType, contract, null, null);
		}

		private object CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue)
		{
			if (contract != null && contract.ContractType == JsonContractType.Linq)
			{
				return CreateJToken(reader, contract);
			}
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.StartObject:
					return CreateObject(reader, objectType, contract, member, existingValue);
				case JsonToken.StartArray:
					return CreateList(reader, objectType, contract, member, existingValue, null);
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.Boolean:
				case JsonToken.Date:
				case JsonToken.Bytes:
					return EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
				case JsonToken.String:
					if (string.IsNullOrEmpty((string)reader.Value) && objectType != typeof(string) && objectType != typeof(object) && contract.IsNullable)
					{
						return null;
					}
					if (objectType == typeof(byte[]))
					{
						return Convert.FromBase64String((string)reader.Value);
					}
					return EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
				case JsonToken.StartConstructor:
				case JsonToken.EndConstructor:
					return reader.Value.ToString();
				case JsonToken.Null:
				case JsonToken.Undefined:
					if (objectType == typeof(DBNull))
					{
						return DBNull.Value;
					}
					return EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
				case JsonToken.Raw:
					return new JRaw((string)reader.Value);
				default:
					throw CreateSerializationException(reader, "Unexpected token while deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (reader.Read());
			throw CreateSerializationException(reader, "Unexpected end when deserializing object.");
		}

		private JsonSerializationException CreateSerializationException(JsonReader reader, string message)
		{
			return CreateSerializationException(reader, message, null);
		}

		private JsonSerializationException CreateSerializationException(JsonReader reader, string message, Exception ex)
		{
			return CreateSerializationException(reader as IJsonLineInfo, message, ex);
		}

		private JsonSerializationException CreateSerializationException(IJsonLineInfo lineInfo, string message, Exception ex)
		{
			message = JsonReader.FormatExceptionMessage(lineInfo, message);
			return new JsonSerializationException(message, ex);
		}

		private JsonConverter GetConverter(JsonContract contract, JsonConverter memberConverter)
		{
			JsonConverter result = null;
			if (memberConverter != null)
			{
				result = memberConverter;
			}
			else if (contract != null)
			{
				JsonConverter matchingConverter;
				if (contract.Converter != null)
				{
					result = contract.Converter;
				}
				else if ((matchingConverter = Serializer.GetMatchingConverter(contract.UnderlyingType)) != null)
				{
					result = matchingConverter;
				}
				else if (contract.InternalConverter != null)
				{
					result = contract.InternalConverter;
				}
			}
			return result;
		}

		private object CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue)
		{
			CheckedRead(reader);
			string text = null;
			if (reader.TokenType == JsonToken.PropertyName)
			{
				string text2 = reader.Value.ToString();
				if (text2.Length > 0 && text2[0] == '$')
				{
					bool flag;
					do
					{
						text2 = reader.Value.ToString();
						if (string.Equals(text2, "$ref", StringComparison.Ordinal))
						{
							CheckedRead(reader);
							if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null)
							{
								throw CreateSerializationException(reader, "JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
							}
							string text3 = ((reader.Value != null) ? reader.Value.ToString() : null);
							CheckedRead(reader);
							if (text3 != null)
							{
								if (reader.TokenType == JsonToken.PropertyName)
								{
									throw CreateSerializationException(reader, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
								}
								return Serializer.ReferenceResolver.ResolveReference(this, text3);
							}
							flag = true;
						}
						else if (string.Equals(text2, "$type", StringComparison.Ordinal))
						{
							CheckedRead(reader);
							string text4 = reader.Value.ToString();
							if ((((member != null) ? member.TypeNameHandling : null) ?? Serializer.TypeNameHandling) != 0)
							{
								string typeName;
								string assemblyName;
								ReflectionUtils.SplitFullyQualifiedTypeName(text4, out typeName, out assemblyName);
								Type type;
								try
								{
									type = Serializer.Binder.BindToType(assemblyName, typeName);
								}
								catch (Exception ex)
								{
									throw CreateSerializationException(reader, "Error resolving type specified in JSON '{0}'.".FormatWith(CultureInfo.InvariantCulture, text4), ex);
								}
								if (type == null)
								{
									throw CreateSerializationException(reader, "Type specified in JSON '{0}' was not resolved.".FormatWith(CultureInfo.InvariantCulture, text4));
								}
								if (objectType != null && !objectType.IsAssignableFrom(type))
								{
									throw CreateSerializationException(reader, "Type specified in JSON '{0}' is not compatible with '{1}'.".FormatWith(CultureInfo.InvariantCulture, type.AssemblyQualifiedName, objectType.AssemblyQualifiedName));
								}
								objectType = type;
								contract = GetContractSafe(type);
							}
							CheckedRead(reader);
							flag = true;
						}
						else if (string.Equals(text2, "$id", StringComparison.Ordinal))
						{
							CheckedRead(reader);
							text = ((reader.Value != null) ? reader.Value.ToString() : null);
							CheckedRead(reader);
							flag = true;
						}
						else
						{
							if (string.Equals(text2, "$values", StringComparison.Ordinal))
							{
								CheckedRead(reader);
								object result = CreateList(reader, objectType, contract, member, existingValue, text);
								CheckedRead(reader);
								return result;
							}
							flag = false;
						}
					}
					while (flag && reader.TokenType == JsonToken.PropertyName);
				}
			}
			if (!HasDefinedType(objectType))
			{
				return CreateJObject(reader);
			}
			if (contract == null)
			{
				throw CreateSerializationException(reader, "Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			switch (contract.ContractType)
			{
			case JsonContractType.Object:
			{
				JsonObjectContract contract4 = (JsonObjectContract)contract;
				if (existingValue == null)
				{
					return CreateAndPopulateObject(reader, contract4, text);
				}
				return PopulateObject(existingValue, reader, contract4, text);
			}
			case JsonContractType.Primitive:
			{
				JsonPrimitiveContract contract3 = (JsonPrimitiveContract)contract;
				if (reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$value", StringComparison.Ordinal))
				{
					CheckedRead(reader);
					object result2 = CreateValueInternal(reader, objectType, contract3, member, existingValue);
					CheckedRead(reader);
					return result2;
				}
				break;
			}
			case JsonContractType.Dictionary:
			{
				JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)contract;
				if (existingValue == null)
				{
					return CreateAndPopulateDictionary(reader, jsonDictionaryContract, text);
				}
				return PopulateDictionary(jsonDictionaryContract.CreateWrapper(existingValue), reader, jsonDictionaryContract, text);
			}
			case JsonContractType.Serializable:
			{
				JsonISerializableContract contract2 = (JsonISerializableContract)contract;
				return CreateISerializable(reader, contract2, text);
			}
			}
			throw CreateSerializationException(reader, "Cannot deserialize JSON object (i.e. {{\"name\":\"value\"}}) into type '{0}'.\r\nThe deserialized type should be a normal .NET type (i.e. not a primitive type like integer, not a collection type like an array or List<T>) or a dictionary type (i.e. Dictionary<TKey, TValue>).\r\nTo force JSON objects to deserialize add the JsonObjectAttribute to the type.".FormatWith(CultureInfo.InvariantCulture, objectType));
		}

		private JsonArrayContract EnsureArrayContract(JsonReader reader, Type objectType, JsonContract contract)
		{
			if (contract == null)
			{
				throw CreateSerializationException(reader, "Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			JsonArrayContract jsonArrayContract = contract as JsonArrayContract;
			if (jsonArrayContract == null)
			{
				throw CreateSerializationException(reader, "Cannot deserialize JSON array (i.e. [1,2,3]) into type '{0}'.\r\nThe deserialized type must be an array or implement a collection interface like IEnumerable, ICollection or IList.\r\nTo force JSON arrays to deserialize add the JsonArrayAttribute to the type.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			return jsonArrayContract;
		}

		private void CheckedRead(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw CreateSerializationException(reader, "Unexpected end when deserializing object.");
			}
		}

		private object CreateList(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue, string reference)
		{
			if (HasDefinedType(objectType))
			{
				JsonArrayContract jsonArrayContract = EnsureArrayContract(reader, objectType, contract);
				if (existingValue == null)
				{
					return CreateAndPopulateList(reader, reference, jsonArrayContract);
				}
				return PopulateList(jsonArrayContract.CreateWrapper(existingValue), reader, reference, jsonArrayContract);
			}
			return CreateJToken(reader, contract);
		}

		private bool HasDefinedType(Type type)
		{
			if (type != null && type != typeof(object))
			{
				return !typeof(JToken).IsSubclassOf(type);
			}
			return false;
		}

		private object EnsureType(JsonReader reader, object value, CultureInfo culture, JsonContract contract, Type targetType)
		{
			if (targetType == null)
			{
				return value;
			}
			Type objectType = ReflectionUtils.GetObjectType(value);
			if (objectType != targetType)
			{
				try
				{
					if (value == null && contract.IsNullable)
					{
						return null;
					}
					if (contract.IsConvertable)
					{
						if (contract.NonNullableUnderlyingType.IsEnum())
						{
							if (value is string)
							{
								return Enum.Parse(contract.NonNullableUnderlyingType, value.ToString(), true);
							}
							if (ConvertUtils.IsInteger(value))
							{
								return Enum.ToObject(contract.NonNullableUnderlyingType, value);
							}
						}
						return Convert.ChangeType(value, contract.NonNullableUnderlyingType, culture);
					}
					return ConvertUtils.ConvertOrCast(value, culture, contract.NonNullableUnderlyingType);
				}
				catch (Exception ex)
				{
					throw CreateSerializationException(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, FormatValueForPrint(value), targetType), ex);
				}
			}
			return value;
		}

		private string FormatValueForPrint(object value)
		{
			if (value == null)
			{
				return "{null}";
			}
			if (value is string)
			{
				return string.Concat("\"", value, "\"");
			}
			return value.ToString();
		}

		private void SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonReader reader, object target)
		{
			if (property.Ignored)
			{
				reader.Skip();
				return;
			}
			object obj = null;
			bool flag = false;
			bool gottenCurrentValue = false;
			ObjectCreationHandling valueOrDefault = property.ObjectCreationHandling.GetValueOrDefault(Serializer.ObjectCreationHandling);
			if ((valueOrDefault == ObjectCreationHandling.Auto || valueOrDefault == ObjectCreationHandling.Reuse) && (reader.TokenType == JsonToken.StartArray || reader.TokenType == JsonToken.StartObject) && property.Readable)
			{
				obj = property.ValueProvider.GetValue(target);
				gottenCurrentValue = true;
				flag = obj != null && !property.PropertyType.IsArray && !ReflectionUtils.InheritsGenericDefinition(property.PropertyType, typeof(ReadOnlyCollection<>)) && !property.PropertyType.IsValueType();
			}
			if (!property.Writable && !flag)
			{
				reader.Skip();
				return;
			}
			if (property.NullValueHandling.GetValueOrDefault(Serializer.NullValueHandling) == NullValueHandling.Ignore && reader.TokenType == JsonToken.Null)
			{
				reader.Skip();
				return;
			}
			if (HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) && JsonReader.IsPrimitiveToken(reader.TokenType) && MiscellaneousUtils.ValueEquals(reader.Value, property.DefaultValue))
			{
				reader.Skip();
				return;
			}
			object currentValue = (flag ? obj : null);
			object obj2 = CreateValueProperty(reader, property, propertyConverter, target, gottenCurrentValue, currentValue);
			if ((!flag || obj2 != obj) && ShouldSetPropertyValue(property, obj2))
			{
				property.ValueProvider.SetValue(target, obj2);
				if (property.SetIsSpecified != null)
				{
					property.SetIsSpecified(target, true);
				}
			}
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool ShouldSetPropertyValue(JsonProperty property, object value)
		{
			if (property.NullValueHandling.GetValueOrDefault(Serializer.NullValueHandling) == NullValueHandling.Ignore && value == null)
			{
				return false;
			}
			if (HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) && MiscellaneousUtils.ValueEquals(value, property.DefaultValue))
			{
				return false;
			}
			if (!property.Writable)
			{
				return false;
			}
			return true;
		}

		private object CreateAndPopulateDictionary(JsonReader reader, JsonDictionaryContract contract, string id)
		{
			if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || Serializer.ConstructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				object dictionary = contract.DefaultCreator();
				IWrappedDictionary wrappedDictionary = contract.CreateWrapper(dictionary);
				PopulateDictionary(wrappedDictionary, reader, contract, id);
				return wrappedDictionary.UnderlyingDictionary;
			}
			throw CreateSerializationException(reader, "Unable to find a default constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
		}

		private object PopulateDictionary(IWrappedDictionary dictionary, JsonReader reader, JsonDictionaryContract contract, string id)
		{
			if (id != null)
			{
				Serializer.ReferenceResolver.AddReference(this, id, dictionary.UnderlyingDictionary);
			}
			contract.InvokeOnDeserializing(dictionary.UnderlyingDictionary, Serializer.Context);
			int depth = reader.Depth;
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.PropertyName:
				{
					object obj = reader.Value;
					try
					{
						if (contract.DictionaryKeyContract == null)
						{
							contract.DictionaryKeyContract = GetContractSafe(contract.DictionaryKeyType);
						}
						try
						{
							obj = EnsureType(reader, obj, CultureInfo.InvariantCulture, contract.DictionaryKeyContract, contract.DictionaryKeyType);
						}
						catch (Exception ex)
						{
							throw CreateSerializationException(reader, "Could not convert string '{0}' to dictionary key type '{1}'. Create a TypeConverter to convert from the string to the key type object.".FormatWith(CultureInfo.InvariantCulture, reader.Value, contract.DictionaryKeyType), ex);
						}
						if (contract.DictionaryValueContract == null)
						{
							contract.DictionaryValueContract = GetContractSafe(contract.DictionaryValueType);
						}
						JsonConverter converter = GetConverter(contract.DictionaryValueContract, null);
						if (!ReadForType(reader, contract.DictionaryValueContract, converter != null, false))
						{
							throw CreateSerializationException(reader, "Unexpected end when deserializing object.");
						}
						dictionary[obj] = CreateValueNonProperty(reader, contract.DictionaryValueType, contract.DictionaryValueContract, converter);
					}
					catch (Exception ex2)
					{
						if (IsErrorHandled(dictionary, contract, obj, reader.Path, ex2))
						{
							HandleError(reader, depth);
							break;
						}
						throw;
					}
					break;
				}
				case JsonToken.EndObject:
					contract.InvokeOnDeserialized(dictionary.UnderlyingDictionary, Serializer.Context);
					return dictionary.UnderlyingDictionary;
				default:
					throw CreateSerializationException(reader, "Unexpected token when deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (reader.Read());
			throw CreateSerializationException(reader, "Unexpected end when deserializing object.");
		}

		private object CreateAndPopulateList(JsonReader reader, string reference, JsonArrayContract contract)
		{
			return CollectionUtils.CreateAndPopulateList(contract.CreatedType, delegate(IList l, bool isTemporaryListReference)
			{
				if (reference != null && isTemporaryListReference)
				{
					throw CreateSerializationException(reader, "Cannot preserve reference to array or readonly list: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				if (contract.OnSerializing != null && isTemporaryListReference)
				{
					throw CreateSerializationException(reader, "Cannot call OnSerializing on an array or readonly list: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				if (contract.OnError != null && isTemporaryListReference)
				{
					throw CreateSerializationException(reader, "Cannot call OnError on an array or readonly list: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				PopulateList(contract.CreateWrapper(l), reader, reference, contract);
			});
		}

		private object PopulateList(IWrappedCollection wrappedList, JsonReader reader, string reference, JsonArrayContract contract)
		{
			object underlyingCollection = wrappedList.UnderlyingCollection;
			if (wrappedList.IsFixedSize)
			{
				reader.Skip();
				return wrappedList.UnderlyingCollection;
			}
			if (reference != null)
			{
				Serializer.ReferenceResolver.AddReference(this, reference, underlyingCollection);
			}
			contract.InvokeOnDeserializing(underlyingCollection, Serializer.Context);
			int depth = reader.Depth;
			int num = 0;
			JsonContract contractSafe = GetContractSafe(contract.CollectionItemType);
			JsonConverter converter = GetConverter(contractSafe, null);
			while (true)
			{
				try
				{
					if (ReadForType(reader, contractSafe, converter != null, true))
					{
						switch (reader.TokenType)
						{
						case JsonToken.Comment:
							break;
						case JsonToken.EndArray:
							contract.InvokeOnDeserialized(underlyingCollection, Serializer.Context);
							return wrappedList.UnderlyingCollection;
						default:
						{
							object value = CreateValueNonProperty(reader, contract.CollectionItemType, contractSafe, converter);
							wrappedList.Add(value);
							break;
						}
						}
						continue;
					}
				}
				catch (Exception ex)
				{
					if (IsErrorHandled(underlyingCollection, contract, num, reader.Path, ex))
					{
						HandleError(reader, depth);
						continue;
					}
					throw;
				}
				finally
				{
					num++;
				}
				break;
			}
			throw CreateSerializationException(reader, "Unexpected end when deserializing array.");
		}

		private object CreateISerializable(JsonReader reader, JsonISerializableContract contract, string id)
		{
			Type underlyingType = contract.UnderlyingType;
			if (!JsonTypeReflector.FullyTrusted)
			{
				throw new JsonSerializationException("Type '{0}' implements ISerializable but cannot be deserialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data.\r\nTo fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add to JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true.".FormatWith(CultureInfo.InvariantCulture, underlyingType));
			}
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, GetFormatterConverter());
			bool flag = false;
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.PropertyName:
				{
					string text = reader.Value.ToString();
					if (!reader.Read())
					{
						throw CreateSerializationException(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
					}
					serializationInfo.AddValue(text, JToken.ReadFrom(reader));
					break;
				}
				case JsonToken.EndObject:
					flag = true;
					break;
				default:
					throw CreateSerializationException(reader, "Unexpected token when deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (!flag && reader.Read());
			if (contract.ISerializableCreator == null)
			{
				throw CreateSerializationException(reader, "ISerializable type '{0}' does not have a valid constructor. To correctly implement ISerializable a constructor that takes SerializationInfo and StreamingContext parameters should be present.".FormatWith(CultureInfo.InvariantCulture, underlyingType));
			}
			object obj = contract.ISerializableCreator(serializationInfo, Serializer.Context);
			if (id != null)
			{
				Serializer.ReferenceResolver.AddReference(this, id, obj);
			}
			contract.InvokeOnDeserializing(obj, Serializer.Context);
			contract.InvokeOnDeserialized(obj, Serializer.Context);
			return obj;
		}

		private object CreateAndPopulateObject(JsonReader reader, JsonObjectContract contract, string id)
		{
			object obj = null;
			if (contract.UnderlyingType.IsInterface() || contract.UnderlyingType.IsAbstract())
			{
				throw CreateSerializationException(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
			if (contract.OverrideConstructor != null)
			{
				if (contract.OverrideConstructor.GetParameters().Length > 0)
				{
					return CreateObjectFromNonDefaultConstructor(reader, contract, contract.OverrideConstructor, id);
				}
				obj = contract.OverrideConstructor.Invoke(null);
			}
			else if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || Serializer.ConstructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor || contract.ParametrizedConstructor == null))
			{
				obj = contract.DefaultCreator();
			}
			else if (contract.ParametrizedConstructor != null)
			{
				return CreateObjectFromNonDefaultConstructor(reader, contract, contract.ParametrizedConstructor, id);
			}
			if (obj == null)
			{
				throw CreateSerializationException(reader, "Unable to find a constructor to use for type {0}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
			PopulateObject(obj, reader, contract, id);
			return obj;
		}

		private object CreateObjectFromNonDefaultConstructor(JsonReader reader, JsonObjectContract contract, ConstructorInfo constructorInfo, string id)
		{
			ValidationUtils.ArgumentNotNull(constructorInfo, "constructorInfo");
			Type underlyingType = contract.UnderlyingType;
			IDictionary<JsonProperty, object> dictionary = ResolvePropertyAndConstructorValues(contract, reader, underlyingType);
			IDictionary<ParameterInfo, object> dictionary2 = ((IEnumerable<ParameterInfo>)constructorInfo.GetParameters()).ToDictionary((Func<ParameterInfo, ParameterInfo>)((ParameterInfo p) => p), (Func<ParameterInfo, object>)((ParameterInfo p) => null));
			IDictionary<JsonProperty, object> dictionary3 = new Dictionary<JsonProperty, object>();
			foreach (KeyValuePair<JsonProperty, object> item in dictionary)
			{
				ParameterInfo key = dictionary2.ForgivingCaseSensitiveFind((KeyValuePair<ParameterInfo, object> kv) => kv.Key.Name, item.Key.UnderlyingName).Key;
				if (key != null)
				{
					dictionary2[key] = item.Value;
				}
				else
				{
					dictionary3.Add(item);
				}
			}
			object obj = constructorInfo.Invoke(dictionary2.Values.ToArray());
			if (id != null)
			{
				Serializer.ReferenceResolver.AddReference(this, id, obj);
			}
			contract.InvokeOnDeserializing(obj, Serializer.Context);
			foreach (KeyValuePair<JsonProperty, object> item2 in dictionary3)
			{
				JsonProperty key2 = item2.Key;
				object value = item2.Value;
				if (ShouldSetPropertyValue(item2.Key, item2.Value))
				{
					key2.ValueProvider.SetValue(obj, value);
				}
				else
				{
					if (key2.Writable || value == null)
					{
						continue;
					}
					JsonContract jsonContract = Serializer.ContractResolver.ResolveContract(key2.PropertyType);
					if (jsonContract.ContractType == JsonContractType.Array)
					{
						JsonArrayContract jsonArrayContract = jsonContract as JsonArrayContract;
						object value2 = key2.ValueProvider.GetValue(obj);
						if (value2 == null)
						{
							continue;
						}
						IWrappedCollection wrappedCollection = jsonArrayContract.CreateWrapper(value2);
						IWrappedCollection wrappedCollection2 = jsonArrayContract.CreateWrapper(value);
						foreach (object item3 in wrappedCollection2)
						{
							wrappedCollection.Add(item3);
						}
					}
					else
					{
						if (jsonContract.ContractType != JsonContractType.Dictionary)
						{
							continue;
						}
						JsonDictionaryContract jsonDictionaryContract = jsonContract as JsonDictionaryContract;
						object value3 = key2.ValueProvider.GetValue(obj);
						if (value3 == null)
						{
							continue;
						}
						IWrappedDictionary wrappedDictionary = jsonDictionaryContract.CreateWrapper(value3);
						IWrappedDictionary wrappedDictionary2 = jsonDictionaryContract.CreateWrapper(value);
						foreach (DictionaryEntry item4 in wrappedDictionary2)
						{
							wrappedDictionary.Add(item4.Key, item4.Value);
						}
					}
				}
			}
			contract.InvokeOnDeserialized(obj, Serializer.Context);
			return obj;
		}

		private IDictionary<JsonProperty, object> ResolvePropertyAndConstructorValues(JsonObjectContract contract, JsonReader reader, Type objectType)
		{
			IDictionary<JsonProperty, object> dictionary = new Dictionary<JsonProperty, object>();
			bool flag = false;
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.PropertyName:
				{
					string text = reader.Value.ToString();
					JsonProperty jsonProperty = contract.ConstructorParameters.GetClosestMatchProperty(text) ?? contract.Properties.GetClosestMatchProperty(text);
					if (jsonProperty != null)
					{
						if (jsonProperty.PropertyContract == null)
						{
							jsonProperty.PropertyContract = GetContractSafe(jsonProperty.PropertyType);
						}
						JsonConverter converter = GetConverter(jsonProperty.PropertyContract, jsonProperty.MemberConverter);
						if (!ReadForType(reader, jsonProperty.PropertyContract, converter != null, false))
						{
							throw CreateSerializationException(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
						}
						if (!jsonProperty.Ignored)
						{
							dictionary[jsonProperty] = CreateValueProperty(reader, jsonProperty, converter, null, true, null);
						}
						else
						{
							reader.Skip();
						}
					}
					else
					{
						if (!reader.Read())
						{
							throw CreateSerializationException(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
						}
						if (Serializer.MissingMemberHandling == MissingMemberHandling.Error)
						{
							throw CreateSerializationException(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, text, objectType.Name));
						}
						reader.Skip();
					}
					break;
				}
				case JsonToken.EndObject:
					flag = true;
					break;
				default:
					throw CreateSerializationException(reader, "Unexpected token when deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (!flag && reader.Read());
			return dictionary;
		}

		private bool ReadForType(JsonReader reader, JsonContract contract, bool hasConverter, bool inArray)
		{
			if (hasConverter)
			{
				return reader.Read();
			}
			switch ((contract != null) ? contract.InternalReadType : ReadType.Read)
			{
			case ReadType.Read:
				do
				{
					if (!reader.Read())
					{
						return false;
					}
				}
				while (reader.TokenType == JsonToken.Comment);
				return true;
			case ReadType.ReadAsInt32:
				reader.ReadAsInt32();
				break;
			case ReadType.ReadAsDecimal:
				reader.ReadAsDecimal();
				break;
			case ReadType.ReadAsBytes:
				reader.ReadAsBytes();
				break;
			case ReadType.ReadAsString:
				reader.ReadAsString();
				break;
			case ReadType.ReadAsDateTime:
				reader.ReadAsDateTime();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return reader.TokenType != JsonToken.None;
		}

		private object PopulateObject(object newObject, JsonReader reader, JsonObjectContract contract, string id)
		{
			contract.InvokeOnDeserializing(newObject, Serializer.Context);
			Dictionary<JsonProperty, PropertyPresence> dictionary = contract.Properties.ToDictionary((JsonProperty m) => m, (JsonProperty m) => PropertyPresence.None);
			if (id != null)
			{
				Serializer.ReferenceResolver.AddReference(this, id, newObject);
			}
			int depth = reader.Depth;
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.PropertyName:
				{
					string text = reader.Value.ToString();
					try
					{
						JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(text);
						if (closestMatchProperty == null)
						{
							if (Serializer.MissingMemberHandling == MissingMemberHandling.Error)
							{
								throw CreateSerializationException(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, text, contract.UnderlyingType.Name));
							}
							reader.Skip();
							break;
						}
						if (closestMatchProperty.PropertyContract == null)
						{
							closestMatchProperty.PropertyContract = GetContractSafe(closestMatchProperty.PropertyType);
						}
						JsonConverter converter = GetConverter(closestMatchProperty.PropertyContract, closestMatchProperty.MemberConverter);
						if (!ReadForType(reader, closestMatchProperty.PropertyContract, converter != null, false))
						{
							throw CreateSerializationException(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
						}
						SetPropertyPresence(reader, closestMatchProperty, dictionary);
						SetPropertyValue(closestMatchProperty, converter, reader, newObject);
					}
					catch (Exception ex2)
					{
						if (IsErrorHandled(newObject, contract, text, reader.Path, ex2))
						{
							HandleError(reader, depth);
							break;
						}
						throw;
					}
					break;
				}
				case JsonToken.EndObject:
					foreach (KeyValuePair<JsonProperty, PropertyPresence> item in dictionary)
					{
						JsonProperty key = item.Key;
						PropertyPresence value = item.Value;
						if (value != 0 && value != PropertyPresence.Null)
						{
							continue;
						}
						try
						{
							switch (value)
							{
							case PropertyPresence.None:
								if (key.Required == Required.AllowNull || key.Required == Required.Always)
								{
									throw CreateSerializationException(reader, "Required property '{0}' not found in JSON.".FormatWith(CultureInfo.InvariantCulture, key.PropertyName));
								}
								if (key.PropertyContract == null)
								{
									key.PropertyContract = GetContractSafe(key.PropertyType);
								}
								if (HasFlag(key.DefaultValueHandling.GetValueOrDefault(Serializer.DefaultValueHandling), DefaultValueHandling.Populate) && key.Writable)
								{
									key.ValueProvider.SetValue(newObject, EnsureType(reader, key.DefaultValue, CultureInfo.InvariantCulture, key.PropertyContract, key.PropertyType));
								}
								break;
							case PropertyPresence.Null:
								if (key.Required == Required.Always)
								{
									throw CreateSerializationException(reader, "Required property '{0}' expects a value but got null.".FormatWith(CultureInfo.InvariantCulture, key.PropertyName));
								}
								break;
							}
						}
						catch (Exception ex)
						{
							if (IsErrorHandled(newObject, contract, key.PropertyName, reader.Path, ex))
							{
								HandleError(reader, depth);
								continue;
							}
							throw;
						}
					}
					contract.InvokeOnDeserialized(newObject, Serializer.Context);
					return newObject;
				default:
					throw CreateSerializationException(reader, "Unexpected token when deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (reader.Read());
			throw CreateSerializationException(reader, "Unexpected end when deserializing object.");
		}

		private void SetPropertyPresence(JsonReader reader, JsonProperty property, Dictionary<JsonProperty, PropertyPresence> requiredProperties)
		{
			if (property != null)
			{
				requiredProperties[property] = ((reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.Undefined) ? PropertyPresence.Null : PropertyPresence.Value);
			}
		}

		private void HandleError(JsonReader reader, int initialDepth)
		{
			ClearErrorContext();
			reader.Skip();
			while (reader.Depth > initialDepth + 1)
			{
				reader.Read();
			}
		}
	}
}
