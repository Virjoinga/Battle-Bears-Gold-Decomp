using System.Collections;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
	public bool isInvincible;

	[SerializeField]
	private int ownerID;

	public GameObject shieldPrefab;

	protected GameObject currentShield;

	public Component[] renderers;

	public Material normalMaterial;

	public Material cloakMaterial;

	public Material hitMaterial;

	private Material _activeMaterial;

	private Material _shrinkMaterial;

	private ICloak _cloak;

	private bool _isShrunk;

	public AudioClip[] hitSounds;

	public AudioClip[] hitSayings;

	protected AudioSource myAudio;

	protected bool isShowingHit;

	private float nextHitSoundAllowedTime;

	private float nextHitTalkAllowedTime;

	private float hitTalkDelay = 5f;

	public DamageReceiverProxy linkedProxy;

	public Transform headSpot;

	public AudioClip headshotSound;

	protected bool deathWasFromDeathArea;

	protected float _deathAreaDamageMult = 9001f;

	private float cumulativeDmg;

	protected float _radiationDmg;

	public Material ActiveMaterial
	{
		get
		{
			if (_activeMaterial == null)
			{
				_activeMaterial = ((_cloak == null) ? normalMaterial : cloakMaterial);
			}
			return _activeMaterial;
		}
		set
		{
			_activeMaterial = value;
		}
	}

	public Material ShrinkMaterial
	{
		get
		{
			if (_shrinkMaterial == null)
			{
				_shrinkMaterial = (Material)Resources.Load("Materials/Characters/Shrink/shrink_" + BBRQuality.SkinQuality);
				_shrinkMaterial.mainTexture = normalMaterial.mainTexture;
			}
			return _shrinkMaterial;
		}
	}

	public ICloak Cloak
	{
		get
		{
			return _cloak;
		}
		set
		{
			if (_cloak == value)
			{
				return;
			}
			if (_cloak != null)
			{
				_cloak.EndCloak();
			}
			_cloak = value;
			if (_cloak != null)
			{
				_cloak.StartCloak();
				ActiveMaterial = cloakMaterial;
			}
			else
			{
				ActiveMaterial = ((!_isShrunk) ? normalMaterial : ShrinkMaterial);
			}
			if (!isShowingHit)
			{
				Component[] array = renderers;
				for (int i = 0; i < array.Length; i++)
				{
					Renderer renderer = (Renderer)array[i];
					renderer.material = ActiveMaterial;
				}
			}
		}
	}

	public bool IsCloaking
	{
		get
		{
			return _cloak != null;
		}
	}

	public bool isShrunk
	{
		get
		{
			return _isShrunk;
		}
		set
		{
			if (_isShrunk == value)
			{
				return;
			}
			Debug.Log("SETTING SHRUNK: " + value);
			_isShrunk = value;
			if (_cloak != null)
			{
				return;
			}
			ActiveMaterial = ((!_isShrunk) ? normalMaterial : ShrinkMaterial);
			if (!isShowingHit)
			{
				Component[] array = renderers;
				for (int i = 0; i < array.Length; i++)
				{
					Renderer renderer = (Renderer)array[i];
					renderer.material = ActiveMaterial;
				}
			}
		}
	}

	public int OwnerID
	{
		get
		{
			return ownerID;
		}
		set
		{
			ownerID = value;
		}
	}

	public bool WasDeathFromDeathArea
	{
		get
		{
			return deathWasFromDeathArea;
		}
	}

	public void OnDisableShield()
	{
		if (currentShield != null)
		{
			Object.Destroy(currentShield);
		}
		isInvincible = false;
	}

	public virtual void OnKilledByDeathArea()
	{
		OnDisableShield();
		deathWasFromDeathArea = true;
		OnTakeDamage(HUD.Instance.PlayerController.DamageReceiver.CurrentHP * _deathAreaDamageMult, HUD.Instance.PlayerController.OwnerID, true, false, false, true, false, 0f, string.Empty);
		deathWasFromDeathArea = false;
	}

	public virtual void OnTakeDamage(float dmg, int shooterID, bool isExplosion, bool isMelee, bool isHeadshot, bool sendNotification, bool endOfGameOverride = false, float radiationDmg = 0f, string customDeathSfx = "")
	{
		if (isHeadshot && headshotSound != null && myAudio != null)
		{
			myAudio.PlayOneShot(headshotSound, SoundManager.Instance.getEffectsVolume());
		}
	}

	public void OnUpdateProxy()
	{
		if (linkedProxy != null)
		{
			linkedProxy.OnProxyHit();
		}
	}

	protected IEnumerator displayHit(float dmg)
	{
		isShowingHit = true;
		Component[] array = renderers;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer r = (Renderer)array[i];
			r.material = hitMaterial;
		}
		cumulativeDmg += dmg;
		if (cumulativeDmg > 30f)
		{
			cumulativeDmg = 0f;
			if (Time.time > nextHitSoundAllowedTime)
			{
				if (Random.value < 0.25f && hitSayings.Length > 0 && Time.time > nextHitTalkAllowedTime)
				{
					AudioClip clipToPlay2 = hitSayings[Random.Range(0, hitSayings.Length)];
					myAudio.PlayOneShot(clipToPlay2, SoundManager.Instance.getEffectsVolume());
					nextHitTalkAllowedTime = Time.time + clipToPlay2.length + hitTalkDelay;
				}
				else if (hitSounds.Length > 0)
				{
					AudioClip clipToPlay = hitSounds[Random.Range(0, hitSounds.Length)];
					myAudio.PlayOneShot(clipToPlay, SoundManager.Instance.getEffectsVolume());
					nextHitSoundAllowedTime = Time.time + clipToPlay.length;
				}
			}
		}
		yield return new WaitForSeconds(0.3f);
		Component[] array2 = renderers;
		for (int j = 0; j < array2.Length; j++)
		{
			Renderer r2 = (Renderer)array2[j];
			r2.material = ActiveMaterial;
		}
		yield return new WaitForSeconds(0.1f);
		isShowingHit = false;
	}
}
