using System;
using System.Reflection;

namespace SkyVu.Common.JsonParser
{
	internal struct PropertyMetadata
	{
		public MemberInfo Info;

		public bool IsField;

		public Type Type;
	}
}
