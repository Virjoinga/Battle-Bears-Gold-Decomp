using System.Collections;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class Hashtable : Dictionary<object, object>
	{
		public new object this[object key]
		{
			get
			{
				object value = null;
				TryGetValue(key, out value);
				return value;
			}
			set
			{
				base[key] = value;
			}
		}

		public Hashtable()
		{
		}

		public Hashtable(int x)
			: base(x)
		{
		}

		public new IEnumerator GetEnumerator()
		{
			return ((IDictionary)this).GetEnumerator();
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (object key in base.Keys)
			{
				list.Add(string.Concat("(", key.GetType(), ")", key, "=(", key.GetType(), ")", this[key]));
			}
			return string.Join(", ", list.ToArray());
		}
	}
}
