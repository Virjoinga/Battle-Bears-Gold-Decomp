using UnityEngine;

public class BearzookaMissile : Projectile
{
	public GameObject explosionSmoke;

	private bool hasExploded;

	protected override void OnTriggerEnter(Collider c)
	{
		if (!hasExploded)
		{
			if (explosionSmoke != null)
			{
				Object.Instantiate(explosionSmoke, base.transform.position, Quaternion.Euler(base.transform.right));
			}
			hasExploded = true;
			base.OnTriggerEnter(c);
		}
	}

	protected override void OnCollisionEnter(Collision c)
	{
		if (!hasExploded)
		{
			if (explosionSmoke != null)
			{
				Object.Instantiate(explosionSmoke, base.transform.position, Quaternion.Euler(base.transform.forward));
			}
			hasExploded = true;
			base.OnCollisionEnter(c);
		}
	}
}
