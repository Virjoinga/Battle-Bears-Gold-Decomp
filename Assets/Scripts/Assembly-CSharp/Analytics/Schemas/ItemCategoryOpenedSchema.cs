using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ItemCategoryOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.itemCategoryOpened;
			}
		}

		public ItemCategoryOpenedSchema(ItemTypeParameter itemType)
			: base(itemType)
		{
		}
	}
}
