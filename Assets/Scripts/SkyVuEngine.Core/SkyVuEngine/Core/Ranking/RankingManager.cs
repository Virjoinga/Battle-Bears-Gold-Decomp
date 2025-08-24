using SkyVu.Common.Enums;

namespace SkyVuEngine.Core.Ranking
{
	public class RankingManager
	{
		public static RankingStrategy CreateStrategy(Games game)
		{
			if (game == Games.RainbowFart)
			{
				return new CategoryStrategy(game);
			}
			return null;
		}
	}
}
