using UnityEngine;

public class Projectile : ConfigurableNetworkObject
{
	public ConfigurableNetworkObject objectToSpawn;

	protected bool hasSpawned;

	public string spawnItemOverride = string.Empty;

	private TeslaShield tesla;

	public bool ignoreOwner;

	public bool explodeOnCollision = true;

	public bool explodeOnTriggerEnter = true;

	[SerializeField]
	protected bool _tryToSpawnOnDisable;

	[SerializeField]
	protected bool _orientX;

	[SerializeField]
	protected bool _orientY;

	[SerializeField]
	protected bool _orientZ;

	[SerializeField]
	protected bool shakeScreen;

	[SerializeField]
	protected float shakeDuration;

	[SerializeField]
	protected float shakeStrength;

	[SerializeField]
	protected float explosionShakeRadius;

	[SerializeField]
	protected float shakeFunctionExponent = 3f;

	protected string EquipmentNames
	{
		get
		{
			return equipmentNames;
		}
	}

	protected virtual void OnTriggerEnter(Collider c)
	{
		tesla = c.GetComponent<TeslaShield>();
		if ((!(tesla != null) || !tesla.PlayerOnOwnersTeam(base.OwnerID)) && explodeOnTriggerEnter)
		{
			Explode(c.gameObject);
		}
	}

	protected virtual void OnCollisionEnter(Collision c)
	{
		if (explodeOnCollision)
		{
			Explode(c.gameObject);
		}
	}

	public virtual void Explode(GameObject objectHit)
	{
		if (ignoreOwner)
		{
			PlayerController component = objectHit.GetComponent<PlayerController>();
			if (component != null && component.OwnerID == base.OwnerID)
			{
				return;
			}
		}
		if (objectToSpawn != null && !hasSpawned)
		{
			CreateExplosion();
			hasSpawned = true;
		}
		DoCameraShake();
		TryDestroy();
	}

	protected virtual void CreateExplosion()
	{
		GameObject gameObject = Object.Instantiate(objectToSpawn.gameObject, base.transform.position, Quaternion.identity) as GameObject;
		ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
		component.OwnerID = base.OwnerID;
		component.DamageMultiplier = base.DamageMultiplier;
		component.SetItemOverride(spawnItemOverride);
		component.SetEquipmentNames(equipmentNames);
		if (base.rigidbody != null && (_orientX || _orientY || _orientZ))
		{
			gameObject.transform.LookAt(base.transform.position + base.rigidbody.velocity.normalized);
			Vector3 eulerAngles = gameObject.transform.rotation.eulerAngles;
			if (!_orientX)
			{
				eulerAngles.x = 0f;
			}
			if (!_orientY)
			{
				eulerAngles.y = 0f;
			}
			if (!_orientZ)
			{
				eulerAngles.z = 0f;
			}
			gameObject.transform.eulerAngles = eulerAngles;
		}
		if (tesla != null && gameObject.collider != null)
		{
			Physics.IgnoreCollision(tesla.PlayerController.DamageReceiver.collider, gameObject.collider);
		}
	}

	protected void DoCameraShake()
	{
		if (shakeScreen && Camera.main != null)
		{
			float f = (explosionShakeRadius - Vector3.Distance(base.transform.position, Camera.main.transform.position)) / explosionShakeRadius;
			float num = Mathf.Pow(f, shakeFunctionExponent);
			if (num > 0f)
			{
				ShakeCamera shakeCamera = Camera.main.gameObject.AddComponent<ShakeCamera>();
				shakeCamera.shakeDuration = shakeDuration;
				shakeCamera.shakeStrength = shakeStrength * num;
			}
		}
	}

	public virtual void TryDestroy()
	{
		Object.Destroy(base.gameObject);
	}

	public virtual void OnDisable()
	{
		if (_tryToSpawnOnDisable && objectToSpawn != null && !hasSpawned)
		{
			CreateExplosion();
		}
	}
}
