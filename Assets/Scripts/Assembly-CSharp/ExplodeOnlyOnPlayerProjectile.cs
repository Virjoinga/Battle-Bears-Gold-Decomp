using UnityEngine;

public class ExplodeOnlyOnPlayerProjectile : Projectile
{
	public override void Explode(GameObject objectHit)
	{
		if (objectHit.GetComponent<DamageReceiver>() != null)
		{
			base.Explode(objectHit);
		}
	}
}
