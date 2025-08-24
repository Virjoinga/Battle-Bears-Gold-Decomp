using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class EnergyRefillAttemptedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.energyRefillAttempted;
			}
		}

		public EnergyRefillAttemptedSchema(VirtualCurrencyAmountParameter currentGasAmount)
			: base(currentGasAmount)
		{
			_parameters.Add(new VirtualCurrencyNameParameter(VirtualCurrencyNameParameter.CurrencyName.GAS));
			_parameters.Add(new VirtualCurrencyTypeParameter(VirtualCurrencyNameParameter.CurrencyName.GAS));
		}
	}
}
