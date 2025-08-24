namespace Analytics.Parameters
{
	public class MyTeamSkillParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myTeamSkill;
			}
		}

		public MyTeamSkillParameter(int amount)
			: base(amount)
		{
		}
	}
}
