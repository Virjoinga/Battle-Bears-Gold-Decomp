using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	private Transform myTransform;

	private Animation bombAnimator;

	public Team team;

	private bool isHeld;

	private PlayerController holdingPlayer;

	public GameObject timerDisplay;

	private GameObject currentTimerSystem;

	private Transform timerMount;

	private bool hasTimerStarted;

	public GameObject wickParticles;

	public AudioClip beep;

	private AudioSource myAudio;

	private Collider myCollider;

	private void Awake()
	{
		myAudio = base.audio;
		myTransform = base.transform;
		myCollider = base.collider;
		bombAnimator = myTransform.Find("bomb_anim").animation;
		timerMount = myTransform.Find("timerMount");
		wickParticles.SetActive(false);
	}

	private void Start()
	{
		OnAddTimer();
		StartCoroutine(delayedAddInitialPulse());
	}

	private IEnumerator delayedAddInitialPulse()
	{
		while (HUD.Instance == null || HUD.Instance.PlayerController == null)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (team == HUD.Instance.PlayerController.Team)
		{
			currentTimerSystem.SendMessage("OnStartPulse");
		}
	}

	public void OnAddTimer()
	{
		currentTimerSystem = Object.Instantiate(timerDisplay) as GameObject;
		currentTimerSystem.transform.parent = timerMount;
		currentTimerSystem.transform.localPosition = Vector3.zero;
		currentTimerSystem.transform.localEulerAngles = Vector3.zero;
		currentTimerSystem.transform.localScale = Vector3.one;
		bombAnimator["bomb_spin"].wrapMode = WrapMode.Loop;
		bombAnimator.Play("bomb_spin");
	}

	public void OnDropped()
	{
		myCollider.enabled = true;
		isHeld = false;
		holdingPlayer = null;
		OnAddTimer();
		currentTimerSystem.SendMessage("OnStartPulse");
	}

	protected void OnTriggerEnter(Collider c)
	{
		PlayerController componentInChildren = c.transform.root.GetComponentInChildren<PlayerController>();
		if (componentInChildren != null && !componentInChildren.isRemote && componentInChildren.Team == team && !componentInChildren.IsDead && !isHeld && componentInChildren.IsBombPickupAllowed)
		{
			float num = 0f;
			if (componentInChildren.Team == Team.RED)
			{
				num = (float)CTFManager.Instance.RedTimeLeft / 1000f;
			}
			else if (componentInChildren.Team == Team.BLUE)
			{
				num = (float)CTFManager.Instance.BlueTimeLeft / 1000f;
			}
			if (num > 5f || num <= 0f)
			{
				CTFManager.Instance.OnRequestBombPickup(componentInChildren);
			}
		}
	}

	public void OnGrabbedByPlayer(PlayerController p)
	{
		if (!hasTimerStarted)
		{
			hasTimerStarted = true;
			wickParticles.SetActive(true);
			bombAnimator["wick"].layer = 1;
			bombAnimator["wick"].speed = bombAnimator["wick"].length / ((float)CTFManager.Instance.EXPLODE_TIME / 1000f);
			bombAnimator.Play("wick");
			if (myAudio != null)
			{
				myAudio.Play();
			}
			myCollider.enabled = false;
			StartCoroutine(periodicBombPulse());
			StartCoroutine(periodicBombBeep());
		}
		isHeld = true;
		myTransform.parent = p.WeaponManager.rightWeaponMountpoint;
		myTransform.localEulerAngles = Vector3.zero;
		myTransform.localPosition = Vector3.zero;
		myTransform.localScale = Vector3.one;
		bombAnimator.Stop("bomb_spin");
		holdingPlayer = p;
		Object.Destroy(currentTimerSystem);
	}

	private IEnumerator periodicBombBeep()
	{
		double delay = 30.0;
		while (delay > 0.0)
		{
			delay = ((team != 0) ? ((double)(CTFManager.Instance.BlueTimeLeft / 1000) / 20.0) : ((double)(CTFManager.Instance.RedTimeLeft / 1000) / 20.0));
			yield return new WaitForSeconds((float)delay);
			if (delay * 20.0 < 30.0 && myAudio != null && beep != null && base.gameObject.activeInHierarchy && myAudio.enabled)
			{
				myAudio.PlayOneShot(beep);
			}
		}
	}

	private IEnumerator periodicBombPulse()
	{
		bombAnimator["pulse"].layer = 0;
		float maxAnimationSpeed = 3f;
		while (true)
		{
			float delay2 = 0f;
			delay2 = ((team != 0) ? ((float)CTFManager.Instance.BlueTimeLeft / 1000f / 10f) : ((float)CTFManager.Instance.RedTimeLeft / 1000f / 10f));
			float animationSpeed = 1f;
			if (delay2 < 2f)
			{
				animationSpeed = 1f + (maxAnimationSpeed - 1f) * (1f - delay2 / 2f);
			}
			bombAnimator["pulse"].speed = animationSpeed;
			yield return new WaitForSeconds(delay2 / animationSpeed);
			bombAnimator.Play("pulse");
		}
	}

	private void OnDropFromPlayer()
	{
		if (holdingPlayer != null && !holdingPlayer.isRemote)
		{
			holdingPlayer.OnLocalReleaseBomb();
			holdingPlayer = null;
			myCollider.enabled = true;
		}
	}
}
