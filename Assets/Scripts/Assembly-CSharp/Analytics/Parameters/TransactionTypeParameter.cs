namespace Analytics.Parameters
{
	public class TransactionTypeParameter : StringParameter
	{
		public enum Type
		{
			SALE = 0,
			PURCHASE = 1,
			TRADE = 2
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.transactionType;
			}
		}

		public TransactionTypeParameter(Type type)
			: base(type.ToString())
		{
		}
	}
}
