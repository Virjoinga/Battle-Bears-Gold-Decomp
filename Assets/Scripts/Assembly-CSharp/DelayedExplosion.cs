using System.Collections;
using UnityEngine;

public class DelayedExplosion : ConfigurableNetworkObject
{
	public float delay = 5f;

	public ConfigurableNetworkObject explosion;

	public Transform spawnPosition;

	public bool destroyAfterSpawn = true;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(delayedExplode());
	}

	private IEnumerator delayedExplode()
	{
		yield return new WaitForSeconds(delay);
		GameObject newExplosion = Object.Instantiate(explosion.gameObject, base.transform.position, Quaternion.identity) as GameObject;
		ConfigurableNetworkObject i = newExplosion.GetComponent<ConfigurableNetworkObject>();
		ForwardSettings(i);
		i.OwnerID = base.OwnerID;
		i.DamageMultiplier = base.DamageMultiplier;
		if (spawnPosition != null)
		{
			newExplosion.transform.position = spawnPosition.position;
		}
		if (destroyAfterSpawn)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
