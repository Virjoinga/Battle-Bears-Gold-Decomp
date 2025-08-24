using UnityEngine;

public class AcmeRocket : ConfigurableNetworkObject
{
	public ConfigurableNetworkObject explosion;

	public float explosionDelay = 3f;

	public AudioClip wickSound;

	private void OnCollisionEnter(Collision c)
	{
		handleCollision(c.gameObject);
	}

	private void OnTriggerEnter(Collider c)
	{
		handleCollision(c.gameObject);
	}

	private new void Start()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("explosionDelay", ref explosionDelay, equipmentNames);
		}
	}

	private void handleCollision(GameObject collider)
	{
		if (collider.layer == LayerMask.NameToLayer("Shield"))
		{
			OnHitShield();
		}
		AudioSource audioSource = base.GetComponent<AudioSource>();
		if (wickSound != null)
		{
			audioSource.clip = wickSound;
			audioSource.loop = true;
			audioSource.Play();
		}
		Animation animation = base.GetComponent<Animation>();
		if (animation != null)
		{
			animation.Stop();
			animation["wick"].speed = 0.5f;
			animation.Play("wick");
		}
		DelayedExplosion delayedExplosion = base.gameObject.AddComponent(typeof(DelayedExplosion)) as DelayedExplosion;
		ForwardSettings(delayedExplosion);
		delayedExplosion.delay = explosionDelay;
		delayedExplosion.explosion = explosion;
		delayedExplosion.OwnerID = base.OwnerID;
		delayedExplosion.DamageMultiplier = base.DamageMultiplier;
	}

	public void OnHitShield()
	{
		GameObject gameObject = Object.Instantiate(explosion.gameObject, base.transform.position, Quaternion.identity) as GameObject;
		NetworkObject component = gameObject.GetComponent<NetworkObject>();
		component.OwnerID = base.OwnerID;
		component.DamageMultiplier = base.DamageMultiplier;
		Object.Destroy(base.gameObject);
	}
}
