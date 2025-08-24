using System.Collections.Generic;
using System.Linq;
using Utils.Comparers;

namespace Analytics.Parameters
{
	public class RoyaleMatchResultParameters : NonTeamMatchResultParameters
	{
		public RoyaleMatchResultParameters(Report.GameReport report, int myId)
		{
			List<IEventParameter> list = new List<IEventParameter>();
			Report.Player player = report.players.First((Report.Player p) => p.id == myId);
			AddMyParams(list, player);
			List<Report.Player> list2 = new List<Report.Player>(report.players);
			list2.Sort(new RoyalePlayerReportComparer(report as Report.RoyaleGameReport));
			if (GameManager.Instance.WillSendReport())
			{
				AddWinningPlayerParams(list, list2[0]);
			}
			list.AddRange(new OppositionParameters(list2, player.id));
			_eventParameters = list.ToArray();
		}
	}
}
