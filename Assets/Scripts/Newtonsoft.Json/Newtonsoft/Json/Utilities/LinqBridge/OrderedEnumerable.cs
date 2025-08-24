using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities.LinqBridge
{
	internal sealed class OrderedEnumerable<T, K> : IOrderedEnumerable<T>, IEnumerable<T>, IEnumerable
	{
		private readonly IEnumerable<T> _source;

		private readonly List<Comparison<T>> _comparisons;

		public OrderedEnumerable(IEnumerable<T> source, Func<T, K> keySelector, IComparer<K> comparer, bool descending)
			: this(source, (List<Comparison<T>>)null, keySelector, comparer, descending)
		{
		}

		private OrderedEnumerable(IEnumerable<T> source, List<Comparison<T>> comparisons, Func<T, K> keySelector, IComparer<K> comparer, bool descending)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			_source = source;
			comparer = comparer ?? Comparer<K>.Default;
			if (comparisons == null)
			{
				comparisons = new List<Comparison<T>>(4);
			}
			comparisons.Add((T x, T y) => ((!descending) ? 1 : (-1)) * comparer.Compare(keySelector(x), keySelector(y)));
			_comparisons = comparisons;
		}

		public IOrderedEnumerable<T> CreateOrderedEnumerable<KK>(Func<T, KK> keySelector, IComparer<KK> comparer, bool descending)
		{
			return new OrderedEnumerable<T, KK>(_source, _comparisons, keySelector, comparer, descending);
		}

		public IEnumerator<T> GetEnumerator()
		{
			List<Tuple<T, int>> list = _source.Select(TagPosition).ToList();
			list.Sort(delegate(Tuple<T, int> x, Tuple<T, int> y)
			{
				List<Comparison<T>> comparisons = _comparisons;
				for (int i = 0; i < comparisons.Count; i++)
				{
					int num = comparisons[i](x.First, y.First);
					if (num != 0)
					{
						return num;
					}
				}
				return x.Second.CompareTo(y.Second);
			});
			return list.Select(GetFirst).GetEnumerator();
		}

		private static Tuple<T, int> TagPosition(T e, int i)
		{
			return new Tuple<T, int>(e, i);
		}

		private static T GetFirst(Tuple<T, int> pv)
		{
			return pv.First;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
