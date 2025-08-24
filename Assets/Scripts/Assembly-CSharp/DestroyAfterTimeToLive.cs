using UnityEngine;

public class DestroyAfterTimeToLive : MonoBehaviour
{
	[SerializeField]
	private string _itemName = string.Empty;

	private void Awake()
	{
		float pVal = 5f;
		Item itemByName = ServiceManager.Instance.GetItemByName(_itemName);
		if (itemByName != null)
		{
			itemByName.UpdateProperty("timeToLive", ref pVal, "|");
		}
		Object.Destroy(base.gameObject, pVal);
	}
}
