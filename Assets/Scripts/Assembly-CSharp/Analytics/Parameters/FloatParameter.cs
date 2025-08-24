namespace Analytics.Parameters
{
	public abstract class FloatParameter : EventParameter
	{
		public FloatParameter(float amount)
		{
			base.Value = amount;
		}
	}
}
