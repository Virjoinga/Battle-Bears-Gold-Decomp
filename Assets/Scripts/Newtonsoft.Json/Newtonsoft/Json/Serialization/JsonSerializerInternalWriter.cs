using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonSerializerInternalWriter : JsonSerializerInternalBase
	{
		private readonly List<object> _serializeStack = new List<object>();

		private JsonSerializerProxy _internalSerializer;

		public JsonSerializerInternalWriter(JsonSerializer serializer)
			: base(serializer)
		{
		}

		public void Serialize(JsonWriter jsonWriter, object value)
		{
			if (jsonWriter == null)
			{
				throw new ArgumentNullException("jsonWriter");
			}
			SerializeValue(jsonWriter, value, GetContractSafe(value), null, null);
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (_internalSerializer == null)
			{
				_internalSerializer = new JsonSerializerProxy(this);
			}
			return _internalSerializer;
		}

		private JsonContract GetContractSafe(object value)
		{
			if (value == null)
			{
				return null;
			}
			return Serializer.ContractResolver.ResolveContract(value.GetType());
		}

		private void SerializePrimitive(JsonWriter writer, object value, JsonPrimitiveContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			if (contract.UnderlyingType == typeof(byte[]) && ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				writer.WriteStartObject();
				WriteTypeProperty(writer, contract.CreatedType);
				writer.WritePropertyName("$value");
				writer.WriteValue(value);
				writer.WriteEndObject();
			}
			else
			{
				writer.WriteValue(value);
			}
		}

		private void SerializeValue(JsonWriter writer, object value, JsonContract valueContract, JsonProperty member, JsonContract collectionValueContract)
		{
			JsonConverter jsonConverter = ((member != null) ? member.Converter : null);
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			if ((jsonConverter != null || (jsonConverter = valueContract.Converter) != null || (jsonConverter = Serializer.GetMatchingConverter(valueContract.UnderlyingType)) != null || (jsonConverter = valueContract.InternalConverter) != null) && jsonConverter.CanWrite)
			{
				SerializeConvertable(writer, jsonConverter, value, valueContract);
				return;
			}
			switch (valueContract.ContractType)
			{
			case JsonContractType.Object:
				SerializeObject(writer, value, (JsonObjectContract)valueContract, member, collectionValueContract);
				break;
			case JsonContractType.Array:
			{
				JsonArrayContract jsonArrayContract = (JsonArrayContract)valueContract;
				SerializeList(writer, jsonArrayContract.CreateWrapper(value), jsonArrayContract, member, collectionValueContract);
				break;
			}
			case JsonContractType.Primitive:
				SerializePrimitive(writer, value, (JsonPrimitiveContract)valueContract, member, collectionValueContract);
				break;
			case JsonContractType.String:
				SerializeString(writer, value, (JsonStringContract)valueContract);
				break;
			case JsonContractType.Dictionary:
			{
				JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)valueContract;
				SerializeDictionary(writer, jsonDictionaryContract.CreateWrapper(value), jsonDictionaryContract, member, collectionValueContract);
				break;
			}
			case JsonContractType.Serializable:
				SerializeISerializable(writer, (ISerializable)value, (JsonISerializableContract)valueContract, member, collectionValueContract);
				break;
			case JsonContractType.Linq:
				((JToken)value).WriteTo(writer, (Serializer.Converters != null) ? Serializer.Converters.ToArray() : null);
				break;
			}
		}

		private bool ShouldWriteReference(object value, JsonProperty property, JsonContract contract)
		{
			if (value == null)
			{
				return false;
			}
			if (contract.ContractType == JsonContractType.Primitive || contract.ContractType == JsonContractType.String)
			{
				return false;
			}
			bool? flag = null;
			if (property != null)
			{
				flag = property.IsReference;
			}
			if (!flag.HasValue)
			{
				flag = contract.IsReference;
			}
			if (!flag.HasValue)
			{
				flag = ((contract.ContractType != JsonContractType.Array) ? new bool?(HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects)) : new bool?(HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays)));
			}
			if (!flag.Value)
			{
				return false;
			}
			return Serializer.ReferenceResolver.IsReferenced(this, value);
		}

		private void WriteMemberInfoProperty(JsonWriter writer, object memberValue, JsonProperty property, JsonContract contract)
		{
			string propertyName = property.PropertyName;
			object defaultValue = property.DefaultValue;
			if ((property.NullValueHandling.GetValueOrDefault(Serializer.NullValueHandling) == NullValueHandling.Ignore && memberValue == null) || (HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) && MiscellaneousUtils.ValueEquals(memberValue, defaultValue)))
			{
				return;
			}
			if (ShouldWriteReference(memberValue, property, contract))
			{
				writer.WritePropertyName(propertyName);
				WriteReference(writer, memberValue);
			}
			else if (CheckForCircularReference(memberValue, property.ReferenceLoopHandling, contract))
			{
				if (memberValue == null && property.Required == Required.Always)
				{
					throw new JsonSerializationException("Cannot write a null value for property '{0}'. Property requires a value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
				}
				writer.WritePropertyName(propertyName);
				SerializeValue(writer, memberValue, contract, property, null);
			}
		}

		private bool CheckForCircularReference(object value, ReferenceLoopHandling? referenceLoopHandling, JsonContract contract)
		{
			if (value == null || contract.ContractType == JsonContractType.Primitive || contract.ContractType == JsonContractType.String)
			{
				return true;
			}
			if (_serializeStack.IndexOf(value) != -1)
			{
				switch (referenceLoopHandling.GetValueOrDefault(Serializer.ReferenceLoopHandling))
				{
				case ReferenceLoopHandling.Error:
					throw new JsonSerializationException("Self referencing loop detected for type '{0}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
				case ReferenceLoopHandling.Ignore:
					return false;
				case ReferenceLoopHandling.Serialize:
					return true;
				default:
					throw new InvalidOperationException("Unexpected ReferenceLoopHandling value: '{0}'".FormatWith(CultureInfo.InvariantCulture, Serializer.ReferenceLoopHandling));
				}
			}
			return true;
		}

		private void WriteReference(JsonWriter writer, object value)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("$ref");
			writer.WriteValue(Serializer.ReferenceResolver.GetReference(this, value));
			writer.WriteEndObject();
		}

		internal static bool TryConvertToString(object value, Type type, out string s)
		{
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && !(converter is ComponentConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
			{
				s = converter.ConvertToInvariantString(value);
				return true;
			}
			if (value is Type)
			{
				s = ((Type)value).AssemblyQualifiedName;
				return true;
			}
			s = null;
			return false;
		}

		private void SerializeString(JsonWriter writer, object value, JsonStringContract contract)
		{
			contract.InvokeOnSerializing(value, Serializer.Context);
			string s;
			TryConvertToString(value, contract.UnderlyingType, out s);
			writer.WriteValue(s);
			contract.InvokeOnSerialized(value, Serializer.Context);
		}

		private void SerializeObject(JsonWriter writer, object value, JsonObjectContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(value, Serializer.Context);
			_serializeStack.Add(value);
			writer.WriteStartObject();
			if (contract.IsReference ?? HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects))
			{
				writer.WritePropertyName("$id");
				writer.WriteValue(Serializer.ReferenceResolver.GetReference(this, value));
			}
			if (ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				WriteTypeProperty(writer, contract.UnderlyingType);
			}
			int top = writer.Top;
			foreach (JsonProperty property in contract.Properties)
			{
				try
				{
					if (!property.Ignored && property.Readable && ShouldSerialize(property, value) && IsSpecified(property, value))
					{
						if (property.PropertyContract == null)
						{
							property.PropertyContract = Serializer.ContractResolver.ResolveContract(property.PropertyType);
						}
						object value2 = property.ValueProvider.GetValue(value);
						JsonContract contract2 = (property.PropertyContract.UnderlyingType.IsSealed() ? property.PropertyContract : GetContractSafe(value2));
						WriteMemberInfoProperty(writer, value2, property, contract2);
					}
				}
				catch (Exception ex)
				{
					if (IsErrorHandled(value, contract, property.PropertyName, writer.ContainerPath, ex))
					{
						HandleError(writer, top);
						continue;
					}
					throw;
				}
			}
			writer.WriteEndObject();
			_serializeStack.RemoveAt(_serializeStack.Count - 1);
			contract.InvokeOnSerialized(value, Serializer.Context);
		}

		private void WriteTypeProperty(JsonWriter writer, Type type)
		{
			writer.WritePropertyName("$type");
			writer.WriteValue(ReflectionUtils.GetTypeName(type, Serializer.TypeNameAssemblyFormat, Serializer.Binder));
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasFlag(PreserveReferencesHandling value, PreserveReferencesHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasFlag(TypeNameHandling value, TypeNameHandling flag)
		{
			return (value & flag) == flag;
		}

		private void SerializeConvertable(JsonWriter writer, JsonConverter converter, object value, JsonContract contract)
		{
			if (ShouldWriteReference(value, null, contract))
			{
				WriteReference(writer, value);
			}
			else if (CheckForCircularReference(value, null, contract))
			{
				_serializeStack.Add(value);
				converter.WriteJson(writer, value, GetInternalSerializer());
				_serializeStack.RemoveAt(_serializeStack.Count - 1);
			}
		}

		private void SerializeList(JsonWriter writer, IWrappedCollection values, JsonArrayContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(values.UnderlyingCollection, Serializer.Context);
			_serializeStack.Add(values.UnderlyingCollection);
			bool flag = contract.IsReference ?? HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays);
			bool flag2 = ShouldWriteType(TypeNameHandling.Arrays, contract, member, collectionValueContract);
			if (flag || flag2)
			{
				writer.WriteStartObject();
				if (flag)
				{
					writer.WritePropertyName("$id");
					writer.WriteValue(Serializer.ReferenceResolver.GetReference(this, values.UnderlyingCollection));
				}
				if (flag2)
				{
					WriteTypeProperty(writer, values.UnderlyingCollection.GetType());
				}
				writer.WritePropertyName("$values");
			}
			if (contract.CollectionItemContract == null)
			{
				contract.CollectionItemContract = Serializer.ContractResolver.ResolveContract(contract.CollectionItemType ?? typeof(object));
			}
			JsonContract jsonContract = (contract.CollectionItemContract.UnderlyingType.IsSealed() ? contract.CollectionItemContract : null);
			writer.WriteStartArray();
			int top = writer.Top;
			int num = 0;
			foreach (object value in values)
			{
				try
				{
					JsonContract jsonContract2 = jsonContract ?? GetContractSafe(value);
					if (ShouldWriteReference(value, null, jsonContract2))
					{
						WriteReference(writer, value);
					}
					else if (CheckForCircularReference(value, null, contract))
					{
						SerializeValue(writer, value, jsonContract2, null, contract.CollectionItemContract);
					}
				}
				catch (Exception ex)
				{
					if (IsErrorHandled(values.UnderlyingCollection, contract, num, writer.ContainerPath, ex))
					{
						HandleError(writer, top);
						continue;
					}
					throw;
				}
				finally
				{
					num++;
				}
			}
			writer.WriteEndArray();
			if (flag || flag2)
			{
				writer.WriteEndObject();
			}
			_serializeStack.RemoveAt(_serializeStack.Count - 1);
			contract.InvokeOnSerialized(values.UnderlyingCollection, Serializer.Context);
		}

		private void SerializeISerializable(JsonWriter writer, ISerializable value, JsonISerializableContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			if (!JsonTypeReflector.FullyTrusted)
			{
				throw new JsonSerializationException("Type '{0}' implements ISerializable but cannot be serialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data.\r\nTo fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add to JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
			}
			contract.InvokeOnSerializing(value, Serializer.Context);
			_serializeStack.Add(value);
			writer.WriteStartObject();
			if (ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				WriteTypeProperty(writer, contract.UnderlyingType);
			}
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new FormatterConverter());
			value.GetObjectData(serializationInfo, Serializer.Context);
			SerializationInfoEnumerator enumerator = serializationInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SerializationEntry current = enumerator.Current;
				writer.WritePropertyName(current.Name);
				SerializeValue(writer, current.Value, GetContractSafe(current.Value), null, null);
			}
			writer.WriteEndObject();
			_serializeStack.RemoveAt(_serializeStack.Count - 1);
			contract.InvokeOnSerialized(value, Serializer.Context);
		}

		private bool ShouldWriteType(TypeNameHandling typeNameHandlingFlag, JsonContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			if (HasFlag(((member != null) ? member.TypeNameHandling : null) ?? Serializer.TypeNameHandling, typeNameHandlingFlag))
			{
				return true;
			}
			if (member != null)
			{
				if ((member.TypeNameHandling ?? Serializer.TypeNameHandling) == TypeNameHandling.Auto && contract.UnderlyingType != member.PropertyType)
				{
					JsonContract jsonContract = Serializer.ContractResolver.ResolveContract(member.PropertyType);
					if (contract.UnderlyingType != jsonContract.CreatedType)
					{
						return true;
					}
				}
			}
			else if (collectionValueContract != null && Serializer.TypeNameHandling == TypeNameHandling.Auto && contract.UnderlyingType != collectionValueContract.UnderlyingType)
			{
				return true;
			}
			return false;
		}

		private void SerializeDictionary(JsonWriter writer, IWrappedDictionary values, JsonDictionaryContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(values.UnderlyingDictionary, Serializer.Context);
			_serializeStack.Add(values.UnderlyingDictionary);
			writer.WriteStartObject();
			if (contract.IsReference ?? HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects))
			{
				writer.WritePropertyName("$id");
				writer.WriteValue(Serializer.ReferenceResolver.GetReference(this, values.UnderlyingDictionary));
			}
			if (ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				WriteTypeProperty(writer, values.UnderlyingDictionary.GetType());
			}
			if (contract.DictionaryValueContract == null)
			{
				contract.DictionaryValueContract = Serializer.ContractResolver.ResolveContract(contract.DictionaryValueType ?? typeof(object));
			}
			JsonContract jsonContract = (contract.DictionaryValueContract.UnderlyingType.IsSealed() ? contract.DictionaryValueContract : null);
			int top = writer.Top;
			foreach (DictionaryEntry value2 in values)
			{
				string propertyName = GetPropertyName(value2);
				propertyName = ((contract.PropertyNameResolver != null) ? contract.PropertyNameResolver(propertyName) : propertyName);
				try
				{
					object value = value2.Value;
					JsonContract jsonContract2 = jsonContract ?? GetContractSafe(value);
					if (ShouldWriteReference(value, null, jsonContract2))
					{
						writer.WritePropertyName(propertyName);
						WriteReference(writer, value);
					}
					else if (CheckForCircularReference(value, null, contract))
					{
						writer.WritePropertyName(propertyName);
						SerializeValue(writer, value, jsonContract2, null, contract.DictionaryValueContract);
					}
				}
				catch (Exception ex)
				{
					if (IsErrorHandled(values.UnderlyingDictionary, contract, propertyName, writer.ContainerPath, ex))
					{
						HandleError(writer, top);
						continue;
					}
					throw;
				}
			}
			writer.WriteEndObject();
			_serializeStack.RemoveAt(_serializeStack.Count - 1);
			contract.InvokeOnSerialized(values.UnderlyingDictionary, Serializer.Context);
		}

		private string GetPropertyName(DictionaryEntry entry)
		{
			if (ConvertUtils.IsConvertible(entry.Key))
			{
				return Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
			}
			string s;
			if (TryConvertToString(entry.Key, entry.Key.GetType(), out s))
			{
				return s;
			}
			return entry.Key.ToString();
		}

		private void HandleError(JsonWriter writer, int initialDepth)
		{
			ClearErrorContext();
			while (writer.Top > initialDepth)
			{
				writer.WriteEnd();
			}
		}

		private bool ShouldSerialize(JsonProperty property, object target)
		{
			if (property.ShouldSerialize == null)
			{
				return true;
			}
			return property.ShouldSerialize(target);
		}

		private bool IsSpecified(JsonProperty property, object target)
		{
			if (property.GetIsSpecified == null)
			{
				return true;
			}
			return property.GetIsSpecified(target);
		}
	}
}
