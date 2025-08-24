using SkyVu.Common.Enums;

namespace SkyVuEngine.Core.Ranking
{
	public abstract class RankingStrategy
	{
		private RankingTypes _rankingType;

		public Games Game { get; set; }

		public int TotalPoints { get; set; }

		public RankingTypes RankingType
		{
			get
			{
				return _rankingType;
			}
		}

		public RankingStrategy(Games game)
		{
			Game = game;
			_rankingType = RankingTypes.Category;
		}

		public virtual bool CalculateRankValue()
		{
			return false;
		}
	}
}
