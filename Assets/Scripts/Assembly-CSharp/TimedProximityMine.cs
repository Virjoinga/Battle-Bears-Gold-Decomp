using System.Collections;
using UnityEngine;

public class TimedProximityMine : ProximityMine
{
	[SerializeField]
	private float _detonationDelay;

	protected override void Awake()
	{
		base.collider.enabled = false;
	}

	protected override void Start()
	{
		ConfigureObject();
		StartCoroutine(BeginDelayedDetonationAfterPrimed());
	}

	public override void ConfigureObject()
	{
		base.ConfigureObject();
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("explosionDelay", ref _detonationDelay, equipmentNames);
		}
	}

	private IEnumerator BeginDelayedDetonationAfterPrimed()
	{
		yield return StartCoroutine(PrimingCoRoutine(primingTime));
		StartCoroutine(DetonateAfterDelay());
	}

	private IEnumerator DetonateAfterDelay()
	{
		yield return new WaitForSeconds(_detonationDelay);
		OnDetonateDeployable(OwningPlayer, false);
	}
}
