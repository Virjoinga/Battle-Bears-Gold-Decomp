public interface IIAPButton
{
	string RealMoneyPrice { get; set; }

	string CurrencyCode { get; set; }

	int InGameCurrencyValue { get; set; }

	IAPButtonCurrencyTypes CurrencyType { get; set; }

	string IAPProductID { get; set; }

	bool Disabled { set; }

	bool BestDeal { set; }
}
