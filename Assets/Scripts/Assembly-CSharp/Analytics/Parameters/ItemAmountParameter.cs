namespace Analytics.Parameters
{
	public class ItemAmountParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.itemAmount;
			}
		}

		public ItemAmountParameter(int amount)
			: base(amount)
		{
		}
	}
}
