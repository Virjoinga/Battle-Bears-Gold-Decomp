using System.Collections;
using UnityEngine;

public class KingslayerDamageSource : LineOfSightDamageSource
{
	protected float bonusDamagePerDifference;

	public AudioClip extraDamageSound;

	protected override void Start()
	{
		base.Start();
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("kill_difference_bonus_dmg", ref bonusDamagePerDifference, equipmentNames);
		}
		StartCoroutine(delayedDestroy());
	}

	private IEnumerator delayedDestroy()
	{
		yield return new WaitForSeconds(0.25f);
		if (myCollider != null)
		{
			Object.Destroy(myCollider);
		}
		Object.Destroy(this);
	}

	protected override void dealDamage(GameObject target)
	{
		if (!checkForActualHit(target))
		{
			return;
		}
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (!(damageReceiver != null))
		{
			return;
		}
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		if (GameManager.Instance != null)
		{
			PlayerStats playerStats = GameManager.Instance.playerStats[base.OwnerID];
			if (playerStats != null)
			{
				num2 = playerStats.NetKills;
			}
			PlayerStats playerStats2 = GameManager.Instance.playerStats[damageReceiver.OwnerID];
			if (playerStats2 != null)
			{
				num3 = playerStats2.NetKills;
			}
			num = Mathf.Max(0f, (float)(num3 - num2) * bonusDamagePerDifference);
		}
		if (num > 0f && extraDamageSound != null)
		{
			base.audio.PlayOneShot(extraDamageSound);
		}
		damageReceiver.OnTakeDamage((damage + num) * base.DamageMultiplier, base.OwnerID, false, false, false, true, false, 0f, string.Empty);
	}
}
