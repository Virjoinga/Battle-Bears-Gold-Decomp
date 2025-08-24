namespace Analytics.Parameters
{
	public class ControlSchemeParameter : StringParameter
	{
		public enum Scheme
		{
			DEFAULT = 0,
			CUSTOM = 1
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.controlScheme;
			}
		}

		public ControlSchemeParameter(Scheme scheme)
			: base(scheme.ToString())
		{
		}
	}
}
