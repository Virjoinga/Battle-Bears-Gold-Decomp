using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class GameCenterIDDictionaries
{
	private class GameCenterDictionaryJSON
	{
		public Dictionary<string, string> Achievements;

		public Dictionary<string, string> Leaderboards;
	}

	public static Dictionary<string, string> Achievements;

	public static Dictionary<string, string> Leaderboards;

	static GameCenterIDDictionaries()
	{
		Achievements = new Dictionary<string, string>
		{
			{ "TUTORIAL_DONE", "net.skyvu.battlebearsgold.achievment.tutorial" },
			{ "DOUBLE_TAP_THAT", "net.skyvu.battlebearsgold.achievment.doubletap" },
			{ "TUTORIAL_SECRET", "net.skyvu.battlebearsgold.achievment.break" },
			{ "BEARER_OF_BAD_NEWS", "net.skyvu.battlebearsgold.achievment.badnews" },
			{ "AWW_YEAH", "net.skyvu.battlebearsgold.achievment.awwyeah" },
			{ "DO_I_TALK_A_LOT", "net.skyvu.battlebearsgold.achievment.talkalot" },
			{ "JUST_A_SMALL_TOWN_BEAR", "net.skyvu.battlebearsgold.achievment.5wins" },
			{ "BATTLE_OF_THE_BEARS", "net.skyvu.battlebearsgold.achievment.25wins" },
			{ "WINNER_WINNER_CHICKEN_DINNER", "net.skyvu.battlebearsgold.achievment.50wins" },
			{ "I_HAVE_GAS", "net.skyvu.battlebearsgold.achievment.5gas" },
			{ "IM_FEELING_GASSY", "net.skyvu.battlebearsgold.achievment.25gas" },
			{ "THE_LEAKY_FAUCET", "net.skyvu.battlebearsgold.achievment.1refill" },
			{ "ENDLESS_REFILLS", "net.skyvu.battlebearsgold.achievment.25refill" },
			{ "QUICK_CHANGE_ARTIST", "net.skyvu.battlebearsgold.achievment.1skin" },
			{ "WE_HAVE_CLASS", "net.skyvu.battlebearsgold.achievment.1class" },
			{ "THE_HONEY_POT", "net.skyvu.battlebearsgold.achievment.2class" },
			{ "GOTTA_COLLECT_THEM_ALL", "net.skyvu.battlebearsgold.achievment.3class" },
			{ "PAWSABILITY", "net.skyvu.battlebearsgold.achievment.2equip" },
			{ "LEVEL2", "net.skyvu.battlebearsgold.achievment.level2" },
			{ "LEVEL3", "net.skyvu.battlebearsgold.achievment.level3" },
			{ "LEVEL4", "net.skyvu.battlebearsgold.achievment.level4" },
			{ "LEVEL5", "net.skyvu.battlebearsgold.achievment.level5" },
			{ "LEVEL6", "net.skyvu.battlebearsgold.achievment.level6" },
			{ "LEVEL7", "net.skyvu.battlebearsgold.achievment.level7" },
			{ "LEVEL8", "net.skyvu.battlebearsgold.achievment.level8" },
			{ "LEVEL9", "net.skyvu.battlebearsgold.achievment.level9" },
			{ "LEVEL10", "net.skyvu.battlebearsgold.achievment.level10" },
			{ "LEVEL25", "net.skyvu.battlebearsgold.achievment.level25" },
			{ "BRONZE", "net.skyvu.battlebearsgold.achievment.bronzerank" },
			{ "SILVER", "net.skyvu.battlebearsgold.achievment.silverrank" },
			{ "GOLD", "net.skyvu.battlebearsgold.achievment.goldrank" },
			{ "DIAMOND", "net.skyvu.battlebearsgold.achievment.diamondrank" },
			{ "BEARZERKER", "net.skyvu.battlebearsgold.achievment.100kills" },
			{ "COME_AT_ME", "net.skyvu.battlebearsgold.achievment.200kills" },
			{ "CANT_BEAR_IT", "net.skyvu.battlebearsgold.achievment.300kills" },
			{ "BETTER_HAVE_MY_HONEY", "net.skyvu.battlebearsgold.achievment.400kills" },
			{ "GRIZZLY_SITUATION", "net.skyvu.battlebearsgold.achievment.500kills" },
			{ "BEARFOOTED", "net.skyvu.battlebearsgold.achievment.10killsinone" },
			{ "PAWSIBLY_INSANE", "net.skyvu.battlebearsgold.achievment.15killsinone" },
			{ "LEAVE_NO_BEAR_BEHIND", "net.skyvu.battlebearsgold.achievment.20killsinone" },
			{ "UNBEARLIEVABLE", "net.skyvu.battlebearsgold.achievment.unbearlievable" },
			{ "PAWSED", "net.skyvu.battlebearsgold.achievment.pawsed" },
			{ "ATTENTION", "net.skyvu.battlebearsgold.achievment.attention" },
			{ "HUGGED_TO_DEATH", "net.skyvu.battlebearsgold.achievment.hugged" },
			{ "PANDAMONIUM", "net.skyvu.battlebearsgold.achievment.pandamonium" },
			{ "HEAVY_SET_GO", "net.skyvu.battlebearsgold.achievment.heavy" },
			{ "POLAR_OPPOSITES", "net.skyvu.battlebearsgold.achievment.polar" },
			{ "DIZZY_DUMMY", "net.skyvu.battlebearsgold.achievment.dizzy" },
			{ "KILL_STREAK_5", "net.skyvu.battlebearsgold.achievment.5killstreak" },
			{ "KILL_STREAK_10", "net.skyvu.battlebearsgold.achievment.10killstreak" },
			{ "KILL_STREAK_15", "net.skyvu.battlebearsgold.achievment.15killstreak" }
		};
		Leaderboards = new Dictionary<string, string>
		{
			{ "LEVEL", "net.skyvu.battlebearsgold.leaderboards.highestlevels" },
			{ "RANK", "net.skyvu.battlebearsgold.leaderboards.highestranks" },
			{ "KILL_STREAK", "net.skyvu.battlebearsgold.leaderboards.longestkillstreak" }
		};
		TextAsset textAsset = (TextAsset)Resources.Load("Config/GamecenterIDs");
		string text = textAsset.text;
		GameCenterDictionaryJSON gameCenterDictionaryJSON = JsonMapper.ToObject<GameCenterDictionaryJSON>(text);
		if (gameCenterDictionaryJSON != null)
		{
			if (gameCenterDictionaryJSON.Achievements != null)
			{
				Achievements = gameCenterDictionaryJSON.Achievements;
				Debug.Log("Achievements update");
			}
			if (gameCenterDictionaryJSON.Leaderboards != null)
			{
				Leaderboards = gameCenterDictionaryJSON.Leaderboards;
			}
		}
	}
}
