using System.Collections.Generic;

public class PlayerStats
{
	private float netDamageDealt;

	private int netKills;

	private Dictionary<int, float> currentAssistsDamage = new Dictionary<int, float>();

	private Dictionary<int, float> damagesDealt = new Dictionary<int, float>();

	private Dictionary<int, int> killsCaused = new Dictionary<int, int>();

	private int totalDeaths;

	public int joulePacksCollected;

	public int currentKillStreak;

	public int longestKillStreak;

	public int killStreakBonusTotal;

	public int stopStreakBonusTotal;

	public int assistBonusTotal;

	public bool hasLeft;

	public int id = -1;

	public string greeName = string.Empty;

	public int level = 1;

	public float skill;

	public int bombHoldBonusTotal;

	public int bombDepositBonusTotal;

	public Team playerTeam;

	public PlayerLoadout playerLoadout;

	public Dictionary<int, float> CurrentAssistsDamage
	{
		get
		{
			return currentAssistsDamage;
		}
	}

	public Dictionary<int, float> DamagesDealt
	{
		get
		{
			return damagesDealt;
		}
	}

	public Dictionary<int, int> KillsCaused
	{
		get
		{
			return killsCaused;
		}
	}

	public int NetKills
	{
		get
		{
			return netKills;
		}
	}

	public float NetDamage
	{
		get
		{
			return netDamageDealt;
		}
	}

	public int NumDeaths
	{
		get
		{
			return totalDeaths;
		}
	}

	public PlayerStats(Team t, PlayerLoadout p, string greeName, int level, float skill, int id)
	{
		this.id = id;
		this.greeName = greeName;
		this.level = level;
		this.skill = skill;
		playerTeam = t;
		playerLoadout = new PlayerLoadout();
		playerLoadout.model = p.model;
		playerLoadout.pid = p.pid;
		playerLoadout.primary = p.primary;
		playerLoadout.secondary = p.secondary;
		playerLoadout.skin = p.skin;
		playerLoadout.special = p.special;
		playerLoadout.melee = p.melee;
		playerLoadout.equipment1 = p.equipment1;
		playerLoadout.equipment2 = p.equipment2;
	}

	public void addDamageDealt(int playerID, float damage)
	{
		if (playerID == -1)
		{
			return;
		}
		if (damagesDealt.ContainsKey(playerID))
		{
			Dictionary<int, float> dictionary;
			Dictionary<int, float> dictionary2 = (dictionary = damagesDealt);
			int key;
			int key2 = (key = playerID);
			float num = dictionary[key];
			dictionary2[key2] = num + damage;
		}
		else
		{
			damagesDealt.Add(playerID, damage);
		}
		if (playerTeam == GameManager.Instance.playerStats[playerID].playerTeam)
		{
			netDamageDealt -= damage;
		}
		else
		{
			netDamageDealt += damage;
		}
		if (playerTeam != GameManager.Instance.playerStats[playerID].playerTeam)
		{
			if (currentAssistsDamage.ContainsKey(playerID))
			{
				Dictionary<int, float> dictionary3;
				Dictionary<int, float> dictionary4 = (dictionary3 = currentAssistsDamage);
				int key;
				int key3 = (key = playerID);
				float num = dictionary3[key];
				dictionary4[key3] = num + damage;
			}
			else
			{
				currentAssistsDamage.Add(playerID, damage);
			}
		}
	}

	public void addDeath()
	{
		totalDeaths++;
	}

	public void addKillsCaused(int playerID)
	{
		if (killsCaused.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary;
			Dictionary<int, int> dictionary2 = (dictionary = killsCaused);
			int key;
			int key2 = (key = playerID);
			key = dictionary[key];
			dictionary2[key2] = key + 1;
		}
		else
		{
			killsCaused.Add(playerID, 1);
		}
		if (id != playerID && (!Preferences.Instance.IsTeamMode || playerTeam != GameManager.Instance.playerStats[playerID].playerTeam))
		{
			netKills++;
		}
	}
}
