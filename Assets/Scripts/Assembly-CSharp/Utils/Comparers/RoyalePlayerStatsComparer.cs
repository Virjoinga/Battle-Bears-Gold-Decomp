using System.Collections.Generic;

namespace Utils.Comparers
{
	public class RoyalePlayerStatsComparer : IComparer<PlayerStats>
	{
		private readonly Dictionary<int, int> _deathTimesById;

		public RoyalePlayerStatsComparer(Dictionary<int, int> deathTimesById)
		{
			_deathTimesById = deathTimesById;
		}

		public int Compare(PlayerStats a, PlayerStats b)
		{
			if (!_deathTimesById.ContainsKey(a.id) && _deathTimesById.ContainsKey(b.id))
			{
				return -1;
			}
			if (_deathTimesById.ContainsKey(a.id) && !_deathTimesById.ContainsKey(b.id))
			{
				return 1;
			}
			if (_deathTimesById.ContainsKey(a.id) && _deathTimesById.ContainsKey(b.id))
			{
				if (_deathTimesById[a.id] > _deathTimesById[b.id])
				{
					return -1;
				}
				if (_deathTimesById[a.id] < _deathTimesById[b.id])
				{
					return 1;
				}
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
