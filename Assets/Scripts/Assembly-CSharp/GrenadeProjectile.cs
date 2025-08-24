using System.Collections;
using UnityEngine;

public class GrenadeProjectile : Projectile
{
	[SerializeField]
	private float _explosionDelay;

	private new void Start()
	{
		Item itemByName = ServiceManager.Instance.GetItemByName(spawnItemOverride);
		_explosionDelay = (float)itemByName.properties["explosionDelay"];
		StartCoroutine(ExplosionDelayRoutine());
	}

	protected override void OnCollisionEnter(Collision c)
	{
		PlayerController component = c.gameObject.GetComponent<PlayerController>();
		if (component != null)
		{
			StopAllCoroutines();
			Explode(null);
		}
	}

	private IEnumerator ExplosionDelayRoutine()
	{
		yield return new WaitForSeconds(_explosionDelay);
		Explode(null);
	}
}
