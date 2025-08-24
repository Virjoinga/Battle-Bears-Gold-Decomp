using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities.LinqBridge
{
	internal interface IOrderedEnumerable<TElement> : IEnumerable<TElement>, IEnumerable
	{
		IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
	}
}
