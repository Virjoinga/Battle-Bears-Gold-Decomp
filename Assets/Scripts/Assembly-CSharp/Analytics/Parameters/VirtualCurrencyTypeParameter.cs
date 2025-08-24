using System;

namespace Analytics.Parameters
{
	public class VirtualCurrencyTypeParameter : StringParameter
	{
		public enum CurrencyType
		{
			GRIND = 0,
			PREMIUM = 1,
			PREMIUM_GRIND = 2
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.virtualCurrencyType;
			}
		}

		public VirtualCurrencyTypeParameter(VirtualCurrencyNameParameter.CurrencyName currency)
			: base(TypeForName(currency).ToString())
		{
		}

		private static CurrencyType TypeForName(VirtualCurrencyNameParameter.CurrencyName currencyName)
		{
			switch (currencyName)
			{
			case VirtualCurrencyNameParameter.CurrencyName.GAS:
				return CurrencyType.PREMIUM;
			case VirtualCurrencyNameParameter.CurrencyName.JOULES:
				return CurrencyType.GRIND;
			default:
				throw new Exception("No currency type defined for currency " + currencyName);
			}
		}
	}
}
