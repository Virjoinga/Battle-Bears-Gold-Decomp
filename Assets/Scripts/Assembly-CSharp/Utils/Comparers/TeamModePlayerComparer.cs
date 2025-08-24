using System.Collections.Generic;

namespace Utils.Comparers
{
	public class TeamModePlayerComparer : IComparer<Report.Player>
	{
		public int Compare(Report.Player a, Report.Player b)
		{
			if (a.TotalKills - a.TotalDeaths < b.TotalKills - b.TotalDeaths)
			{
				return 1;
			}
			if (a.TotalKills - a.TotalDeaths > b.TotalKills - b.TotalDeaths)
			{
				return -1;
			}
			if (a.TotalKills - a.TotalDeaths == b.TotalKills - b.TotalDeaths)
			{
				if (a.TotalDamageDealt < b.TotalDamageDealt)
				{
					return 1;
				}
				if (a.TotalDamageDealt > b.TotalDamageDealt)
				{
					return -1;
				}
			}
			return 0;
		}
	}
}
