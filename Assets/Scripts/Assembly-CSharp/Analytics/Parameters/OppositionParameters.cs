using System.Collections.Generic;
using Analytics.Parameters.Collections;
using UnityEngine;

namespace Analytics.Parameters
{
	public class OppositionParameters : IEventParameterEnumerable
	{
		public OppositionParameters(OppositionAverageLevelParameter oppositionAverageLevel, OppositionAverageSkillParameter oppositionAverageSkill)
			: base(oppositionAverageLevel, oppositionAverageSkill)
		{
		}

		public OppositionParameters(List<PlayerCharacterManager> players, int myId)
		{
			List<IEventParameter> list = new List<IEventParameter>();
			int num = 0;
			float num2 = 0f;
			float num3 = 0f;
			foreach (PlayerCharacterManager player in players)
			{
				if (player.OwnerID != myId)
				{
					num++;
					num3 += (float)player.skill;
					num2 += (float)player.level;
				}
			}
			list.Add(new OppositionAverageLevelParameter(Mathf.RoundToInt(num2 / (float)num)));
			list.Add(new OppositionAverageSkillParameter(Mathf.RoundToInt(num3 / (float)num)));
			_eventParameters = list.ToArray();
		}

		public OppositionParameters(List<Report.Player> players, int myId)
		{
			List<IEventParameter> list = new List<IEventParameter>();
			int num = 0;
			float num2 = 0f;
			float num3 = 0f;
			foreach (Report.Player player in players)
			{
				if (player.id != myId)
				{
					num++;
					num3 += player.skill;
					num2 += (float)player.level;
				}
			}
			list.Add(new OppositionAverageLevelParameter(Mathf.RoundToInt(num2 / (float)num)));
			list.Add(new OppositionAverageSkillParameter(Mathf.RoundToInt(num3 / (float)num)));
			_eventParameters = list.ToArray();
		}
	}
}
