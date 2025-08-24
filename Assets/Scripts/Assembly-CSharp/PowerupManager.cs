using UnityEngine;

public class PowerupManager : MonoBehaviour
{
	private static PowerupManager instance;

	protected PowerupSpawner[] powerupSpawners;

	public static PowerupManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (PowerupManager)Object.FindObjectOfType(typeof(PowerupManager));
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Debug.LogError("Two PowerupManagers found in scene");
			Object.Destroy(this);
		}
		PowerupSpawner[] componentsInChildren = GetComponentsInChildren<PowerupSpawner>(false);
		powerupSpawners = new PowerupSpawner[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			powerupSpawners[i] = componentsInChildren[i];
			powerupSpawners[i].ID = i;
		}
	}

	public virtual void OnInitialSpawn()
	{
		for (int i = 0; i < powerupSpawners.Length; i++)
		{
			powerupSpawners[i].OnSpawn();
		}
	}

	public void OnUsePowerup(int index, int delay)
	{
		powerupSpawners[index].OnTakePowerup(delay);
	}
}
