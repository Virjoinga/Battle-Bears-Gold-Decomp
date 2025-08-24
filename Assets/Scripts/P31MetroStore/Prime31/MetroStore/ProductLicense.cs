using System;

namespace Prime31.MetroStore
{
	public class ProductLicense
	{
		public DateTimeOffset expirationDate { get; set; }

		public bool isActive { get; set; }

		public string productId { get; set; }

		public new string ToString()
		{
			return null;
		}
	}
}
