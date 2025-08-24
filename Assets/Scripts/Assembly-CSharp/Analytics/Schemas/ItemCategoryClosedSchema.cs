using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ItemCategoryClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.itemCategoryClosed;
			}
		}

		public ItemCategoryClosedSchema(ItemTypeParameter itemType)
			: base(itemType)
		{
		}
	}
}
