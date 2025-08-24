using System.Collections;
using UnityEngine;

public class LactoseIntolerance : WeaponBase
{
	public GameObject explosion;

	private float growTime = 5f;

	private float largeTime = 3f;

	private float speedDecrease = 250f;

	private CharacterController charController;

	private bool hasModifiedSpeed;

	public AudioClip[] fartingSounds;

	public AudioClip[] hugeSounds;

	public AudioClip unstableSound;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("growTime", ref growTime, base.EquipmentNames);
		item.UpdateProperty("largeTime", ref largeTime, base.EquipmentNames);
		item.UpdateProperty("slowAmount", ref speedDecrease, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

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
		StartCoroutine(playSelfDestructSequence(0f));
		if (base.NetSyncReporter != null && !dontSendNetworkMessages)
		{
			base.NetSyncReporter.SpawnProjectile(Vector3.zero, Vector3.zero);
		}
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		if (charController == null)
		{
			charController = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		}
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (vel.x == -1f && vel.y == -1f && vel.z == -1f)
		{
			spawnCheeseExplosion(pos);
		}
		else
		{
			StartCoroutine(playSelfDestructSequence((float)delay / 1000f));
		}
	}

	private IEnumerator playSelfDestructSequence(float delay)
	{
		if (!isRemote)
		{
			HUD.Instance.isReloadAllowed = false;
			HUD.Instance.isSpecialAllowed = false;
			base.playerController.WeaponManager.isDisabled = true;
			base.playerController.IsBombPickupAllowed = false;
		}
		base.playerController.BodyAnimator.IsFiring = true;
		base.playerController.isImmuneToStun = true;
		Animation bodyAnimation = base.playerController.BodyAnimator.Animator;
		bodyAnimation.CrossFade("lactose_fireIn");
		yield return new WaitForSeconds(bodyAnimation["lactose_fireIn"].length);
		bodyAnimation["lactose_grow"].speed = bodyAnimation["lactose_grow"].length / growTime;
		bodyAnimation.CrossFade("lactose_grow");
		float growTimeLeft = growTime;
		while (growTimeLeft > 0f)
		{
			AudioClip fartClip = fartingSounds[Random.Range(0, fartingSounds.Length)];
			if (fartClip.length < growTimeLeft)
			{
				myAudio.PlayOneShot(fartClip);
				yield return new WaitForSeconds(fartClip.length);
				growTimeLeft -= fartClip.length;
				continue;
			}
			break;
		}
		yield return new WaitForSeconds(growTimeLeft);
		if (!isRemote)
		{
			hasModifiedSpeed = true;
			float actualForwardSpeedDecrease = ((!(speedDecrease > base.playerController.StatManager[Statistic.MaxForwardMovementSpeed])) ? speedDecrease : base.playerController.StatManager[Statistic.MaxForwardMovementSpeed]);
			float actualSidewaysSpeedDecrease = ((!(speedDecrease > base.playerController.StatManager[Statistic.MaxSidewaysMovementSpeed])) ? speedDecrease : base.playerController.StatManager[Statistic.MaxSidewaysMovementSpeed]);
			float actualBackwardSpeedDecrease = ((!(speedDecrease > base.playerController.StatManager[Statistic.MaxBackwardsMovementSpeed])) ? speedDecrease : base.playerController.StatManager[Statistic.MaxBackwardsMovementSpeed]);
			StatisticMod forwardMod = new StatisticMod(Statistic.MaxForwardMovementSpeed, largeTime, 0f - actualForwardSpeedDecrease);
			StatisticMod sidewaysMod = new StatisticMod(Statistic.MaxSidewaysMovementSpeed, largeTime, 0f - actualSidewaysSpeedDecrease);
			StatisticMod backwardMod = new StatisticMod(Statistic.MaxBackwardsMovementSpeed, largeTime, 0f - actualBackwardSpeedDecrease);
			base.playerController.StatManager.AddStatMod(forwardMod);
			base.playerController.StatManager.AddStatMod(sidewaysMod);
			base.playerController.StatManager.AddStatMod(backwardMod);
		}
		bodyAnimation.CrossFade("lactose_fire");
		float largeTimeLeft = largeTime;
		while (growTimeLeft > 0f)
		{
			AudioClip hugeClip = hugeSounds[Random.Range(0, hugeSounds.Length)];
			if (hugeClip.length < largeTimeLeft)
			{
				myAudio.PlayOneShot(hugeClip);
				yield return new WaitForSeconds(hugeClip.length);
				largeTimeLeft -= hugeClip.length;
				continue;
			}
			break;
		}
		yield return new WaitForSeconds(largeTimeLeft);
		myAudio.PlayOneShot(unstableSound);
		bodyAnimation.CrossFade("lactose_deathLoop");
		yield return new WaitForSeconds(bodyAnimation["lactose_deathLoop"].length);
		if (GameManager.Instance.IsGameSubmitted)
		{
			yield break;
		}
		spawnCheeseExplosion(base.playerController.transform.position);
		if (!isRemote)
		{
			if (base.NetSyncReporter != null && !dontSendNetworkMessages)
			{
				base.NetSyncReporter.SpawnProjectile(base.playerController.transform.position, new Vector3(-1f, -1f, -1f));
			}
			base.playerController.SpawnGibs = false;
			base.playerController.DamageReceiver.OnKilledByDeathArea();
		}
		reset();
	}

	private void spawnCheeseExplosion(Vector3 pos)
	{
		if (GameManager.Instance.IsGameSubmitted || !(base.playerController != null))
		{
			return;
		}
		base.playerController.SpawnGibs = false;
		GameObject gameObject = Object.Instantiate(explosion, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.position = pos;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		ConfigurableNetworkObject componentInChildren = gameObject.GetComponentInChildren<ConfigurableNetworkObject>();
		componentInChildren.SetItemOverride(base.name);
		componentInChildren.SetEquipmentNames(base.EquipmentNames);
		componentInChildren.OwnerID = ownerID;
		componentInChildren.DamageMultiplier = base.playerController.DamageMultiplier;
		GibsController component = gameObject.GetComponent<GibsController>();
		if (component != null && base.playerController != null && base.playerController.CharacterHandle != null && base.playerController.CharacterHandle.Renderers.Length > 0)
		{
			component.skinMaterial = base.playerController.CharacterHandle.Renderers[0].material;
		}
		Collider componentInChildren2 = gameObject.collider;
		if (componentInChildren2 == null)
		{
			componentInChildren2 = gameObject.GetComponentInChildren<Collider>();
		}
		if (charController != null)
		{
			Physics.IgnoreCollision(componentInChildren2, charController);
			return;
		}
		CapsuleCollider capsuleCollider = myTransform.root.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
		if (capsuleCollider != null && componentInChildren2.gameObject.activeInHierarchy && capsuleCollider.gameObject.activeInHierarchy)
		{
			Physics.IgnoreCollision(componentInChildren2, capsuleCollider);
		}
	}

	private void reset()
	{
		StopAllCoroutines();
		if (!isRemote && base.playerController != null)
		{
			base.playerController.IsBombPickupAllowed = true;
			if (hasModifiedSpeed)
			{
				hasModifiedSpeed = false;
			}
			if (HUD.Instance != null)
			{
				HUD.Instance.isReloadAllowed = true;
				HUD.Instance.isSpecialAllowed = true;
			}
			base.playerController.WeaponManager.isDisabled = false;
		}
	}

	public void OnOwnerDead()
	{
		reset();
	}

	public void OnBombDeactivate()
	{
		reset();
	}

	private void OnDisable()
	{
		reset();
	}
}
