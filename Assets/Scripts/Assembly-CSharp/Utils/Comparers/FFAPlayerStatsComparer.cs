using System.Collections.Generic;

namespace Utils.Comparers
{
	public class FFAPlayerStatsComparer : IComparer<PlayerStats>
	{
		public int Compare(PlayerStats a, PlayerStats b)
		{
			if (a.hasLeft && !b.hasLeft)
			{
				return 1;
			}
			if (!a.hasLeft && b.hasLeft)
			{
				return -1;
			}
			if (a.NetKills < b.NetKills)
			{
				return 1;
			}
			if (a.NetKills > b.NetKills)
			{
				return -1;
			}
			if (a.NumDeaths < b.NumDeaths)
			{
				return -1;
			}
			if (a.NumDeaths > b.NumDeaths)
			{
				return 1;
			}
			if (a.NetDamage < b.NetDamage)
			{
				return 1;
			}
			if (a.NetDamage > b.NetDamage)
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
