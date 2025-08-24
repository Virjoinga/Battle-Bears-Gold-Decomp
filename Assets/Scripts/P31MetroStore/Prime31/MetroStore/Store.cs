using System;

namespace Prime31.MetroStore
{
	public static class Store
	{
		public static event Action licenseChangedEvent;

		public static void loadTestingLicenseXmlFile(string relativePathToFile, Action<ListingInformation> completionHandler)
		{
		}

		public static void loadListingInformation(Action<ListingInformation> completionHandler)
		{
		}

		public static LicenseInformation getLicenseInformation()
		{
			return null;
		}

		public static void requestAppPurchase()
		{
		}

		public static string getAppReceipt()
		{
			return null;
		}

		public static void requestProductPurchase(string productId)
		{
		}

		public static ProductLicense getProductLicense(string productId)
		{
			return null;
		}

		public static string getProductReceipt(string productId)
		{
			return null;
		}
	}
}
