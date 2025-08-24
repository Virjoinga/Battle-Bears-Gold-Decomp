namespace Analytics.Parameters
{
	public class LoadoutSlotParameter : StringParameter
	{
		public enum Slot
		{
			LOADOUT_1 = 1,
			LOADOUT_2 = 2,
			LOADOUT_3 = 3
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.loadoutSlot;
			}
		}

		public LoadoutSlotParameter(int slotNumber)
			: base(((Slot)slotNumber).ToString())
		{
		}
	}
}
