using UnityEngine;

public class DrunkAOE : ConfigurableNetworkObject
{
	private Collider myCollider;

	public float duration = 5f;

	private void Awake()
	{
		myCollider = base.collider;
	}

	protected override void Start()
	{
		base.Start();
		if (configureItemName != null && configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			if (itemByName != null)
			{
				itemByName.UpdateProperty("duration", ref duration, equipmentNames);
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
			if (playerCharacterManager != null && (component.Team != playerCharacterManager.team || component.OwnerID == playerCharacterManager.OwnerID))
			{
				component.OnGetDreamy(duration);
			}
		}
	}
}
