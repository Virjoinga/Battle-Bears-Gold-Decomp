using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ItemDescriptionOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.itemDescriptionOpened;
			}
		}

		public ItemDescriptionOpenedSchema(ItemNameParameter itemName, ItemTypeParameter itemType)
			: base(itemName, itemType)
		{
		}
	}
}
