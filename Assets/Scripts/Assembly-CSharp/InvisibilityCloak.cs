using System.Collections;
using UnityEngine;

public class InvisibilityCloak : SpecialAbility, ICloak
{
	private PlayerDamageReceiver playerDamageReceiver;

	public float minAlpha = 0.25f;

	public float maxAlpha = 1f;

	public float fadeSpeed = 0.75f;

	public float unfadeSpeed = 0.15f;

	public float smoothSpeed = 5f;

	private Vector3 oldPos;

	private Transform myTransform;

	private float targetVal = 1f;

	private Transform statusMount;

	private Transform playerHighlight;

	private Transform nameMount;

	private bool isCloaking;

	private bool wasDestroyed;

	public void StartCloak()
	{
		if (!isCloaking)
		{
			isCloaking = true;
			oldPos = myTransform.position;
			playerHighlight = myTransform.root.Find("playerHighlight");
			if (playerHighlight != null)
			{
				playerHighlight.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
			}
			PlayerController component = myTransform.root.GetComponent<PlayerController>();
			if (component != null)
			{
				statusMount = component.statusEffectMount;
			}
			nameMount = myTransform.root.Find("playerName/name");
			if (nameMount != null)
			{
				nameMount.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
			}
			statusMount.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
		}
	}

	public void EndCloak()
	{
		if (isCloaking)
		{
			isCloaking = false;
			StopAllCoroutines();
			if (playerHighlight != null)
			{
				playerHighlight.gameObject.layer = LayerMask.NameToLayer("Player");
			}
			if (statusMount != null)
			{
				statusMount.gameObject.layer = LayerMask.NameToLayer("Player");
			}
			if (nameMount != null)
			{
				nameMount.gameObject.layer = LayerMask.NameToLayer("Player");
			}
			if (!wasDestroyed)
			{
				Object.Destroy(this);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		myTransform = base.transform;
		playerDamageReceiver = GetComponentInChildren(typeof(PlayerDamageReceiver)) as PlayerDamageReceiver;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(delayedStart());
	}

	private void OnDisable()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(fadeIn());
		}
	}

	private void OnDestroy()
	{
		wasDestroyed = true;
		if (playerDamageReceiver.Cloak == this)
		{
			playerDamageReceiver.Cloak = null;
		}
	}

	private IEnumerator delayedStart()
	{
		yield return new WaitForSeconds(0.1f);
		playerDamageReceiver.Cloak = this;
	}

	private IEnumerator fadeIn()
	{
		Color c = playerDamageReceiver.cloakMaterial.color;
		while (targetVal < 1f)
		{
			targetVal += unfadeSpeed;
			c.a = targetVal;
			playerDamageReceiver.cloakMaterial.color = c;
			yield return new WaitForSeconds(0.05f);
		}
		Object.Destroy(this);
	}

	private void Update()
	{
		if (isCloaking)
		{
			Material cloakMaterial = playerDamageReceiver.cloakMaterial;
			Color color = cloakMaterial.color;
			float num = Vector3.Distance(oldPos, myTransform.position);
			oldPos = myTransform.position;
			if (num < 0.1f)
			{
				targetVal -= fadeSpeed * Time.deltaTime;
			}
			else
			{
				targetVal += unfadeSpeed * Time.deltaTime;
			}
			if (targetVal > maxAlpha)
			{
				targetVal = maxAlpha;
			}
			if (targetVal < minAlpha)
			{
				targetVal = minAlpha;
			}
			color.a = Mathf.Lerp(color.a, targetVal, Time.deltaTime * smoothSpeed);
			cloakMaterial.color = color;
		}
	}
}
