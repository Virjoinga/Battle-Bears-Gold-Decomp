using System.Collections;
using UnityEngine;

public class HotCoffee : SpecialItem
{
	public float speedIncrease = 150f;

	public float duration = 10f;

	public ParticleEmitter trail;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/coffee";
		}
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		myTransform.parent = playerController.transform;
		if (!isRemote)
		{
			StatisticMod mod = new StatisticMod(Statistic.MaxForwardMovementSpeed, duration, speedIncrease);
			StatisticMod mod2 = new StatisticMod(Statistic.MaxSidewaysMovementSpeed, duration, speedIncrease);
			StatisticMod mod3 = new StatisticMod(Statistic.MaxBackwardsMovementSpeed, duration, speedIncrease);
			p.StatManager.AddStatMod(mod);
			p.StatManager.AddStatMod(mod2);
			p.StatManager.AddStatMod(mod3);
		}
		playerController.hasCoffee = true;
		StartCoroutine(delayedEnd(delay));
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		item.UpdateProperty("speedIncrease", ref speedIncrease, base.EquipmentNames);
		base.Configure(item);
	}

	private IEnumerator delayedEnd(float delay)
	{
		yield return new WaitForSeconds(duration - delay);
		trail.transform.parent = null;
		trail.emit = false;
		Object.Destroy(base.gameObject);
	}

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		trail.transform.parent = null;
		trail.emit = false;
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		playerController.hasCoffee = false;
	}
}
