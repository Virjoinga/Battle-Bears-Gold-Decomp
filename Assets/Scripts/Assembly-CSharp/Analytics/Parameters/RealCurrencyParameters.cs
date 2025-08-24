using Analytics.Parameters.Collections;

namespace Analytics.Parameters
{
	public class RealCurrencyParameters : IEventParameterEnumerable
	{
		public RealCurrencyParameters(RealCurrencyAmountParameter amountParameter, RealCurrencyTypeParameter typeParameter)
			: base(amountParameter, typeParameter)
		{
		}

		public RealCurrencyParameters()
		{
		}
	}
}
