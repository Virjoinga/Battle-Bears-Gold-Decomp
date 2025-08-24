using System.Collections;
using UnityEngine;

public class DurationDelayedDestroy : ConfigurableNetworkObject
{
	private float _delay;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(DestroyAfterSeconds());
	}

	private IEnumerator DestroyAfterSeconds()
	{
		while (string.IsNullOrEmpty(configureItemName))
		{
			Debug.Log("Waiting for my configureItemName");
			yield return null;
		}
		Item i = ServiceManager.Instance.GetItemByName(configureItemName);
		i.UpdateProperty("duration", ref _delay, equipmentNames);
		yield return new WaitForSeconds(_delay);
		Object.Destroy(base.gameObject);
	}
}
