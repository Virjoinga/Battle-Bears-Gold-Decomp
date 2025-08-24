using System.Collections.Generic;

namespace Utils.Comparers
{
	public class RoyalePlayerReportComparer : IComparer<Report.Player>
	{
		private readonly Dictionary<int, int> _deathTimesById;

		public RoyalePlayerReportComparer(Report.RoyaleGameReport report)
		{
			_deathTimesById = new Dictionary<int, int>();
			foreach (Report.DeathTimesMap deathTime in report.deathTimes)
			{
				_deathTimesById.Add(deathTime.id, deathTime.deathTime);
			}
		}

		public int Compare(Report.Player a, Report.Player b)
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
