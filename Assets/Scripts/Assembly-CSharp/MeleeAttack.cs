using UnityEngine;

public class MeleeAttack : WeaponBase
{
	public float damage;

	public float duration = 3f;

	public float tiredTime = 3f;

	public GameObject spawnObject;

	public Vector3 localOffset = Vector3.zero;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("damage", ref damage, base.EquipmentNames);
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		item.UpdateProperty("tiredTime", ref tiredTime, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (spawnObject != null)
		{
			GameObject gameObject = Object.Instantiate(spawnObject, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.parent = myTransform;
			gameObject.transform.localPosition = localOffset;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.SetItemOverride(base.name);
			component.SetEquipmentNames(base.EquipmentNames);
			component.OwnerID = ownerID;
			component.DamageMultiplier = base.playerController.DamageMultiplier;
			component.MeleeMultiplier = base.playerController.MeleeMultiplier;
			Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), myTransform.root.GetComponent<Collider>());
		}
		if (!dontSendNetworkMessages)
		{
			base.NetSyncReporter.SpawnProjectile(Vector3.zero, Vector3.zero);
		}
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (spawnObject != null)
		{
			GameObject gameObject = Object.Instantiate(spawnObject, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.parent = myTransform;
			gameObject.transform.localPosition = localOffset;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.SetItemOverride(base.name);
			component.SetEquipmentNames(base.EquipmentNames);
			component.OwnerID = ownerID;
			component.DamageMultiplier = base.playerController.DamageMultiplier;
			component.MeleeMultiplier = base.playerController.MeleeMultiplier;
			Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), myTransform.root.GetComponent<Collider>());
		}
	}
}
