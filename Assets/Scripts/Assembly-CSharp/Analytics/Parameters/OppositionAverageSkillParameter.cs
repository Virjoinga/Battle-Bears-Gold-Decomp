namespace Analytics.Parameters
{
	public class OppositionAverageSkillParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.oppositionAverageSkill;
			}
		}

		public OppositionAverageSkillParameter(int amount)
			: base(amount)
		{
		}
	}
}
