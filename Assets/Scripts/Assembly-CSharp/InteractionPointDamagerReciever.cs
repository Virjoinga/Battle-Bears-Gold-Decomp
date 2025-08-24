using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InteractionPointBase))]
public class InteractionPointDamagerReciever : DamageReceiver
{
	private bool isDying;

	private float _health;

	public bool onlyMelee;

	public InteractionPointBase interactionPoint;

	public SettingsBundle damageReciverSettingsBundle;

	public virtual void Start()
	{
	}

	public override void OnTakeDamage(float dmg, int shooterID, bool isExplosion, bool isMelee, bool isHeadshot, bool sendNotification, bool endOfGameOverride = false, float radiationDmg = 0f, string customDeathSfx = "")
	{
		base.OnTakeDamage(dmg, shooterID, isExplosion, isMelee, isHeadshot, sendNotification, endOfGameOverride, radiationDmg, string.Empty);
		if (!isDying)
		{
			if (!onlyMelee)
			{
				DamageHealth(dmg, shooterID);
			}
			else if (isMelee)
			{
				DamageHealth(dmg, shooterID);
			}
		}
	}

	protected void DamageHealth(float dmg, int shooterID)
	{
		_health -= dmg;
		if (_health <= 0f)
		{
			InteractionPointManager.Instance.TriggerInteractionPoint(interactionPoint.interactionPointIndex, shooterID);
			return;
		}
		if (damageReciverSettingsBundle.objectToAnimate != null && damageReciverSettingsBundle.damagedAnimation != null)
		{
			damageReciverSettingsBundle.objectToAnimate.Play(damageReciverSettingsBundle.damagedAnimation.name);
		}
		if (damageReciverSettingsBundle.audioSource != null && damageReciverSettingsBundle.damagedSound != null)
		{
			damageReciverSettingsBundle.audioSource.PlayOneShot(damageReciverSettingsBundle.damagedSound);
		}
	}

	public void Respawn()
	{
		if (!isDying)
		{
			if (damageReciverSettingsBundle.objectToAnimate != null && damageReciverSettingsBundle.destroyedAnimation != null)
			{
				damageReciverSettingsBundle.objectToAnimate.Play(damageReciverSettingsBundle.destroyedAnimation.name);
			}
			if (damageReciverSettingsBundle.audioSource != null && damageReciverSettingsBundle.destroyedSound != null)
			{
				damageReciverSettingsBundle.audioSource.PlayOneShot(damageReciverSettingsBundle.destroyedSound);
			}
			isDying = true;
			StartCoroutine(RespawnCoroutine());
		}
	}

	protected IEnumerator RespawnCoroutine()
	{
		yield return new WaitForSeconds(damageReciverSettingsBundle.respawnTime);
		_health = damageReciverSettingsBundle.health;
		isDying = false;
		if (damageReciverSettingsBundle.objectToAnimate != null && damageReciverSettingsBundle.respawnAnimation != null)
		{
			damageReciverSettingsBundle.objectToAnimate.Play(damageReciverSettingsBundle.respawnAnimation.name);
		}
		if (damageReciverSettingsBundle.audioSource != null && damageReciverSettingsBundle.respawnSound != null)
		{
			damageReciverSettingsBundle.audioSource.PlayOneShot(damageReciverSettingsBundle.respawnSound);
		}
		yield return null;
	}

	protected void Awake()
	{
		base.OwnerID = -1;
		_health = damageReciverSettingsBundle.health;
	}
}
