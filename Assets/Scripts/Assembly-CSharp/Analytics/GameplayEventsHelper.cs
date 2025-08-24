using Analytics.Parameters;
using Analytics.Schemas;

namespace Analytics
{
	public class GameplayEventsHelper : EventHelper
	{
		public static PlayerDiedSchema PlayerDied(bool isSuicide, PlayerCharacterManager killer)
		{
			Stats stats = ServiceManager.Instance.GetStats();
			return new PlayerDiedSchema(new IsSuicideParameter(isSuicide), new UserLevelParameter(stats.level), new UserSkillParameter(stats.skill), (!isSuicide) ? new KillerLoadoutParameters(killer.playerLoadout) : null, (!isSuicide) ? new KillerLevelParameter(killer.level) : null, (!isSuicide) ? new KillerSkillParameter(killer.skill) : null);
		}
	}
}
