using UnityEngine;

public class ShotgunWeapon : WeaponBase
{
	public GameObject shotgunDamageBurst;

	public Transform spawnPoint;

	private CharacterController charController;

	protected override void Start()
	{
		base.Start();
		charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (charController != null)
		{
			createBurst(spawnPoint.position, aimer.forward);
			if (base.NetSyncReporter != null && !dontSendNetworkMessages)
			{
				base.NetSyncReporter.SpawnProjectile(spawnPoint.position, aimer.forward);
			}
			return true;
		}
		return false;
	}

	private void createBurst(Vector3 pos, Vector3 dir)
	{
		GameObject gameObject = Object.Instantiate(shotgunDamageBurst, pos, Quaternion.identity) as GameObject;
		gameObject.BroadcastMessage("SetEquipmentNames", base.EquipmentNames, SendMessageOptions.DontRequireReceiver);
		gameObject.BroadcastMessage("SetItemOverride", base.name, SendMessageOptions.DontRequireReceiver);
		gameObject.transform.LookAt(pos + dir);
		NetworkObject componentInChildren = gameObject.GetComponentInChildren<NetworkObject>();
		componentInChildren.OwnerID = ownerID;
		componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
		Collider componentInChildren2 = gameObject.GetComponent<Collider>();
		if (componentInChildren2 == null)
		{
			componentInChildren2 = gameObject.GetComponentInChildren<Collider>();
		}
		if (!(componentInChildren2 != null))
		{
			return;
		}
		if (charController != null)
		{
			Physics.IgnoreCollision(componentInChildren2, charController);
			return;
		}
		CapsuleCollider capsuleCollider = myTransform.root.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
		if (capsuleCollider != null)
		{
			Physics.IgnoreCollision(componentInChildren2, capsuleCollider);
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 rot, int delay)
	{
		base.OnRemoteAttack(pos, rot, delay);
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		createBurst(pos, rot);
	}
}
