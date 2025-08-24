using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ItemSelectedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.itemSelected;
			}
		}

		public ItemSelectedSchema(ItemNameParameter itemName, ItemTypeParameter itemType)
			: base(itemName, itemType)
		{
		}
	}
}
