using System.Collections;
using UnityEngine;

public class ProximityMine : Mine
{
	public LineRenderer teamLineRenderer;

	public Material redMaterial;

	public Material blueMaterial;

	public float teamLineBlinkDuration;

	private float mineTriggerRadius;

	protected override void Awake()
	{
		base.Awake();
		teamLineRenderer.enabled = false;
	}

	protected override void Start()
	{
		base.Start();
		if (OwningPlayer.Team == Team.RED)
		{
			teamLineRenderer.material = redMaterial;
		}
		else if (OwningPlayer.Team == Team.BLUE)
		{
			teamLineRenderer.material = blueMaterial;
		}
		if (OwningPlayer.Team == GameManager.Instance.Players(GameManager.Instance.localPlayerID).team)
		{
			StartCoroutine(BlinkUntilArmed());
		}
	}

	public override void ConfigureObject()
	{
		base.ConfigureObject();
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("mineTriggerRadius", ref mineTriggerRadius, equipmentNames);
		}
		SphereCollider component = GetComponent<SphereCollider>();
		if (component != null && mineTriggerRadius > 0f)
		{
			component.radius = mineTriggerRadius;
		}
	}

	private IEnumerator BlinkUntilArmed()
	{
		float timeOfStart = Time.time;
		while (Time.time - timeOfStart < primingTime)
		{
			yield return new WaitForSeconds(teamLineBlinkDuration);
			teamLineRenderer.enabled = !teamLineRenderer.enabled;
		}
		teamLineRenderer.enabled = true;
		teamLineRenderer.SetWidth(2f, 2f);
	}
}
