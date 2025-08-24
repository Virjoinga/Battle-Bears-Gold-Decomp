using UnityEngine;

public class DamageReceiverProxy : DamageReceiver
{
	private DamageReceiver proxy;

	public void Awake()
	{
		renderers = GetComponentsInChildren(typeof(MeshRenderer));
	}

	public void OnSetProxy(DamageReceiver p)
	{
		proxy = p;
		p.linkedProxy = this;
		base.OwnerID = p.OwnerID;
	}

	public override void OnTakeDamage(float dmg, int shooterID, bool isExplosion, bool isMelee, bool isHeadshot, bool sendNotification, bool endOfGameOverride = false, float radiationDmg = 0f, string customDeathSfx = "")
	{
		base.OnTakeDamage(dmg, shooterID, isExplosion, isMelee, isHeadshot, sendNotification, endOfGameOverride, radiationDmg, customDeathSfx);
		if (proxy != null)
		{
			if (isExplosion || (!isExplosion && shooterID != base.OwnerID))
			{
				proxy.OnTakeDamage(dmg, shooterID, isExplosion, isMelee, isHeadshot, sendNotification, endOfGameOverride, radiationDmg, customDeathSfx);
			}
			if (!proxy.isInvincible)
			{
				checkForFriendlyFire(shooterID);
			}
		}
	}

	public override void OnKilledByDeathArea()
	{
		proxy.OnKilledByDeathArea();
	}

	public void OnProxyHit()
	{
		if (proxy != null && !proxy.isInvincible && !isShowingHit)
		{
			StartCoroutine(displayHit(0f));
		}
	}

	private void checkForFriendlyFire(int shooterID)
	{
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(shooterID);
		PlayerCharacterManager playerCharacterManager2 = GameManager.Instance.Players(base.OwnerID);
		bool flag = true;
		if (playerCharacterManager != null && playerCharacterManager2 != null && GameManager.Instance.friendlyFireRatio < 0.01f && playerCharacterManager != null && playerCharacterManager2 != null && playerCharacterManager.team == playerCharacterManager2.team && shooterID != base.OwnerID)
		{
			flag = false;
		}
		if (flag && !isShowingHit)
		{
			StartCoroutine(displayHit(0f));
		}
	}
}
