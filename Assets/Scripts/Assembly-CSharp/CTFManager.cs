using System.Collections;
using UnityEngine;

public class CTFManager : MonoBehaviour
{
	public int EXPLODE_TIME = 120000;

	private float BOMB_CREATE_DELAY = 5f;

	private static CTFManager instance;

	public Transform[] bombSpawns = new Transform[2];

	public GameObject[] bombSpawnStations = new GameObject[2];

	public BombDepositArea[] depositSpots = new BombDepositArea[2];

	private Transform[] depositTransforms = new Transform[2];

	private GameObject[] currentBombs = new GameObject[2];

	private bool[] isRemoteRequestingBomb = new bool[2];

	private bool[] isLocalRequestingBomb = new bool[2];

	private int[] currentBombRequestTime = new int[2];

	private int[] currentBombPickupTime = new int[2];

	public GameObject[] timerSystems = new GameObject[2];

	public GameObject bombExplosion;

	public GameObject RedBomb
	{
		get
		{
			return currentBombs[0];
		}
	}

	public GameObject BlueBomb
	{
		get
		{
			return currentBombs[1];
		}
	}

	public int RedTimeLeft
	{
		get
		{
			return timeLeftForTeam(Team.RED);
		}
	}

	public int BlueTimeLeft
	{
		get
		{
			return timeLeftForTeam(Team.BLUE);
		}
	}

	public Transform RedDepositSpot
	{
		get
		{
			return depositTransforms[0];
		}
	}

	public Transform BlueDepositSpot
	{
		get
		{
			return depositTransforms[1];
		}
	}

	public static CTFManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (CTFManager)Object.FindObjectOfType(typeof(CTFManager));
				if (instance == null)
				{
					return null;
				}
			}
			return instance;
		}
	}

	private int timeLeftForTeam(Team team)
	{
		if (currentBombPickupTime[(int)team] == 0)
		{
			return 0;
		}
		return currentBombPickupTime[(int)team] + EXPLODE_TIME - GameManager.Instance.CurrentServerTime;
	}

	private void Awake()
	{
		if (Preferences.Instance.CurrentGameMode != GameMode.CTF)
		{
			Object.Destroy(base.gameObject);
		}
		for (int i = 0; i < 2; i++)
		{
			depositTransforms[i] = depositSpots[i].transform;
			depositSpots[i].GetComponent<Animation>()["drop_idle"].wrapMode = WrapMode.Once;
			depositSpots[i].GetComponent<Animation>().Play("drop_idle");
		}
	}

	private void Start()
	{
		ServiceManager.Instance.UpdateProperty("bomb_fuse_time", ref EXPLODE_TIME);
		EXPLODE_TIME *= 1000;
		ServiceManager.Instance.UpdateProperty("bomb_respawn_time", ref BOMB_CREATE_DELAY);
		createBomb(Team.RED);
		createBomb(Team.BLUE);
	}

	public string getFormattedTime(int time)
	{
		int num = time % 1000 / 100;
		int num2 = time / 1000;
		int num3 = num2 / 60;
		int num4 = num2 % 60;
		return string.Format("{0:0}", num3) + ":" + string.Format("{0:00}", num4) + ":" + string.Format("{0:0}", num) + Random.Range(0, 10);
	}

	private void createBomb(Team team)
	{
		currentBombPickupTime[(int)team] = 0;
		currentBombRequestTime[(int)team] = 0;
		isLocalRequestingBomb[(int)team] = false;
		isRemoteRequestingBomb[(int)team] = false;
		createBomb(team, bombSpawns[(int)team]);
	}

	public void resetBomb(Team team)
	{
		StartCoroutine(delayedCreateBomb(team, BOMB_CREATE_DELAY));
	}

	public void dropBomb(Team team, Vector3 pos)
	{
		if (currentBombs[(int)team] != null)
		{
			Transform transform = currentBombs[(int)team].transform;
			transform.parent = null;
			transform.position = pos;
			transform.eulerAngles = new Vector3(90f, 0f, 0f);
			transform.localScale = Vector3.one;
			currentBombs[(int)team].SendMessage("OnDropped");
		}
	}

	private void createBomb(Team team, Transform mount)
	{
		if (currentBombs[(int)team] != null)
		{
			Object.Destroy(currentBombs[(int)team]);
		}
		currentBombs[(int)team] = Object.Instantiate(Resources.Load("CTF/Bombs/" + ((team != 0) ? "Blue" : "Red")), Vector3.zero, Quaternion.identity) as GameObject;
		currentBombs[(int)team].transform.parent = mount;
		currentBombs[(int)team].transform.localPosition = Vector3.zero;
		currentBombs[(int)team].transform.localEulerAngles = Vector3.zero;
		currentBombs[(int)team].transform.localScale = Vector3.one;
		bombSpawnStations[(int)team].GetComponent<Animation>().Play("bombOut");
	}

	private void setBombTimer(Team team, int delay)
	{
		if (currentBombPickupTime[(int)team] == 0)
		{
			currentBombPickupTime[(int)team] = GameManager.Instance.CurrentServerTime - delay;
			bombSpawnStations[(int)team].GetComponent<Animation>().Play("spawnIdle");
		}
	}

	public void Update()
	{
		for (int i = 0; i < 2; i++)
		{
			if (currentBombPickupTime[i] != 0 && currentBombs[i] != null && !GameManager.Instance.IsGameSubmitted && currentBombPickupTime[i] + EXPLODE_TIME < GameManager.Instance.CurrentServerTime)
			{
				explodeBomb((Team)i, currentBombs[i].transform.position);
			}
		}
	}

	public void explodeBomb(Team team, Vector3 pos)
	{
		if (currentBombs[(int)team] != null)
		{
			currentBombs[(int)team].SendMessage("OnDropFromPlayer");
		}
		GameObject gameObject = Object.Instantiate(bombExplosion, pos, Quaternion.identity) as GameObject;
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.SetItemOverride("BigBomb");
		component.SetEquipmentNames("|");
		component.OwnerID = -1;
		currentBombPickupTime[(int)team] = 0;
		StartCoroutine(delayedCreateBomb(team, BOMB_CREATE_DELAY));
		base.gameObject.AddComponent<ShakeCamera>();
	}

	private IEnumerator delayedCreateBomb(Team team, float delay)
	{
		Object.Destroy(currentBombs[(int)team]);
		currentBombPickupTime[(int)team] = 0;
		yield return new WaitForSeconds(delay);
		createBomb(team);
	}

	public void OnRequestBombPickup(PlayerController playerController)
	{
		if (!isRemoteRequestingBomb[(int)playerController.Team])
		{
			StartCoroutine("sendLocalPickupRequest", playerController);
		}
	}

	public void OnRemoteRequestBombPickup(PlayerController p, int pickupTime)
	{
		isRemoteRequestingBomb[(int)p.Team] = true;
		if (isLocalRequestingBomb[(int)p.Team])
		{
			if (pickupTime < currentBombRequestTime[(int)p.Team])
			{
				currentBombRequestTime[(int)p.Team] = 0;
				StopCoroutine("sendLocalPickupRequest");
				isLocalRequestingBomb[(int)p.Team] = false;
				HUD.Instance.PlayerController.OnLocalStopGrabbingBomb();
				p.OnRemoteStartGrabbingBomb();
			}
		}
		else
		{
			p.OnRemoteStartGrabbingBomb();
		}
	}

	public void OnRemoteBombPickup(PlayerController p, int delay)
	{
		if (isLocalRequestingBomb[(int)p.Team])
		{
			p.OnLocalStopGrabbingBomb();
			StopCoroutine("sendLocalPickupRequest");
		}
		setBombTimer(p.Team, delay);
		isRemoteRequestingBomb[(int)p.Team] = false;
		isLocalRequestingBomb[(int)p.Team] = false;
		currentBombRequestTime[(int)p.Team] = 0;
		p.OnRemoteGetBomb(currentBombs[(int)p.Team], timerSystems[(int)p.Team]);
	}

	private IEnumerator sendLocalPickupRequest(PlayerController p)
	{
		isLocalRequestingBomb[(int)p.Team] = true;
		currentBombRequestTime[(int)p.Team] = GameManager.Instance.CurrentServerTime;
		p.OnLocalStartGrabbingBomb(currentBombs[(int)p.Team], currentBombRequestTime[(int)p.Team]);
		yield return new WaitForSeconds(2f);
		isLocalRequestingBomb[(int)p.Team] = false;
		currentBombRequestTime[(int)p.Team] = 0;
		isRemoteRequestingBomb[(int)p.Team] = false;
		if (!p.IsDead)
		{
			setBombTimer(p.Team, 0);
			p.OnLocalGetBomb(currentBombs[(int)p.Team]);
		}
	}
}
