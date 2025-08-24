using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities.LinqBridge
{
	internal static class Enumerable
	{
		private static class Futures<T>
		{
			public static readonly Func<T> Default = () => default(T);

			public static readonly Func<T> Undefined = delegate
			{
				throw new InvalidOperationException();
			};
		}

		private static class Sequence<T>
		{
			public static readonly IEnumerable<T> Empty = new T[0];
		}

		private sealed class Grouping<K, V> : List<V>, IGrouping<K, V>, IEnumerable<V>, IEnumerable
		{
			public K Key { get; private set; }

			internal Grouping(K key)
			{
				Key = key;
			}
		}

		public static IEnumerable<TSource> AsEnumerable<TSource>(IEnumerable<TSource> source)
		{
			return source;
		}

		public static IEnumerable<TResult> Empty<TResult>()
		{
			return Sequence<TResult>.Empty;
		}

		public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
		{
			CheckNotNull(source, "source");
			return CastYield<TResult>(source);
		}

		private static IEnumerable<TResult> CastYield<TResult>(IEnumerable source)
		{
			foreach (object item in source)
			{
				yield return (TResult)item;
			}
		}

		public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
		{
			CheckNotNull(source, "source");
			return OfTypeYield<TResult>(source);
		}

		private static IEnumerable<TResult> OfTypeYield<TResult>(IEnumerable source)
		{
			foreach (object item in source)
			{
				if (item is TResult)
				{
					yield return (TResult)item;
				}
			}
		}

		public static IEnumerable<int> Range(int start, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, null);
			}
			long num = (long)start + (long)count;
			if (num - 1 >= int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("count", count, null);
			}
			return RangeYield(start, num);
		}

		private static IEnumerable<int> RangeYield(int start, long end)
		{
			for (int i = start; i < end; i++)
			{
				yield return i;
			}
		}

		public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, null);
			}
			return RepeatYield(element, count);
		}

		private static IEnumerable<TResult> RepeatYield<TResult>(TResult element, int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return element;
			}
		}

		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			CheckNotNull(predicate, "predicate");
			return source.Where((TSource item, int i) => predicate(item));
		}

		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			CheckNotNull(source, "source");
			CheckNotNull(predicate, "predicate");
			return WhereYield(source, predicate);
		}

		private static IEnumerable<TSource> WhereYield<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int i = 0;
			foreach (TSource item in source)
			{
				if (predicate(item, i++))
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			CheckNotNull(selector, "selector");
			return source.Select((TSource item, int i) => selector(item));
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			CheckNotNull(source, "source");
			CheckNotNull(selector, "selector");
			return SelectYield(source, selector);
		}

		private static IEnumerable<TResult> SelectYield<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			int i = 0;
			foreach (TSource item in source)
			{
				yield return selector(item, i++);
			}
		}

		public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			CheckNotNull(selector, "selector");
			return source.SelectMany((TSource item, int i) => selector(item));
		}

		public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			CheckNotNull(selector, "selector");
			return source.SelectMany(selector, (TSource item, TResult subitem) => subitem);
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			CheckNotNull(collectionSelector, "collectionSelector");
			return source.SelectMany((TSource item, int i) => collectionSelector(item), resultSelector);
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			CheckNotNull(source, "source");
			CheckNotNull(collectionSelector, "collectionSelector");
			CheckNotNull(resultSelector, "resultSelector");
			return source.SelectManyYield(collectionSelector, resultSelector);
		}

		private static IEnumerable<TResult> SelectManyYield<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			int i = 0;
			foreach (TSource item in source)
			{
				foreach (TCollection subitem in collectionSelector(item, i++))
				{
					yield return resultSelector(item, subitem);
				}
			}
		}

		public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			CheckNotNull(predicate, "predicate");
			return source.TakeWhile((TSource item, int i) => predicate(item));
		}

		public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			CheckNotNull(source, "source");
			CheckNotNull(predicate, "predicate");
			return source.TakeWhileYield(predicate);
		}

		private static IEnumerable<TSource> TakeWhileYield<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int i = 0;
			foreach (TSource item in source)
			{
				if (predicate(item, i++))
				{
					yield return item;
					continue;
				}
				break;
			}
		}

		private static TSource FirstImpl<TSource>(this IEnumerable<TSource> source, Func<TSource> empty)
		{
			CheckNotNull(source, "source");
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				if (list.Count <= 0)
				{
					return empty();
				}
				return list[0];
			}
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				return enumerator.MoveNext() ? enumerator.Current : empty();
			}
		}

		public static TSource First<TSource>(this IEnumerable<TSource> source)
		{
			return source.FirstImpl(Futures<TSource>.Undefined);
		}

		public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).First();
		}

		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.FirstImpl(Futures<TSource>.Default);
		}

		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).FirstOrDefault();
		}

		private static TSource LastImpl<TSource>(this IEnumerable<TSource> source, Func<TSource> empty)
		{
			CheckNotNull(source, "source");
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				if (list.Count <= 0)
				{
					return empty();
				}
				return list[list.Count - 1];
			}
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					return empty();
				}
				TSource current = enumerator.Current;
				while (enumerator.MoveNext())
				{
					current = enumerator.Current;
				}
				return current;
			}
		}

		public static TSource Last<TSource>(this IEnumerable<TSource> source)
		{
			return source.LastImpl(Futures<TSource>.Undefined);
		}

		public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).Last();
		}

		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.LastImpl(Futures<TSource>.Default);
		}

		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).LastOrDefault();
		}

		private static TSource SingleImpl<TSource>(this IEnumerable<TSource> source, Func<TSource> empty)
		{
			CheckNotNull(source, "source");
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					TSource current = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						return current;
					}
					throw new InvalidOperationException();
				}
				return empty();
			}
		}

		public static TSource Single<TSource>(this IEnumerable<TSource> source)
		{
			return source.SingleImpl(Futures<TSource>.Undefined);
		}

		public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).Single();
		}

		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.SingleImpl(Futures<TSource>.Default);
		}

		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).SingleOrDefault();
		}

		public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
		{
			CheckNotNull(source, "source");
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, null);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return list[index];
			}
			try
			{
				return source.SkipWhile((TSource item, int i) => i < index).First();
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentOutOfRangeException("index", index, null);
			}
		}

		public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
		{
			CheckNotNull(source, "source");
			if (index < 0)
			{
				return default(TSource);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				if (index >= list.Count)
				{
					return default(TSource);
				}
				return list[index];
			}
			return source.SkipWhile((TSource item, int i) => i < index).FirstOrDefault();
		}

		public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
		{
			CheckNotNull(source, "source");
			return ReverseYield(source);
		}

		private static IEnumerable<TSource> ReverseYield<TSource>(IEnumerable<TSource> source)
		{
			Stack<TSource> stack = new Stack<TSource>();
			foreach (TSource item in source)
			{
				stack.Push(item);
			}
			foreach (TSource item2 in stack)
			{
				yield return item2;
			}
		}

		public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
		{
			return source.Where((TSource item, int i) => i < count);
		}

		public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
		{
			return source.Where((TSource item, int i) => i >= count);
		}

		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			CheckNotNull(predicate, "predicate");
			return source.SkipWhile((TSource item, int i) => predicate(item));
		}

		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			CheckNotNull(source, "source");
			CheckNotNull(predicate, "predicate");
			return SkipWhileYield(source, predicate);
		}

		private static IEnumerable<TSource> SkipWhileYield<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			using (IEnumerator<TSource> e = source.GetEnumerator())
			{
				int num = 0;
				while (true)
				{
					if (!e.MoveNext())
					{
						yield break;
					}
					if (!predicate(e.Current, num))
					{
						break;
					}
					num++;
				}
				do
				{
					yield return e.Current;
				}
				while (e.MoveNext());
			}
		}

		public static int Count<TSource>(this IEnumerable<TSource> source)
		{
			CheckNotNull(source, "source");
			ICollection collection = source as ICollection;
			if (collection == null)
			{
				return source.Aggregate(0, (int count, TSource item) => checked(count + 1));
			}
			return collection.Count;
		}

		public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).Count();
		}

		public static long LongCount<TSource>(this IEnumerable<TSource> source)
		{
			CheckNotNull(source, "source");
			Array array = source as Array;
			if (array == null)
			{
				return source.Aggregate(0L, (long count, TSource item) => count + 1);
			}
			return array.LongLength;
		}

		public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).LongCount();
		}

		public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			CheckNotNull(first, "first");
			CheckNotNull(second, "second");
			return ConcatYield(first, second);
		}

		private static IEnumerable<TSource> ConcatYield<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			foreach (TSource item in first)
			{
				yield return item;
			}
			foreach (TSource item2 in second)
			{
				yield return item2;
			}
		}

		public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			CheckNotNull(source, "source");
			return new List<TSource>(source);
		}

		public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
		{
			return source.ToList().ToArray();
		}

		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
		{
			return source.Distinct(null);
		}

		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			CheckNotNull(source, "source");
			return DistinctYield(source, comparer);
		}

		private static IEnumerable<TSource> DistinctYield<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			Dictionary<TSource, object> set = new Dictionary<TSource, object>(comparer);
			bool gotNull = false;
			foreach (TSource item in source)
			{
				if (item == null)
				{
					if (gotNull)
					{
						continue;
					}
					gotNull = true;
				}
				else
				{
					if (set.ContainsKey(item))
					{
						continue;
					}
					set.Add(item, null);
				}
				yield return item;
			}
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToLookup(keySelector, (TSource e) => e, null);
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return source.ToLookup(keySelector, (TSource e) => e, comparer);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToLookup(keySelector, elementSelector, null);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			CheckNotNull(keySelector, "keySelector");
			CheckNotNull(elementSelector, "elementSelector");
			Lookup<TKey, TElement> lookup = new Lookup<TKey, TElement>(comparer);
			foreach (TSource item in source)
			{
				TKey key = keySelector(item);
				Grouping<TKey, TElement> grouping = (Grouping<TKey, TElement>)lookup.Find(key);
				if (grouping == null)
				{
					grouping = new Grouping<TKey, TElement>(key);
					lookup.Add(grouping);
				}
				grouping.Add(elementSelector(item));
			}
			return lookup;
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.GroupBy(keySelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return source.GroupBy(keySelector, (TSource e) => e, comparer);
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.GroupBy(keySelector, elementSelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			CheckNotNull(keySelector, "keySelector");
			CheckNotNull(elementSelector, "elementSelector");
			return source.ToLookup(keySelector, elementSelector, comparer);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
		{
			return source.GroupBy(keySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			CheckNotNull(keySelector, "keySelector");
			CheckNotNull(resultSelector, "resultSelector");
			return from g in source.ToLookup(keySelector, comparer)
				select resultSelector(g.Key, g);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			return source.GroupBy(keySelector, elementSelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			CheckNotNull(keySelector, "keySelector");
			CheckNotNull(elementSelector, "elementSelector");
			CheckNotNull(resultSelector, "resultSelector");
			return from g in source.ToLookup(keySelector, elementSelector, comparer)
				select resultSelector(g.Key, g);
		}

		public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
		{
			CheckNotNull(source, "source");
			CheckNotNull(func, "func");
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new InvalidOperationException();
				}
				return enumerator.Renumerable().Skip(1).Aggregate(enumerator.Current, func);
			}
		}

		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			return source.Aggregate(seed, func, (TAccumulate r) => r);
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			CheckNotNull(source, "source");
			CheckNotNull(func, "func");
			CheckNotNull(resultSelector, "resultSelector");
			TAccumulate val = seed;
			foreach (TSource item in source)
			{
				val = func(val, item);
			}
			return resultSelector(val);
		}

		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Union(second, null);
		}

		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			return first.Concat(second).Distinct(comparer);
		}

		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
		{
			return source.DefaultIfEmpty(default(TSource));
		}

		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
		{
			CheckNotNull(source, "source");
			return DefaultIfEmptyYield(source, defaultValue);
		}

		private static IEnumerable<TSource> DefaultIfEmptyYield<TSource>(IEnumerable<TSource> source, TSource defaultValue)
		{
			using (IEnumerator<TSource> e = source.GetEnumerator())
			{
				if (!e.MoveNext())
				{
					yield return defaultValue;
					yield break;
				}
				do
				{
					yield return e.Current;
				}
				while (e.MoveNext());
			}
		}

		public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			CheckNotNull(source, "source");
			CheckNotNull(predicate, "predicate");
			foreach (TSource item in source)
			{
				if (!predicate(item))
				{
					return false;
				}
			}
			return true;
		}

		public static bool Any<TSource>(this IEnumerable<TSource> source)
		{
			CheckNotNull(source, "source");
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				return enumerator.MoveNext();
			}
		}

		public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).Any();
		}

		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
		{
			return source.Contains(value, null);
		}

		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			CheckNotNull(source, "source");
			if (comparer == null)
			{
				ICollection<TSource> collection = source as ICollection<TSource>;
				if (collection != null)
				{
					return collection.Contains(value);
				}
			}
			comparer = comparer ?? EqualityComparer<TSource>.Default;
			return source.Any((TSource item) => comparer.Equals(item, value));
		}

		public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.SequenceEqual(second, null);
		}

		public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			CheckNotNull(first, "frist");
			CheckNotNull(second, "second");
			comparer = comparer ?? EqualityComparer<TSource>.Default;
			using (IEnumerator<TSource> enumerator = first.GetEnumerator())
			{
				using (IEnumerator<TSource> enumerator2 = second.GetEnumerator())
				{
					do
					{
						if (!enumerator.MoveNext())
						{
							return !enumerator2.MoveNext();
						}
						if (!enumerator2.MoveNext())
						{
							return false;
						}
					}
					while (comparer.Equals(enumerator.Current, enumerator2.Current));
				}
			}
			return false;
		}

		private static TSource MinMaxImpl<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> lesser)
		{
			CheckNotNull(source, "source");
			return source.Aggregate((TSource a, TSource item) => (!lesser(a, item)) ? item : a);
		}

		private static TSource? MinMaxImpl<TSource>(this IEnumerable<TSource?> source, TSource? seed, Func<TSource?, TSource?, bool> lesser) where TSource : struct
		{
			CheckNotNull(source, "source");
			return source.Aggregate(seed, (TSource? a, TSource? item) => (!lesser(a, item)) ? item : a);
		}

		public static TSource Min<TSource>(this IEnumerable<TSource> source)
		{
			Comparer<TSource> comparer = Comparer<TSource>.Default;
			return source.MinMaxImpl((TSource x, TSource y) => comparer.Compare(x, y) < 0);
		}

		public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select(selector).Min();
		}

		public static TSource Max<TSource>(this IEnumerable<TSource> source)
		{
			Comparer<TSource> comparer = Comparer<TSource>.Default;
			return source.MinMaxImpl((TSource x, TSource y) => comparer.Compare(x, y) > 0);
		}

		public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select(selector).Max();
		}

		private static IEnumerable<T> Renumerable<T>(this IEnumerator<T> e)
		{
			do
			{
				yield return e.Current;
			}
			while (e.MoveNext());
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderBy(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			CheckNotNull(keySelector, "keySelector");
			return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, false);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderByDescending(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			CheckNotNull(source, "keySelector");
			return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, true);
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ThenBy(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			return source.CreateOrderedEnumerable(keySelector, comparer, false);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ThenByDescending(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			return source.CreateOrderedEnumerable(keySelector, comparer, true);
		}

		private static IEnumerable<TSource> IntersectExceptImpl<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer, bool flag)
		{
			CheckNotNull(first, "first");
			CheckNotNull(second, "second");
			List<TSource> list = new List<TSource>();
			Dictionary<TSource, bool> flags = new Dictionary<TSource, bool>(comparer);
			foreach (TSource item in first.Where((TSource k) => !flags.ContainsKey(k)))
			{
				flags.Add(item, !flag);
				list.Add(item);
			}
			foreach (TSource item2 in second.Where(flags.ContainsKey))
			{
				flags[item2] = flag;
			}
			return list.Where((TSource item) => flags[item]);
		}

		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Intersect(second, null);
		}

		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			return first.IntersectExceptImpl(second, comparer, true);
		}

		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Except(second, null);
		}

		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			return first.IntersectExceptImpl(second, comparer, false);
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToDictionary(keySelector, null);
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return source.ToDictionary(keySelector, (TSource e) => e);
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToDictionary(keySelector, elementSelector, null);
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			CheckNotNull(source, "source");
			CheckNotNull(keySelector, "keySelector");
			CheckNotNull(elementSelector, "elementSelector");
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);
			foreach (TSource item in source)
			{
				dictionary.Add(keySelector(item), elementSelector(item));
			}
			return dictionary;
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			CheckNotNull(outer, "outer");
			CheckNotNull(inner, "inner");
			CheckNotNull(outerKeySelector, "outerKeySelector");
			CheckNotNull(innerKeySelector, "innerKeySelector");
			CheckNotNull(resultSelector, "resultSelector");
			ILookup<TKey, TInner> lookup = inner.ToLookup(innerKeySelector, comparer);
			return from o in outer
				from i in lookup[outerKeySelector(o)]
				select resultSelector(o, i);
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			CheckNotNull(outer, "outer");
			CheckNotNull(inner, "inner");
			CheckNotNull(outerKeySelector, "outerKeySelector");
			CheckNotNull(innerKeySelector, "innerKeySelector");
			CheckNotNull(resultSelector, "resultSelector");
			ILookup<TKey, TInner> lookup = inner.ToLookup(innerKeySelector, comparer);
			return outer.Select((TOuter o) => resultSelector(o, lookup[outerKeySelector(o)]));
		}

		[DebuggerStepThrough]
		private static void CheckNotNull<T>(T value, string name) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static int Sum(this IEnumerable<int> source)
		{
			CheckNotNull(source, "source");
			int num = 0;
			foreach (int item in source)
			{
				num = checked(num + item);
			}
			return num;
		}

		public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			return source.Select(selector).Sum();
		}

		public static double Average(this IEnumerable<int> source)
		{
			CheckNotNull(source, "source");
			long num = 0L;
			long num2 = 0L;
			checked
			{
				foreach (int item in source)
				{
					num += item;
					num2++;
				}
				if (num2 == 0)
				{
					throw new InvalidOperationException();
				}
				return (double)num / (double)num2;
			}
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			return source.Select(selector).Average();
		}

		public static int? Sum(this IEnumerable<int?> source)
		{
			CheckNotNull(source, "source");
			int num = 0;
			foreach (int? item in source)
			{
				num = checked(num + (item ?? 0));
			}
			return num;
		}

		public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select(selector).Sum();
		}

		public static double? Average(this IEnumerable<int?> source)
		{
			CheckNotNull(source, "source");
			long num = 0L;
			long num2 = 0L;
			checked
			{
				foreach (int? item in source.Where((int? n) => n.HasValue))
				{
					num += item.Value;
					num2++;
				}
				if (num2 == 0)
				{
					return null;
				}
				return new double?(num) / (double)num2;
			}
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select(selector).Average();
		}

		public static int? Min(this IEnumerable<int?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((int? x) => x.HasValue).MinMaxImpl(null, (int? min, int? x) => min < x);
		}

		public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select(selector).Min();
		}

		public static int? Max(this IEnumerable<int?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((int? x) => x.HasValue).MinMaxImpl(null, (int? max, int? x) => !x.HasValue || (max.HasValue && x.Value < max.Value));
		}

		public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select(selector).Max();
		}

		public static long Sum(this IEnumerable<long> source)
		{
			CheckNotNull(source, "source");
			long num = 0L;
			foreach (long item in source)
			{
				num = checked(num + item);
			}
			return num;
		}

		public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			return source.Select(selector).Sum();
		}

		public static double Average(this IEnumerable<long> source)
		{
			CheckNotNull(source, "source");
			long num = 0L;
			long num2 = 0L;
			checked
			{
				foreach (long item in source)
				{
					num += item;
					num2++;
				}
				if (num2 == 0)
				{
					throw new InvalidOperationException();
				}
				return (double)num / (double)num2;
			}
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			return source.Select(selector).Average();
		}

		public static long? Sum(this IEnumerable<long?> source)
		{
			CheckNotNull(source, "source");
			long num = 0L;
			foreach (long? item in source)
			{
				num = checked(num + (item ?? 0));
			}
			return num;
		}

		public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select(selector).Sum();
		}

		public static double? Average(this IEnumerable<long?> source)
		{
			CheckNotNull(source, "source");
			long num = 0L;
			long num2 = 0L;
			checked
			{
				foreach (long? item in source.Where((long? n) => n.HasValue))
				{
					num += item.Value;
					num2++;
				}
				if (num2 == 0)
				{
					return null;
				}
				return new double?(num) / (double)num2;
			}
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select(selector).Average();
		}

		public static long? Min(this IEnumerable<long?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((long? x) => x.HasValue).MinMaxImpl(null, (long? min, long? x) => min < x);
		}

		public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select(selector).Min();
		}

		public static long? Max(this IEnumerable<long?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((long? x) => x.HasValue).MinMaxImpl(null, (long? max, long? x) => !x.HasValue || (max.HasValue && x.Value < max.Value));
		}

		public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select(selector).Max();
		}

		public static float Sum(this IEnumerable<float> source)
		{
			CheckNotNull(source, "source");
			float num = 0f;
			foreach (float item in source)
			{
				float num2 = item;
				num += num2;
			}
			return num;
		}

		public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			return source.Select(selector).Sum();
		}

		public static float Average(this IEnumerable<float> source)
		{
			CheckNotNull(source, "source");
			float num = 0f;
			long num2 = 0L;
			foreach (float item in source)
			{
				float num3 = item;
				num += num3;
				num2 = checked(num2 + 1);
			}
			if (num2 == 0)
			{
				throw new InvalidOperationException();
			}
			return num / (float)num2;
		}

		public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			return source.Select(selector).Average();
		}

		public static float? Sum(this IEnumerable<float?> source)
		{
			CheckNotNull(source, "source");
			float num = 0f;
			foreach (float? item in source)
			{
				num += item ?? 0f;
			}
			return num;
		}

		public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select(selector).Sum();
		}

		public static float? Average(this IEnumerable<float?> source)
		{
			CheckNotNull(source, "source");
			float num = 0f;
			long num2 = 0L;
			foreach (float? item in source.Where((float? n) => n.HasValue))
			{
				num += item.Value;
				num2 = checked(num2 + 1);
			}
			if (num2 == 0)
			{
				return null;
			}
			return new float?(num) / (float)num2;
		}

		public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select(selector).Average();
		}

		public static float? Min(this IEnumerable<float?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((float? x) => x.HasValue).MinMaxImpl(null, (float? min, float? x) => min < x);
		}

		public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select(selector).Min();
		}

		public static float? Max(this IEnumerable<float?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((float? x) => x.HasValue).MinMaxImpl(null, (float? max, float? x) => !x.HasValue || (max.HasValue && x.Value < max.Value));
		}

		public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select(selector).Max();
		}

		public static double Sum(this IEnumerable<double> source)
		{
			CheckNotNull(source, "source");
			double num = 0.0;
			foreach (double item in source)
			{
				double num2 = item;
				num += num2;
			}
			return num;
		}

		public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			return source.Select(selector).Sum();
		}

		public static double Average(this IEnumerable<double> source)
		{
			CheckNotNull(source, "source");
			double num = 0.0;
			long num2 = 0L;
			foreach (double item in source)
			{
				double num3 = item;
				num += num3;
				num2 = checked(num2 + 1);
			}
			if (num2 == 0)
			{
				throw new InvalidOperationException();
			}
			return num / (double)num2;
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			return source.Select(selector).Average();
		}

		public static double? Sum(this IEnumerable<double?> source)
		{
			CheckNotNull(source, "source");
			double num = 0.0;
			foreach (double? item in source)
			{
				num += item ?? 0.0;
			}
			return num;
		}

		public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select(selector).Sum();
		}

		public static double? Average(this IEnumerable<double?> source)
		{
			CheckNotNull(source, "source");
			double num = 0.0;
			long num2 = 0L;
			foreach (double? item in source.Where((double? n) => n.HasValue))
			{
				num += item.Value;
				num2 = checked(num2 + 1);
			}
			if (num2 == 0)
			{
				return null;
			}
			return new double?(num) / (double)num2;
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select(selector).Average();
		}

		public static double? Min(this IEnumerable<double?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((double? x) => x.HasValue).MinMaxImpl(null, (double? min, double? x) => min < x);
		}

		public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select(selector).Min();
		}

		public static double? Max(this IEnumerable<double?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((double? x) => x.HasValue).MinMaxImpl(null, (double? max, double? x) => !x.HasValue || (max.HasValue && x.Value < max.Value));
		}

		public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select(selector).Max();
		}

		public static decimal Sum(this IEnumerable<decimal> source)
		{
			CheckNotNull(source, "source");
			decimal result = 0m;
			foreach (decimal item in source)
			{
				result += item;
			}
			return result;
		}

		public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select(selector).Sum();
		}

		public static decimal Average(this IEnumerable<decimal> source)
		{
			CheckNotNull(source, "source");
			decimal num = 0m;
			long num2 = 0L;
			foreach (decimal item in source)
			{
				num += item;
				num2 = checked(num2 + 1);
			}
			if (num2 == 0)
			{
				throw new InvalidOperationException();
			}
			return num / (decimal)num2;
		}

		public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select(selector).Average();
		}

		public static decimal? Sum(this IEnumerable<decimal?> source)
		{
			CheckNotNull(source, "source");
			decimal value = 0m;
			foreach (decimal? item in source)
			{
				value += item ?? 0m;
			}
			return value;
		}

		public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select(selector).Sum();
		}

		public static decimal? Average(this IEnumerable<decimal?> source)
		{
			CheckNotNull(source, "source");
			decimal value = 0m;
			long num = 0L;
			foreach (decimal? item in source.Where((decimal? n) => n.HasValue))
			{
				value += item.Value;
				num = checked(num + 1);
			}
			if (num == 0)
			{
				return null;
			}
			return (decimal?)value / (decimal?)num;
		}

		public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select(selector).Average();
		}

		public static decimal? Min(this IEnumerable<decimal?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((decimal? x) => x.HasValue).MinMaxImpl(null, (decimal? min, decimal? x) => min < x);
		}

		public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select(selector).Min();
		}

		public static decimal? Max(this IEnumerable<decimal?> source)
		{
			CheckNotNull(source, "source");
			return source.Where((decimal? x) => x.HasValue).MinMaxImpl(null, (decimal? max, decimal? x) => !x.HasValue || (max.HasValue && x.Value < max.Value));
		}

		public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select(selector).Max();
		}
	}
}
