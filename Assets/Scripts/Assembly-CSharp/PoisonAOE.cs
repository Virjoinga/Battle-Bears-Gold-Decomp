using UnityEngine;

public class PoisonAOE : ConfigurableNetworkObject
{
	private Collider myCollider;

	[SerializeField]
	private bool _ignoreOwner;

	[SerializeField]
	private PoisonColor _poisonColor;

	[SerializeField]
	private string _customDeathSfx;

	public float damagePerSecond = 10f;

	public float duration = 10f;

	public float radius = 300f;

	private void Awake()
	{
		myCollider = base.collider;
	}

	private new void Start()
	{
		if (configureItemName == null || !(configureItemName != string.Empty) || ServiceManager.Instance == null)
		{
			return;
		}
		Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
		if (itemByName != null)
		{
			itemByName.UpdateProperty("damage", ref damagePerSecond, equipmentNames);
			itemByName.UpdateProperty("duration", ref duration, equipmentNames);
			itemByName.UpdateProperty("range", ref radius, equipmentNames);
		}
		if (myCollider != null)
		{
			SphereCollider sphereCollider = (SphereCollider)myCollider;
			if (sphereCollider != null)
			{
				sphereCollider.radius = radius;
			}
		}
	}

	public void OnTriggerEnter(Collider c)
	{
		Physics.IgnoreCollision(myCollider, c);
		handleCollision(c.gameObject);
	}

	public void OnCollisionEnter(Collision c)
	{
		Physics.IgnoreCollision(myCollider, c.collider);
		handleCollision(c.gameObject);
	}

	private void handleCollision(GameObject target)
	{
		PlayerController component = target.GetComponent<PlayerController>();
		if (component != null && !component.isRemote)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(base.OwnerID);
			if (playerCharacterManager != null && (component.Team != playerCharacterManager.team || (component.OwnerID == playerCharacterManager.OwnerID && !_ignoreOwner)))
			{
				Poison poison = component.gameObject.AddComponent<Poison>();
				poison.poisonAmount = damagePerSecond * base.DamageMultiplier;
				poison.poisonDuration = duration;
				poison.poisonerPlayerID = base.OwnerID;
				poison.customDeathSfx = _customDeathSfx;
				component.CurrentPoisonColor = _poisonColor;
			}
		}
	}
}
