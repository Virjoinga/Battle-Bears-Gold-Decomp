using System;

namespace Analytics.Parameters
{
	public class MatchTypeParameter : StringParameter
	{
		public enum Type
		{
			FREE_FOR_ALL = 0,
			KING_OF_THE_HILL = 1,
			PLANT_THE_BOMB = 2,
			TEAM_BATTLE = 3,
			BATTLE_ROYALE = 4
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.matchType;
			}
		}

		public MatchTypeParameter(GameMode gameMode)
			: base(TypeFromGameMode(gameMode).ToString())
		{
		}

		private static Type TypeFromGameMode(GameMode gameMode)
		{
			switch (gameMode)
			{
			case GameMode.FFA:
				return Type.FREE_FOR_ALL;
			case GameMode.TB:
				return Type.TEAM_BATTLE;
			case GameMode.CTF:
				return Type.PLANT_THE_BOMB;
			case GameMode.KOTH:
				return Type.KING_OF_THE_HILL;
			case GameMode.ROYL:
				return Type.BATTLE_ROYALE;
			default:
				throw new Exception("No GameType defined for GameMode " + gameMode);
			}
		}
	}
}
