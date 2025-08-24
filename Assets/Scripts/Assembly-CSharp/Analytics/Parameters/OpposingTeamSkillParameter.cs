namespace Analytics.Parameters
{
	public class OpposingTeamSkillParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.opposingTeamSkill;
			}
		}

		public OpposingTeamSkillParameter(int amount)
			: base(amount)
		{
		}
	}
}
