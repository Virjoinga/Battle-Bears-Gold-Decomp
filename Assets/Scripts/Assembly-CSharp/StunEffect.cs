using UnityEngine;

public class StunEffect : NetworkObject
{
	public string configItemName = "SuckerPunch";

	public string EquipmentNames = string.Empty;

	public float stunTime = 2f;

	public AudioClip hitSound;

	private void Start()
	{
		if (configItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configItemName);
			itemByName.UpdateProperty("duration", ref stunTime, EquipmentNames);
		}
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
		if (hitSound != null)
		{
			base.audio.PlayOneShot(hitSound);
		}
		PlayerController playerController = col.GetComponent(typeof(PlayerController)) as PlayerController;
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(base.OwnerID);
		if (playerController != null && playerCharacterManager != null && playerCharacterManager.team != playerController.Team && !playerController.isImmuneToStun)
		{
			playerController.OnStun(stunTime);
		}
		base.gameObject.layer = LayerMask.NameToLayer("Gibs");
		Object.Destroy(this);
	}
}
