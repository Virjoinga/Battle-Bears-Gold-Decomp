namespace Analytics.Parameters
{
	public class ProModeStateParameter : StringParameter
	{
		public enum State
		{
			NONE = 0,
			JUMP = 1,
			RADAR = 2,
			JUMP_AND_RADAR = 3
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.proModeState;
			}
		}

		public ProModeStateParameter(bool ownsJump, bool ownsRadar)
			: base(FromOwnership(ownsJump, ownsRadar).ToString())
		{
		}

		private static State FromOwnership(bool ownsJump, bool ownsRadar)
		{
			if (ownsJump && ownsRadar)
			{
				return State.JUMP_AND_RADAR;
			}
			if (!ownsJump && ownsRadar)
			{
				return State.RADAR;
			}
			if (ownsJump && !ownsRadar)
			{
				return State.JUMP;
			}
			return State.NONE;
		}
	}
}
