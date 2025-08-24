using System.Collections;
using UnityEngine;

public class ConstantStreamWeapon : WeaponBase
{
	private Transform spawnPoint;

	public float raycastOffset = 50f;

	public LayerMask layersToHit;

	public float damage = 1f;

	public float radiationDamage;

	public float streamRange = 1000f;

	public GameObject streamPrefab;

	private GameObject stream;

	private Transform streamStart;

	private Transform streamEnd;

	private Renderer hitEffect1;

	private Renderer hitEffect2;

	private ParticleEmitter missEffect;

	private Transform bodyRotator;

	private float streamTimeLeft;

	public AudioClip startSound;

	public AudioClip endSound;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("damage", ref damage, base.EquipmentNames);
		item.UpdateProperty("range", ref streamRange, base.EquipmentNames);
		item.UpdateProperty("radiation", ref radiationDamage, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Awake()
	{
		base.Awake();
		spawnPoint = myTransform.Find("spawn");
		if (!(streamPrefab != null))
		{
			return;
		}
		stream = Object.Instantiate(streamPrefab) as GameObject;
		if (stream != null)
		{
			streamStart = stream.transform.Find("soaker/start");
			streamEnd = stream.transform.Find("soaker/end");
		}
		if (streamEnd != null)
		{
			Transform transform = streamEnd.Find("Particle System");
			if (transform != null)
			{
				missEffect = transform.GetComponent(typeof(ParticleEmitter)) as ParticleEmitter;
			}
			hitEffect1 = streamEnd.Find("Contact1").renderer;
			hitEffect2 = streamEnd.Find("Contact2").renderer;
		}
		if (missEffect != null)
		{
			missEffect.emit = false;
		}
		if (stream != null)
		{
			stream.transform.parent = spawnPoint;
			stream.transform.localPosition = Vector3.zero;
			stream.SetActive(false);
		}
	}

	protected override void Start()
	{
		base.Start();
		if (base.playerController != null)
		{
			bodyRotator = base.playerController.bodyRotator;
		}
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (base.playerController == null || bodyRotator == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren<PlayerController>();
			if (base.playerController != null)
			{
				bodyRotator = base.playerController.bodyRotator;
			}
		}
		if (!stream.activeInHierarchy)
		{
			if (startSound != null)
			{
				myAudio.PlayOneShot(startSound);
			}
			myAudio.Play();
			stream.SetActive(true);
		}
		RaycastHit hitInfo;
		if (Physics.Raycast(spawnPoint.position - aimer.forward * raycastOffset, aimer.forward, out hitInfo, streamRange, layersToHit))
		{
			if (streamEnd != null)
			{
				streamEnd.position = hitInfo.point;
				if (missEffect != null)
				{
					missEffect.emit = false;
				}
				hitEffect1.enabled = true;
				hitEffect2.enabled = true;
			}
			Transform transform = hitInfo.transform;
			DamageReceiver d = transform.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			dealDamage(d);
		}
		else if (streamEnd != null)
		{
			streamEnd.position = spawnPoint.position + aimer.forward * streamRange;
			if (missEffect != null)
			{
				missEffect.emit = true;
			}
			hitEffect1.enabled = false;
			hitEffect2.enabled = false;
		}
		streamStart.LookAt(streamEnd);
		streamEnd.eulerAngles = streamStart.eulerAngles;
		if (streamTimeLeft <= 0f)
		{
			streamTimeLeft = firingTime + 0.11f;
			StartCoroutine(stopStream());
		}
		else
		{
			streamTimeLeft = firingTime + 0.11f;
		}
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		stream.SetActive(true);
		if (base.playerController == null || bodyRotator == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren<PlayerController>();
			if (base.playerController != null)
			{
				bodyRotator = base.playerController.bodyRotator;
			}
		}
		RaycastHit hitInfo;
		if (Physics.Raycast(spawnPoint.position - aimer.forward * raycastOffset, bodyRotator.forward, out hitInfo, streamRange, layersToHit))
		{
			if (streamEnd != null)
			{
				streamEnd.position = hitInfo.point;
				if (missEffect != null)
				{
					missEffect.emit = false;
				}
				hitEffect1.enabled = true;
				hitEffect2.enabled = true;
			}
		}
		else if (streamEnd != null)
		{
			streamEnd.position = spawnPoint.position + bodyRotator.forward * streamRange;
			if (missEffect != null)
			{
				missEffect.emit = true;
			}
			hitEffect1.enabled = false;
			hitEffect2.enabled = false;
		}
		streamStart.LookAt(streamEnd);
		streamEnd.eulerAngles = streamStart.eulerAngles;
		if (streamTimeLeft <= 0f)
		{
			streamTimeLeft = firingTime + 0.11f;
			StartCoroutine(stopStream());
		}
		else
		{
			streamTimeLeft = firingTime + 0.11f;
		}
	}

	private IEnumerator stopStream()
	{
		while (streamTimeLeft > 0f)
		{
			streamTimeLeft -= 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
		if (endSound != null)
		{
			myAudio.PlayOneShot(endSound);
		}
		myAudio.Stop();
		stream.SetActive(false);
		if (missEffect != null)
		{
			missEffect.gameObject.SetActive(true);
			missEffect.emit = false;
		}
	}

	protected virtual void dealDamage(DamageReceiver d)
	{
		if (d != null && !d.isInvincible)
		{
			OnDealDirectDamage(d, damage * base.playerController.DamageMultiplier, radiationDamage);
		}
	}
}
