using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	internal static class ConvertUtils
	{
		internal struct TypeConvertKey : IEquatable<TypeConvertKey>
		{
			private readonly Type _initialType;

			private readonly Type _targetType;

			public Type InitialType
			{
				get
				{
					return _initialType;
				}
			}

			public Type TargetType
			{
				get
				{
					return _targetType;
				}
			}

			public TypeConvertKey(Type initialType, Type targetType)
			{
				_initialType = initialType;
				_targetType = targetType;
			}

			public override int GetHashCode()
			{
				return _initialType.GetHashCode() ^ _targetType.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (!(obj is TypeConvertKey))
				{
					return false;
				}
				return Equals((TypeConvertKey)obj);
			}

			public bool Equals(TypeConvertKey other)
			{
				if (_initialType == other._initialType)
				{
					return _targetType == other._targetType;
				}
				return false;
			}
		}

		private static readonly ThreadSafeStore<TypeConvertKey, Func<object, object>> CastConverters = new ThreadSafeStore<TypeConvertKey, Func<object, object>>(CreateCastConverter);

		public static TypeCode GetTypeCode(this IConvertible convertible)
		{
			return convertible.GetTypeCode();
		}

		public static TypeCode GetTypeCode(object o)
		{
			return System.Convert.GetTypeCode(o);
		}

		public static TypeCode GetTypeCode(Type t)
		{
			return Type.GetTypeCode(t);
		}

		public static IConvertible ToConvertible(object o)
		{
			return o as IConvertible;
		}

		public static bool IsConvertible(object o)
		{
			return o is IConvertible;
		}

		public static bool IsConvertible(Type t)
		{
			return typeof(IConvertible).IsAssignableFrom(t);
		}

		private static Func<object, object> CreateCastConverter(TypeConvertKey t)
		{
			MethodInfo method = t.TargetType.GetMethod("op_Implicit", new Type[1] { t.InitialType });
			if (method == null)
			{
				method = t.TargetType.GetMethod("op_Explicit", new Type[1] { t.InitialType });
			}
			if (method == null)
			{
				return null;
			}
			MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => call(null, o);
		}

		public static object Convert(object initialValue, CultureInfo culture, Type targetType)
		{
			if (initialValue == null)
			{
				throw new ArgumentNullException("initialValue");
			}
			if (ReflectionUtils.IsNullableType(targetType))
			{
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			Type type = initialValue.GetType();
			if (targetType == type)
			{
				return initialValue;
			}
			if (IsConvertible(initialValue) && IsConvertible(targetType))
			{
				if (targetType.IsEnum())
				{
					if (initialValue is string)
					{
						return Enum.Parse(targetType, initialValue.ToString(), true);
					}
					if (IsInteger(initialValue))
					{
						return Enum.ToObject(targetType, initialValue);
					}
				}
				return System.Convert.ChangeType(initialValue, targetType, culture);
			}
			if (initialValue is string && typeof(Type).IsAssignableFrom(targetType))
			{
				return Type.GetType((string)initialValue, true);
			}
			if (targetType.IsInterface() || targetType.IsGenericTypeDefinition() || targetType.IsAbstract())
			{
				throw new ArgumentException("Target type {0} is not a value type or a non-abstract class.".FormatWith(CultureInfo.InvariantCulture, targetType), "targetType");
			}
			if (initialValue is string)
			{
				if (targetType == typeof(Guid))
				{
					return new Guid((string)initialValue);
				}
				if (targetType == typeof(Uri))
				{
					return new Uri((string)initialValue);
				}
				if (targetType == typeof(TimeSpan))
				{
					return TimeSpan.Parse((string)initialValue);
				}
			}
			TypeConverter converter = GetConverter(type);
			if (converter != null && converter.CanConvertTo(targetType))
			{
				return converter.ConvertTo(null, culture, initialValue, targetType);
			}
			TypeConverter converter2 = GetConverter(targetType);
			if (converter2 != null && converter2.CanConvertFrom(type))
			{
				return converter2.ConvertFrom(null, culture, initialValue);
			}
			if (initialValue == DBNull.Value)
			{
				if (ReflectionUtils.IsNullable(targetType))
				{
					return EnsureTypeAssignable(null, type, targetType);
				}
				throw new Exception("Can not convert null {0} into non-nullable {1}.".FormatWith(CultureInfo.InvariantCulture, type, targetType));
			}
			if (initialValue is INullable)
			{
				return EnsureTypeAssignable(ToValue((INullable)initialValue), type, targetType);
			}
			throw new Exception("Can not convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, type, targetType));
		}

		public static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object convertedValue)
		{
			return MiscellaneousUtils.TryAction(() => Convert(initialValue, culture, targetType), out convertedValue);
		}

		public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
		{
			if (targetType == typeof(object))
			{
				return initialValue;
			}
			if (initialValue == null && ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			object convertedValue;
			if (TryConvert(initialValue, culture, targetType, out convertedValue))
			{
				return convertedValue;
			}
			return EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
		}

		private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
		{
			Type type = ((value != null) ? value.GetType() : null);
			if (value != null)
			{
				if (targetType.IsAssignableFrom(type))
				{
					return value;
				}
				Func<object, object> func = CastConverters.Get(new TypeConvertKey(type, targetType));
				if (func != null)
				{
					return func(value);
				}
			}
			else if (ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			throw new Exception("Could not cast or convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, (initialType != null) ? initialType.ToString() : "{null}", targetType));
		}

		public static object ToValue(INullable nullableValue)
		{
			if (nullableValue == null)
			{
				return null;
			}
			if (nullableValue is SqlInt32)
			{
				return ToValue((SqlInt32)(object)nullableValue);
			}
			if (nullableValue is SqlInt64)
			{
				return ToValue((SqlInt64)(object)nullableValue);
			}
			if (nullableValue is SqlBoolean)
			{
				return ToValue((SqlBoolean)(object)nullableValue);
			}
			if (nullableValue is SqlString)
			{
				return ToValue((SqlString)(object)nullableValue);
			}
			if (nullableValue is SqlDateTime)
			{
				return ToValue((SqlDateTime)(object)nullableValue);
			}
			throw new Exception("Unsupported INullable type: {0}".FormatWith(CultureInfo.InvariantCulture, nullableValue.GetType()));
		}

		internal static TypeConverter GetConverter(Type t)
		{
			return JsonTypeReflector.GetTypeConverter(t);
		}

		public static bool IsInteger(object value)
		{
			switch (GetTypeCode(value))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return true;
			default:
				return false;
			}
		}
	}
}
