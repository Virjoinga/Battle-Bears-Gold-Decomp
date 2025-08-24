using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsOverlay : MonoBehaviour
{
	public AnimatedLoadoutScroller loadoutSelector;

	public TextMesh redScore;

	public TextMesh blueScore;

	public Transform[] redPlayerDisplays;

	public Transform[] bluePlayerDisplays;

	public TextMesh tapText;

	public GameObject toggleLoadoutButton;

	public TextMesh killerName;

	public TextMesh killerLevelText;

	public Transform killerRankMount;

	public Transform killerIconMount;

	public Transform killerPrimaryMount;

	public Transform killerSecondaryMount;

	public Transform killerMeleeMount;

	public Transform killerSpecialMount;

	public Transform killerEquipment1Mount;

	public Transform killerEquipment2Mount;

	private bool ableToClose;

	private List<GameObject> icons = new List<GameObject>();

	private void Start()
	{
		OnUpdateStats();
	}

	private void ToggleLoadout()
	{
		if (loadoutSelector.gameObject.activeInHierarchy)
		{
			loadoutSelector.gameObject.SetActive(false);
		}
		else
		{
			loadoutSelector.gameObject.SetActive(true);
		}
	}

	public void OnUpdateKillerInfo(PlayerStats killer)
	{
		if (killer == null)
		{
			return;
		}
		killerName.text = killer.greeName;
		killerLevelText.text = killer.level.ToString();
		Rank rank = ServiceManager.GetRank(killer.skill);
		Object @object = Resources.Load("Icons/Rank/" + rank);
		if (@object != null)
		{
			GameObject gameObject = Object.Instantiate(@object) as GameObject;
			gameObject.transform.parent = killerRankMount;
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.layer = LayerMask.NameToLayer("HUD");
		}
		Object object2 = Resources.Load("Icons/Characters/" + killer.playerLoadout.model.name + "/" + killer.playerLoadout.skin.name + ((killer.playerTeam != 0) ? "_blue" : "_red"));
		if (object2 != null)
		{
			GameObject gameObject2 = Object.Instantiate(object2) as GameObject;
			gameObject2.transform.parent = killerIconMount;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.layer = LayerMask.NameToLayer("HUD");
		}
		object2 = Resources.Load("Icons/Weapons/" + killer.playerLoadout.model.name + "/" + killer.playerLoadout.primary.name);
		if (object2 != null)
		{
			GameObject gameObject3 = Object.Instantiate(object2) as GameObject;
			gameObject3.transform.parent = killerPrimaryMount;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localEulerAngles = Vector3.zero;
			gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject3.layer = LayerMask.NameToLayer("HUD");
		}
		object2 = Resources.Load("Icons/Weapons/" + killer.playerLoadout.model.name + "/" + killer.playerLoadout.secondary.name);
		if (object2 != null)
		{
			GameObject gameObject4 = Object.Instantiate(object2) as GameObject;
			gameObject4.transform.parent = killerSecondaryMount;
			gameObject4.transform.localPosition = Vector3.zero;
			gameObject4.transform.localEulerAngles = Vector3.zero;
			gameObject4.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject4.layer = LayerMask.NameToLayer("HUD");
		}
		object2 = Resources.Load("Icons/Weapons/" + killer.playerLoadout.model.name + "/" + killer.playerLoadout.melee.name);
		if (object2 != null)
		{
			GameObject gameObject5 = Object.Instantiate(object2) as GameObject;
			gameObject5.transform.parent = killerMeleeMount;
			gameObject5.transform.localPosition = Vector3.zero;
			gameObject5.transform.localEulerAngles = Vector3.zero;
			gameObject5.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject5.layer = LayerMask.NameToLayer("HUD");
		}
		if (killer.playerLoadout.special != null)
		{
			object2 = Resources.Load("Icons/Specials/" + killer.playerLoadout.special.name);
			if (object2 != null)
			{
				GameObject gameObject6 = Object.Instantiate(object2) as GameObject;
				gameObject6.transform.parent = killerSpecialMount;
				gameObject6.transform.localPosition = Vector3.zero;
				gameObject6.transform.localEulerAngles = Vector3.zero;
				gameObject6.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject6.layer = LayerMask.NameToLayer("HUD");
			}
		}
		if (killer.playerLoadout.equipment1 != null)
		{
			object2 = Resources.Load("Icons/Equipment/" + killer.playerLoadout.equipment1.name);
			if (object2 != null)
			{
				GameObject gameObject7 = Object.Instantiate(object2) as GameObject;
				gameObject7.transform.parent = killerEquipment1Mount;
				gameObject7.transform.localPosition = Vector3.zero;
				gameObject7.transform.localEulerAngles = Vector3.zero;
				gameObject7.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject7.layer = LayerMask.NameToLayer("HUD");
			}
		}
		if (killer.playerLoadout.equipment2 != null)
		{
			object2 = Resources.Load("Icons/Equipment/" + killer.playerLoadout.equipment2.name);
			if (object2 != null)
			{
				GameObject gameObject8 = Object.Instantiate(object2) as GameObject;
				gameObject8.transform.parent = killerEquipment2Mount;
				gameObject8.transform.localPosition = Vector3.zero;
				gameObject8.transform.localEulerAngles = Vector3.zero;
				gameObject8.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject8.layer = LayerMask.NameToLayer("HUD");
			}
		}
	}

	public void OnUpdateStats()
	{
		foreach (GameObject icon in icons)
		{
			Object.Destroy(icon);
		}
		icons.Clear();
		List<PlayerStats> list = new List<PlayerStats>();
		List<PlayerStats> list2 = new List<PlayerStats>();
		List<PlayerStats> list3 = new List<PlayerStats>();
		foreach (KeyValuePair<int, PlayerStats> playerStat in GameManager.Instance.playerStats)
		{
			list3.Add(playerStat.Value);
			if (playerStat.Value.playerTeam == Team.RED)
			{
				list2.Add(playerStat.Value);
			}
			else
			{
				list.Add(playerStat.Value);
			}
		}
		list3.Sort(delegate(PlayerStats a, PlayerStats b)
		{
			if (a.NetKills < b.NetKills)
			{
				return 1;
			}
			if (a.NetKills > b.NetKills)
			{
				return -1;
			}
			if (a.NetKills == b.NetKills)
			{
				if (a.NetDamage < b.NetDamage)
				{
					return 1;
				}
				if (a.NetDamage > b.NetDamage)
				{
					return -1;
				}
			}
			return 0;
		});
		list.Sort(delegate(PlayerStats a, PlayerStats b)
		{
			if (a.NetKills < b.NetKills)
			{
				return 1;
			}
			if (a.NetKills > b.NetKills)
			{
				return -1;
			}
			if (a.NetKills == b.NetKills)
			{
				if (a.NetDamage < b.NetDamage)
				{
					return 1;
				}
				if (a.NetDamage > b.NetDamage)
				{
					return -1;
				}
			}
			return 0;
		});
		list2.Sort(delegate(PlayerStats a, PlayerStats b)
		{
			if (a.NetKills < b.NetKills)
			{
				return 1;
			}
			if (a.NetKills > b.NetKills)
			{
				return -1;
			}
			if (a.NetKills == b.NetKills)
			{
				if (a.NetDamage < b.NetDamage)
				{
					return 1;
				}
				if (a.NetDamage > b.NetDamage)
				{
					return -1;
				}
			}
			return 0;
		});
		if (Preferences.Instance.CurrentGameMode == GameMode.CTF || Preferences.Instance.CurrentGameMode == GameMode.TB || Preferences.Instance.CurrentGameMode == GameMode.KOTH)
		{
			SetupTeamBasedOverlay(list2, list);
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
		{
			SetupFFAOverlay(list3);
		}
	}

	private void SetupTeamBasedOverlay(List<PlayerStats> redPlayers, List<PlayerStats> bluePlayers)
	{
		if (Preferences.Instance.CurrentGameMode == GameMode.CTF)
		{
			if (GameManager.Instance.RedDeposits == -1000)
			{
				redScore.text = "-";
			}
			else
			{
				redScore.text = GameManager.Instance.RedDeposits.ToString();
			}
			if (GameManager.Instance.BlueDeposits == -1000)
			{
				blueScore.text = "-";
			}
			else
			{
				blueScore.text = GameManager.Instance.BlueDeposits.ToString();
			}
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.TB)
		{
			if (GameManager.Instance.RedKills == -1000)
			{
				redScore.text = "-";
			}
			else
			{
				redScore.text = GameManager.Instance.RedKills.ToString();
			}
			if (GameManager.Instance.BlueKills == -1000)
			{
				blueScore.text = "-";
			}
			else
			{
				blueScore.text = GameManager.Instance.BlueKills.ToString();
			}
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.KOTH && KOTHManager.Instance != null)
		{
			redScore.text = KOTHManager.Instance.GetTeamScore(Team.RED).ToString();
			blueScore.text = KOTHManager.Instance.GetTeamScore(Team.BLUE).ToString();
		}
		for (int i = 0; i < bluePlayerDisplays.Length; i++)
		{
			if (i >= bluePlayers.Count)
			{
				(bluePlayerDisplays[i].Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(bluePlayerDisplays[i].Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(bluePlayerDisplays[i].Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(bluePlayerDisplays[i].Find("level_text").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(bluePlayerDisplays[i].Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				bluePlayerDisplays[i].Find("highlight").gameObject.SetActive(false);
				continue;
			}
			bluePlayerDisplays[i].Find("highlight").gameObject.SetActive(bluePlayers[i].id == GameManager.Instance.localPlayerID);
			(bluePlayerDisplays[i].Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = bluePlayers[i].greeName;
			(bluePlayerDisplays[i].Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = bluePlayers[i].NetKills.ToString();
			(bluePlayerDisplays[i].Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = bluePlayers[i].NumDeaths.ToString();
			(bluePlayerDisplays[i].Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = bluePlayers[i].id.ToString();
			bluePlayerDisplays[i].Find("level_text").GetComponent<TextMesh>().text = bluePlayers[i].level.ToString();
			Rank rank = ServiceManager.GetRank(bluePlayers[i].skill);
			Object @object = Resources.Load("Icons/Rank/" + rank);
			if (@object != null)
			{
				GameObject gameObject = Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = bluePlayerDisplays[i].Find("rank_mount");
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.layer = LayerMask.NameToLayer("HUD");
				icons.Add(gameObject);
			}
			string text = bluePlayers[i].playerLoadout.model.name;
			string text2 = bluePlayers[i].playerLoadout.skin.name;
			Object object2 = Resources.Load("Icons/Characters/" + text + "/" + text2 + "_blue");
			if (object2 != null)
			{
				GameObject gameObject2 = Object.Instantiate(object2) as GameObject;
				gameObject2.transform.parent = bluePlayerDisplays[i].transform.Find("mount");
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localEulerAngles = Vector3.zero;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject2.layer = LayerMask.NameToLayer("HUD");
				icons.Add(gameObject2);
			}
		}
		for (int j = 0; j < redPlayerDisplays.Length; j++)
		{
			if (j >= redPlayers.Count)
			{
				(redPlayerDisplays[j].Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(redPlayerDisplays[j].Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(redPlayerDisplays[j].Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(redPlayerDisplays[j].Find("level_text").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(redPlayerDisplays[j].Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				redPlayerDisplays[j].Find("highlight").gameObject.SetActive(false);
				continue;
			}
			redPlayerDisplays[j].Find("highlight").gameObject.SetActive(redPlayers[j].id == GameManager.Instance.localPlayerID);
			(redPlayerDisplays[j].Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = redPlayers[j].greeName;
			(redPlayerDisplays[j].Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = redPlayers[j].NetKills.ToString();
			(redPlayerDisplays[j].Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = redPlayers[j].NumDeaths.ToString();
			(redPlayerDisplays[j].Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = redPlayers[j].id.ToString();
			redPlayerDisplays[j].Find("level_text").GetComponent<TextMesh>().text = redPlayers[j].level.ToString();
			Rank rank2 = ServiceManager.GetRank(redPlayers[j].skill);
			Object object3 = Resources.Load("Icons/Rank/" + rank2);
			if (object3 != null)
			{
				GameObject gameObject3 = Object.Instantiate(object3) as GameObject;
				gameObject3.transform.parent = redPlayerDisplays[j].Find("rank_mount");
				gameObject3.transform.localEulerAngles = Vector3.zero;
				gameObject3.transform.localPosition = Vector3.zero;
				gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject3.layer = LayerMask.NameToLayer("HUD");
				icons.Add(gameObject3);
			}
			string text3 = redPlayers[j].playerLoadout.model.name;
			string text4 = redPlayers[j].playerLoadout.skin.name;
			Object object4 = Resources.Load("Icons/Characters/" + text3 + "/" + text4 + "_red");
			if (object4 != null)
			{
				GameObject gameObject4 = Object.Instantiate(object4) as GameObject;
				gameObject4.transform.parent = redPlayerDisplays[j].transform.Find("mount");
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localEulerAngles = Vector3.zero;
				gameObject4.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject4.layer = LayerMask.NameToLayer("HUD");
				icons.Add(gameObject4);
			}
		}
	}

	private void SetupFFAOverlay(List<PlayerStats> allPlayers)
	{
		List<Transform> allPlayerDisplayObjects = GetAllPlayerDisplayObjects();
		for (int i = 0; i < allPlayerDisplayObjects.Count; i++)
		{
			Transform transform = allPlayerDisplayObjects[i];
			if (i < allPlayers.Count)
			{
				PlayerStats playerStats = allPlayers[i];
				if (i >= allPlayerDisplayObjects.Count)
				{
					Debug.LogError("More players than display slots for the stats overlay!");
					break;
				}
				transform.Find("highlight").gameObject.SetActive(playerStats.id == GameManager.Instance.localPlayerID);
				(transform.Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = playerStats.greeName;
				(transform.Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = playerStats.NetKills.ToString();
				(transform.Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = playerStats.NumDeaths.ToString();
				(transform.Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = playerStats.id.ToString();
				transform.Find("level_text").GetComponent<TextMesh>().text = playerStats.level.ToString();
				Rank rank = ServiceManager.GetRank(playerStats.skill);
				Object @object = Resources.Load("Icons/Rank/" + rank);
				if (@object != null)
				{
					GameObject gameObject = Object.Instantiate(@object) as GameObject;
					gameObject.transform.parent = transform.Find("rank_mount");
					gameObject.transform.localEulerAngles = Vector3.zero;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
					gameObject.layer = LayerMask.NameToLayer("HUD");
					icons.Add(gameObject);
				}
				string text = playerStats.playerLoadout.model.name;
				string text2 = playerStats.playerLoadout.skin.name;
				string text3 = ((playerStats.id != GameManager.Instance.localPlayerID) ? "_blue" : "_red");
				Object object2 = Resources.Load("Icons/Characters/" + text + "/" + text2 + text3);
				if (object2 != null)
				{
					GameObject gameObject2 = Object.Instantiate(object2) as GameObject;
					gameObject2.transform.parent = transform.transform.Find("mount");
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localEulerAngles = Vector3.zero;
					gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
					gameObject2.layer = LayerMask.NameToLayer("HUD");
					icons.Add(gameObject2);
				}
			}
			else
			{
				(transform.Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(transform.Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(transform.Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(transform.Find("level_text").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(transform.Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				transform.Find("highlight").gameObject.SetActive(false);
			}
		}
	}

	private List<Transform> GetAllPlayerDisplayObjects()
	{
		List<Transform> list = new List<Transform>();
		list.AddRange(redPlayerDisplays);
		list.AddRange(bluePlayerDisplays);
		return list;
	}

	private void Update()
	{
		if (MogaController.Instance.connection == 1 && MogaController.Instance.ButtonPressed(96))
		{
			TryClose();
		}
	}

	public void startRespawnCountdown()
	{
		int val = 5;
		if (Preferences.Instance.CurrentGameMode == GameMode.CTF)
		{
			ServiceManager.Instance.UpdateProperty("ptb_respawn_time", ref val);
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.TB)
		{
			ServiceManager.Instance.UpdateProperty("respawn_time", ref val);
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
		{
			ServiceManager.Instance.UpdateProperty("ffa_respawn_time", ref val);
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.KOTH)
		{
			ServiceManager.Instance.UpdateProperty("koth_respawn_time", ref val);
		}
		StartCoroutine(respawnCountdown(val));
	}

	private IEnumerator respawnCountdown(int respawnTime)
	{
		while (respawnTime > 0)
		{
			tapText.text = "Respawn in\n" + respawnTime + " seconds";
			yield return new WaitForSeconds(1f);
			respawnTime--;
		}
		tapText.text = "Tap to\ncontinue";
		ableToClose = true;
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (b.name == "exitCollider")
		{
			TryClose();
		}
		else if (b.name == "loadoutToggle")
		{
			ToggleLoadout();
		}
	}

	private void TryClose()
	{
		if (ableToClose)
		{
			ableToClose = false;
			RespawnPlayer();
			Object.Destroy(base.gameObject);
		}
	}

	private void RespawnPlayer()
	{
		Item item = ((loadoutSelector.SelectedLoadout != 0) ? ServiceManager.Instance.GetItemByName(LoadoutManager.Instance.LoadoutPrefix + loadoutSelector.SelectedLoadout) : null);
		if (item != null && loadoutSelector.SelectedLoadout != LoadoutManager.Instance.CurrentLoadout.loadoutNumber && ServiceManager.Instance.IsItemBought(item.id))
		{
			int pid = ServiceManager.Instance.GetStats().pid;
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(pid);
			if (playerCharacterManager != null)
			{
				playerCharacterManager.ChangePlayerLoadout(LoadoutManager.Instance.GetLoadoutByNumber(loadoutSelector.SelectedLoadout, string.Empty), LoadoutManager.Instance.GetLastCharacterForLoadout(loadoutSelector.SelectedLoadout));
			}
		}
		else
		{
			HUD.Instance.PlayerController.RespawnPlayer();
		}
	}

	public void OnDisable()
	{
	}
}
