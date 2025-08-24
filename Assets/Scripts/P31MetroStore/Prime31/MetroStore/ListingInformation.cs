using System.Collections.Generic;

namespace Prime31.MetroStore
{
	public class ListingInformation
	{
		public uint ageRating { get; set; }

		public string currentMarket { get; set; }

		public string description { get; set; }

		public string formattedPrice { get; set; }

		public string name { get; set; }

		public Dictionary<string, ProductListing> productListings { get; set; }
	}
}
