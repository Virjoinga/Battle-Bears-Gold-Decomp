using System.Collections;
using UnityEngine;

public class RainbowShot : MeleeAttack
{
	private Transform spawnPoint;

	public float raycastOffset = 40f;

	public LayerMask layersToHit;

	public float streamRange = 1000f;

	public GameObject streamPrefab;

	private GameObject stream;

	private Transform streamStart;

	private Transform streamEnd;

	private ParticleEmitter hitEffect;

	private Transform bodyRotator;

	public GameObject headPrefab;

	private GameObject head;

	private Transform streamTransform;

	public AudioClip rainbowStart;

	public AudioClip rainbowBeam;

	private bool beamActive;

	private bool beamOn;

	public bool CancelBeam { get; set; }

	protected override void Awake()
	{
		base.Awake();
		spawnPoint = myTransform.Find("spawn");
	}

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("range", ref streamRange, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Start()
	{
		base.Start();
		CancelBeam = false;
		if (base.playerController != null)
		{
			bodyRotator = base.playerController.bodyRotator;
		}
		if (streamPrefab != null)
		{
			stream = Object.Instantiate(streamPrefab) as GameObject;
			streamStart = stream.transform.Find("start");
			streamEnd = stream.transform.Find("end");
			hitEffect = streamEnd.Find("Particle System").GetComponent(typeof(ParticleEmitter)) as ParticleEmitter;
			hitEffect.emit = false;
			streamTransform = stream.transform;
			streamTransform.parent = spawnPoint;
			streamTransform.localPosition = Vector3.zero;
			stream.SetActive(false);
		}
		OnFindAimer();
	}

	private void Update()
	{
		if (CancelBeam)
		{
			CancelBeamImmediately();
			StopAllCoroutines();
		}
		if (!beamOn)
		{
			return;
		}
		streamTransform.localScale = new Vector3(1f, 1f, 1f);
		if (base.playerController == null || bodyRotator == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
			bodyRotator = base.playerController.bodyRotator;
			OnFindAimer();
		}
		if (!stream.activeInHierarchy)
		{
			stream.SetActive(true);
		}
		Vector3 vector = (isRemote ? bodyRotator.forward : aimer.forward);
		bool flag = false;
		Ray ray = new Ray(spawnPoint.position - vector * raycastOffset, vector);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, streamRange, layersToHit))
		{
			if (HitMyself(hitInfo))
			{
				ray.origin = hitInfo.point + Vector3.Scale(vector, hitInfo.collider.bounds.extents);
				flag = Physics.Raycast(ray, out hitInfo, streamRange, layersToHit);
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			Transform transform = hitInfo.transform;
			DamageReceiver damageReceiver = transform.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			if (streamEnd != null && (damageReceiver == null || damageReceiver.OwnerID != base.OwnerID))
			{
				streamEnd.position = hitInfo.point;
				hitEffect.emit = true;
			}
			if (!isRemote && damageReceiver != null && damageReceiver.OwnerID != base.OwnerID && !damageReceiver.isInvincible)
			{
				OnDealDirectDamage(damageReceiver, damage * Time.deltaTime * base.playerController.DamageMultiplier);
			}
		}
		else if (streamEnd != null)
		{
			streamEnd.position = spawnPoint.position + vector * streamRange;
			hitEffect.emit = false;
		}
		streamStart.LookAt(streamEnd);
		streamEnd.eulerAngles = streamStart.eulerAngles;
	}

	private bool HitMyself(RaycastHit hit)
	{
		DamageReceiver component = hit.transform.GetComponent<DamageReceiver>();
		return component != null && component.OwnerID == base.OwnerID;
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (!beamActive)
		{
			beamActive = true;
			StartCoroutine(beamStop());
		}
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		if (!beamActive)
		{
			beamActive = true;
			StartCoroutine(beamStop());
		}
	}

	private IEnumerator beamStop()
	{
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		yield return new WaitForSeconds(0.2f);
		head = Object.Instantiate(headPrefab) as GameObject;
		head.transform.parent = base.playerController.WeaponManager.leftWeaponMountpoint;
		head.transform.localPosition = Vector3.zero;
		head.transform.localEulerAngles = Vector3.zero;
		head.transform.localScale = new Vector3(1f, 1f, 1f);
		head.GetComponent<Renderer>().material = (base.playerController.transform.Find("playerModel").GetComponentInChildren(typeof(Renderer)) as Renderer).material;
		if (rainbowStart != null)
		{
			myAudio.PlayOneShot(rainbowStart);
		}
		yield return new WaitForSeconds(0.8f);
		beamOn = true;
		if (rainbowBeam != null)
		{
			myAudio.clip = rainbowBeam;
			myAudio.loop = true;
			myAudio.Play();
		}
		yield return new WaitForSeconds(duration - 1.5f);
		StartCoroutine(endBeam());
	}

	private IEnumerator endBeam()
	{
		myAudio.Stop();
		beamActive = false;
		beamOn = false;
		stream.SetActive(false);
		hitEffect.emit = false;
		yield return new WaitForSeconds(0.25f);
		Object.Destroy(head);
	}

	private void CancelBeamImmediately()
	{
		myAudio.Stop();
		beamActive = false;
		beamOn = false;
		CancelBeam = false;
		stream.SetActive(false);
		hitEffect.emit = false;
		Object.Destroy(head);
	}

	private void OnDestroy()
	{
		if (head != null)
		{
			Object.Destroy(head);
		}
	}
}
