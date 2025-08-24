namespace Analytics.Parameters
{
	public abstract class BooleanParameter : EventParameter
	{
		public BooleanParameter(bool value)
		{
			base.Value = value;
		}
	}
}
