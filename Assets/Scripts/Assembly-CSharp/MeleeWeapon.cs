using UnityEngine;

public class MeleeWeapon : WeaponBase
{
	public float damage = 25f;

	public bool useBothHands;

	public GameObject hitEffect;

	public bool isExplosive;

	private bool isAttacking;

	public AudioClip hitSound;

	private bool hasDealtDamage;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("melee_damage", ref damage, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Start()
	{
		base.Start();
		Collider collider = null;
		CharacterController characterController = myTransform.root.GetComponentInChildren(typeof(CharacterController)) as CharacterController;
		if (characterController != null)
		{
			collider = characterController.collider;
			if (base.collider != null && characterController.enabled && base.collider.enabled)
			{
				Physics.IgnoreCollision(base.collider, characterController);
			}
		}
		else
		{
			CapsuleCollider capsuleCollider = myTransform.root.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			if (capsuleCollider != null)
			{
				collider = capsuleCollider.collider;
				if (base.collider != null && base.collider.enabled && capsuleCollider != null && capsuleCollider.enabled)
				{
					Physics.IgnoreCollision(base.collider, capsuleCollider);
				}
			}
		}
		if (!(collider != null))
		{
			return;
		}
		Component[] componentsInChildren = myTransform.GetComponentsInChildren(typeof(Collider));
		Component[] array = componentsInChildren;
		foreach (Component component in array)
		{
			Collider collider2 = component as Collider;
			if (collider.enabled && collider2.enabled)
			{
				Physics.IgnoreCollision(collider2, collider);
			}
		}
	}

	public void Reset()
	{
		if (base.collider != null)
		{
			base.collider.enabled = false;
			base.collider.enabled = true;
		}
		isAttacking = false;
		hasDealtDamage = false;
		Start();
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		isAttacking = true;
		SpawnAttackEffect();
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		SpawnAttackEffect();
	}

	private void SpawnAttackEffect()
	{
		if (attackEffect != null)
		{
			GameObject gameObject = Object.Instantiate(attackEffect) as GameObject;
			gameObject.transform.parent = base.playerController.bodyRotator;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
		}
	}

	public void OnTriggerEnter(Collider c)
	{
		if (isAttacking)
		{
			if (hitEffect != null && base.collider != null)
			{
				Object.Instantiate(hitEffect, base.collider.bounds.center, Quaternion.identity);
			}
			dealDamage(c.gameObject);
			if (base.collider != null && base.collider.enabled && c.enabled)
			{
				Physics.IgnoreCollision(c, base.collider);
			}
		}
	}

	public void OnCollisionEnter(Collision c)
	{
		if (isAttacking)
		{
			if (hitEffect != null)
			{
				Object.Instantiate(hitEffect, c.contacts[0].point, Quaternion.identity);
			}
			dealDamage(c.gameObject);
			if (c.collider != null)
			{
				Physics.IgnoreCollision(c.collider, base.collider);
			}
		}
	}

	public void OnDealDamageFromSubObject(GameObject target, Collider c)
	{
		if (isAttacking)
		{
			if (hitEffect != null)
			{
				Object.Instantiate(hitEffect, c.bounds.center, Quaternion.identity);
			}
			dealDamage(target);
		}
	}

	private void dealDamage(GameObject target)
	{
		if (hasDealtDamage)
		{
			return;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren<PlayerController>();
		}
		if (target == null || base.playerController == null || base.playerController.WeaponManager.isDisabled)
		{
			return;
		}
		if (myAudio != null && hitSound != null)
		{
			myAudio.PlayOneShot(hitSound, SoundManager.Instance.getEffectsVolume());
		}
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (!(damageReceiver != null) || damageReceiver.OwnerID == base.OwnerID)
		{
			return;
		}
		Transform root = myTransform.root;
		Transform root2 = target.transform.root;
		float num = Vector3.Angle(root.forward, root2.forward);
		float num2 = 1f;
		if (num < 35f && GameManager.Instance != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(damageReceiver.OwnerID);
			PlayerCharacterManager playerCharacterManager2 = GameManager.Instance.Players(base.OwnerID);
			if (playerCharacterManager2 != null && playerCharacterManager != null && (!(GameManager.Instance.friendlyFireRatio < 0.01f) || (playerCharacterManager2.team != playerCharacterManager.team && base.OwnerID != damageReceiver.OwnerID)))
			{
				num2 = 2f;
				Object.Instantiate(Resources.Load("CommonEffects/backstab_hit"), damageReceiver.transform.position, Quaternion.identity);
				base.playerController.OnPlayBackstabSound();
			}
		}
		damageReceiver.OnTakeDamage(damage * base.playerController.DamageMultiplier * base.playerController.MeleeMultiplier * num2, base.OwnerID, isExplosive, true, false, true, false, 0f, string.Empty);
		hasDealtDamage = true;
	}
}
