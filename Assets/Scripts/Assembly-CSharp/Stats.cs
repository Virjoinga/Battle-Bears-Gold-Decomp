public class Stats
{
	public int pid = -1;

	public int games_left;

	public int max_games;

	public int seconds_to_refill;

	public int joules;

	public int gas;

	public double skill;

	public int exp;

	public double level;

	public double age_in_minutes;

	public bool guest;

	public static Stats Test()
	{
		Stats stats = new Stats();
		stats.joules = 4000;
		stats.gas = 2;
		stats.skill = 150.0;
		stats.exp = 0;
		return stats;
	}
}
