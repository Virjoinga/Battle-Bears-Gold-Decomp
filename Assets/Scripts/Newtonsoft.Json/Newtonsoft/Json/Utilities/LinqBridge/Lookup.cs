using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities.LinqBridge
{
	internal sealed class Lookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable<IGrouping<TKey, TElement>>, IEnumerable
	{
		private readonly Dictionary<TKey, IGrouping<TKey, TElement>> _map;

		public int Count
		{
			get
			{
				return _map.Count;
			}
		}

		public IEnumerable<TElement> this[TKey key]
		{
			get
			{
				IGrouping<TKey, TElement> value;
				if (!_map.TryGetValue(key, out value))
				{
					return Enumerable.Empty<TElement>();
				}
				return value;
			}
		}

		internal Lookup(IEqualityComparer<TKey> comparer)
		{
			_map = new Dictionary<TKey, IGrouping<TKey, TElement>>(comparer);
		}

		internal void Add(IGrouping<TKey, TElement> item)
		{
			_map.Add(item.Key, item);
		}

		internal IEnumerable<TElement> Find(TKey key)
		{
			IGrouping<TKey, TElement> value;
			if (!_map.TryGetValue(key, out value))
			{
				return null;
			}
			return value;
		}

		public bool Contains(TKey key)
		{
			return _map.ContainsKey(key);
		}

		public IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			foreach (KeyValuePair<TKey, IGrouping<TKey, TElement>> pair in _map)
			{
				KeyValuePair<TKey, IGrouping<TKey, TElement>> keyValuePair = pair;
				TKey key = keyValuePair.Key;
				KeyValuePair<TKey, IGrouping<TKey, TElement>> keyValuePair2 = pair;
				yield return resultSelector(key, keyValuePair2.Value);
			}
		}

		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
		{
			return _map.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
