using System.Collections;
using UnityEngine;

public class FlamethrowerWeapon : WeaponBase
{
	public GameObject flames;

	[SerializeField]
	private string _fireInAnim;

	[SerializeField]
	private string _fireLoopAnim;

	private float duration = 5f;

	private float speedIncrease = 50f;

	private CharacterController charController;

	private GameObject currentFlames;

	private Transform currentFlameTransform;

	private bool hasModifiedSpeed;

	public AudioClip[] screamClips;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		item.UpdateProperty("speedIncrease", ref speedIncrease, base.EquipmentNames);
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
		StartCoroutine(playAttackSequence(duration));
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
		StartCoroutine(playAttackSequence(duration - (float)delay / 1000f));
	}

	private IEnumerator playAttackSequence(float duration)
	{
		if (base.playerController.BodyAnimator.IsFiring)
		{
			yield break;
		}
		if (!isRemote)
		{
			HUD.Instance.isReloadAllowed = false;
			HUD.Instance.isSpecialAllowed = false;
			base.playerController.WeaponManager.isDisabled = true;
		}
		base.playerController.BodyAnimator.IsFiring = true;
		Animation bodyAnimation = base.playerController.BodyAnimator.Animator;
		if (bodyAnimation.GetComponent<Animation>().GetClip(_fireInAnim) != null)
		{
			bodyAnimation.CrossFade(_fireInAnim);
			yield return new WaitForSeconds(bodyAnimation[_fireInAnim].length);
		}
		if (!isRemote)
		{
			hasModifiedSpeed = true;
			StatisticMod forwardMod = new StatisticMod(Statistic.MaxForwardMovementSpeed, duration, speedIncrease);
			StatisticMod sidewaysMod = new StatisticMod(Statistic.MaxSidewaysMovementSpeed, duration, speedIncrease);
			StatisticMod backwardMod = new StatisticMod(Statistic.MaxBackwardsMovementSpeed, duration, speedIncrease);
			base.playerController.StatManager.AddStatMod(forwardMod);
			base.playerController.StatManager.AddStatMod(sidewaysMod);
			base.playerController.StatManager.AddStatMod(backwardMod);
		}
		currentFlames = Object.Instantiate(flames, Vector3.zero, Quaternion.identity) as GameObject;
		currentFlameTransform = currentFlames.transform;
		currentFlameTransform.localScale = new Vector3(1f, 1f, 1f);
		ConfigurableNetworkObject i = currentFlames.GetComponent<ConfigurableNetworkObject>();
		i.SetItemOverride(base.name);
		i.SetEquipmentNames(base.EquipmentNames);
		i.OwnerID = ownerID;
		i.DamageMultiplier = base.playerController.DamageMultiplier;
		Collider newCollider = currentFlames.GetComponent<Collider>();
		if (newCollider == null)
		{
			newCollider = currentFlames.GetComponentInChildren<Collider>();
		}
		if (charController != null)
		{
			Physics.IgnoreCollision(newCollider, charController);
		}
		else
		{
			CapsuleCollider capsule = myTransform.root.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			if (capsule != null && capsule.enabled && newCollider.enabled)
			{
				Physics.IgnoreCollision(newCollider, capsule);
			}
		}
		if (bodyAnimation.GetComponent<Animation>().GetClip(_fireLoopAnim) != null)
		{
			bodyAnimation.CrossFade(_fireLoopAnim);
		}
		yield return new WaitForSeconds(duration);
		if (GameManager.Instance != null && !GameManager.Instance.IsGameSubmitted)
		{
			base.playerController.WeaponManager.OnPlayReload(0f);
			reset();
		}
	}

	private void LateUpdate()
	{
		if (currentFlameTransform != null && base.playerController != null)
		{
			currentFlameTransform.position = base.playerController.DamageReceiver.headSpot.position;
			currentFlameTransform.rotation = base.playerController.bodyRotator.rotation;
			if (myAudio != null && screamClips.Length > 0 && !myAudio.isPlaying)
			{
				myAudio.clip = screamClips[Random.Range(0, screamClips.Length)];
				myAudio.Play();
			}
		}
	}

	private void reset()
	{
		StopAllCoroutines();
		if (currentFlames != null)
		{
			Transform transform = currentFlames.transform.Find("flame");
			if (transform != null)
			{
				transform.GetComponent<ParticleSystem>().enableEmission = false;
				transform.transform.parent = currentFlames.transform.parent;
				DelayedDestroy delayedDestroy = transform.gameObject.AddComponent<DelayedDestroy>();
				delayedDestroy.delay = 2f;
			}
			Object.Destroy(currentFlames);
		}
		if (!isRemote && base.playerController != null)
		{
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
