using System.Collections;
using UnityEngine;

public class BuzzkillBee : ConfigurableNetworkObject
{
	public ConfigurableNetworkObject explosion;

	public float explosionDelay = 3f;

	public float flyOffDelay = 3f;

	private Animation myAnimation;

	private Transform myTransform;

	private Rigidbody myRigidbody;

	private bool hasExploded;

	public string spawnItemOverride = string.Empty;

	private void Awake()
	{
		myAnimation = base.animation;
		myTransform = base.transform;
		myRigidbody = base.rigidbody;
		myAnimation["wings"].layer = 0;
		myAnimation.Play("wings");
		myAnimation["bob"].layer = 1;
		myAnimation.Play("bob");
	}

	private new void Start()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("explosionDelay", ref explosionDelay, equipmentNames);
			itemByName.UpdateProperty("flyOffDelay", ref flyOffDelay, equipmentNames);
		}
		StartCoroutine(delayedFlyAway(flyOffDelay));
	}

	private void OnCollisionEnter(Collision c)
	{
		handleCollision(c.gameObject);
	}

	private void OnTriggerEnter(Collider c)
	{
		handleCollision(c.gameObject);
	}

	private void handleCollision(GameObject target)
	{
		base.animation.Stop("bob");
		if (target.layer == LayerMask.NameToLayer("Player") || target.layer == LayerMask.NameToLayer("Shield"))
		{
			OnDestroyOnImpact();
			return;
		}
		StopAllCoroutines();
		DelayedExplosion delayedExplosion = base.gameObject.AddComponent(typeof(DelayedExplosion)) as DelayedExplosion;
		ForwardSettings(delayedExplosion);
		delayedExplosion.delay = explosionDelay;
		delayedExplosion.explosion = explosion;
		delayedExplosion.OwnerID = base.OwnerID;
		delayedExplosion.DamageMultiplier = base.DamageMultiplier;
		Object.Destroy(myRigidbody);
		Object.Destroy(base.collider);
		Object.Destroy(this);
	}

	public void OnDestroyOnImpact()
	{
		if (explosion != null && !hasExploded)
		{
			GameObject gameObject = Object.Instantiate(explosion.gameObject, base.transform.position, Quaternion.identity) as GameObject;
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.OwnerID = base.OwnerID;
			component.DamageMultiplier = base.DamageMultiplier;
			component.SetItemOverride(spawnItemOverride);
			component.SetEquipmentNames(equipmentNames);
			hasExploded = true;
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator delayedFlyAway(float delay)
	{
		yield return new WaitForSeconds(delay);
		float scale = 1f;
		Vector3 vel = myRigidbody.velocity;
		while (scale > 0f)
		{
			yield return new WaitForSeconds(0.1f);
			vel.x *= 0.85f;
			vel.z *= 0.85f;
			vel.y += 15f;
			myRigidbody.velocity = vel;
			myTransform.localScale = new Vector3(scale, scale, scale);
			scale -= 0.01f;
		}
		Object.Destroy(base.gameObject);
	}
}
