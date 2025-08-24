using System.Collections;
using UnityEngine;

public class MGSBox : SpecialItem
{
	protected Animation myAnimation;

	private Transform playerModel;

	private DamageReceiverProxy proxy;

	private Vector3 lastPos;

	private Transform targetTransform;

	private Collider targetCollider;

	public Transform cameraSpot;

	private AudioSource myAudio;

	public AudioClip mgsBoxWalkSound;

	private Transform playerArrowHighlight;

	private TextMesh playerNameText;

	private string originalName = string.Empty;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/mgsbox";
		}
	}

	protected override void Awake()
	{
		base.Awake();
		myAnimation = base.GetComponent<Animation>();
		myAnimation["Walk"].speed = 2f;
		proxy = GetComponent(typeof(DamageReceiverProxy)) as DamageReceiverProxy;
		myAudio = base.GetComponent<AudioSource>();
		base.enabled = false;
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
		playerArrowHighlight = playerController.transform.Find("playerHighlight");
		if (playerArrowHighlight != null)
		{
			playerArrowHighlight.localScale = Vector3.zero;
		}
		Transform transform = playerController.transform.Find("playerName");
		if (transform != null)
		{
			playerNameText = transform.GetComponentInChildren<TextMesh>();
		}
		if (playerNameText != null)
		{
			originalName = playerNameText.text;
			playerNameText.text = string.Empty;
		}
		if (!isRemote && playerController.WeaponManager is SatelliteSecondaryWeaponManager)
		{
			SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)playerController.WeaponManager;
			satelliteSecondaryWeaponManager.DeactivateSecondaryWeapon();
		}
		playerModel = playerController.transform.Find("playerModel");
		base.isRemote = isRemote;
		targetTransform = playerController.transform;
		targetCollider = playerController.GetComponent<Collider>();
		base.enabled = true;
		Debug.Log("setting canJump to false");
		playerController.CanJump = false;
		if (!isRemote)
		{
			HUD.Instance.isReloadAllowed = false;
			myTransform.parent = playerModel.transform;
			playerController.PlayerCam.NormalPosition = cameraSpot.localPosition;
			myTransform.parent = null;
		}
		playerModel.localScale = Vector3.zero;
		playerController.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		playerController.WeaponManager.isDisabled = true;
		playerController.WeaponManager.OnStopFiring();
		playerController.WeaponManager.StopMelee();
		RainbowShot componentInChildren = playerController.GetComponentInChildren<RainbowShot>();
		if (componentInChildren != null)
		{
			componentInChildren.CancelBeam = true;
		}
		lastPos = targetTransform.position;
		playerController.IsBombPickupAllowed = false;
		playerController.gameObject.BroadcastMessage("OnEnterMGSBox", SendMessageOptions.DontRequireReceiver);
	}

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		StartCoroutine(delayedDisable(delay));
	}

	private void LateUpdate()
	{
		Vector3 vector = targetTransform.position - lastPos;
		lastPos = targetTransform.position;
		myTransform.position = new Vector3(targetTransform.position.x, targetCollider.bounds.min.y, targetTransform.position.z);
		if (playerController != null && playerController.Director != null && playerController.Director.Fire)
		{
			OnDeactivate(0f);
			base.enabled = false;
			Debug.Log("setting canJump to true");
			playerController.CanJump = true;
			return;
		}
		float magnitude = vector.magnitude;
		if (magnitude > 1f && !myAnimation.isPlaying)
		{
			if (mgsBoxWalkSound != null)
			{
				myAudio.PlayOneShot(mgsBoxWalkSound);
			}
			myAnimation.Play("Walk");
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
		playerController.IsBombPickupAllowed = true;
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
			playerController.gameObject.BroadcastMessage("OnExitMGSBox", SendMessageOptions.DontRequireReceiver);
			playerController.gameObject.layer = LayerMask.NameToLayer("Player");
		}
		if (playerModel != null)
		{
			playerModel.localScale = new Vector3(1f, 1f, 1f);
		}
		if (playerArrowHighlight != null)
		{
			playerArrowHighlight.localScale = new Vector3(1f, 1f, 1f);
		}
		if (playerNameText != null)
		{
			playerNameText.text = originalName;
		}
		playerController.WeaponManager.isDisabled = false;
		if (!isRemote && HUD.Instance != null)
		{
			HUD.Instance.isReloadAllowed = true;
		}
	}
}
