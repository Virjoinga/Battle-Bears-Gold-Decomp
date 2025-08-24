using Analytics.Parameters.Collections;

namespace Analytics.Parameters
{
	public class ItemParameters : IEventParameterEnumerable
	{
		public ItemParameters(ItemAmountParameter amountParameter, ItemNameParameter nameParameter, ItemTypeParameter typeParameter)
			: base(amountParameter, nameParameter, typeParameter)
		{
		}

		public ItemParameters()
		{
		}
	}
}
