using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Utilities
{
	internal static class CollectionUtils
	{
		public static IEnumerable<T> CastValid<T>(this IEnumerable enumerable)
		{
			ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
			return (from object o in enumerable
				where o is T
				select o).Cast<T>();
		}

		public static bool IsNullOrEmpty<T>(ICollection<T> collection)
		{
			if (collection != null)
			{
				return collection.Count == 0;
			}
			return true;
		}

		public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
		{
			if (initial == null)
			{
				throw new ArgumentNullException("initial");
			}
			if (collection == null)
			{
				return;
			}
			foreach (T item in collection)
			{
				initial.Add(item);
			}
		}

		public static void AddRange(this IList initial, IEnumerable collection)
		{
			ValidationUtils.ArgumentNotNull(initial, "initial");
			ListWrapper<object> initial2 = new ListWrapper<object>(initial);
			initial2.AddRange(collection.Cast<object>());
		}

		public static IList CreateGenericList(Type listType)
		{
			ValidationUtils.ArgumentNotNull(listType, "listType");
			return (IList)ReflectionUtils.CreateGeneric(typeof(List<>), listType);
		}

		public static bool IsDictionaryType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				return true;
			}
			if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IDictionary<, >)))
			{
				return true;
			}
			return false;
		}

		public static IWrappedCollection CreateCollectionWrapper(object list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			Type collectionDefinition;
			if (ReflectionUtils.ImplementsGenericDefinition(list.GetType(), typeof(ICollection<>), out collectionDefinition))
			{
				Type collectionItemType = ReflectionUtils.GetCollectionItemType(collectionDefinition);
				Func<Type, IList<object>, object> instanceCreator = delegate(Type t, IList<object> a)
				{
					ConstructorInfo constructor = t.GetConstructor(new Type[1] { collectionDefinition });
					return constructor.Invoke(new object[1] { list });
				};
				return (IWrappedCollection)ReflectionUtils.CreateGeneric(typeof(CollectionWrapper<>), new Type[1] { collectionItemType }, instanceCreator, list);
			}
			if (list is IList)
			{
				return new CollectionWrapper<object>((IList)list);
			}
			throw new Exception("Can not create ListWrapper for type {0}.".FormatWith(CultureInfo.InvariantCulture, list.GetType()));
		}

		public static IWrappedDictionary CreateDictionaryWrapper(object dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
			Type dictionaryDefinition;
			if (ReflectionUtils.ImplementsGenericDefinition(dictionary.GetType(), typeof(IDictionary<, >), out dictionaryDefinition))
			{
				Type dictionaryKeyType = ReflectionUtils.GetDictionaryKeyType(dictionaryDefinition);
				Type dictionaryValueType = ReflectionUtils.GetDictionaryValueType(dictionaryDefinition);
				Func<Type, IList<object>, object> instanceCreator = delegate(Type t, IList<object> a)
				{
					ConstructorInfo constructor = t.GetConstructor(new Type[1] { dictionaryDefinition });
					return constructor.Invoke(new object[1] { dictionary });
				};
				return (IWrappedDictionary)ReflectionUtils.CreateGeneric(typeof(DictionaryWrapper<, >), new Type[2] { dictionaryKeyType, dictionaryValueType }, instanceCreator, dictionary);
			}
			if (dictionary is IDictionary)
			{
				return new DictionaryWrapper<object, object>((IDictionary)dictionary);
			}
			throw new Exception("Can not create DictionaryWrapper for type {0}.".FormatWith(CultureInfo.InvariantCulture, dictionary.GetType()));
		}

		public static object CreateAndPopulateList(Type listType, Action<IList, bool> populateList)
		{
			ValidationUtils.ArgumentNotNull(listType, "listType");
			ValidationUtils.ArgumentNotNull(populateList, "populateList");
			bool flag = false;
			IList list;
			Type implementingType;
			if (listType.IsArray)
			{
				list = new List<object>();
				flag = true;
			}
			else if (!ReflectionUtils.InheritsGenericDefinition(listType, typeof(ReadOnlyCollection<>), out implementingType))
			{
				list = (typeof(IList).IsAssignableFrom(listType) ? (ReflectionUtils.IsInstantiatableType(listType) ? ((IList)Activator.CreateInstance(listType)) : ((listType != typeof(IList)) ? null : new List<object>())) : ((!ReflectionUtils.ImplementsGenericDefinition(listType, typeof(ICollection<>))) ? null : ((!ReflectionUtils.IsInstantiatableType(listType)) ? null : CreateCollectionWrapper(Activator.CreateInstance(listType)))));
			}
			else
			{
				Type type = implementingType.GetGenericArguments()[0];
				Type type2 = ReflectionUtils.MakeGenericType(typeof(IEnumerable<>), type);
				bool flag2 = false;
				ConstructorInfo[] constructors = listType.GetConstructors();
				foreach (ConstructorInfo constructorInfo in constructors)
				{
					IList<ParameterInfo> parameters = constructorInfo.GetParameters();
					if (parameters.Count == 1 && type2.IsAssignableFrom(parameters[0].ParameterType))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					throw new Exception("Read-only type {0} does not have a public constructor that takes a type that implements {1}.".FormatWith(CultureInfo.InvariantCulture, listType, type2));
				}
				list = CreateGenericList(type);
				flag = true;
			}
			if (list == null)
			{
				throw new Exception("Cannot create and populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, listType));
			}
			populateList(list, flag);
			if (flag)
			{
				if (listType.IsArray)
				{
					list = ToArray(((List<object>)list).ToArray(), ReflectionUtils.GetCollectionItemType(listType));
				}
				else if (ReflectionUtils.InheritsGenericDefinition(listType, typeof(ReadOnlyCollection<>)))
				{
					list = (IList)ReflectionUtils.CreateInstance(listType, list);
				}
			}
			else if (list is IWrappedCollection)
			{
				return ((IWrappedCollection)list).UnderlyingCollection;
			}
			return list;
		}

		public static Array ToArray(Array initial, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Array array = Array.CreateInstance(type, initial.Length);
			Array.Copy(initial, 0, array, 0, initial.Length);
			return array;
		}

		public static bool AddDistinct<T>(this IList<T> list, T value)
		{
			return list.AddDistinct(value, EqualityComparer<T>.Default);
		}

		public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
		{
			if (list.ContainsValue(value, comparer))
			{
				return false;
			}
			list.Add(value);
			return true;
		}

		public static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			foreach (TSource item in source)
			{
				if (comparer.Equals(item, value))
				{
					return true;
				}
			}
			return false;
		}

		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			bool result = true;
			foreach (T value in values)
			{
				if (!list.AddDistinct(value, comparer))
				{
					result = false;
				}
			}
			return result;
		}

		public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			int num = 0;
			foreach (T item in collection)
			{
				if (predicate(item))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value, IEqualityComparer<TSource> comparer)
		{
			int num = 0;
			foreach (TSource item in list)
			{
				if (comparer.Equals(item, value))
				{
					return num;
				}
				num++;
			}
			return -1;
		}
	}
}
