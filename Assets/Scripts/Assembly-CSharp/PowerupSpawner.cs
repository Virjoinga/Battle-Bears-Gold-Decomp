using System.Collections;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
	public GameObject powerup;

	public float respawnTime = 10f;

	private int id;

	private bool isSpawning;

	protected GameObject currentlySpawnedPowerup;

	public int ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public virtual void OnSpawn()
	{
		GameObject gameObject = Object.Instantiate(powerup, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.name = powerup.name + " " + id;
		currentlySpawnedPowerup = gameObject;
	}

	public void OnTakePowerup(int timeDelay)
	{
		if (currentlySpawnedPowerup != null)
		{
			Object.Destroy(currentlySpawnedPowerup);
		}
		if (!isSpawning)
		{
			if ((double)timeDelay / 1000.0 > 3.0)
			{
				timeDelay = 3000;
			}
			StartCoroutine(delayedRespawn(respawnTime - (float)timeDelay / 1000f));
		}
	}

	private IEnumerator delayedRespawn(float time)
	{
		isSpawning = true;
		yield return new WaitForSeconds(time);
		OnSpawn();
		isSpawning = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawCube(base.transform.position, new Vector3(50f, 50f, 50f));
	}
}
