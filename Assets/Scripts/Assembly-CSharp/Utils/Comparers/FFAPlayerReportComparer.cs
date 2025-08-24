using System.Collections.Generic;

namespace Utils.Comparers
{
	public class FFAPlayerReportComparer : IComparer<Report.Player>
	{
		public int Compare(Report.Player a, Report.Player b)
		{
			if (a.hasLeft && !b.hasLeft)
			{
				return 1;
			}
			if (!a.hasLeft && b.hasLeft)
			{
				return -1;
			}
			if (a.TotalKills < b.TotalKills)
			{
				return 1;
			}
			if (a.TotalKills > b.TotalKills)
			{
				return -1;
			}
			if (a.TotalDeaths < b.TotalDeaths)
			{
				return -1;
			}
			if (a.TotalDeaths > b.TotalDeaths)
			{
				return 1;
			}
			if (a.TotalDamageDealt < b.TotalDamageDealt)
			{
				return 1;
			}
			if (a.TotalDamageDealt > b.TotalDamageDealt)
			{
				return -1;
			}
			if (a.id < b.id)
			{
				return -1;
			}
			if (a.id > b.id)
			{
				return 1;
			}
			return 0;
		}
	}
}
