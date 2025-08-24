using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	internal static class JsonTypeReflector
	{
		public const string IdPropertyName = "$id";

		public const string RefPropertyName = "$ref";

		public const string TypePropertyName = "$type";

		public const string ValuePropertyName = "$value";

		public const string ArrayValuesPropertyName = "$values";

		public const string ShouldSerializePrefix = "ShouldSerialize";

		public const string SpecifiedPostfix = "Specified";

		private static readonly ThreadSafeStore<ICustomAttributeProvider, Type> JsonConverterTypeCache = new ThreadSafeStore<ICustomAttributeProvider, Type>(GetJsonConverterTypeFromAttribute);

		private static bool? _dynamicCodeGeneration;

		private static bool? _fullyTrusted;

		public static bool DynamicCodeGeneration
		{
			get
			{
				if (!_dynamicCodeGeneration.HasValue)
				{
					try
					{
						new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
						new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess).Demand();
						new SecurityPermission(SecurityPermissionFlag.SkipVerification).Demand();
						new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
						new SecurityPermission(PermissionState.Unrestricted).Demand();
						_dynamicCodeGeneration = true;
					}
					catch (Exception)
					{
						_dynamicCodeGeneration = false;
					}
				}
				return _dynamicCodeGeneration.Value;
			}
		}

		public static bool FullyTrusted
		{
			get
			{
				if (!_fullyTrusted.HasValue)
				{
					try
					{
						new SecurityPermission(PermissionState.Unrestricted).Demand();
						_fullyTrusted = true;
					}
					catch (Exception)
					{
						_fullyTrusted = false;
					}
				}
				return _fullyTrusted.Value;
			}
		}

		public static ReflectionDelegateFactory ReflectionDelegateFactory
		{
			get
			{
				if (DynamicCodeGeneration)
				{
					return DynamicReflectionDelegateFactory.Instance;
				}
				return LateBoundReflectionDelegateFactory.Instance;
			}
		}

		public static JsonContainerAttribute GetJsonContainerAttribute(Type type)
		{
			return CachedAttributeGetter<JsonContainerAttribute>.GetAttribute(type.GetCustomAttributeProvider());
		}

		public static JsonObjectAttribute GetJsonObjectAttribute(Type type)
		{
			return GetJsonContainerAttribute(type) as JsonObjectAttribute;
		}

		public static JsonArrayAttribute GetJsonArrayAttribute(Type type)
		{
			return GetJsonContainerAttribute(type) as JsonArrayAttribute;
		}

		public static MemberSerialization GetObjectMemberSerialization(Type objectType)
		{
			JsonObjectAttribute jsonObjectAttribute = GetJsonObjectAttribute(objectType);
			if (jsonObjectAttribute == null)
			{
				return MemberSerialization.OptOut;
			}
			return jsonObjectAttribute.MemberSerialization;
		}

		private static Type GetJsonConverterType(ICustomAttributeProvider attributeProvider)
		{
			return JsonConverterTypeCache.Get(attributeProvider);
		}

		private static Type GetJsonConverterTypeFromAttribute(ICustomAttributeProvider attributeProvider)
		{
			JsonConverterAttribute attribute = GetAttribute<JsonConverterAttribute>(attributeProvider);
			if (attribute == null)
			{
				return null;
			}
			return attribute.ConverterType;
		}

		public static JsonConverter GetJsonConverter(ICustomAttributeProvider attributeProvider, Type targetConvertedType)
		{
			object obj = null;
			obj = attributeProvider as MemberInfo;
			Type jsonConverterType = GetJsonConverterType(attributeProvider);
			if (jsonConverterType != null)
			{
				JsonConverter jsonConverter = JsonConverterAttribute.CreateJsonConverterInstance(jsonConverterType);
				if (!jsonConverter.CanConvert(targetConvertedType))
				{
					throw new JsonSerializationException("JsonConverter {0} on {1} is not compatible with member type {2}.".FormatWith(CultureInfo.InvariantCulture, jsonConverter.GetType().Name, obj, targetConvertedType.Name));
				}
				return jsonConverter;
			}
			return null;
		}

		public static TypeConverter GetTypeConverter(Type type)
		{
			return TypeDescriptor.GetConverter(type);
		}

		private static T GetAttribute<T>(Type type) where T : Attribute
		{
			T attribute = ReflectionUtils.GetAttribute<T>(type.GetCustomAttributeProvider(), true);
			if (attribute != null)
			{
				return attribute;
			}
			Type[] interfaces = type.GetInterfaces();
			foreach (Type o in interfaces)
			{
				attribute = ReflectionUtils.GetAttribute<T>(o.GetCustomAttributeProvider(), true);
				if (attribute != null)
				{
					return attribute;
				}
			}
			return null;
		}

		private static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
		{
			T attribute = ReflectionUtils.GetAttribute<T>(memberInfo.GetCustomAttributeProvider(), true);
			if (attribute != null)
			{
				return attribute;
			}
			if (memberInfo.DeclaringType != null)
			{
				Type[] interfaces = memberInfo.DeclaringType.GetInterfaces();
				foreach (Type targetType in interfaces)
				{
					MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(targetType, memberInfo);
					if (memberInfoFromType != null)
					{
						attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType.GetCustomAttributeProvider(), true);
						if (attribute != null)
						{
							return attribute;
						}
					}
				}
			}
			return null;
		}

		public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider) where T : Attribute
		{
			object obj = null;
			obj = attributeProvider;
			Type type = obj as Type;
			if (type != null)
			{
				return GetAttribute<T>(type);
			}
			MemberInfo memberInfo = obj as MemberInfo;
			if (memberInfo != null)
			{
				return GetAttribute<T>(memberInfo);
			}
			return ReflectionUtils.GetAttribute<T>(attributeProvider, true);
		}
	}
}
