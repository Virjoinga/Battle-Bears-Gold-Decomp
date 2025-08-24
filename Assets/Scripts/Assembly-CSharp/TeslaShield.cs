using System.Collections;
using UnityEngine;

public class TeslaShield : SpecialItem
{
	public float duration = 30f;

	public float scaleModifier;

	private Transform attachPoint;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/teslashield";
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		item.UpdateProperty("diameter", ref scaleModifier, base.EquipmentNames);
		base.Configure(item);
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		myTransform.parent = playerController.transform.FindChild("playerModel");
		myTransform.localPosition = new Vector3(0f, -35f, 0f);
		myTransform.localEulerAngles = Vector3.zero;
		myTransform.localScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
		StartCoroutine(delayedEnd(delay));
	}

	public override void OnDeactivate(float delay)
	{
		base.OnDeactivate(delay);
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator delayedEnd(float delay)
	{
		yield return new WaitForSeconds(duration - delay);
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	public bool PlayerOnOwnersTeam(int playerIDToCheck)
	{
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(playerIDToCheck);
		if (playerCharacterManager != null)
		{
			return base.PlayerController.Team == playerCharacterManager.team;
		}
		Debug.LogError("Other player ID not found");
		return false;
	}
}
