namespace Analytics.Parameters
{
	public abstract class IntParameter : EventParameter
	{
		public IntParameter(int amount)
		{
			base.Value = amount;
		}
	}
}
