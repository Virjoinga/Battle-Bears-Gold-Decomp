using UnityEngine;

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

	public Stats()
    {
        joules = 1;
        gas = 1;
        skill = 150.0;
        exp = 0;
    }

	public static Stats Test()
	{
		Stats stats = new Stats();
		stats.joules = 0;
		stats.gas = 0;
		stats.skill = 150.0;
		stats.exp = 0;
		stats.pid = Random.Range(0, int.MaxValue);
		return stats;
	}
}
