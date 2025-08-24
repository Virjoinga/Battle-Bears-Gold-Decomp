using UnityEngine;

public class DamageOverTimeArea : MonoBehaviour
{
	public float damagePerSecond;

	private float amount;

	public void OnTriggerEnter(Collider c)
	{
		dealDamage(c.gameObject);
	}

	public void OnTriggerStay(Collider c)
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

	private void FixedUpdate()
	{
		amount = damagePerSecond * Time.fixedDeltaTime;
	}

	private void dealDamage(GameObject target)
	{
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (damageReceiver != null)
		{
			damageReceiver.OnTakeDamage(amount, -1, true, false, false, false, false, 0f, string.Empty);
		}
	}
}
