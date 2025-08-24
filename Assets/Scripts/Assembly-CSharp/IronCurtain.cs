using System.Collections;
using UnityEngine;

public class IronCurtain : SpecialItem
{
	public float duration = 30f;

	public Vector3 localOffset;

	public float scaleModifier;

	private Animation myAnimation;

	private Collider myCollider;

	public AudioClip activateSound;

	public AudioClip deactivateSound;

	private AudioSource myAudio;

	private GameObject _target;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/ironcurtain";
		}
	}

	protected override void Awake()
	{
		base.Awake();
		myAnimation = base.animation;
		myCollider = base.collider;
		myAudio = base.audio;
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		myTransform.parent = playerController.transform;
		myTransform.localPosition = localOffset;
		myTransform.localEulerAngles = Vector3.zero;
		myTransform.localScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
		StartCoroutine(delayedEnd(delay));
		_target = base.transform.parent.gameObject;
		base.transform.parent = null;
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		item.UpdateProperty("diameter", ref scaleModifier, base.EquipmentNames);
		base.Configure(item);
	}

	private IEnumerator delayedEnd(float delay)
	{
		if (activateSound != null)
		{
			myAudio.PlayOneShot(activateSound);
		}
		myAnimation.Play("Enter");
		yield return new WaitForSeconds(myAnimation["Enter"].length);
		myAnimation.Play("idle");
		float exitLength = myAnimation["Exit"].length;
		yield return new WaitForSeconds(duration - delay - exitLength);
		StartCoroutine(destroyCurtain(delay));
	}

	private IEnumerator destroyCurtain(float delay)
	{
		float exitLength = myAnimation["Exit"].length;
		Object.Destroy(myCollider);
		myAnimation.Play("Exit");
		if (deactivateSound != null)
		{
			myAudio.PlayOneShot(deactivateSound);
		}
		yield return new WaitForSeconds(exitLength - delay);
		Object.Destroy(base.gameObject);
	}

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		StopAllCoroutines();
		StartCoroutine(destroyCurtain(delay));
	}

	private void OnTriggerEnter(Collider c)
	{
		if (!c.CompareTag("Player"))
		{
			myAnimation.Play("hit");
			myAnimation.PlayQueued("idle");
			AcmeRocket acmeRocket = c.gameObject.GetComponent(typeof(AcmeRocket)) as AcmeRocket;
			if (acmeRocket != null)
			{
				acmeRocket.OnHitShield();
			}
			BuzzkillBee buzzkillBee = c.gameObject.GetComponent(typeof(BuzzkillBee)) as BuzzkillBee;
			if (buzzkillBee != null)
			{
				buzzkillBee.OnDestroyOnImpact();
			}
		}
	}

	private void Update()
	{
		if (_target != null)
		{
			base.transform.localEulerAngles = _target.transform.localEulerAngles;
			base.transform.position = _target.transform.position + base.transform.forward.normalized * localOffset.z * 0.8f;
		}
	}
}
