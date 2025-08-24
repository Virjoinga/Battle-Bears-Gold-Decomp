using Analytics.Parameters.Collections;

namespace Analytics.Parameters
{
	public class VirtualCurrencyParameters : IEventParameterEnumerable
	{
		public VirtualCurrencyParameters(VirtualCurrencyAmountParameter amountParameter, VirtualCurrencyNameParameter nameParameter, VirtualCurrencyTypeParameter typeParameter)
			: base(amountParameter, nameParameter, typeParameter)
		{
		}

		public VirtualCurrencyParameters()
		{
		}
	}
}
