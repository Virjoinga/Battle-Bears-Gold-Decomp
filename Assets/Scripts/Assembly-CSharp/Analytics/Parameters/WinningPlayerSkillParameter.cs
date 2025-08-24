namespace Analytics.Parameters
{
	public class WinningPlayerSkillParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.winningPlayerSkill;
			}
		}

		public WinningPlayerSkillParameter(int amount)
			: base(amount)
		{
		}
	}
}
