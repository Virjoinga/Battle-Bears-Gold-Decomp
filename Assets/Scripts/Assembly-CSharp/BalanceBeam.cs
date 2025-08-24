using UnityEngine;

public class BalanceBeam : ConstantStreamWeapon
{
	protected float bonusDamagePerDifference;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("score_difference_bonus_dmg", ref bonusDamagePerDifference, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	public override void PlayAttackAnimation(float startTime, float speed)
	{
	}

	public override void BeginConstantFireEffects()
	{
		base.BeginConstantFireEffects();
		if (myAnimation != null && myAnimation["fire"] != null)
		{
			myAnimation.CrossFade("fire");
		}
	}

	public override void EndConstantFireEffects()
	{
		base.EndConstantFireEffects();
		myAnimation.Stop();
	}

	protected override void dealDamage(DamageReceiver d)
	{
		if (!(d != null) || d.isInvincible)
		{
			return;
		}
		float num = 0f;
		if (GameManager.Instance != null && base.playerController != null)
		{
			int num2 = 0;
			int num3 = 0;
			if (Preferences.Instance.CurrentGameMode == GameMode.CTF)
			{
				if (base.playerController.Team == Team.RED)
				{
					num2 = GameManager.Instance.RedDeposits;
					num3 = GameManager.Instance.BlueDeposits;
				}
				else if (base.playerController.Team == Team.BLUE)
				{
					num2 = GameManager.Instance.BlueDeposits;
					num3 = GameManager.Instance.RedDeposits;
				}
			}
			else if (Preferences.Instance.CurrentGameMode == GameMode.TB)
			{
				if (base.playerController.Team == Team.RED)
				{
					num2 = GameManager.Instance.RedKills;
					num3 = GameManager.Instance.BlueKills;
				}
				else if (base.playerController.Team == Team.BLUE)
				{
					num2 = GameManager.Instance.BlueKills;
					num3 = GameManager.Instance.RedKills;
				}
			}
			num = (float)(num3 - num2) * bonusDamagePerDifference;
		}
		OnDealDirectDamage(d, Mathf.Max(1f, damage + num) * base.playerController.DamageMultiplier);
	}
}
