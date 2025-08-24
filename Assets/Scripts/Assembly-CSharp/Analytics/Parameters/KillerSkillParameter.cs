namespace Analytics.Parameters
{
	public class KillerSkillParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.killerSkill;
			}
		}

		public KillerSkillParameter(int amount)
			: base(amount)
		{
		}
	}
}
