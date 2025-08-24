using System.Collections;
using UnityEngine;

public class OcoProjectile : Projectile
{
	private Animation myAnimation;

	private Rigidbody myRigidbody;

	private Vector3 originalVelocity;

	private SimpleConstantDamageSource coreDamageSource;

	private float duration = 10f;

	public AudioClip flyingLoopSound;

	public AudioClip startFlyingSound;

	private void Awake()
	{
		myAnimation = base.animation;
		myRigidbody = base.rigidbody;
		coreDamageSource = GetComponentInChildren<SimpleConstantDamageSource>();
		if (coreDamageSource != null)
		{
			coreDamageSource.SetItemOverride(spawnItemOverride);
		}
	}

	private new void Start()
	{
		if (spawnItemOverride != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(spawnItemOverride);
			itemByName.UpdateProperty("duration", ref duration, equipmentNames);
		}
		if (coreDamageSource != null)
		{
			coreDamageSource.OwnerID = base.OwnerID;
			coreDamageSource.DamageMultiplier = base.DamageMultiplier;
		}
	}

	private void OnNetworkDelay(float timeAlreadyElapsed)
	{
		StartCoroutine(startProjectileBehaviour(timeAlreadyElapsed));
	}

	private IEnumerator startProjectileBehaviour(float timeAlreadyElapsed)
	{
		originalVelocity = myRigidbody.velocity;
		float fadeInTime = 2.75f - timeAlreadyElapsed;
		myAnimation["fadeIn"].speed = myAnimation["fadeIn"].length / fadeInTime;
		myAnimation.Play("fadeIn");
		myRigidbody.velocity = Vector3.zero;
		yield return new WaitForSeconds(fadeInTime);
		if (coreDamageSource != null)
		{
			coreDamageSource.gameObject.layer = LayerMask.NameToLayer("Explosion");
		}
		base.audio.clip = startFlyingSound;
		base.audio.Play();
		base.gameObject.layer = LayerMask.NameToLayer("Projectile");
		myRigidbody.velocity = originalVelocity;
		myAnimation.CrossFade("idle", 0.5f);
		yield return new WaitForSeconds(startFlyingSound.length);
		base.audio.loop = true;
		base.audio.clip = flyingLoopSound;
		base.audio.Play();
		yield return new WaitForSeconds(duration - startFlyingSound.length);
		Explode(null);
	}
}
