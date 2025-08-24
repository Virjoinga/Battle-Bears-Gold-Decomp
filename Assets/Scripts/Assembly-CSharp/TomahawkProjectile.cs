using System.Collections;
using UnityEngine;

public class TomahawkProjectile : NetworkObject
{
	public string configItemName = "Tomahawk";

	public string EquipmentNames = string.Empty;

	public float hawkTime = 10f;

	public Renderer hawkRenderer;

	public GameObject collisionParticle;

	public BIRD_MOUNT birdType;

	private float damage = 10f;

	private void Awake()
	{
		if (hawkRenderer != null)
		{
			hawkRenderer.enabled = false;
			StartCoroutine(delayedShowHawk());
		}
	}

	private void Start()
	{
		if (configItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configItemName);
			itemByName.UpdateProperty("duration", ref hawkTime, EquipmentNames);
			itemByName.UpdateProperty("damage", ref damage, EquipmentNames);
		}
		base.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(100f, 0f, 0f), ForceMode.VelocityChange);
	}

	private IEnumerator delayedShowHawk()
	{
		yield return new WaitForSeconds(0.1f);
		hawkRenderer.enabled = true;
	}

	private void OnCollisionEnter(Collision c)
	{
		handleCollision(c.gameObject);
	}

	private void OnTriggerEnter(Collider c)
	{
		handleCollision(c.gameObject);
	}

	private void handleCollision(GameObject col)
	{
		if (collisionParticle != null)
		{
			Object.Instantiate(collisionParticle, base.transform.position, Quaternion.identity);
		}
		PlayerController playerController = col.GetComponent(typeof(PlayerController)) as PlayerController;
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(base.OwnerID);
		if (playerController != null && playerCharacterManager != null && playerCharacterManager.team != playerController.Team)
		{
			playerController.OnBirded(hawkTime, birdType, base.OwnerID, base.DamageMultiplier * damage);
			Object.Destroy(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
