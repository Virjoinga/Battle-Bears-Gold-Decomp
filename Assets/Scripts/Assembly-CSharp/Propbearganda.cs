using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propbearganda : SpecialItem
{
	private Dictionary<string, PlayerController> playersAffected = new Dictionary<string, PlayerController>();

	public float bonus = 0.2f;

	public float duration = 30f;

	public Vector3 localOffset;

	public float diameter = 1f;

	private Animation myAnimation;

	private Rigidbody myRigidbody;

	public ParticleEmitter noteEmitter;

	private AudioSource myAudio;

	public AudioClip propbeargandaStartSound;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/propbearganda";
		}
	}

	protected override void Awake()
	{
		base.Awake();
		myAnimation = base.animation;
		myRigidbody = base.rigidbody;
		myAudio = base.audio;
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("bonus", ref bonus, base.EquipmentNames);
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		item.UpdateProperty("diameter", ref diameter, base.EquipmentNames);
		base.Configure(item);
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		myTransform.parent = playerController.transform;
		myTransform.localPosition = localOffset;
		myTransform.localEulerAngles = Vector3.zero;
		SphereCollider component = GetComponent<SphereCollider>();
		if (component != null)
		{
			component.radius = diameter;
		}
		myTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		StartCoroutine(delayedEnd(delay));
	}

	private IEnumerator delayedEnd(float delay)
	{
		if (propbeargandaStartSound != null)
		{
			myAudio.PlayOneShot(propbeargandaStartSound);
		}
		myAnimation.Play("Enter");
		yield return new WaitForSeconds(myAnimation["Enter"].length);
		myAudio.Play();
		myAnimation.Play("idle");
		float exitLength = myAnimation["Exit"].length;
		yield return new WaitForSeconds(duration - delay - exitLength);
		StartCoroutine(delayedDestroy(delay));
	}

	private IEnumerator delayedDestroy(float delay)
	{
		float exitLength = myAnimation["Exit"].length;
		myAudio.Stop();
		noteEmitter.emit = false;
		noteEmitter.transform.parent = null;
		myAnimation.Play("Exit");
		yield return new WaitForSeconds(exitLength - delay);
		myRigidbody.WakeUp();
		myTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		myTransform.position = new Vector3(100000f, -1000000f, 0f);
		yield return new WaitForSeconds(0.2f);
		myRigidbody.WakeUp();
		Object.Destroy(base.gameObject);
	}

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		StopAllCoroutines();
		StartCoroutine(delayedDestroy(delay));
	}

	private void OnTriggerEnter(Collider c)
	{
		PlayerController playerController = c.gameObject.GetComponent(typeof(PlayerController)) as PlayerController;
		if (playerController != null && playerController.Team == base.playerController.Team && !playersAffected.ContainsKey(playerController.name))
		{
			playerController.OnGetPropbearganda(bonus);
			playersAffected.Add(playerController.name, playerController);
		}
	}

	private void OnTriggerExit(Collider c)
	{
		PlayerController playerController = c.gameObject.GetComponent(typeof(PlayerController)) as PlayerController;
		if (playerController != null && playerController.Team == base.playerController.Team && playersAffected.ContainsKey(playerController.name))
		{
			playerController.OnLosePropbearganda(bonus);
			playersAffected.Remove(playerController.name);
		}
	}

	private void OnDestroy()
	{
		foreach (KeyValuePair<string, PlayerController> item in playersAffected)
		{
			item.Value.OnLosePropbearganda(bonus);
		}
		playersAffected.Clear();
	}
}
