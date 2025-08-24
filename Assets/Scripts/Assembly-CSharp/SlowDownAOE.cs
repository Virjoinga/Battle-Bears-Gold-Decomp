using UnityEngine;

public class SlowDownAOE : ConfigurableNetworkObject
{
	private Collider myCollider;

	public float duration = 10f;

	public float radius = 300f;

	public float slowAmount = -1f;

	public GameObject hitEffect;

	private void Awake()
	{
		myCollider = base.GetComponent<Collider>();
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
			itemByName.UpdateProperty("slowAmount", ref slowAmount, equipmentNames);
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

	private void OnTriggerEnter(Collider c)
	{
		PlayerController playerController = c.gameObject.GetComponent(typeof(PlayerController)) as PlayerController;
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(base.OwnerID);
		DamageReceiverProxy component = c.gameObject.GetComponent<DamageReceiverProxy>();
		if (component != null)
		{
			PlayerCharacterManager playerCharacterManager2 = GameManager.Instance.Players(component.OwnerID);
			playerController = playerCharacterManager2.PlayerController;
		}
		if (playerController != null && playerController.Team != playerCharacterManager.team)
		{
			playerController.OnSlowed(duration, slowAmount);
			if (hitEffect != null)
			{
				GameObject gameObject = Object.Instantiate(hitEffect) as GameObject;
				Transform transform = gameObject.transform;
				Transform transform2 = playerController.transform;
				transform.position = transform2.position;
				transform.rotation = transform2.rotation;
				Vector3 eulerAngles = transform.eulerAngles;
				transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y + 180f, eulerAngles.z);
				transform.parent = transform2;
				transform.Translate(new Vector3(0f, -20f, -35f));
			}
		}
	}
}
