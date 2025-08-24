using System.Collections.Generic;
using UnityEngine;

public class DeployableSlowField : ConfigurableNetworkObject
{
	public struct SlowedPlayer
	{
		public StatisticMod forwardMod;

		public StatisticMod sidewaysMod;

		public StatisticMod backwardMod;

		public PlayerController controller;
	}

	public string itemOverride = string.Empty;

	private Dictionary<string, SlowedPlayer> playersAffected = new Dictionary<string, SlowedPlayer>();

	public float slowAmount;

	public float slowMult = 1f;

	public float duration = 30f;

	public AudioClip slowdownSound;

	private Team ownerTeam;

	protected override void Start()
	{
		base.Start();
		ownerTeam = GameManager.Instance.Players(base.OwnerID).team;
		Item itemByName = ServiceManager.Instance.GetItemByName(itemOverride);
		if (itemByName != null)
		{
			itemByName.UpdateProperty("speedMultiplier", ref slowMult, equipmentNames);
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		PlayerController playerController = c.gameObject.GetComponent(typeof(PlayerController)) as PlayerController;
		if (playerController != null && playerController.Team != ownerTeam && !playersAffected.ContainsKey(playerController.name))
		{
			playerController.slowCount++;
			if (!playerController.isRemote && playerController.StatManager[Statistic.MaxForwardMovementSpeed] > slowAmount && playerController.StatManager[Statistic.MaxBackwardsMovementSpeed] > slowAmount && playerController.StatManager[Statistic.MaxSidewaysMovementSpeed] > slowAmount)
			{
				StatisticMod statisticMod = new StatisticMod(Statistic.MaxForwardMovementSpeed, duration, 0f - slowAmount, slowMult);
				StatisticMod statisticMod2 = new StatisticMod(Statistic.MaxSidewaysMovementSpeed, duration, 0f - slowAmount, slowMult);
				StatisticMod statisticMod3 = new StatisticMod(Statistic.MaxBackwardsMovementSpeed, duration, 0f - slowAmount, slowMult);
				playerController.StatManager.AddStatMod(statisticMod);
				playerController.StatManager.AddStatMod(statisticMod2);
				playerController.StatManager.AddStatMod(statisticMod3);
				playerController.OnPlayGenericSound(slowdownSound);
				SlowedPlayer value = default(SlowedPlayer);
				value.forwardMod = statisticMod;
				value.sidewaysMod = statisticMod2;
				value.backwardMod = statisticMod3;
				value.controller = playerController;
				playersAffected.Add(playerController.name, value);
			}
			else if (playerController.isRemote)
			{
				SlowedPlayer value2 = default(SlowedPlayer);
				value2.controller = playerController;
				playersAffected.Add(playerController.name, value2);
			}
		}
	}

	private void OnTriggerExit(Collider c)
	{
		PlayerController playerController = c.gameObject.GetComponent(typeof(PlayerController)) as PlayerController;
		if (playerController != null && playerController.Team != ownerTeam && playersAffected.ContainsKey(playerController.name))
		{
			playerController.slowCount--;
			SlowedPlayer slowedPlayer = playersAffected[playerController.name];
			if (slowedPlayer.forwardMod != null && slowedPlayer.sidewaysMod != null && slowedPlayer.backwardMod != null)
			{
				playerController.StatManager.RemoveStatMod(slowedPlayer.forwardMod);
				playerController.StatManager.RemoveStatMod(slowedPlayer.sidewaysMod);
				playerController.StatManager.RemoveStatMod(slowedPlayer.backwardMod);
			}
			playersAffected.Remove(playerController.name);
		}
	}

	private void OnDestroy()
	{
		foreach (KeyValuePair<string, SlowedPlayer> item in playersAffected)
		{
			item.Value.controller.slowCount--;
			if (item.Value.forwardMod != null && item.Value.sidewaysMod != null && item.Value.backwardMod != null)
			{
				item.Value.controller.StatManager.RemoveStatMod(item.Value.forwardMod);
				item.Value.controller.StatManager.RemoveStatMod(item.Value.sidewaysMod);
				item.Value.controller.StatManager.RemoveStatMod(item.Value.backwardMod);
			}
		}
		playersAffected.Clear();
	}
}
