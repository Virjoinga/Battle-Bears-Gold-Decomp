using UnityEngine;

public class LineOfSightDamageSource : ConfigurableNetworkObject
{
	[SerializeField]
	private bool _useChargedDamage;

	[SerializeField]
	private float _radiationDamage;

	public float damage;

	public LayerMask layerMask;

	protected Collider myCollider;

	protected Transform myTransform;

	protected virtual void Awake()
	{
		myCollider = base.GetComponent<Collider>();
		myTransform = base.transform;
	}

	protected new virtual void Start()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			if (_useChargedDamage)
			{
				itemByName.UpdateProperty("chargedDamage", ref damage, equipmentNames);
				return;
			}
			itemByName.UpdateProperty("damage", ref damage, equipmentNames);
			itemByName.UpdateProperty("radiation", ref _radiationDamage, equipmentNames);
		}
	}

	public void OnTriggerEnter(Collider c)
	{
		if (myCollider != null)
		{
			Physics.IgnoreCollision(myCollider, c);
		}
		dealDamage(c.gameObject);
	}

	public void OnCollisionEnter(Collision c)
	{
		if (myCollider != null)
		{
			Physics.IgnoreCollision(myCollider, c.collider);
		}
		dealDamage(c.gameObject);
	}

	protected bool checkForActualHit(GameObject target)
	{
		bool result = false;
		if (target != null && target.GetComponent<Collider>() != null && myTransform != null)
		{
			Bounds bounds = target.GetComponent<Collider>().bounds;
			Vector3[] array = new Vector3[2]
			{
				new Vector3(bounds.center.x, bounds.max.y, bounds.center.z),
				new Vector3(bounds.center.x, bounds.min.y, bounds.center.z)
			};
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector = array[i] - base.transform.position;
				RaycastHit hitInfo;
				if (!Physics.Raycast(myTransform.position, vector.normalized, out hitInfo, vector.magnitude, layerMask))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	protected virtual void dealDamage(GameObject target)
	{
		if (checkForActualHit(target))
		{
			DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			if (damageReceiver != null)
			{
				damageReceiver.OnTakeDamage(damage * base.DamageMultiplier, base.OwnerID, false, false, false, true, false, _radiationDamage, string.Empty);
			}
		}
	}
}
