using UnityEngine;

public class SingleShotDamageSource : ConfigurableNetworkObject
{
	private AudioSource myAudio;

	public float damage = 10f;

	public int maxBouncesAllowed;

	public float bounceDmgReduction = 0.2f;

	private int numBounces;

	private bool hasSpawned;

	public ConfigurableNetworkObject objectToSpawn;

	public GameObject playerHitParticlePrefab;

	public string spawnItemOverride = string.Empty;

	public bool destroyOnCollision;

	public bool useChargedDamage;

	private void Awake()
	{
		myAudio = base.GetComponent<AudioSource>();
	}

	private new void Start()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			if (useChargedDamage)
			{
				itemByName.UpdateProperty("chargedDamage", ref damage, equipmentNames);
			}
			else
			{
				itemByName.UpdateProperty("damage", ref damage, equipmentNames);
			}
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		numBounces++;
		handleCollision(c.gameObject);
	}

	private void OnTriggerEnter(Collider c)
	{
		TeslaShield component = c.GetComponent<TeslaShield>();
		if (!(component != null) || !component.PlayerOnOwnersTeam(base.OwnerID))
		{
			handleCollision(c.gameObject);
		}
	}

	public void Explode(GameObject objectHit)
	{
		handleCollision(objectHit);
	}

	protected virtual void handleCollision(GameObject target)
	{
		if (myAudio != null)
		{
			myAudio.PlayOneShot(myAudio.clip);
		}
		if (objectToSpawn != null && !hasSpawned)
		{
			GameObject gameObject = Object.Instantiate(objectToSpawn.gameObject, base.transform.position, Quaternion.identity) as GameObject;
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.OwnerID = base.OwnerID;
			component.DamageMultiplier = base.DamageMultiplier;
			component.SetItemOverride(spawnItemOverride);
			component.SetEquipmentNames(equipmentNames);
			hasSpawned = true;
		}
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (damageReceiver != null)
		{
			if (playerHitParticlePrefab != null)
			{
				Object.Instantiate(playerHitParticlePrefab, base.transform.position, base.transform.rotation);
			}
			if (maxBouncesAllowed == 0)
			{
				damageReceiver.OnTakeDamage(damage * base.DamageMultiplier, base.OwnerID, false, false, false, true, false, 0f, string.Empty);
			}
			else
			{
				float num = Mathf.Min(1f, 1f - (float)(numBounces - 1) * bounceDmgReduction);
				damageReceiver.OnTakeDamage(damage * num * base.DamageMultiplier, base.OwnerID, false, false, false, true, false, 0f, string.Empty);
			}
			base.gameObject.layer = LayerMask.NameToLayer("Gibs");
		}
		else if (numBounces > maxBouncesAllowed)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Gibs");
		}
		if (destroyOnCollision)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
