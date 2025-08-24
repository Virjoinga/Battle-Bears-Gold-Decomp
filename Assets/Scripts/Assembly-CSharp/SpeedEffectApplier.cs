using UnityEngine;

public class SpeedEffectApplier : EffectApplier
{
	private Team _localPlayerTeam = Team.NONE;

	[SerializeField]
	private float _speedDuration = 2f;

	[SerializeField]
	private float _speedAmount;

	[SerializeField]
	private float _speedPercentage = 0.75f;

	private void Start()
	{
		if (ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(itemOverride);
			if (itemByName != null)
			{
				itemByName.UpdateProperty("duration", ref _speedDuration, "|");
				itemByName.UpdateProperty("speedIncrease", ref _speedAmount, "|");
				itemByName.UpdateProperty("speedMultiplier", ref _speedPercentage, "|");
			}
		}
		if (!(GameManager.Instance != null))
		{
			return;
		}
		ConfigurableNetworkObject component = GetComponent<ConfigurableNetworkObject>();
		if (component != null)
		{
			PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(component.OwnerID);
			if (playerCharacterManager != null)
			{
				_localPlayerTeam = playerCharacterManager.team;
			}
		}
	}

	protected override void ApplyEffect(PlayerController pc)
	{
		if (_localPlayerTeam != pc.Team && !pc.isRemote && pc.StatManager != null)
		{
			StatisticMod mod = new StatisticMod(Statistic.MaxForwardMovementSpeed, _speedDuration, _speedAmount, _speedPercentage);
			StatisticMod mod2 = new StatisticMod(Statistic.MaxBackwardsMovementSpeed, _speedDuration, _speedAmount, _speedPercentage);
			StatisticMod mod3 = new StatisticMod(Statistic.MaxSidewaysMovementSpeed, _speedDuration, _speedAmount, _speedPercentage);
			pc.StatManager.AddStatMod(mod);
			pc.StatManager.AddStatMod(mod2);
			pc.StatManager.AddStatMod(mod3);
			pc.StartSlowedDurationCoroutine(_speedDuration, _speedAmount);
		}
	}
}
