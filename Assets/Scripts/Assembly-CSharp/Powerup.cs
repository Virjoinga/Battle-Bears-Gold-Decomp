using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
	public GameObject pickupEffect;

	private PlayerController playerController;

	protected string EquipmentNames
	{
		get
		{
			if (playerController != null && playerController.CharacterManager != null)
			{
				return playerController.CharacterManager.EquipmentNames;
			}
			return string.Empty;
		}
	}

	protected virtual void Start()
	{
		if (base.rigidbody != null)
		{
			base.rigidbody.WakeUp();
		}
	}

	protected abstract void Configure(Item item);

	protected void OnTriggerEnter(Collider c)
	{
		playerController = c.gameObject.GetComponent(typeof(PlayerController)) as PlayerController;
		if (playerController != null)
		{
			if (pickupEffect != null)
			{
				GameObject gameObject = Object.Instantiate(pickupEffect) as GameObject;
				if (gameObject != null)
				{
					gameObject.transform.parent = playerController.transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localEulerAngles = Vector3.zero;
					gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
			if (!playerController.isRemote)
			{
				if (ServiceManager.Instance != null)
				{
					Configure(ServiceManager.Instance.GetItemByName(base.name.Split(' ')[0]));
				}
				OnPickup(base.gameObject, playerController);
			}
		}
		playerController = null;
	}

	protected abstract void OnPickup(GameObject obj, PlayerController p);
}
