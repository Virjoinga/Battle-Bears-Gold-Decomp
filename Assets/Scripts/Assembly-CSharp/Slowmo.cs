using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slowmo : SpecialItem
{
	public struct SlowedPlayer
	{
		public StatisticMod forwardMod;

		public StatisticMod sidewaysMod;

		public StatisticMod backwardMod;

		public PlayerController controller;
	}

	private Dictionary<string, SlowedPlayer> playersAffected = new Dictionary<string, SlowedPlayer>();

	public float slowAmount = 100f;

	public float slowMult = 1f;

	public float duration = 30f;

	public Vector3 localOffset;

	public float scaleModifier = 1f;

	private Rigidbody myRigidbody;

	public ParticleEmitter verticalEmitter;

	public ParticleEmitter horizontalEmitter;

	public AudioClip slowdownSound;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/slowfield";
		}
	}

	protected override void Awake()
	{
		base.Awake();
		myRigidbody = base.GetComponent<Rigidbody>();
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		item.UpdateProperty("slowAmount", ref slowAmount, base.EquipmentNames);
		item.UpdateProperty("diameter", ref scaleModifier, base.EquipmentNames);
		base.Configure(item);
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		myTransform.parent = playerController.transform;
		myTransform.localPosition = localOffset;
		myTransform.localEulerAngles = Vector3.zero;
		myTransform.localScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
		StartCoroutine(delayedEnd(delay));
	}

	private IEnumerator delayedEnd(float delay)
	{
		yield return new WaitForSeconds(duration - delay);
		StartCoroutine(delayedDestroy());
	}

	private IEnumerator delayedDestroy()
	{
		verticalEmitter.emit = false;
		verticalEmitter.transform.parent = null;
		horizontalEmitter.emit = false;
		horizontalEmitter.transform.parent = null;
		myRigidbody.WakeUp();
		myTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		myTransform.position = new Vector3(100000f, -1000000f, 0f);
		yield return new WaitForSeconds(0.2f);
		myRigidbody.WakeUp();
		Object.Destroy(base.gameObject);
	}

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		StopAllCoroutines();
		StartCoroutine(delayedDestroy());
	}

	private void OnTriggerEnter(Collider c)
	{
		PlayerController playerController = c.gameObject.GetComponent(typeof(PlayerController)) as PlayerController;
		if (playerController != null && playerController.Team != base.playerController.Team && !playersAffected.ContainsKey(playerController.name))
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
			else if (isRemote)
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
		if (playerController != null && playerController.Team != base.playerController.Team && playersAffected.ContainsKey(playerController.name))
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
