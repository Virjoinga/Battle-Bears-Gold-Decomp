using System;

namespace Newtonsoft.Json.Utilities
{
	internal class Convertible
	{
		private object _underlyingValue;

		public Convertible(object o)
		{
			_underlyingValue = o;
		}

		public TypeCode GetTypeCode()
		{
			return ConvertUtils.GetTypeCode(_underlyingValue);
		}

		public bool ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(_underlyingValue, provider);
		}

		public byte ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(_underlyingValue, provider);
		}

		public char ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(_underlyingValue, provider);
		}

		public DateTime ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(_underlyingValue, provider);
		}

		public decimal ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(_underlyingValue, provider);
		}

		public double ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(_underlyingValue, provider);
		}

		public short ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(_underlyingValue, provider);
		}

		public int ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(_underlyingValue, provider);
		}

		public long ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(_underlyingValue, provider);
		}

		public sbyte ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(_underlyingValue, provider);
		}

		public float ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(_underlyingValue, provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return Convert.ToString(_underlyingValue, provider);
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			return Convert.ChangeType(_underlyingValue, conversionType, provider);
		}

		public ushort ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(_underlyingValue, provider);
		}

		public uint ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(_underlyingValue, provider);
		}

		public ulong ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(_underlyingValue, provider);
		}
	}
}
