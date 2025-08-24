using System.Collections;
using UnityEngine;

public class HealTent : SpecialItem
{
	public float inputDisableTime = 1.5f;

	public float healAmount = 2f;

	public float healInterval = 1.5f;

	protected Animation myAnimation;

	private Transform playerModel;

	private Transform targetTransform;

	private Collider targetCollider;

	private DamageReceiverProxy proxy;

	public Transform cameraSpot;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/healingtent";
		}
	}

	protected override void Awake()
	{
		base.Awake();
		myAnimation = base.animation;
		proxy = GetComponent(typeof(DamageReceiverProxy)) as DamageReceiverProxy;
		base.enabled = false;
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("healAmount", ref healAmount, base.EquipmentNames);
		item.UpdateProperty("healInterval", ref healInterval, base.EquipmentNames);
		item.UpdateProperty("inputDisableTime", ref inputDisableTime, base.EquipmentNames);
		base.Configure(item);
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		if (proxy != null)
		{
			proxy.OnSetProxy(playerController.DamageReceiver);
		}
		if (myAnimation != null)
		{
			myAnimation.Play("Enter");
		}
		playerModel = playerController.transform.Find("playerModel");
		targetTransform = playerController.transform;
		targetCollider = playerController.collider;
		base.isRemote = isRemote;
		base.enabled = false;
		playerController.CanJump = true;
		playerController.WeaponManager.StopMelee();
		playerController.WeaponManager.isDisabled = true;
		if (!isRemote && playerController.WeaponManager is SatelliteSecondaryWeaponManager)
		{
			SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)playerController.WeaponManager;
			satelliteSecondaryWeaponManager.DeactivateSecondaryWeapon();
		}
		playerModel.localScale = Vector3.zero;
		playerController.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		playerController.WeaponManager.OnStopFiring();
		if (!isRemote)
		{
			HUD.Instance.isReloadAllowed = false;
			myTransform.parent = playerModel.transform;
			playerController.PlayerCam.NormalPosition = cameraSpot.localPosition;
			myTransform.parent = null;
			StartCoroutine(allowForResume(inputDisableTime));
			StartCoroutine("heal");
		}
	}

	private IEnumerator heal()
	{
		while (true)
		{
			if (playerController != null)
			{
				playerController.DamageReceiver.addHealth(healAmount);
			}
			yield return new WaitForSeconds(healInterval);
		}
	}

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		StartCoroutine(delayedDisable(delay));
	}

	private IEnumerator allowForResume(float delay)
	{
		SimpleControllerPerformer perf = (SimpleControllerPerformer)playerController.Performer;
		perf.LockInputs = true;
		yield return new WaitForSeconds(delay);
		perf.LockInputs = false;
		base.enabled = true;
		base.PlayerController.CanJump = false;
	}

	private void LateUpdate()
	{
		myTransform.position = new Vector3(targetTransform.position.x, targetCollider.bounds.min.y, targetTransform.position.z);
		if (playerController != null && playerController.Director != null && playerController.Director.Movement.magnitude > float.Epsilon)
		{
			OnDeactivate(0f);
			StopCoroutine("heal");
			base.enabled = false;
			playerController.CanJump = true;
		}
	}

	private IEnumerator delayedDisable(float delay)
	{
		float disableDelay = 0f;
		if (myAnimation != null)
		{
			disableDelay = myAnimation["Exit"].length;
			myAnimation.Play("Exit");
		}
		yield return new WaitForSeconds(disableDelay);
		myTransform.localScale = Vector3.zero;
		yield return new WaitForSeconds(0.5f - delay);
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (playerController.PlayerCam != null && playerController.PlayerCam.enabled)
		{
			playerController.PlayerCam.OnResetNormalPosition();
		}
		if (playerController != null)
		{
			playerController.WeaponManager.isDisabled = false;
			playerController.WeaponManager.OnResumeFromStun();
			playerController.gameObject.layer = LayerMask.NameToLayer("Player");
		}
		if (playerModel != null)
		{
			playerModel.localScale = new Vector3(1f, 1f, 1f);
		}
		if (!isRemote && HUD.Instance != null)
		{
			HUD.Instance.isReloadAllowed = true;
		}
	}
}
