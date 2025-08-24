using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Utilities
{
	internal static class TypeExtensions
	{
		public static MemberTypes MemberType(this MemberInfo memberInfo)
		{
			return memberInfo.MemberType;
		}

		public static Module Module(this Type type)
		{
			return type.Module;
		}

		public static bool ContainsGenericParameters(this Type type)
		{
			return type.ContainsGenericParameters;
		}

		public static bool IsInterface(this Type type)
		{
			return type.IsInterface;
		}

		public static bool IsGenericType(this Type type)
		{
			return type.IsGenericType;
		}

		public static bool IsGenericTypeDefinition(this Type type)
		{
			return type.IsGenericTypeDefinition;
		}

		public static Type BaseType(this Type type)
		{
			return type.BaseType;
		}

		public static bool IsEnum(this Type type)
		{
			return type.IsEnum;
		}

		public static bool IsClass(this Type type)
		{
			return type.IsClass;
		}

		public static bool IsSealed(this Type type)
		{
			return type.IsSealed;
		}

		public static bool IsAbstract(this Type type)
		{
			return type.IsAbstract;
		}

		public static bool IsVisible(this Type type)
		{
			return type.IsVisible;
		}

		public static bool IsValueType(this Type type)
		{
			return type.IsValueType;
		}

		public static bool AssignableToTypeName(this Type type, string fullTypeName, out Type match)
		{
			for (Type type2 = type; type2 != null; type2 = type2.BaseType())
			{
				if (string.Equals(type2.FullName, fullTypeName, StringComparison.Ordinal))
				{
					match = type2;
					return true;
				}
			}
			Type[] interfaces = type.GetInterfaces();
			foreach (Type type3 in interfaces)
			{
				if (string.Equals(type3.Name, fullTypeName, StringComparison.Ordinal))
				{
					match = type;
					return true;
				}
			}
			match = null;
			return false;
		}

		public static bool AssignableToTypeName(this Type type, string fullTypeName)
		{
			Type match;
			return type.AssignableToTypeName(fullTypeName, out match);
		}

		public static MethodInfo GetGenericMethod(this Type type, string name, params Type[] parameterTypes)
		{
			IEnumerable<MethodInfo> enumerable = from method in type.GetMethods()
				where method.Name == name
				select method;
			foreach (MethodInfo item in enumerable)
			{
				if (item.HasParameters(parameterTypes))
				{
					return item;
				}
			}
			return null;
		}

		public static bool HasParameters(this MethodInfo method, params Type[] parameterTypes)
		{
			Type[] array = (from parameter in method.GetParameters()
				select parameter.ParameterType).ToArray();
			if (array.Length != parameterTypes.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ToString() != parameterTypes[i].ToString())
				{
					return false;
				}
			}
			return true;
		}

		public static IEnumerable<Type> GetAllInterfaces(this Type target)
		{
			try
			{
				Type[] interfaces = target.GetInterfaces();
				foreach (Type i in interfaces)
				{
					yield return i;
					try
					{
						Type[] interfaces2 = i.GetInterfaces();
						for (int k = 0; k < interfaces2.Length; k++)
						{
							yield return interfaces2[k];
						}
					}
					finally
					{
					}
				}
			}
			finally
			{
			}
		}

		public static IEnumerable<MethodInfo> GetAllMethods(this Type target)
		{
			List<Type> list = target.GetAllInterfaces().ToList();
			list.Add(target);
			return from type in list
				from method in type.GetMethods()
				select method;
		}
	}
}
