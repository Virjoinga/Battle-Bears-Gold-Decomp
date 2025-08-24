using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities.LinqBridge
{
	[Serializable]
	internal struct Tuple<TFirst, TSecond> : IEquatable<Tuple<TFirst, TSecond>>
	{
		public TFirst First { get; private set; }

		public TSecond Second { get; private set; }

		public Tuple(TFirst first, TSecond second)
		{
			this = default(Tuple<TFirst, TSecond>);
			First = first;
			Second = second;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Tuple<TFirst, TSecond>)
			{
				return ((ValueType)this).Equals((object)(Tuple<TFirst, TSecond>)obj);
			}
			return false;
		}

		public bool Equals(Tuple<TFirst, TSecond> other)
		{
			if (EqualityComparer<TFirst>.Default.Equals(other.First, First))
			{
				return EqualityComparer<TSecond>.Default.Equals(other.Second, Second);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 2049903426;
			num = -1521134295 * num + EqualityComparer<TFirst>.Default.GetHashCode(First);
			return -1521134295 * num + EqualityComparer<TSecond>.Default.GetHashCode(Second);
		}

		public override string ToString()
		{
			return string.Format("{{ First = {0}, Second = {1} }}", First, Second);
		}
	}
}
