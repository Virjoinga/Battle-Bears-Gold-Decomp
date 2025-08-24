using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Utilities
{
	internal static class EnumUtils
	{
		public static IList<T> GetFlagsValues<T>(T value) where T : struct
		{
			Type typeFromHandle = typeof(T);
			if (!typeFromHandle.IsDefined(typeof(FlagsAttribute), false))
			{
				throw new Exception("Enum type {0} is not a set of flags.".FormatWith(CultureInfo.InvariantCulture, typeFromHandle));
			}
			Type underlyingType = Enum.GetUnderlyingType(value.GetType());
			ulong num = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			EnumValues<ulong> namesAndValues = GetNamesAndValues<T>();
			IList<T> list = new List<T>();
			foreach (EnumValue<ulong> item in namesAndValues)
			{
				if ((num & item.Value) == item.Value && item.Value != 0)
				{
					list.Add((T)Convert.ChangeType(item.Value, underlyingType, CultureInfo.CurrentCulture));
				}
			}
			if (list.Count == 0 && namesAndValues.SingleOrDefault((EnumValue<ulong> v) => v.Value == 0) != null)
			{
				list.Add(default(T));
			}
			return list;
		}

		public static EnumValues<ulong> GetNamesAndValues<T>() where T : struct
		{
			return GetNamesAndValues<ulong>(typeof(T));
		}

		public static EnumValues<TUnderlyingType> GetNamesAndValues<TUnderlyingType>(Type enumType) where TUnderlyingType : struct
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			ValidationUtils.ArgumentTypeIsEnum(enumType, "enumType");
			IList<object> values = GetValues(enumType);
			IList<string> names = GetNames(enumType);
			EnumValues<TUnderlyingType> enumValues = new EnumValues<TUnderlyingType>();
			for (int i = 0; i < values.Count; i++)
			{
				try
				{
					enumValues.Add(new EnumValue<TUnderlyingType>(names[i], (TUnderlyingType)Convert.ChangeType(values[i], typeof(TUnderlyingType), CultureInfo.CurrentCulture)));
				}
				catch (OverflowException innerException)
				{
					throw new Exception(string.Format(CultureInfo.InvariantCulture, "Value from enum with the underlying type of {0} cannot be added to dictionary with a value type of {1}. Value was too large: {2}", Enum.GetUnderlyingType(enumType), typeof(TUnderlyingType), Convert.ToUInt64(values[i], CultureInfo.InvariantCulture)), innerException);
				}
			}
			return enumValues;
		}

		public static IList<object> GetValues(Type enumType)
		{
			if (!enumType.IsEnum())
			{
				throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");
			}
			List<object> list = new List<object>();
			IEnumerable<FieldInfo> enumerable = from field in enumType.GetFields()
				where field.IsLiteral
				select field;
			foreach (FieldInfo item in enumerable)
			{
				object value = item.GetValue(enumType);
				list.Add(value);
			}
			return list;
		}

		public static IList<string> GetNames(Type enumType)
		{
			if (!enumType.IsEnum())
			{
				throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");
			}
			List<string> list = new List<string>();
			IEnumerable<FieldInfo> enumerable = from field in enumType.GetFields()
				where field.IsLiteral
				select field;
			foreach (FieldInfo item in enumerable)
			{
				list.Add(item.Name);
			}
			return list;
		}
	}
}
