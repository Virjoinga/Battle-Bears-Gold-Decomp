using System.Collections;
using UnityEngine;

public class SixthSenseGoggles : SpecialItem
{
	public float duration = 30f;

	private Camera highlightCamera;

	public AudioClip startSound;

	public AudioClip endSound;

	private AudioSource myAudio;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/goggles";
		}
	}

	protected override void Awake()
	{
		base.Awake();
		myAudio = base.audio;
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		base.Configure(item);
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		myTransform.parent = playerController.transform;
		if (!isRemote)
		{
			highlightCamera = p.PlayerCam.transform.Find("highlightCamera").camera;
			highlightCamera.enabled = true;
		}
		if (startSound != null)
		{
			myAudio.PlayOneShot(startSound);
		}
		StartCoroutine(delayedEnd(delay));
	}

	private IEnumerator delayedEnd(float delay)
	{
		float soundEndLength = 0f;
		if (endSound != null)
		{
			soundEndLength = endSound.length;
		}
		yield return new WaitForSeconds(duration - delay - soundEndLength);
		StartCoroutine(delayedDestroy());
	}

	private IEnumerator delayedDestroy()
	{
		float soundEndLength = 0f;
		if (endSound != null)
		{
			soundEndLength = endSound.length;
		}
		if (endSound != null)
		{
			myAudio.PlayOneShot(endSound);
			yield return new WaitForSeconds(soundEndLength);
		}
		Object.Destroy(base.gameObject);
	}

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		StopAllCoroutines();
		StartCoroutine(delayedDestroy());
	}

	private void OnDestroy()
	{
		if (!isRemote && playerController != null && highlightCamera != null)
		{
			highlightCamera.enabled = false;
		}
	}
}
