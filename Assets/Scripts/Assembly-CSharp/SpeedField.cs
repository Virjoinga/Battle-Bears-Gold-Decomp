using System.Collections.Generic;
using UnityEngine;

public class SpeedField : EffectField
{
	public struct SpeededPlayer
	{
		public StatisticMod forwardMod;

		public StatisticMod sidewaysMod;

		public StatisticMod backwardMod;

		public PlayerController controller;
	}

	private Dictionary<string, SpeededPlayer> _playersAffected = new Dictionary<string, SpeededPlayer>();

	[SerializeField]
	private new float _duration = 30f;

	[SerializeField]
	private float _speedAmount = 100f;

	[SerializeField]
	private float _speedPercentage = 1f;

	protected override void Start()
	{
		base.Start();
		if (ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			if (itemByName != null)
			{
				itemByName.UpdateProperty("duration", ref _duration, "|");
				itemByName.UpdateProperty("speedIncrease", ref _speedAmount, "|");
				itemByName.UpdateProperty("speedMultiplier", ref _speedPercentage, "|");
			}
		}
	}

	protected override void ApplyEffect(PlayerController pc)
	{
		base.ApplyEffect(pc);
		if (!pc.isRemote)
		{
			StatisticMod statisticMod = new StatisticMod(Statistic.MaxForwardMovementSpeed, _duration, _speedAmount, _speedPercentage);
			StatisticMod statisticMod2 = new StatisticMod(Statistic.MaxBackwardsMovementSpeed, _duration, _speedAmount, _speedPercentage);
			StatisticMod statisticMod3 = new StatisticMod(Statistic.MaxSidewaysMovementSpeed, _duration, _speedAmount, _speedPercentage);
			pc.StatManager.AddStatMod(statisticMod);
			pc.StatManager.AddStatMod(statisticMod2);
			pc.StatManager.AddStatMod(statisticMod3);
			SpeededPlayer value = default(SpeededPlayer);
			value.forwardMod = statisticMod;
			value.backwardMod = statisticMod2;
			value.sidewaysMod = statisticMod3;
			_playersAffected.Add(pc.name, value);
		}
	}

	protected override void RemoveEffect(PlayerController pc)
	{
		base.RemoveEffect(pc);
		if (pc != null && _playersAffected.ContainsKey(pc.name))
		{
			SpeededPlayer speededPlayer = _playersAffected[pc.name];
			pc.StatManager.RemoveStatMod(speededPlayer.forwardMod);
			pc.StatManager.RemoveStatMod(speededPlayer.backwardMod);
			pc.StatManager.RemoveStatMod(speededPlayer.sidewaysMod);
			_playersAffected.Remove(pc.name);
		}
	}
}
