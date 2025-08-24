using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Serialization
{
	public class DefaultContractResolver : IContractResolver
	{
		private static readonly IContractResolver _instance = new DefaultContractResolver(true);

		private static readonly IList<JsonConverter> BuiltInConverters = new List<JsonConverter>
		{
			new BinaryConverter(),
			new KeyValuePairConverter(),
			new XmlNodeConverter(),
			new DataSetConverter(),
			new DataTableConverter(),
			new BsonObjectIdConverter()
		};

		private static Dictionary<ResolverContractKey, JsonContract> _sharedContractCache;

		private static readonly object _typeContractCacheLock = new object();

		private Dictionary<ResolverContractKey, JsonContract> _instanceContractCache;

		private readonly bool _sharedCache;

		internal static IContractResolver Instance
		{
			get
			{
				return _instance;
			}
		}

		public bool DynamicCodeGeneration
		{
			get
			{
				return JsonTypeReflector.DynamicCodeGeneration;
			}
		}

		public BindingFlags DefaultMembersSearchFlags { get; set; }

		public bool SerializeCompilerGeneratedMembers { get; set; }

		public bool IgnoreSerializableInterface { get; set; }

		public DefaultContractResolver()
			: this(false)
		{
		}

		public DefaultContractResolver(bool shareCache)
		{
			DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public;
			_sharedCache = shareCache;
		}

		private Dictionary<ResolverContractKey, JsonContract> GetCache()
		{
			if (_sharedCache)
			{
				return _sharedContractCache;
			}
			return _instanceContractCache;
		}

		private void UpdateCache(Dictionary<ResolverContractKey, JsonContract> cache)
		{
			if (_sharedCache)
			{
				_sharedContractCache = cache;
			}
			else
			{
				_instanceContractCache = cache;
			}
		}

		public virtual JsonContract ResolveContract(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ResolverContractKey key = new ResolverContractKey(GetType(), type);
			Dictionary<ResolverContractKey, JsonContract> cache = GetCache();
			JsonContract value;
			if (cache == null || !cache.TryGetValue(key, out value))
			{
				value = CreateContract(type);
				lock (_typeContractCacheLock)
				{
					cache = GetCache();
					Dictionary<ResolverContractKey, JsonContract> dictionary = ((cache != null) ? new Dictionary<ResolverContractKey, JsonContract>(cache) : new Dictionary<ResolverContractKey, JsonContract>());
					dictionary[key] = value;
					UpdateCache(dictionary);
				}
			}
			return value;
		}

		protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			List<MemberInfo> list = (from m in ReflectionUtils.GetFieldsAndProperties(objectType, DefaultMembersSearchFlags)
				where !ReflectionUtils.IsIndexedProperty(m)
				select m).ToList();
			List<MemberInfo> list2 = (from m in ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				where !ReflectionUtils.IsIndexedProperty(m)
				select m).ToList();
			List<MemberInfo> list3 = new List<MemberInfo>();
			foreach (MemberInfo item in list2)
			{
				if (SerializeCompilerGeneratedMembers || !item.IsDefined(typeof(CompilerGeneratedAttribute), true))
				{
					if (list.Contains(item))
					{
						list3.Add(item);
					}
					else if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(item.GetCustomAttributeProvider()) != null)
					{
						list3.Add(item);
					}
				}
			}
			return list3;
		}

		protected virtual JsonObjectContract CreateObjectContract(Type objectType)
		{
			JsonObjectContract jsonObjectContract = new JsonObjectContract(objectType);
			InitializeContract(jsonObjectContract);
			jsonObjectContract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(jsonObjectContract.NonNullableUnderlyingType);
			jsonObjectContract.Properties.AddRange(CreateProperties(jsonObjectContract.NonNullableUnderlyingType, jsonObjectContract.MemberSerialization));
			if (jsonObjectContract.NonNullableUnderlyingType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any((ConstructorInfo c) => c.IsDefined(typeof(JsonConstructorAttribute), true)))
			{
				ConstructorInfo attributeConstructor = GetAttributeConstructor(jsonObjectContract.NonNullableUnderlyingType);
				if (attributeConstructor != null)
				{
					jsonObjectContract.OverrideConstructor = attributeConstructor;
					jsonObjectContract.ConstructorParameters.AddRange(CreateConstructorParameters(attributeConstructor, jsonObjectContract.Properties));
				}
			}
			else if (jsonObjectContract.DefaultCreator == null || jsonObjectContract.DefaultCreatorNonPublic)
			{
				ConstructorInfo parametrizedConstructor = GetParametrizedConstructor(jsonObjectContract.NonNullableUnderlyingType);
				if (parametrizedConstructor != null)
				{
					jsonObjectContract.ParametrizedConstructor = parametrizedConstructor;
					jsonObjectContract.ConstructorParameters.AddRange(CreateConstructorParameters(parametrizedConstructor, jsonObjectContract.Properties));
				}
			}
			return jsonObjectContract;
		}

		private ConstructorInfo GetAttributeConstructor(Type objectType)
		{
			IList<ConstructorInfo> list = (from c in objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where c.IsDefined(typeof(JsonConstructorAttribute), true)
				select c).ToList();
			if (list.Count > 1)
			{
				throw new Exception("Multiple constructors with the JsonConstructorAttribute.");
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return null;
		}

		private ConstructorInfo GetParametrizedConstructor(Type objectType)
		{
			IList<ConstructorInfo> list = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).ToList();
			if (list.Count == 1)
			{
				return list[0];
			}
			return null;
		}

		protected virtual IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(constructor.DeclaringType);
			ParameterInfo[] array = parameters;
			foreach (ParameterInfo parameterInfo in array)
			{
				JsonProperty jsonProperty = memberProperties.GetClosestMatchProperty(parameterInfo.Name);
				if (jsonProperty != null && jsonProperty.PropertyType != parameterInfo.ParameterType)
				{
					jsonProperty = null;
				}
				JsonProperty jsonProperty2 = CreatePropertyFromConstructorParameter(jsonProperty, parameterInfo);
				if (jsonProperty2 != null)
				{
					jsonPropertyCollection.AddProperty(jsonProperty2);
				}
			}
			return jsonPropertyCollection;
		}

		protected virtual JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
		{
			JsonProperty jsonProperty = new JsonProperty();
			jsonProperty.PropertyType = parameterInfo.ParameterType;
			bool allowNonPublicAccess;
			bool hasExplicitAttribute;
			SetPropertySettingsFromAttributes(jsonProperty, parameterInfo.GetCustomAttributeProvider(), parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out allowNonPublicAccess, out hasExplicitAttribute);
			jsonProperty.Readable = false;
			jsonProperty.Writable = true;
			if (matchingMemberProperty != null)
			{
				jsonProperty.PropertyName = ((jsonProperty.PropertyName != parameterInfo.Name) ? jsonProperty.PropertyName : matchingMemberProperty.PropertyName);
				jsonProperty.Converter = jsonProperty.Converter ?? matchingMemberProperty.Converter;
				jsonProperty.MemberConverter = jsonProperty.MemberConverter ?? matchingMemberProperty.MemberConverter;
				jsonProperty.DefaultValue = jsonProperty.DefaultValue ?? matchingMemberProperty.DefaultValue;
				jsonProperty.Required = ((jsonProperty.Required != 0) ? jsonProperty.Required : matchingMemberProperty.Required);
				jsonProperty.IsReference = jsonProperty.IsReference ?? matchingMemberProperty.IsReference;
				jsonProperty.NullValueHandling = jsonProperty.NullValueHandling ?? matchingMemberProperty.NullValueHandling;
				jsonProperty.DefaultValueHandling = jsonProperty.DefaultValueHandling ?? matchingMemberProperty.DefaultValueHandling;
				jsonProperty.ReferenceLoopHandling = jsonProperty.ReferenceLoopHandling ?? matchingMemberProperty.ReferenceLoopHandling;
				jsonProperty.ObjectCreationHandling = jsonProperty.ObjectCreationHandling ?? matchingMemberProperty.ObjectCreationHandling;
				jsonProperty.TypeNameHandling = jsonProperty.TypeNameHandling ?? matchingMemberProperty.TypeNameHandling;
			}
			return jsonProperty;
		}

		protected virtual JsonConverter ResolveContractConverter(Type objectType)
		{
			return JsonTypeReflector.GetJsonConverter(objectType.GetCustomAttributeProvider(), objectType);
		}

		private Func<object> GetDefaultCreator(Type createdType)
		{
			return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);
		}

		private void InitializeContract(JsonContract contract)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(contract.NonNullableUnderlyingType);
			if (jsonContainerAttribute != null)
			{
				contract.IsReference = jsonContainerAttribute._isReference;
			}
			contract.Converter = ResolveContractConverter(contract.NonNullableUnderlyingType);
			contract.InternalConverter = JsonSerializer.GetMatchingConverter(BuiltInConverters, contract.NonNullableUnderlyingType);
			if (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, true) || contract.CreatedType.IsValueType())
			{
				contract.DefaultCreator = GetDefaultCreator(contract.CreatedType);
				contract.DefaultCreatorNonPublic = !contract.CreatedType.IsValueType() && ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null;
			}
			ResolveCallbackMethods(contract, contract.NonNullableUnderlyingType);
		}

		private void ResolveCallbackMethods(JsonContract contract, Type t)
		{
			if (t.BaseType() != null)
			{
				ResolveCallbackMethods(contract, t.BaseType());
			}
			MethodInfo onSerializing;
			MethodInfo onSerialized;
			MethodInfo onDeserializing;
			MethodInfo onDeserialized;
			MethodInfo onError;
			GetCallbackMethodsForType(t, out onSerializing, out onSerialized, out onDeserializing, out onDeserialized, out onError);
			if (onSerializing != null)
			{
				contract.OnSerializing = onSerializing;
			}
			if (onSerialized != null)
			{
				contract.OnSerialized = onSerialized;
			}
			if (onDeserializing != null)
			{
				contract.OnDeserializing = onDeserializing;
			}
			if (onDeserialized != null)
			{
				contract.OnDeserialized = onDeserialized;
			}
			if (onError != null)
			{
				contract.OnError = onError;
			}
		}

		private void GetCallbackMethodsForType(Type type, out MethodInfo onSerializing, out MethodInfo onSerialized, out MethodInfo onDeserializing, out MethodInfo onDeserialized, out MethodInfo onError)
		{
			onSerializing = null;
			onSerialized = null;
			onDeserializing = null;
			onDeserialized = null;
			onError = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (!methodInfo.ContainsGenericParameters)
				{
					Type prevAttributeType = null;
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (IsValidCallback(methodInfo, parameters, typeof(OnSerializingAttribute), onSerializing, ref prevAttributeType))
					{
						onSerializing = methodInfo;
					}
					if (IsValidCallback(methodInfo, parameters, typeof(OnSerializedAttribute), onSerialized, ref prevAttributeType))
					{
						onSerialized = methodInfo;
					}
					if (IsValidCallback(methodInfo, parameters, typeof(OnDeserializingAttribute), onDeserializing, ref prevAttributeType))
					{
						onDeserializing = methodInfo;
					}
					if (IsValidCallback(methodInfo, parameters, typeof(OnDeserializedAttribute), onDeserialized, ref prevAttributeType))
					{
						onDeserialized = methodInfo;
					}
					if (IsValidCallback(methodInfo, parameters, typeof(OnErrorAttribute), onError, ref prevAttributeType))
					{
						onError = methodInfo;
					}
				}
			}
		}

		protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
		{
			JsonDictionaryContract jsonDictionaryContract = new JsonDictionaryContract(objectType);
			InitializeContract(jsonDictionaryContract);
			jsonDictionaryContract.PropertyNameResolver = ResolvePropertyName;
			return jsonDictionaryContract;
		}

		protected virtual JsonArrayContract CreateArrayContract(Type objectType)
		{
			JsonArrayContract jsonArrayContract = new JsonArrayContract(objectType);
			InitializeContract(jsonArrayContract);
			return jsonArrayContract;
		}

		protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
		{
			JsonPrimitiveContract jsonPrimitiveContract = new JsonPrimitiveContract(objectType);
			InitializeContract(jsonPrimitiveContract);
			return jsonPrimitiveContract;
		}

		protected virtual JsonLinqContract CreateLinqContract(Type objectType)
		{
			JsonLinqContract jsonLinqContract = new JsonLinqContract(objectType);
			InitializeContract(jsonLinqContract);
			return jsonLinqContract;
		}

		protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
		{
			JsonISerializableContract jsonISerializableContract = new JsonISerializableContract(objectType);
			InitializeContract(jsonISerializableContract);
			ConstructorInfo constructor = jsonISerializableContract.NonNullableUnderlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(SerializationInfo),
				typeof(StreamingContext)
			}, null);
			if (constructor != null)
			{
				MethodCall<object, object> methodCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(constructor);
				jsonISerializableContract.ISerializableCreator = (object[] args) => methodCall(null, args);
			}
			return jsonISerializableContract;
		}

		protected virtual JsonStringContract CreateStringContract(Type objectType)
		{
			JsonStringContract jsonStringContract = new JsonStringContract(objectType);
			InitializeContract(jsonStringContract);
			return jsonStringContract;
		}

		protected virtual JsonContract CreateContract(Type objectType)
		{
			Type type = ReflectionUtils.EnsureNotNullableType(objectType);
			if (JsonConvert.IsJsonPrimitiveType(type))
			{
				return CreatePrimitiveContract(objectType);
			}
			if (JsonTypeReflector.GetJsonObjectAttribute(type) != null)
			{
				return CreateObjectContract(objectType);
			}
			if (JsonTypeReflector.GetJsonArrayAttribute(type) != null)
			{
				return CreateArrayContract(objectType);
			}
			if (type == typeof(JToken) || type.IsSubclassOf(typeof(JToken)))
			{
				return CreateLinqContract(objectType);
			}
			if (CollectionUtils.IsDictionaryType(type))
			{
				return CreateDictionaryContract(objectType);
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return CreateArrayContract(objectType);
			}
			if (CanConvertToString(type))
			{
				return CreateStringContract(objectType);
			}
			if (!IgnoreSerializableInterface && typeof(ISerializable).IsAssignableFrom(type))
			{
				return CreateISerializableContract(objectType);
			}
			return CreateObjectContract(objectType);
		}

		internal static bool CanConvertToString(Type type)
		{
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && !(converter is ComponentConverter) && !(converter is ReferenceConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
			{
				return true;
			}
			if (type == typeof(Type) || type.IsSubclassOf(typeof(Type)))
			{
				return true;
			}
			return false;
		}

		private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, MethodInfo currentCallback, ref Type prevAttributeType)
		{
			if (!method.IsDefined(attributeType, false))
			{
				return false;
			}
			if (currentCallback != null)
			{
				throw new Exception("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith(CultureInfo.InvariantCulture, method, currentCallback, GetClrTypeFullName(method.DeclaringType), attributeType));
			}
			if (prevAttributeType != null)
			{
				throw new Exception("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith(CultureInfo.InvariantCulture, prevAttributeType, attributeType, GetClrTypeFullName(method.DeclaringType), method));
			}
			if (method.IsVirtual)
			{
				throw new Exception("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith(CultureInfo.InvariantCulture, method, GetClrTypeFullName(method.DeclaringType), attributeType));
			}
			if (method.ReturnType != typeof(void))
			{
				throw new Exception("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method));
			}
			if (attributeType == typeof(OnErrorAttribute))
			{
				if (parameters == null || parameters.Length != 2 || parameters[0].ParameterType != typeof(StreamingContext) || parameters[1].ParameterType != typeof(ErrorContext))
				{
					throw new Exception("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext), typeof(ErrorContext)));
				}
			}
			else if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
			{
				throw new Exception("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext)));
			}
			prevAttributeType = attributeType;
			return true;
		}

		internal static string GetClrTypeFullName(Type type)
		{
			if (type.IsGenericTypeDefinition() || !type.ContainsGenericParameters())
			{
				return type.FullName;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", type.Namespace, type.Name);
		}

		protected virtual IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			List<MemberInfo> serializableMembers = GetSerializableMembers(type);
			if (serializableMembers == null)
			{
				throw new JsonSerializationException("Null collection of seralizable members returned.");
			}
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(type);
			foreach (MemberInfo item in serializableMembers)
			{
				JsonProperty jsonProperty = CreateProperty(item, memberSerialization);
				if (jsonProperty != null)
				{
					jsonPropertyCollection.AddProperty(jsonProperty);
				}
			}
			return jsonPropertyCollection.OrderBy((JsonProperty p) => p.Order ?? (-1)).ToList();
		}

		protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
		{
			if (DynamicCodeGeneration)
			{
				return new DynamicValueProvider(member);
			}
			return new ReflectionValueProvider(member);
		}

		protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty jsonProperty = new JsonProperty();
			jsonProperty.PropertyType = ReflectionUtils.GetMemberUnderlyingType(member);
			jsonProperty.DeclaringType = member.DeclaringType;
			jsonProperty.ValueProvider = CreateMemberValueProvider(member);
			bool allowNonPublicAccess;
			bool hasExplicitAttribute;
			SetPropertySettingsFromAttributes(jsonProperty, member.GetCustomAttributeProvider(), member.Name, member.DeclaringType, memberSerialization, out allowNonPublicAccess, out hasExplicitAttribute);
			jsonProperty.Readable = ReflectionUtils.CanReadMemberValue(member, allowNonPublicAccess);
			jsonProperty.Writable = ReflectionUtils.CanSetMemberValue(member, allowNonPublicAccess, hasExplicitAttribute);
			jsonProperty.ShouldSerialize = CreateShouldSerializeTest(member);
			SetIsSpecifiedActions(jsonProperty, member, allowNonPublicAccess);
			return jsonProperty;
		}

		private void SetPropertySettingsFromAttributes(JsonProperty property, ICustomAttributeProvider attributeProvider, string name, Type declaringType, MemberSerialization memberSerialization, out bool allowNonPublicAccess, out bool hasExplicitAttribute)
		{
			hasExplicitAttribute = false;
			JsonPropertyAttribute attribute = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
			if (attribute != null)
			{
				hasExplicitAttribute = true;
			}
			bool flag = JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null;
			string propertyName = ((attribute == null || attribute.PropertyName == null) ? name : attribute.PropertyName);
			property.PropertyName = ResolvePropertyName(propertyName);
			property.UnderlyingName = name;
			if (attribute != null)
			{
				property.Required = attribute.Required;
				property.Order = attribute._order;
			}
			else
			{
				property.Required = Required.Default;
			}
			property.Ignored = flag || (memberSerialization == MemberSerialization.OptIn && attribute == null);
			property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider, property.PropertyType);
			property.MemberConverter = JsonTypeReflector.GetJsonConverter(attributeProvider, property.PropertyType);
			DefaultValueAttribute attribute2 = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider);
			property.DefaultValue = ((attribute2 != null) ? attribute2.Value : null);
			property.NullValueHandling = ((attribute != null) ? attribute._nullValueHandling : null);
			property.DefaultValueHandling = ((attribute != null) ? attribute._defaultValueHandling : null);
			property.ReferenceLoopHandling = ((attribute != null) ? attribute._referenceLoopHandling : null);
			property.ObjectCreationHandling = ((attribute != null) ? attribute._objectCreationHandling : null);
			property.TypeNameHandling = ((attribute != null) ? attribute._typeNameHandling : null);
			property.IsReference = ((attribute != null) ? attribute._isReference : null);
			allowNonPublicAccess = false;
			if ((DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
			{
				allowNonPublicAccess = true;
			}
			if (attribute != null)
			{
				allowNonPublicAccess = true;
			}
		}

		private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
		{
			MethodInfo method = member.DeclaringType.GetMethod("ShouldSerialize" + member.Name, ReflectionUtils.EmptyTypes);
			if (method == null || method.ReturnType != typeof(bool))
			{
				return null;
			}
			MethodCall<object, object> shouldSerializeCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => (bool)shouldSerializeCall(o);
		}

		private void SetIsSpecifiedActions(JsonProperty property, MemberInfo member, bool allowNonPublicAccess)
		{
			MemberInfo memberInfo = member.DeclaringType.GetProperty(member.Name + "Specified");
			if (memberInfo == null)
			{
				memberInfo = member.DeclaringType.GetField(member.Name + "Specified");
			}
			if (memberInfo != null && ReflectionUtils.GetMemberUnderlyingType(memberInfo) == typeof(bool))
			{
				Func<object, object> specifiedPropertyGet = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(memberInfo);
				property.GetIsSpecified = (object o) => (bool)specifiedPropertyGet(o);
				if (ReflectionUtils.CanSetMemberValue(memberInfo, allowNonPublicAccess, false))
				{
					property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(memberInfo);
				}
			}
		}

		protected internal virtual string ResolvePropertyName(string propertyName)
		{
			return propertyName;
		}
	}
}
