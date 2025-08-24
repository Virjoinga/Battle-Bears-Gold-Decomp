using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamouflageCloak : SpecialItem, ICloak
{
	private PlayerDamageReceiver playerDamageReceiver;

	public float minAlpha = 0.25f;

	public float maxAlpha = 1f;

	public float fadeSpeed = 0.75f;

	public float unfadeSpeed = 0.15f;

	public float smoothSpeed = 5f;

	private Vector3 oldPos;

	private float targetVal = 1f;

	public float duration = 30f;

	private List<Component> weaponRenderers = new List<Component>();

	private Dictionary<Component, Material> weaponMaterials = new Dictionary<Component, Material>();

	public AudioClip hideSound;

	private Transform statusMount;

	private Transform playerHighlight;

	private Transform nameMount;

	private bool isCloaking;

	private bool wasDestroyed;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/cloaking";
		}
	}

	public void StartCloak()
	{
		if (!isCloaking)
		{
			isCloaking = true;
			weaponRenderers.Clear();
			Material cloakMaterial = playerDamageReceiver.cloakMaterial;
			Color color = ((playerController.Team != Team.BLUE) ? Color.red : new Color(0f, 0.3f, 1f));
			cloakMaterial.color = color;
			PlayerController component = myTransform.root.GetComponent<PlayerController>();
			if (component != null)
			{
				statusMount = component.statusEffectMount;
				statusMount.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
			}
			playerHighlight = myTransform.root.Find("playerHighlight");
			if (playerHighlight != null)
			{
				playerHighlight.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
			}
			nameMount = myTransform.root.Find("playerName/name");
			if (nameMount != null)
			{
				nameMount.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
			}
			updateWeapon();
		}
	}

	public void EndCloak()
	{
		if (!isCloaking)
		{
			return;
		}
		isCloaking = false;
		StopAllCoroutines();
		if (weaponRenderers != null)
		{
			foreach (Component weaponRenderer in weaponRenderers)
			{
				if (weaponRenderer != null)
				{
					if (weaponRenderer is ParticleSystemRenderer || weaponRenderer is ParticleRenderer)
					{
						weaponRenderer.gameObject.SetActive(true);
					}
					(weaponRenderer as Renderer).material = weaponMaterials[weaponRenderer];
				}
			}
		}
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
			Object.Destroy(base.gameObject);
		}
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("minVisibility", ref minAlpha, base.EquipmentNames);
		item.UpdateProperty("maxVisibility", ref maxAlpha, base.EquipmentNames);
		item.UpdateProperty("fadeSpeed", ref fadeSpeed, base.EquipmentNames);
		item.UpdateProperty("unfadeSpeed", ref unfadeSpeed, base.EquipmentNames);
		item.UpdateProperty("smoothSpeed", ref smoothSpeed, base.EquipmentNames);
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		base.Configure(item);
	}

	protected override void Start()
	{
		base.Start();
		oldPos = myTransform.position;
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		p.CamoCloak = this;
		myTransform.parent = p.transform;
		playerDamageReceiver = p.GetComponentInChildren(typeof(PlayerDamageReceiver)) as PlayerDamageReceiver;
		StartCoroutine(delayedStart());
		StartCoroutine(delayedDisable(delay));
		p.OnPlayGenericSound(hideSound);
	}

	private IEnumerator delayedStart()
	{
		yield return new WaitForSeconds(0.1f);
		playerDamageReceiver.Cloak = this;
	}

	public void updateWeapon()
	{
		if (playerController.WeaponManager is WeaponManagerB100)
		{
			WeaponManagerB100 weaponManagerB = (WeaponManagerB100)playerController.WeaponManager;
			SetWeaponInvisible(weaponManagerB.MeleeWeapon);
			SetWeaponInvisible(weaponManagerB.PrimaryWeapon);
			SetWeaponInvisible(weaponManagerB.SecondaryWeapon);
		}
		else if (playerController.WeaponManager is SatelliteSecondaryWeaponManager)
		{
			SatelliteSecondaryWeaponManager satelliteSecondaryWeaponManager = (SatelliteSecondaryWeaponManager)playerController.WeaponManager;
			SetWeaponInvisible(satelliteSecondaryWeaponManager.CurrentWeapon);
			SetWeaponInvisible(satelliteSecondaryWeaponManager.SecondaryWeapon);
		}
		else
		{
			SetWeaponInvisible(playerController.WeaponManager.CurrentWeapon);
		}
	}

	private void SetWeaponInvisible(WeaponBase weap)
	{
		if (weap != null)
		{
			Component[] componentsInChildren = weap.GetComponentsInChildren(typeof(Renderer));
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!weaponRenderers.Contains(componentsInChildren[i]))
				{
					weaponRenderers.Add(componentsInChildren[i]);
				}
			}
		}
		foreach (Component weaponRenderer in weaponRenderers)
		{
			if (!weaponMaterials.ContainsKey(weaponRenderer))
			{
				Material material = new Material((weaponRenderer as Renderer).material);
				weaponMaterials.Add(weaponRenderer, (weaponRenderer as Renderer).material);
				if (weaponRenderer is ParticleSystemRenderer || weaponRenderer is ParticleRenderer)
				{
					weaponRenderer.gameObject.SetActive(false);
				}
				else
				{
					(weaponRenderer as Renderer).material = playerDamageReceiver.cloakMaterial;
				}
			}
		}
	}

	private IEnumerator delayedDisable(float delay)
	{
		yield return new WaitForSeconds(duration - delay);
		base.enabled = false;
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

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		base.enabled = false;
	}

	private IEnumerator fadeIn()
	{
		Color c = playerDamageReceiver.cloakMaterial.color;
		while (targetVal < 1f)
		{
			targetVal += unfadeSpeed;
			c.a = targetVal;
			playerDamageReceiver.cloakMaterial.color = c;
			if (weaponRenderers != null)
			{
				foreach (Component w in weaponRenderers)
				{
					if (w != null)
					{
						if (w is ParticleSystemRenderer || w is ParticleRenderer)
						{
							w.gameObject.SetActive(true);
						}
						(w as Renderer).material.color = c;
					}
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
		Object.Destroy(base.gameObject);
	}

	private void Update()
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
		if (weaponRenderers == null)
		{
			return;
		}
		foreach (Component weaponRenderer in weaponRenderers)
		{
			if (weaponRenderer != null && !(weaponRenderer is ParticleSystemRenderer) && !(weaponRenderer is ParticleRenderer))
			{
				(weaponRenderer as Renderer).material = cloakMaterial;
			}
		}
	}
}
