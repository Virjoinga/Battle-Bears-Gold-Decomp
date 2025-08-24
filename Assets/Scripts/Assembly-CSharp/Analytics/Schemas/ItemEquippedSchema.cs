using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ItemEquippedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.itemEquipped;
			}
		}

		public ItemEquippedSchema(ItemNameParameter itemName, ItemTypeParameter itemType)
			: base(itemName, itemType)
		{
		}
	}
}
