using System;
using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class KOTHManager : MonoBehaviour
{
	private static readonly string _satellitePrefabStr = "KOTH/Satellite";

	private static readonly string _satelliteExplosionPrefabStr = "KOTH/KOTHExplosion";

	private float _satelliteSpeed = 5000f;

	private GameObject _satelliteExplosionPrefab;

	private GameObject _satelliteResourcePrefab;

	private static KOTHManager _instance;

	private float _timeBetweenPoints = 30f;

	[SerializeField]
	private GameObject[] _points;

	private KOTHPoint _currentPoint;

	private int[] _teamScores = new int[Enum.GetValues(typeof(Team)).Length];

	public float SatelliteSpeed
	{
		get
		{
			return _satelliteSpeed;
		}
	}

	public GameObject SatelliteExplosionPrefab
	{
		get
		{
			return _satelliteExplosionPrefab;
		}
	}

	public GameObject SatelliteResourcePrefab
	{
		get
		{
			return _satelliteResourcePrefab;
		}
	}

	public static KOTHManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public KOTHPoint CurrentPoint
	{
		get
		{
			return _currentPoint;
		}
	}

	public int GetTeamScore(Team team)
	{
		return _teamScores[(int)team];
	}

	private void Awake()
	{
		_instance = this;
		_satelliteResourcePrefab = (GameObject)Resources.Load(_satellitePrefabStr);
		_satelliteExplosionPrefab = (GameObject)Resources.Load(_satelliteExplosionPrefabStr);
	}

	private void Start()
	{
		if (Preferences.Instance != null && Preferences.Instance.CurrentGameMode != GameMode.KOTH)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		for (int i = 0; i < _points.Length; i++)
		{
			_points[i].name += i;
			_points[i].SetActive(false);
		}
		StartCoroutine(SwitchPointsCoroutine());
	}

	private void PickNextPoint()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		int num = UnityEngine.Random.Range(0, _points.Length - 1);
		if (_currentPoint != null)
		{
			_currentPoint.PointDeactivated();
			AwardPointsForPlayersInPoint();
			if (num == _currentPoint.pointIndex)
			{
				num = ((_currentPoint.pointIndex + 1 < _points.Length) ? (_currentPoint.pointIndex + 1) : 0);
			}
		}
		_currentPoint = new KOTHPoint(_points[num], num);
		_currentPoint.PointActivated();
		NotifyOtherPlayersOfPointChange(num);
	}

	private int GetRandomPointIndex()
	{
		return UnityEngine.Random.Range(0, _points.Length - 1);
	}

	private IEnumerator SwitchPointsCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(_timeBetweenPoints);
			PickNextPoint();
		}
	}

	private void AwardPointsForPlayersInPoint()
	{
		if (!PhotonNetwork.isMasterClient || _currentPoint == null || !(_currentPoint.tracker != null))
		{
			return;
		}
		bool[] array = new bool[_teamScores.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = false;
		}
		foreach (PlayerController item in _currentPoint.tracker.controllersInCollider)
		{
			if (!array[(int)item.Team] && !item.IsDead)
			{
				SetScore(item.Team, _teamScores[(int)item.Team] + 1);
				array[(int)item.Team] = true;
				NotifyOtherPlayersOfScore(item.Team, _teamScores[(int)item.Team]);
			}
		}
	}

	private void NotifyOtherPlayersOfScore(Team team, int score)
	{
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(GameManager.Instance.localPlayerID);
			if (playerCharacterManager != null && playerCharacterManager.PlayerController != null && playerCharacterManager.PlayerController.NetSync != null)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = playerCharacterManager.OwnerID;
				hashtable[(byte)1] = team;
				hashtable[(byte)2] = score;
				playerCharacterManager.PlayerController.NetSync.SetAction(58, hashtable);
			}
		}
	}

	private void NotifyOtherPlayersOfPointChange(int pointIndex)
	{
		if (GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(GameManager.Instance.localPlayerID);
			if (playerCharacterManager != null && playerCharacterManager.PlayerController != null && playerCharacterManager.PlayerController.NetSync != null)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = playerCharacterManager.OwnerID;
				hashtable[(byte)1] = pointIndex;
				playerCharacterManager.PlayerController.NetSync.SetAction(59, hashtable);
			}
		}
	}

	public void SetScore(Team team, int score)
	{
		_teamScores[(int)team] = score;
		HUD.Instance.OnSetScore(team, _teamScores[(int)team]);
	}

	public void ChangePoint(int pointIndex)
	{
		if (_currentPoint != null)
		{
			_currentPoint.PointDeactivated();
		}
		_currentPoint = new KOTHPoint(_points[pointIndex], pointIndex);
		_currentPoint.PointActivated();
	}
}
