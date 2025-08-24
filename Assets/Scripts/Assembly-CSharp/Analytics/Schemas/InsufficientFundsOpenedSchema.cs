using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class InsufficientFundsOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.insufficientFundsOpened;
			}
		}

		public InsufficientFundsOpenedSchema(ItemNameParameter itemName, VirtualCurrencyParameters virtualCurrency, CurrentGasParameter currentGas = null, CurrentJoulesParameter currentJoules = null)
		{
			_parameters.Add(itemName);
			_parameters.AddRange(virtualCurrency);
			if (currentGas != null)
			{
				_parameters.Add(currentGas);
			}
			if (currentJoules != null)
			{
				_parameters.Add(currentJoules);
			}
		}
	}
}
