using UnityEngine;

public class DeathArea : MonoBehaviour
{
	public void OnTriggerEnter(Collider c)
	{
		dealDamage(c.gameObject);
	}

	public void OnCollisionEnter(Collision c)
	{
		dealDamage(c.gameObject);
	}

	public void OnCollisionStay(Collision c)
	{
		dealDamage(c.gameObject);
	}

	private void dealDamage(GameObject target)
	{
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (damageReceiver != null)
		{
			damageReceiver.OnKilledByDeathArea();
		}
	}
}
