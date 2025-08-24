using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities.LinqBridge
{
	internal interface IGrouping<TKey, TElement> : IEnumerable<TElement>, IEnumerable
	{
		TKey Key { get; }
	}
}
