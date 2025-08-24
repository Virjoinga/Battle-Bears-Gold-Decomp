using UnityEngine;

public class StockRifle : SniperRifle
{
	private class StockPrice
	{
		public string change { get; set; }
	}

	private float stockPrice;

	public Renderer barrelRenderer;

	public Material redMaterial;

	public ParticleSystem burstFire;

	public ParticleSystem coinEffect;

	public GameObject redTracerPrefab;

	private float marketDownDamageChange;

	private float marketDownLockTimeChange;

	private float marketDownLockRadiusChange;

	private float marketDownWarningTimeChange;

	private float marketDownCooldownChange;

	private int marketDownClipSizeChange;

	public AudioClip[] redFireSounds;

	public override void ConfigureWeapon(Item item)
	{
		base.ConfigureWeapon(item);
		if (stockPrice < 0f)
		{
			item.UpdateProperty("market_down_damage_change", ref marketDownDamageChange, base.EquipmentNames);
			item.UpdateProperty("market_down_lockTime_change", ref marketDownLockTimeChange, base.EquipmentNames);
			item.UpdateProperty("market_down_lockRadius_change", ref marketDownLockRadiusChange, base.EquipmentNames);
			item.UpdateProperty("market_down_warningTime_change", ref marketDownWarningTimeChange, base.EquipmentNames);
			item.UpdateProperty("market_down_cooldown_change", ref marketDownCooldownChange, base.EquipmentNames);
			item.UpdateProperty("market_down_clipSize_change", ref marketDownClipSizeChange, base.EquipmentNames);
			damage += marketDownDamageChange;
			targettingTime += marketDownLockTimeChange;
			lockRadius += marketDownLockRadiusChange;
			warningTime += marketDownWarningTimeChange;
			firingTime += marketDownCooldownChange;
			clipSize += marketDownClipSizeChange;
			if (!isRemote && item.type != "melee" && HUD.Instance != null)
			{
				HUD.Instance.OnSetClipSize(clipSize);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		ServiceManager.Instance.UpdateProperty("stock_quote", ref stockPrice);
		adjustGunMode();
	}

	private void adjustGunMode()
	{
		if (stockPrice < 0f)
		{
			barrelRenderer.material = redMaterial;
			burstFire.renderer.material.mainTextureOffset = new Vector2(0.5f, 0f);
			tracerPrefab = redTracerPrefab;
			fireSounds = redFireSounds;
		}
		else
		{
			burstFire.renderer.material.mainTextureOffset = new Vector2(0f, 0f);
		}
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		burstFire.Emit(10);
		coinEffect.Emit(1);
		return true;
	}
}
