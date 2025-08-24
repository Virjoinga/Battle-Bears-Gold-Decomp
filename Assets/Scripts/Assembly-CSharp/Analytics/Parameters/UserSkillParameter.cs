namespace Analytics.Parameters
{
	public class UserSkillParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.userSkill;
			}
		}

		public UserSkillParameter(double amount)
			: this((int)amount)
		{
		}

		public UserSkillParameter(int amount)
			: base(amount)
		{
		}
	}
}
