using UnityEngine;

public class CumulativeStats : MonoBehaviour
{
	private static CumulativeStats instance;

	public int numberOfTeamspeaks;

	public int numberOfWoohoos;

	public int numWins;

	public int numGascansBought;

	public int numGasConverted;

	public int numJoulesBought;

	public int numRefills;

	public int numClassesBought;

	public int numEquipmentsBought;

	public int numOpponentKills;

	public int numGamesPlayed;

	private bool hasLoaded;

	public static CumulativeStats Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private string[] userPrefsList()
	{
		return new string[11]
		{
			"numberOfTeamspeaks" + ServiceManager.Instance.GetStats().pid,
			"numberOfWoohoos" + ServiceManager.Instance.GetStats().pid,
			"numWins" + ServiceManager.Instance.GetStats().pid,
			"numGascansBought" + ServiceManager.Instance.GetStats().pid,
			"numGasConverted" + ServiceManager.Instance.GetStats().pid,
			"numJoulesBought" + ServiceManager.Instance.GetStats().pid,
			"numRefills" + ServiceManager.Instance.GetStats().pid,
			"numClassesBought" + ServiceManager.Instance.GetStats().pid,
			"numEquipmentsBought" + ServiceManager.Instance.GetStats().pid,
			"numOpponentKills" + ServiceManager.Instance.GetStats().pid,
			"numGamesPlayed" + ServiceManager.Instance.GetStats().pid
		};
	}

	private void DownloadedBlobResultCallback(bool success)
	{
		if (success)
		{
			setInternalVariables();
		}
		else
		{
			Debug.Log("failed to download blob");
		}
	}

	private void setInternalVariables()
	{
		numberOfTeamspeaks = PlayerPrefs.GetInt("numberOfTeamspeaks" + ServiceManager.Instance.GetStats().pid, 0);
		numberOfWoohoos = PlayerPrefs.GetInt("numberOfWoohoos" + ServiceManager.Instance.GetStats().pid, 0);
		numWins = PlayerPrefs.GetInt("numWins" + ServiceManager.Instance.GetStats().pid, 0);
		numGascansBought = PlayerPrefs.GetInt("numGascansBought" + ServiceManager.Instance.GetStats().pid, 0);
		numGasConverted = PlayerPrefs.GetInt("numGasConverted" + ServiceManager.Instance.GetStats().pid, 0);
		numJoulesBought = PlayerPrefs.GetInt("numJoulesBought" + ServiceManager.Instance.GetStats().pid, 0);
		numRefills = PlayerPrefs.GetInt("numRefills" + ServiceManager.Instance.GetStats().pid, 0);
		numClassesBought = PlayerPrefs.GetInt("numClassesBought" + ServiceManager.Instance.GetStats().pid, 0);
		numEquipmentsBought = PlayerPrefs.GetInt("numEquipmentsBought" + ServiceManager.Instance.GetStats().pid, 0);
		numOpponentKills = PlayerPrefs.GetInt("numOpponentKills" + ServiceManager.Instance.GetStats().pid, 0);
		numGamesPlayed = PlayerPrefs.GetInt("numGamesPlayed" + ServiceManager.Instance.GetStats().pid, 0);
	}

	public void OnLoadStats()
	{
		if (!hasLoaded)
		{
			setInternalVariables();
			hasLoaded = true;
		}
	}

	public void OnSaveStats()
	{
		if (ServiceManager.Instance != null && ServiceManager.Instance.GetStats() != null)
		{
			PlayerPrefs.SetInt("numberOfTeamspeaks" + ServiceManager.Instance.GetStats().pid, numberOfTeamspeaks);
			PlayerPrefs.SetInt("numberOfWoohoos" + ServiceManager.Instance.GetStats().pid, numberOfWoohoos);
			PlayerPrefs.SetInt("numWins" + ServiceManager.Instance.GetStats().pid, numWins);
			PlayerPrefs.SetInt("numGascansBought" + ServiceManager.Instance.GetStats().pid, numGascansBought);
			PlayerPrefs.SetInt("numGasConverted" + ServiceManager.Instance.GetStats().pid, numGasConverted);
			PlayerPrefs.SetInt("numJoulesBought" + ServiceManager.Instance.GetStats().pid, numJoulesBought);
			PlayerPrefs.SetInt("numRefills" + ServiceManager.Instance.GetStats().pid, numRefills);
			PlayerPrefs.SetInt("numClassesBought" + ServiceManager.Instance.GetStats().pid, numClassesBought);
			PlayerPrefs.SetInt("numEquipmentsBought" + ServiceManager.Instance.GetStats().pid, numEquipmentsBought);
			PlayerPrefs.SetInt("numOpponentKills" + ServiceManager.Instance.GetStats().pid, numOpponentKills);
			PlayerPrefs.SetInt("numGamesPlayed" + ServiceManager.Instance.GetStats().pid, numGamesPlayed);
		}
	}

	private void OnApplicationQuit()
	{
		OnSaveStats();
	}
}
