namespace Analytics.Parameters
{
	public abstract class StringParameter : EventParameter
	{
		public StringParameter(string value)
		{
			base.Value = value;
		}
	}
}
