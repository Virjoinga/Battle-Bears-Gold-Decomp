public static class GameModeExtensions
{
	public static bool IsTeam(this GameMode gameMode)
	{
		return gameMode != 0 && gameMode != GameMode.ROYL;
	}
}
