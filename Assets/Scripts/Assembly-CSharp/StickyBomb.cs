using UnityEngine;

public class StickyBomb : DelayedGravityProjectile
{
	public AudioClip stickSound;

	private void OnCollisionEnter(Collision c)
	{
		OnStick();
	}

	private void OnTriggerEnter(Collider c)
	{
		TeslaShield component = c.GetComponent<TeslaShield>();
		if (!(component != null) || !component.PlayerOnOwnersTeam(ownerID))
		{
			OnStick();
		}
	}

	private void OnStick()
	{
		if (stickSound != null && base.GetComponent<AudioSource>() != null)
		{
			base.GetComponent<AudioSource>().PlayOneShot(stickSound);
		}
		Object.Destroy(GetComponent(typeof(ConstantForce)));
		myRigidbody.Sleep();
		Object.Destroy(this);
	}
}
