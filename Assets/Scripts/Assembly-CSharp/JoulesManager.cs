using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class JoulesManager : MonoBehaviour
{
	private static readonly string _joulesPrefabDir = "Powerups/JoulesPack";

	private static GameObject _joulesPrefab;

	private int _joulesIndex;

	private Dictionary<int, GameObject> _activeJoules = new Dictionary<int, GameObject>();

	public PlayerController PlayerCont { get; set; }

	private void Awake()
	{
		_joulesPrefab = Resources.Load(_joulesPrefabDir) as GameObject;
	}

	public void SpawnJoulesDrop(int index = 0)
	{
		GameObject gameObject = Object.Instantiate(_joulesPrefab, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.name = "JoulesPack";
		JoulesPack component = gameObject.GetComponent<JoulesPack>();
		if (component != null)
		{
			component.OwnerID = PlayerCont.OwnerID;
			component.Index = ((index != 0) ? index : _joulesIndex++);
			_activeJoules.Add(component.Index, gameObject);
			if (!PlayerCont.isRemote)
			{
				Hashtable hashtable = new Hashtable();
				hashtable[(byte)0] = component.OwnerID;
				hashtable[(byte)1] = component.Index;
				PlayerCont.NetSync.SetAction(41, hashtable);
			}
		}
	}

	public void DespawnJoulesDrop(int index)
	{
		if (_activeJoules.ContainsKey(index))
		{
			Object.Destroy(_activeJoules[index]);
			_activeJoules.Remove(index);
		}
	}
}
