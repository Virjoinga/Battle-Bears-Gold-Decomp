using System.Collections.Generic;
using System.Linq;
using Analytics.Parameters.Collections;

namespace Analytics.Parameters
{
	public class TeamsParameters : IEventParameterEnumerable
	{
		public TeamsParameters(MyTeamCountParameter myTeamCount, MyTeamLevelParameter myTeamLevel, MyTeamSkillParameter myTeamSkill, OpposingTeamCountParameter opposingTeamCount, OpposingTeamLevelParameter opposingTeamLevel, OpposingTeamSkillParameter opposingTeamSkill)
			: base(myTeamCount, myTeamLevel, myTeamSkill, opposingTeamCount, opposingTeamLevel, opposingTeamSkill)
		{
		}

		public TeamsParameters(List<PlayerCharacterManager> players, int myId)
		{
			List<IEventParameter> list = new List<IEventParameter>();
			PlayerCharacterManager playerCharacterManager = players.First((PlayerCharacterManager p) => p.OwnerID == myId);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			foreach (PlayerCharacterManager player in players)
			{
				if (player.team == playerCharacterManager.team)
				{
					num++;
					num2 += player.skill;
					num3 += player.level;
				}
				else
				{
					num4++;
					num6 += player.skill;
					num5 += player.level;
				}
			}
			list.Add(new MyTeamCountParameter(num));
			list.Add(new MyTeamLevelParameter(num3));
			list.Add(new MyTeamSkillParameter(num2));
			list.Add(new OpposingTeamCountParameter(num4));
			list.Add(new OpposingTeamLevelParameter(num5));
			list.Add(new OpposingTeamSkillParameter(num6));
			_eventParameters = list.ToArray();
		}

		public TeamsParameters(List<Report.Player> players, int myTeam)
		{
			List<IEventParameter> list = new List<IEventParameter>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			foreach (Report.Player player in players)
			{
				if (player.team_id == myTeam)
				{
					num++;
					num2 += (int)player.skill;
					num3 += player.level;
				}
				else
				{
					num4++;
					num6 += (int)player.skill;
					num5 += player.level;
				}
			}
			list.Add(new MyTeamCountParameter(num));
			list.Add(new MyTeamLevelParameter(num3));
			list.Add(new MyTeamSkillParameter(num2));
			list.Add(new OpposingTeamCountParameter(num4));
			list.Add(new OpposingTeamLevelParameter(num5));
			list.Add(new OpposingTeamSkillParameter(num6));
			_eventParameters = list.ToArray();
		}
	}
}
